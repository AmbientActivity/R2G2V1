using Keebee.AAT.RESTClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Keebee.AAT.Backup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pathSource = ConfigurationManager.AppSettings["SourcePath"];
            var pathBackup = ConfigurationManager.AppSettings["BackupPath"];

            var computerName = Environment.MachineName;
            var mediaSourcePath = $@"\\{computerName}\sqlexpress\KeebeeAATFilestream\Media";
            const string videoCaptureSourcePath = @"C:\VideoCaptures";

            var diBackup = new DirectoryInfo(pathBackup);
            if (!diBackup.Exists) return;

            var drive = Path.GetPathRoot(pathBackup);
            var di = new DriveInfo(drive);

            if (di.IsReady)
            {
                // backup deployment folders minus the media
                BackupFiles(pathSource, pathBackup, excludeFolders: new [] { Path.Combine(pathSource, "Media") });

                // back up the media
                BackupFiles(mediaSourcePath, pathBackup);

                // back up the video captures
                BackupFiles(videoCaptureSourcePath, pathBackup);

                // create the database scripts
                CreateScriptSeedResidents(pathBackup);
                CreateScriptSeedConfigs(pathBackup);
            }
            else
            {
                //TODO: log error
            }
        }

        public static void BackupFiles(string source, string destination, string[] excludeFolders = null)
        {
            // Data structure to hold names of subfolders to be examined for files
            var dirs = new Stack<string>(20);
            var driveSource = Path.GetPathRoot(source);
            if (driveSource == null)
            {
                //TODO: log issue
                return;
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
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    //TODO: log issue
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    //TODO: log issue
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

                        var pathDest = pathSource.Contains("KeebeeAATFilestream")
                            ? Path.Combine(destination, $@"Deployments\{pathSource.Replace(@"\KeebeeAATFilestream\", string.Empty)}")
                            : Path.Combine(destination, pathSource);

                        if (!Directory.Exists(pathDest))
                        {
                            Directory.CreateDirectory(pathDest);
                        }

                        var destFilePath = Path.Combine(pathDest, fiSource.Name);
                        if (File.Exists(destFilePath))
                        {
                            if (IsFileIdentical(fiSource.FullName, destFilePath))
                                continue;
                        }

                        File.Copy(fiSource.FullName, Path.Combine(pathDest, fiSource.Name), true);
                    }
                    catch (FileNotFoundException e)
                    {
                        //TODO: log issue
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (var dir in subDirs.Select(x => x).Except(excludeFolders ?? new string[0]))
                        dirs.Push(dir);
            }
        }

        private static bool IsFileIdentical(string file1, string file2)
        {
            return new FileInfo(file1).Length == new FileInfo(file2).Length 
                && (File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2)));
        }

        private static void CreateScriptSeedConfigs(string pathBackup)
        {
            var opsClient = new OperationsClient();
            var configs = opsClient.GetConfigs().ToArray();

            if (!configs.Any()) return;

            var pathScript = $@"{pathBackup}\Deployments\Install\Database\SQL Server\SeedConfigurations.sql";

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
                        $"VALUES({c.Id}, '{c.Description.Replace("'","''")}', {isActive}, {isActiveEventLog})");
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[Configs] OFF");

                    // insert config details
                    var configDetails = opsClient.GetConfigWithDetails(c.Id).ConfigDetails;
                    sw.WriteLine();
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[ConfigDetails] ON");
                    foreach (var cd in configDetails)
                    {
                        sw.WriteLine(
                            "INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) " +
                            $"VALUES ({cd.Id}, {c.Id}, {cd.PhidgetType.Id}, {cd.PhidgetStyleType.Id}, {cd.ResponseType.Id}, '{cd.Description}')");
                    }
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[ConfigDetails] OFF");
                    sw.WriteLine();
                }
                sw.WriteLine();
            }

            var pathPowerShell = $@"{pathBackup}\Deployments\Install\Database\PowerShell\SeedConfigurations.ps1";

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
                sw.WriteLine("$query = Invoke-SqlQuery -Query " + "\"SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" + "\"" + " -Server $server -Database " + "\"master" + "\"");
                sw.WriteLine("$databaseCount = $query.DatabaseCount");
                sw.WriteLine();
                sw.WriteLine("# if the database doesn't exist, don't attempt anything");
                sw.WriteLine("if ($databaseCount -eq 0) {");
                sw.WriteLine("    Write-Host -ForegroundColor yellow " + "\"" + "`nR2G2 database does not exist." + "`n" + "\"");
                sw.WriteLine("}");
                sw.WriteLine("else");
                sw.WriteLine("{");
                sw.WriteLine("    Try");
                sw.WriteLine("    {");
                sw.WriteLine("        Write-Host " + "\"" + "Seeding configurations..." + "\"" + "-NoNewline");
                sw.WriteLine("        $queryFile = $path + " + "\"SeedConfigurations.sql" + "\"");
                sw.WriteLine("        Invoke-SqlQuery -File $queryFile -Server $server -Database $database");
                sw.WriteLine("        Write-Host " + "\"done.`n" + "\"");
                sw.WriteLine();
                sw.WriteLine("        Write-Host " + "\"" + "Configurations seeded successfully!`n" + "\"");
                sw.WriteLine("    }");
                sw.WriteLine("    Catch");
                sw.WriteLine("    {");
                sw.WriteLine("        Write-Host -ForegroundColor red $_.Exception.Message");
                sw.WriteLine("    }");
                sw.Write("}");
            }

            var pathBatch = $@"{pathBackup}\Deployments\Install\Utility\SeedConfigurations.bat";

            if (File.Exists(pathBatch))
                File.Delete(pathBatch);

            using (var sw = new StreamWriter(pathBatch))
            {
                sw.WriteLine("@ECHO OFF");
                sw.WriteLine("PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command " + "\"& " + @"'C:\Deployments\Install\Database\PowerShell\SeedConfigurations.ps1'" + "\"");
                sw.Write("pause");
            }
        }

        private static void CreateScriptSeedResidents(string pathBackup)
        {
            var opsClient = new OperationsClient();
            var residents = opsClient.GetResidents().ToArray();

            if (!residents.Any()) return;

            var pathScript = $@"{pathBackup}\Deployments\Install\Database\SQL Server\SeedResidents.sql";

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
                    sw.WriteLine($"SELECT 0, {r.Id}, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");

                    sw.WriteLine();
                    sw.WriteLine("--- Activity 2 - ResponseType 'MatchingGame' ---");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\activities\matching-game\shapes\' AND [FileType] = 'png'");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\activities\matching-game\sounds\' AND [FileType] = 'mp3'");

                    sw.WriteLine();
                    sw.WriteLine("--- Activity 5 - ResponseType 'Radio' ---");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\audio\music\' AND [FileType] = 'mp3'");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\audio\radio-shows\' AND [FileType] = 'mp3'");

                    sw.WriteLine();
                    sw.WriteLine("--- Activity 5 - ResponseType 'Television' ---");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\videos\tv-shows\' AND [FileType] = 'mp4'");
                    sw.WriteLine(
                        "INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine($"SELECT 0, {r.Id}, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + " +
                                 $@"'{r.Id}\videos\home-movies\' AND [FileType] = 'mp4'");
                    sw.WriteLine();
                }
            }

            var pathPowerShell = $@"{pathBackup}\Deployments\Install\Database\PowerShell\SeedResidents.ps1";

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
                sw.WriteLine("$query = Invoke-SqlQuery -Query " + "\"SELECT COUNT(*) AS DatabaseCount FROM master.sys.databases WHERE name = N'$database'" + "\"" + " -Server $server -Database " + "\"master" + "\"");
                sw.WriteLine("$databaseCount = $query.DatabaseCount");
                sw.WriteLine();
                sw.WriteLine("# if the database doesn't exist, don't attempt anything");
                sw.WriteLine("if ($databaseCount -eq 0) {");
                sw.WriteLine("    Write-Host -ForegroundColor yellow " + "\"" + "`nR2G2 database does not exist." + "`n" + "\"");
                sw.WriteLine("}");
                sw.WriteLine("else");
                sw.WriteLine("{");
                sw.WriteLine("    Try");
                sw.WriteLine("    {");
                sw.WriteLine("        Write-Host " + "\"" + "Seeding residents..." + "\"" + "-NoNewline");
                sw.WriteLine("        $queryFile = $path + " + "\"SeedResidents.sql" + "\"");
                sw.WriteLine("        Invoke-SqlQuery -File $queryFile -Server $server -Database $database");
                sw.WriteLine("        Write-Host " + "\"done.`n" + "\"");
                sw.WriteLine();
                sw.WriteLine("        Write-Host " + "\"" + "Residents seeded successfully!`n" + "\"");
                sw.WriteLine("    }");
                sw.WriteLine("    Catch");
                sw.WriteLine("    {");
                sw.WriteLine("        Write-Host -ForegroundColor red $_.Exception.Message");
                sw.WriteLine("    }");
                sw.Write("}");
            }

            var pathBatch = $@"{pathBackup}\Deployments\Install\Utility\SeedResidents.bat";

            if (File.Exists(pathBatch))
                File.Delete(pathBatch);

            using (var sw = new StreamWriter(pathBatch))
            {
                sw.WriteLine("@ECHO OFF");
                sw.WriteLine("PowerShell -NoProfile -ExecutionPolicy Unrestricted -Command " + "\"& " + @"'C:\Deployments\Install\Database\PowerShell\SeedResidents.ps1'" + "\"");
                sw.Write("pause");
            }
        }
    }
}