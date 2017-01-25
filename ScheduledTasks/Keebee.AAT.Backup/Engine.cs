using Keebee.AAT.RESTClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Keebee.AAT.Backup
{
    public class Engine
    {
        private const string SqlFilestreamName = "KeebeeAATFilestream";
        private const string RestoreResidentsFilename = "RestoreResidents";
        private const string RestoreConfigurationsFilename = "RestoreConfigurations";
        private const string InstallR2G2Filename = "INSTALL_R2G2.ps1";

        private readonly string _pathDeployments;
        private readonly string _pathVideoCaptures;
        private readonly string _pathBackup;

        private readonly string _pathSqlMedia;
        private readonly string _deploymentsFolder;
        private readonly string _mediaBackupPath;

        private bool _residentsExist;

        public Engine()
        {
            _pathDeployments = ConfigurationManager.AppSettings["DeploymentsPath"];
            _pathVideoCaptures = ConfigurationManager.AppSettings["VideoCapturesPath"];
            _pathBackup = ConfigurationManager.AppSettings["BackupPath"];

            var computerName = Environment.MachineName;
            _pathSqlMedia = $@"\\{computerName}\sqlexpress\{SqlFilestreamName}\Media";

            _deploymentsFolder = _pathDeployments.Replace(Path.GetPathRoot(_pathDeployments), string.Empty);
            _mediaBackupPath = $@"{_deploymentsFolder}\Media";
        }

        public void DoBackup()
        {
            try
            {
                var logText = new StringBuilder();
                var pathLogs = $@"{_pathBackup}\BackupLogs";

                var logFilename = InitializeLogFile(pathLogs);
#if DEBUG
                Console.WriteLine($"{Environment.NewLine}Examining files...");
#endif
                using (var w = File.AppendText(logFilename))
                {
                    var diBackup = new DirectoryInfo(_pathBackup);
                    if (!diBackup.Exists) return;

                    var rootBackup = Path.GetPathRoot(_pathBackup);
                    var driveBackup = new DriveInfo(rootBackup);

                    if (driveBackup.IsReady)
                    {
                        // backup deployment folders (except media)
                        logText.Append(BackupFiles(_pathDeployments, _pathBackup, 
                            excludeFolders: new[] { Path.Combine(_pathDeployments, "Media") }));

                        // backup the media
                        logText.Append(BackupFiles(_pathSqlMedia, _pathBackup));

                        // backup video captures
                        logText.Append(BackupFiles(_pathVideoCaptures, _pathBackup));

                        // delete obsolete deployment folders (except media)
                        logText.Append(RemoveObsoleteFolders(_pathDeployments, _pathBackup, 
                            excludeFolders: new[] { Path.Combine(_pathBackup, _mediaBackupPath) }));

                        // delete obsolete media folders
                        logText.Append(RemoveObsoleteFolders(_pathSqlMedia, _pathBackup));

                        // delete obsolete video capture folders
                        logText.Append(RemoveObsoleteFolders(_pathVideoCaptures, _pathBackup));

                        // delete obsolete deployment files (except media)
                        logText.Append(RemoveObsoleteFiles(_pathDeployments, _pathBackup,
                                excludeFolders: new[] { Path.Combine(_pathBackup, _mediaBackupPath) }));

                        // delete obsolete media files
                        logText.Append(RemoveObsoleteFiles(_pathSqlMedia, _pathBackup));

                        // delete obsolete video capture files
                        logText.Append(RemoveObsoleteFiles(_pathVideoCaptures, _pathBackup));

                        // create the database scripts
                        logText.Append(CreateScriptRestoreResidents($@"{_pathBackup}\{_deploymentsFolder}"));
                        logText.Append(CreateScriptRestoreConfigurations($@"{_pathBackup}\{_deploymentsFolder}"));
                        logText.Append(CreateInstallPowerShellScript($@"{_pathBackup}\{_deploymentsFolder}"));
                    }
                    else
                    {
                        logText.Append($"--- ERROR --- Main: Backup drive {driveBackup.Name} is not accessible.");
                    }

                    const string noChangesMessage = "No actions taken - all files and folders are up to date";
                    w.WriteLine("--------------------------------------------");
                    w.WriteLine($"Backup Summary for {DateTime.Now.ToLongDateString()}");
                    w.WriteLine("--------------------------------------------");
                    w.Write(logText.Length <= 0
                        ? noChangesMessage
                        : logText.ToString());
#if DEBUG
                    Console.WriteLine($"{Environment.NewLine}Backup Completed{Environment.NewLine}");
                    if (logText.Length <= 0)
                        Console.WriteLine(noChangesMessage);
                    Console.WriteLine($"{Environment.NewLine}Press any key to exit...");
                    Console.ReadKey();
#endif
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.Message);
#endif
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

            var message = $"Log file: {filePath}{Environment.NewLine}";
#if DEBUG
            Console.Write(message);
#endif
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
                    var message = $"--- ERROR --- CreateBackupFolders: Source drive for {source} is not available{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
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
                        var message = $"--- ERROR --- CreateBackupFolders: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        var message = $"--- ERROR --- CreateBackupFolders: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
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
                        var message = $"Folder '{directory.FullName}' was created{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                    }

                    foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]))
                        dirs.Push(dir);
                }
            }
            catch (Exception e)
            {
                var message = $"--- ERROR --- CreateBackupFolders: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            return sb.ToString();
        }

        private string BackupFiles(string source, string destination, string[] excludeFolders = null)
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
                    var message = $"--- ERROR --- BackupFiles: Source drive for {source} is not ready{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
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
                        var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }

                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(currentDir);
                    }

                    catch (UnauthorizedAccessException e)
                    {
                        var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }

                    catch (DirectoryNotFoundException e)
                    {
                        var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
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

                            var pathDest = pathSource.Contains(SqlFilestreamName)
                                ? Path.Combine(destination,
                                    $@"{_deploymentsFolder}\{pathSource.Replace($@"\{SqlFilestreamName}\", string.Empty)}")
                                : Path.Combine(destination, pathSource);

                            if (!Directory.Exists(pathDest))
                                Directory.CreateDirectory(pathDest);

                            var destFilePath = Path.Combine(pathDest, fiSource.Name);

                            if (fiSource.Name == InstallR2G2Filename)
                                continue;

                            if (File.Exists(destFilePath))
                            {
                                if (IsFileIdentical(fiSource.FullName, destFilePath))
                                    continue;
                            }

                            File.Copy(fiSource.FullName, Path.Combine(pathDest, fiSource.Name), true);
                            var message = $"File '{fiSource.Name}' was copied to '{pathDest}'{Environment.NewLine}";
#if DEBUG
                            Console.Write(message);
#endif
                            sb.Append(message);
                        }
                        catch (FileNotFoundException e)
                        {
                            var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                            Console.Write(message);
#endif
                            sb.Append(message);
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
                var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }
            return sb.ToString();
        }

        private static bool IsFileIdentical(string file1, string file2)
        {
            return new FileInfo(file1).Length == new FileInfo(file2).Length
                && (File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2)));
        }

        private string RemoveObsoleteFolders(string source, string destination, string[] excludeFolders = null)
        {
            var sb = new StringBuilder();

            try
            {
                // Data structure to hold names of subfolders
                var dirs = new Stack<string>(20);

                var driveSource = source.Contains(SqlFilestreamName)
                    ? source
                    : Path.GetPathRoot(source);

                var driveDest = Path.GetPathRoot(destination);

                if (driveDest == null)
                {
                    var message = $"--- ERROR --- RemoveObsoleteFolders: Destination drive for {destination} is not available";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
                }

                var pathDestination = source.Contains(SqlFilestreamName)
                    ? Path.Combine(destination, _mediaBackupPath)
                    : Path.Combine(destination, source.Replace(driveSource, string.Empty));

                if (!Directory.Exists(pathDestination))
                {
                    var message = $"--- ERROR --- RemoveObsoleteFolders: {pathDestination} does not exist{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
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
                        var message = $"--- ERROR --- RemoveObsoleteFolders: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        var message = $"--- ERROR --- RemoveObsoleteFolders: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
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

                    var pathSource = source.Contains(SqlFilestreamName)
                        ? $@"{source}{currentDir.Replace($@"{pathDest}\{_mediaBackupPath}", string.Empty)}"
                        : $"{driveSource}{currentDir.Replace($@"{pathDest}\", string.Empty)}";

                    var directorySource = new DirectoryInfo(pathSource);
                    var directoryDest = new DirectoryInfo(currentDir);

                    if (!directorySource.Exists)
                    {
                        directoryDest.Delete(true);
                        var message = $"Obsolete folder '{directoryDest.FullName}' was deleted{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
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
                var message = $"--- ERROR --- RemoveObsoleteFolders: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            return sb.ToString();
        }

        private string RemoveObsoleteFiles(string source, string destination, string[] excludeFolders = null)
        {
            var sb = new StringBuilder();

            try
            {
                // Data structure to hold names of subfolders to be examined for files
                var dirs = new Stack<string>(20);
                var driveSource = source.Contains(SqlFilestreamName)
                    ? source
                    : Path.GetPathRoot(source);

                var pathDest = source.Contains(SqlFilestreamName)
                    ? Path.Combine(destination, _mediaBackupPath)
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
                        var message = $"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        var message = $"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }

                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(currentDir)
                            .Where(x => !x.Contains(RestoreResidentsFilename))
                            .Where(x => !x.Contains(RestoreConfigurationsFilename))
                            .ToArray();
                    }

                    catch (UnauthorizedAccessException e)
                    {
                        var message = $"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                        continue;
                    }

                    catch (DirectoryNotFoundException e)
                    {
                        var message = $"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
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

                            var subPathDest = source.Contains(SqlFilestreamName)
                                ? fiDest.DirectoryName.Replace($@"{pathDest}\{_mediaBackupPath}\", string.Empty)
                                : fiDest.DirectoryName.Replace($@"{pathDest}\", string.Empty);

                            var pathSource = source.Contains(SqlFilestreamName)
                                ? Path.Combine(source, subPathDest)
                                : $"{driveSource}{subPathDest}";

                            var sourceFilePath = Path.Combine(pathSource, fiDest.Name);

                            if (File.Exists(sourceFilePath)) continue;

                            File.Delete(file);
                            var message = $"Obsolete file '{file}' was deleted{Environment.NewLine}";
#if DEBUG
                            Console.Write(message);
#endif
                            sb.Append(message);
                        }
                        catch (FileNotFoundException e)
                        {
                            var message = $"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                            Console.Write(message);
#endif
                            sb.Append(message);
                        }
                    }

                    foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]).ToArray())
                        dirs.Push(dir);
                }
            }
            catch (Exception e)
            {
                var message = $"--- ERROR --- RemoveObsoleteFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            return sb.ToString();
        }

        private string CreateScriptRestoreResidents(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var opsClient = new OperationsClient();
                var residents = opsClient.GetResidents().ToArray();

                if (!residents.Any())
                {
                    var message = $"--- WARNING --- CreateScriptRestoreResidents: No residents found{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    _residentsExist = false;
                    return message;
                }

                _residentsExist = true;
                var pathScript = $@"{path}\Install\Database\SQL Server\RestoreResidents.sql";

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
                var message = $"--- ERROR --- CreateScriptRestoreResidents: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            sb.Append(CreatePowerShellScript(path, "RestoreResidents", "Residents"));

            return sb.ToString();
        }

        private static string CreateScriptRestoreConfigurations(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var opsClient = new OperationsClient();
                var configs = opsClient.GetConfigs().ToArray();

                if (!configs.Any())
                {
                    var message = $"--- WARNING --- CreateScriptRestoreConfigurations: No configurations found{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
                }

                var pathScript = $@"{path}\Install\Database\SQL Server\RestoreConfigurations.sql";

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
                var message = $"--- ERROR --- CreateScriptRestoreConfigurations: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            sb.Append(CreatePowerShellScript(path, "RestoreConfigurations", "Configurations"));

            return sb.ToString();
        }

        private static string CreatePowerShellScript(string path, string filename, string description)
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
                    sw.WriteLine("Try");
                    sw.WriteLine("{");
                    sw.WriteLine("    # check if the database exists");
                    sw.WriteLine("    $query = Invoke-SqlQuery -Query " +
                                      "\"SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" +
                                      "\"" + " -Server $server -Database " + "\"master" + "\"");
                    sw.WriteLine("    $databaseCount = $query.DatabaseCount");
                    sw.WriteLine();
                    sw.WriteLine("    # if the database doesn't exist, don't attempt anything");
                    sw.WriteLine("    if ($databaseCount -eq 0) {");
                    sw.WriteLine("        Write-Host -ForegroundColor yellow " + "\"" + "`nR2G2 database does not exist." + "`n" + "\"");
                    sw.WriteLine("    }");
                    sw.WriteLine("    else");
                    sw.WriteLine("    {");
                    sw.WriteLine("        Write-Host " + "\"" + $"Restoring {description.ToLower()}..." + "\"" + "-NoNewline");
                    sw.WriteLine("        $queryFile = $path + " + $"\"{filename}.sql" + "\"");
                    sw.WriteLine("        Invoke-SqlQuery -File $queryFile -Server $server -Database $database");
                    sw.WriteLine("        Write-Host " + "\"done." + "\"");
                    sw.WriteLine("    }");
                    sw.WriteLine("}");
                    sw.WriteLine("Catch");
                    sw.WriteLine("{");
                    sw.WriteLine("    throw $_.Exception.Message");
                    sw.Write("}");
                }
            }
            catch (Exception e)
            {
                var message = $"--- ERROR --- CreatePowerShellScript: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            return sb.ToString();
        }

        private string CreateInstallPowerShellScript(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var pathScript = $@"{path}\Install\PowerShell\INSTALL_R2G2.ps1";
                const string pathPowerShell = @"C:\Deployments\Install\PowerShell";
                const string pathPowerShellData = @"C:\Deployments\Install\Database\PowerShell";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("Try");
                    sw.WriteLine("{");
                    sw.WriteLine("    Write-Host -ForegroundColor green " + "\"" + "`nInstalling R2G2...`n" + "\"");
                    sw.WriteLine();
                    sw.WriteLine("    $installPath = " + "\"" + $@"{pathPowerShell}" + "\"");
                    sw.WriteLine("    $installPathData = " + "\"" + $@"{pathPowerShellData}" + "\"");
                    sw.WriteLine();
                    sw.WriteLine(@"    invoke-expression -Command $installPath\CreateEventLogSources.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\CreateMessageQueues.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\CreateScheduledTasks.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\CreateLocalWebuser.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\CreateWebApplications.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPathData\CreateDatabase.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPathData\DropAndCreateTables.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPathData\SeedData.ps1");
                    if (_residentsExist)
                        sw.WriteLine(@"    invoke-expression -Command $installPathData\RestoreResidents.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPathData\RestoreConfigurations.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\InstallServices.ps1");
                    sw.WriteLine();
                    sw.WriteLine("    Write-Host -ForegroundColor green " + "\"" + "`nR2G2 successfully installed.`n" + "\"");

                    sw.WriteLine("}");
                    sw.WriteLine("Catch");
                    sw.WriteLine("{");
                    sw.WriteLine("    Write-Host -ForegroundColor red $_.Exception.Message");
                    sw.WriteLine("    Write-Host -ForegroundColor yellow " + "\"" + "`nInstallation aborted.`n" + "\"");
                    sw.Write("}");
                }
            }

            catch (Exception e)
            {
                var message = $"--- ERROR --- CreateInstallPowerShellScript: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                sb.Append(message);
            }

            return sb.ToString();
        }
    }
}
