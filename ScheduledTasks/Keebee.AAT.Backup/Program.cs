using Keebee.AAT.RESTClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Keebee.AAT.Backup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                const string sqlFilestreamName = "KeebeeAATFilestream";
                var pathDeployments = ConfigurationManager.AppSettings["DeploymentsPath"];
                var pathVideoCaptures = ConfigurationManager.AppSettings["VideoCapturesPath"];
                var pathBackup = ConfigurationManager.AppSettings["BackupPath"];

                var computerName = Environment.MachineName;
                var pathSqlMedia = $@"\\{computerName}\sqlexpress\{sqlFilestreamName}\Media";

                var deploymentsFolder = pathDeployments.Replace(Path.GetPathRoot(pathDeployments), string.Empty);
                var mediaBackupPath = $@"{deploymentsFolder}\Media";

                var logText = new StringBuilder();
                var pathLogs = $@"{pathBackup}\BackupLogs";

                var logFilename = InitializeLogFile(pathLogs);

                using (var w = File.AppendText(logFilename))
                {
                    var diBackup = new DirectoryInfo(pathBackup);
                    if (!diBackup.Exists) return;

                    var rootBackup = Path.GetPathRoot(pathBackup);
                    var driveBackup = new DriveInfo(rootBackup);

                    if (driveBackup.IsReady)
                    {
                        // backup deployment folders (except media)
                        logText.Append(
                            BackupFiles(pathDeployments, pathBackup, deploymentsFolder, sqlFilestreamName,
                                excludeFolders: new[] {Path.Combine(pathDeployments, "Media")}));

                        // back up the media
                        logText.Append(
                            BackupFiles(pathSqlMedia, pathBackup, deploymentsFolder, sqlFilestreamName));

                        // back up video captures
                        logText.Append(
                            BackupFiles(pathVideoCaptures, pathBackup, deploymentsFolder, sqlFilestreamName));

                        // delete obsolete deployment folders (except media)
                        logText.Append(
                            RemoveObsoleteFolders(pathDeployments, pathBackup, mediaBackupPath, sqlFilestreamName,
                                excludeFolders: new[] {Path.Combine(pathBackup, mediaBackupPath)}));

                        // delete obsolete media folders
                        logText.Append(
                            RemoveObsoleteFolders(pathSqlMedia, pathBackup, mediaBackupPath, sqlFilestreamName));

                        // delete obsolete video capture folders
                        logText.Append(
                            RemoveObsoleteFolders(pathVideoCaptures, pathBackup, mediaBackupPath, sqlFilestreamName));

                        // delete obsolete deployment files (except media)
                        logText.Append(
                            RemoveObsoleteFiles(pathDeployments, pathBackup, mediaBackupPath, sqlFilestreamName,
                                excludeFolders: new[] {Path.Combine(pathBackup, mediaBackupPath)}));

                        // delete obsolete media files
                        logText.Append(
                            RemoveObsoleteFiles(pathSqlMedia, pathBackup, mediaBackupPath, sqlFilestreamName));

                        // delete obsolete video capture files
                        logText.Append(
                            RemoveObsoleteFiles(pathVideoCaptures, pathBackup, mediaBackupPath, sqlFilestreamName));

                        // create the database scripts
                        logText.Append(
                            CreateScriptSeedResidents($@"{pathBackup}\{deploymentsFolder}"));
                        logText.Append(
                            CreateScriptSeedConfigurations($@"{pathBackup}\{deploymentsFolder}"));
                    }
                    else
                    {
                        logText.Append($"--- ERROR --- Main: Backup drive {driveBackup.Name} is not accessible.");
                    }
                    w.WriteLine("------------------------------------");
                    w.WriteLine($"Backup Summary for {DateTime.Now.ToLongDateString()}");
                    w.WriteLine("------------------------------------");
                    w.Write(logText.Length <= 0
                        ? "No actions taken - all files and folders are up to date"
                        : logText.ToString());
                }
            }
            catch (Exception e)
            {
                // for debugging only
                Console.Write(e.Message);
            }
        }

        private static string InitializeLogFile(string pathLog)
        {
            var directory = new DirectoryInfo(pathLog);
            if (!directory.Exists)
                directory.Create();

            var dateString = DateTime.Now.ToString("yyyy-MM-dd");
            var filePath = $@"{pathLog}\BackupLog_{dateString.Replace("-", "_")}.log";
            if (File.Exists(filePath))
                File.Delete(filePath);

            return filePath;
        }

        private static string CreateBackupFolders(string source, string destination, string[] excludeFolders = null)
        {
            var sb = new StringBuilder();

            try
            {
                // Data structure to hold names of subfolders
                var dirs = new Stack<string>(20);
                var driveSource = Path.GetPathRoot(source);
                if (driveSource == null)
                {
                    return $"--- ERROR --- CreateBackupFolders: Source drive for {source} is not available{Environment.NewLine}";
                }

                if (!Directory.Exists(source))
                {
                    throw new ArgumentException();
                }
                dirs.Push(source);

                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        sb.Append($"--- ERROR --- CreateBackupFolders: {e.Message}{Environment.NewLine}");
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        sb.Append($"--- ERROR --- CreateBackupFolders: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    var subPathSource = currentDir.Replace(driveSource, string.Empty);
                    var pathDest = subPathSource.Contains("KeebeeAATFilestream")
                        ? Path.Combine(destination,
                            $@"Deployments\{subPathSource.Replace(@"\KeebeeAATFilestream\", string.Empty)}")
                        : Path.Combine(destination, subPathSource);

                    var directory = new DirectoryInfo(pathDest);
                    if (!directory.Exists)
                    {
                        directory.Create();
                        sb.Append($"Folder '{directory.FullName}' was created{Environment.NewLine}");
                    }

                    foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]))
                        dirs.Push(dir);
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- CreateBackupFolders: {e.Message}{Environment.NewLine}");
            }

            return sb.ToString();
        }

        private static string BackupFiles(string source, string destination, string deploymentsFolder,
            string sqlFileStreamName, string[] excludeFolders = null)
        {
            var sb = new StringBuilder();

            try
            {
                sb.Append(CreateBackupFolders(source, destination, excludeFolders));

                // Data structure to hold names of subfolders to be examined for files
                var dirs = new Stack<string>(20);
                var driveSource = Path.GetPathRoot(source);
                if (driveSource == null)
                {
                    return $"--- ERROR --- BackupFiles: Source drive for {source} is not ready{Environment.NewLine}";
                }

                if (!Directory.Exists(source))
                {
                    throw new ArgumentException();
                }
                dirs.Push(source);

                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir);
                    }
                    // An UnauthorizedAccessException exception will be thrown if we do not have
                    // discovery permission on a folder or file. It may or may not be acceptable 
                    // to ignore the exception and continue enumerating the remaining files and 
                    // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                    // will be raised. This will happen if currentDir has been deleted by
                    // another application or thread after our call to Directory.Exists. The 
                    // choice of which exceptions to catch depends entirely on the specific task 
                    // you are intending to perform and also on how much you know with certainty 
                    // about the systems on which this code will run.
                    catch (UnauthorizedAccessException e)
                    {
                        sb.Append($"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        sb.Append($"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(currentDir);
                    }

                    catch (UnauthorizedAccessException e)
                    {
                        sb.Append($"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    catch (DirectoryNotFoundException e)
                    {
                        sb.Append($"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    // copy files
                    foreach (var file in files)
                    {
                        try
                        {
                            var fiSource = new FileInfo(file);
                            if (fiSource.DirectoryName == null) continue;

                            var pathSource = fiSource.DirectoryName.Replace(driveSource, string.Empty);

                            var pathDest = pathSource.Contains(sqlFileStreamName)
                                ? Path.Combine(destination,
                                    $@"{deploymentsFolder}\{pathSource.Replace($@"\{sqlFileStreamName}\", string.Empty)}")
                                : Path.Combine(destination, pathSource);

                            if (!Directory.Exists(pathDest))
                                Directory.CreateDirectory(pathDest);

                            var destFilePath = Path.Combine(pathDest, fiSource.Name);
                            if (File.Exists(destFilePath))
                            {
                                if (IsFileIdentical(fiSource.FullName, destFilePath))
                                    continue;
                            }

                            File.Copy(fiSource.FullName, Path.Combine(pathDest, fiSource.Name), true);
                            sb.Append($"File '{fiSource.Name}' was copied to '{pathDest}'{Environment.NewLine}");
                        }
                        catch (FileNotFoundException e)
                        {
                            sb.Append($"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}");
                        }
                    }

                    // Push the subdirectories onto the stack for traversal.
                    // This could also be done before handing the files.
                    foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]))
                        dirs.Push(dir);
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}");
            }
            return sb.ToString();
        }

        private static bool IsFileIdentical(string file1, string file2)
        {
            return new FileInfo(file1).Length == new FileInfo(file2).Length 
                && (File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2)));
        }

        private static string RemoveObsoleteFolders(string source, string destination, string mediaBackupPath, string sqlFileStreamName, string[] excludeFolders = null)
        {
            var sb = new StringBuilder();

            try
            {
                // Data structure to hold names of subfolders
                var dirs = new Stack<string>(20);

                var driveSource = source.Contains(sqlFileStreamName)
                    ? source
                    : Path.GetPathRoot(source);

                var driveDest = Path.GetPathRoot(destination);

                if (driveDest == null)
                {
                    return $"--- ERROR --- RemoveObsoleteFolders: Destination drive for {destination} is not available";
                }

                var pathDestination = source.Contains(sqlFileStreamName)
                    ? Path.Combine(destination, mediaBackupPath)
                    : Path.Combine(destination, source.Replace(driveSource, string.Empty));

                if (!Directory.Exists(pathDestination))
                {
                    return $"--- ERROR --- RemoveObsoleteFolders: {pathDestination} does not exist{Environment.NewLine}";
                }

                dirs.Push(pathDestination);

                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        sb.Append($"--- ERROR --- RemoveObsoleteFolders: {e.Message}{Environment.NewLine}");
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        sb.Append($"--- ERROR --- RemoveObsoleteFolders: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    // remove trainling slash if it exists
                    string pathDest;
                    if (destination.EndsWith(@"\"))
                    {
                        var indexOfSlash = destination.LastIndexOf(@"\", StringComparison.Ordinal);
                        pathDest = destination.Substring(0, indexOfSlash);
                    }
                    else
                    {
                        pathDest = destination;
                    }

                    var pathSource = source.Contains(sqlFileStreamName)
                        ? $@"{source}{currentDir.Replace($@"{pathDest}\{mediaBackupPath}", string.Empty)}"
                        : $"{driveSource}{currentDir.Replace($@"{pathDest}\", string.Empty)}";

                    var directorySource = new DirectoryInfo(pathSource);
                    var directoryDest = new DirectoryInfo(currentDir);

                    if (!directorySource.Exists)
                    {
                        directoryDest.Delete(true);
                        sb.Append($"Obsolete folder '{directoryDest.Name}' was deleted{Environment.NewLine}");
                    }
                    else
                    {
                        foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]))
                            dirs.Push(dir);
                    }
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- RemoveObsoleteFolders: {e.Message}{Environment.NewLine}");
            }

            return sb.ToString();
        }

        private static string RemoveObsoleteFiles(string source, string destination, string mediaBackupPath, string sqlFileStreamName, string[] excludeFolders = null)
        {
            var sb = new StringBuilder();

            try
            {
                // Data structure to hold names of subfolders to be examined for files
                var dirs = new Stack<string>(20);
                var driveSource = source.Contains(sqlFileStreamName)
                    ? source
                    : Path.GetPathRoot(source);

                var pathDest = source.Contains(sqlFileStreamName)
                    ? Path.Combine(destination, mediaBackupPath)
                    : Path.Combine(destination, source.Replace(driveSource, string.Empty));

                if (!Directory.Exists(pathDest))
                {
                    throw new ArgumentException();
                }

                dirs.Push(pathDest);

                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        sb.Append($"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        sb.Append($"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(currentDir)
                            .Where(x => !x.Contains("SeedResidents"))
                            .Where(x => !x.Contains("SeedConfigurations"))
                            .ToArray();
                    }

                    catch (UnauthorizedAccessException e)
                    {
                        sb.Append($"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    catch (DirectoryNotFoundException e)
                    {
                        sb.Append($"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}");
                        continue;
                    }

                    // delete obsolete files
                    foreach (var file in files)
                    {
                        try
                        {
                            var fiDest = new FileInfo(file);
                            if (fiDest.DirectoryName == null) continue;

                            // remove trainling slash if it exists
                            if (destination.EndsWith(@"\"))
                            {
                                var indexOfSlash = destination.LastIndexOf(@"\", StringComparison.Ordinal);
                                pathDest = pathDest.Substring(0, indexOfSlash);
                            }
                            else
                            {
                                pathDest = destination;
                            }

                            var subPathDest = source.Contains(sqlFileStreamName)
                                ? fiDest.DirectoryName.Replace($@"{pathDest}\{mediaBackupPath}\", string.Empty)
                                : fiDest.DirectoryName.Replace($@"{pathDest}\", string.Empty);

                            var pathSource = source.Contains(sqlFileStreamName)
                                ? Path.Combine(source, subPathDest)
                                : $"{driveSource}{subPathDest}";

                            var sourceFilePath = Path.Combine(pathSource, fiDest.Name);

                            if (File.Exists(sourceFilePath)) continue;

                            File.Delete(file);
                            sb.Append($"Obsolete file '{file}' was deleted{Environment.NewLine}");
                        }
                        catch (FileNotFoundException e)
                        {
                            sb.Append($"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}");
                        }
                    }

                    foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]).ToArray())
                        dirs.Push(dir);
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}");
            }

            return sb.ToString();
        }

        private static string CreateScriptSeedResidents(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var opsClient = new OperationsClient();
                var residents = opsClient.GetResidents().ToArray();

                if (!residents.Any())
                    return $"--- WARNING --- CreateScriptSeedResidents: No residents found{Environment.NewLine}";

                var pathScript = $@"{path}\Install\Database\SQL Server\SeedResidents.sql";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("DECLARE @pathProfile varchar(max)");
                    sw.WriteLine("SET @pathProfile = FileTableRootPath() + " + @"'\Media\Profiles\'");
                    sw.WriteLine();

                    // insert residents
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[Residents] ON");
                    foreach (var r in residents)
                    {
                        var allowVideoCapturing = r.AllowVideoCapturing ? 1 : 0;

                        sw.WriteLine(
                            "INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) " +
                            $"VALUES({r.Id}, '{r.FirstName.Replace("'", "''")}', '{r.LastName.Replace("'", "''")}', '{r.Gender}', {r.GameDifficultyLevel}, {allowVideoCapturing}, GETDATE(), GETDATE())");
                    }
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[Residents] OFF");

                    // insert resident media
                    foreach (var r in residents)
                    {
                        sw.WriteLine();
                        sw.WriteLine($"--- ResidentId {r.Id} ---");
                        sw.WriteLine("--- Activity 1 - ResponseType 'SlideShow' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");

                        sw.WriteLine();
                        sw.WriteLine("--- Activity 2 - ResponseType 'MatchingGame' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\activities\matching-game\shapes\' AND [FileType] = 'png'");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\activities\matching-game\sounds\' AND [FileType] = 'mp3'");

                        sw.WriteLine();
                        sw.WriteLine("--- Activity 5 - ResponseType 'Radio' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\audio\music\' AND [FileType] = 'mp3'");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\audio\radio-shows\' AND [FileType] = 'mp3'");

                        sw.WriteLine();
                        sw.WriteLine("--- Activity 5 - ResponseType 'Television' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\videos\tv-shows\' AND [FileType] = 'mp4'");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                            $@"'{r.Id}\videos\home-movies\' AND [FileType] = 'mp4'");
                        sw.WriteLine();
                    }
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- CreateScriptSeedResidents: {e.Message}{Environment.NewLine}");
            }

            sb.Append(CreatePowershellScript(path, "SeedResidents", "Residents"));
            sb.Append(CreateBatchFile(path, "9a_SeedResidents", "SeedResidents"));

            return sb.ToString();
        }

        private static string CreateScriptSeedConfigurations(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var opsClient = new OperationsClient();
                var configs = opsClient.GetConfigs().ToArray();

                if (!configs.Any())
                    return $"--- WARNING --- CreateScriptSeedConfigurations: No configurations found{Environment.NewLine}";

                var pathScript = $@"{path}\Install\Database\SQL Server\SeedConfigurations.sql";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("-- remove all exisiting confgis");
                    sw.WriteLine("DELETE FROM [dbo].[ConfigDetails]");
                    sw.WriteLine("GO");
                    sw.WriteLine("DELETE FROM [dbo].[Configs]");
                    sw.WriteLine("GO");

                    // interate through configs
                    foreach (var c in configs)
                    {
                        var isActive = c.IsActive ? 1 : 0;
                        var isActiveEventLog = c.IsActiveEventLog ? 1 : 0;

                        // insert config
                        sw.WriteLine("SET IDENTITY_INSERT [dbo].[Configs] ON");
                        sw.WriteLine(
                            "INSERT [dbo].[Configs] ([Id], [Description], [IsActive], [IsActiveEventLog]) " +
                            $"VALUES({c.Id}, '{c.Description.Replace("'", "''")}', {isActive}, {isActiveEventLog})");
                        sw.WriteLine("SET IDENTITY_INSERT [dbo].[Configs] OFF");

                        // insert config details
                        var configDetails = opsClient.GetConfigWithDetails(c.Id).ConfigDetails;
                        sw.WriteLine();
                        sw.WriteLine("SET IDENTITY_INSERT [dbo].[ConfigDetails] ON");
                        foreach (var cd in configDetails)
                        {
                            sw.WriteLine(
                                "INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) " +
                                $"VALUES ({cd.Id}, {c.Id}, {cd.PhidgetType.Id}, {cd.PhidgetStyleType.Id}, {cd.ResponseType.Id}, '{cd.Description.Replace("'", "''")}')");
                        }
                        sw.WriteLine("SET IDENTITY_INSERT [dbo].[ConfigDetails] OFF");
                        sw.WriteLine();
                    }
                    sw.WriteLine();
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- CreateScriptSeedResidents: {e.Message}{Environment.NewLine}");
            }

            sb.Append(CreatePowershellScript(path, "SeedConfigurations", "Configurations"));
            sb.Append(CreateBatchFile(path, "9b_SeedConfigurations", "SeedConfigurations"));

            return sb.ToString();
        }

        private static string CreatePowershellScript(string path, string filename, string description)
        {
            var sb = new StringBuilder();

            try
            {
                var pathPowerShell = $@"{path}\Install\Database\PowerShell\{filename}.ps1";

                if (File.Exists(pathPowerShell))
                    File.Delete(pathPowerShell);

                using (var sw = new StreamWriter(pathPowerShell))
                {
                    sw.WriteLine("$server = $env:COMPUTERNAME + " + "\"" + @"\SQLEXPRESS" + "\"");
                    sw.WriteLine("$database = " + "\"" + "KeebeeAAT" + "\"");
                    sw.WriteLine("$path = " + "\"" + @"C:\Deployments\Install\Database\SQL Server\" + "\"");
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine("# check if the database exists");
                    sw.WriteLine("$query = Invoke-SqlQuery -Query " +
                                 "\"SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" +
                                 "\"" + " -Server $server -Database " + "\"master" + "\"");
                    sw.WriteLine("$databaseCount = $query.DatabaseCount");
                    sw.WriteLine();
                    sw.WriteLine("# if the database doesn't exist, don't attempt anything");
                    sw.WriteLine("if ($databaseCount -eq 0) {");
                    sw.WriteLine("    Write-Host -ForegroundColor yellow " + "\"" + "`nR2G2 database does not exist." +
                                 "`n" + "\"");
                    sw.WriteLine("}");
                    sw.WriteLine("else");
                    sw.WriteLine("{");
                    sw.WriteLine("    Try");
                    sw.WriteLine("    {");
                    sw.WriteLine("        Write-Host " + "\"" + $"Seeding {description.ToLower()}..." + "\"" +
                                 "-NoNewline");
                    sw.WriteLine("        $queryFile = $path + " + $"\"{filename}.sql" + "\"");
                    sw.WriteLine("        Invoke-SqlQuery -File $queryFile -Server $server -Database $database");
                    sw.WriteLine("        Write-Host " + "\"done.`n" + "\"");
                    sw.WriteLine();
                    sw.WriteLine("        Write-Host " + "\"" + $"{description} seeded successfully!`n" + "\"");
                    sw.WriteLine("    }");
                    sw.WriteLine("    Catch");
                    sw.WriteLine("    {");
                    sw.WriteLine("        Write-Host -ForegroundColor red $_.Exception.Message");
                    sw.WriteLine("    }");
                    sw.Write("}");
                }
            }
            catch (Exception e)
            {
                sb.Append($"--- ERROR --- CreatePowershellScript: {e.Message}{Environment.NewLine}");
            }

            return sb.ToString();
        }

        private static string CreateBatchFile(string path, string batchName, string powershellName)
        {
            var sb = new StringBuilder();

            try
            {
                var pathBatch = $@"{path}\Install\{batchName}.bat";

                if (File.Exists(pathBatch))
                    File.Delete(pathBatch);

                using (var sw = new StreamWriter(pathBatch))
                {
                    sw.WriteLine("@ECHO OFF");
                    sw.WriteLine("PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command " + "\"& " +
                                 $@"'C:\Deployments\Install\Database\PowerShell\{powershellName}.ps1'" + "\"");
                    sw.Write("pause");
                }
            }

            catch (Exception e)
            {
                sb.Append($"--- ERROR --- CreateBatchFile: {e.Message}{Environment.NewLine}");
            }

            return sb.ToString();
        }
    }
}