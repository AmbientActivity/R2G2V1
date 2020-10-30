using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Keebee.AAT.Backup
{
    public class Engine
    {
        private const string SqlFilestreamName = "KeebeeAATFilestream";
        private const string RestorePublicProfileFilename = "RestorePublicProfile";
        private const string RestoreResidentsFilename = "RestoreResidents";
        private const string RestoreConfigurationsFilename = "RestoreConfigurations";
        private const string InstallAbbyFilename = "INSTALL_ABBY.ps1";
        private const string ProfilesFolder = "Profiles";
        private const string MediaFolder = "Media";
        private const string BackupLogsFolder = "BackupLogs";
        private readonly string[] _applicationRoots = { "Install", "ScheduledTasks", "Services", "UI", "Web" };


        private readonly string _rootDeployments;
        private readonly string _rootVideoCaptures;
        private readonly string _rootBackup;

        private readonly string _deploymentsFolder;
        private readonly string _videoCapturesFolder;
        private readonly string _pathMediaBackup;

        private IEnumerable<MediaPathType> _mediaPathTypes;
        private BackupFileModel[] _mediaFiles;
        private bool _residentsExist;
        private int[] _residentIds;
        private readonly MediaSourcePath _mediaSourcePath;

        internal class BackupFileModel
        {
            public Guid StreamId { get; set; }
            public int ResidentId { get; set; }
            public int MediaPathTypeId { get; set; }
            public string MediaPath { get; set; }
            public string Filename { get; set; }
            public string FolderPath { get; set; }
            public string FullPath { get; set; }
            public bool IsLinked { get; set; }
        }

        public Engine()
        {
            _rootDeployments = ConfigurationManager.AppSettings["DeploymentsPath"];
            _rootVideoCaptures = ConfigurationManager.AppSettings["VideoCapturesPath"];
            _rootBackup = ConfigurationManager.AppSettings["BackupPath"];

            _deploymentsFolder = _rootDeployments.Replace(Path.GetPathRoot(_rootDeployments), string.Empty);
            _videoCapturesFolder = _rootVideoCaptures.Replace(Path.GetPathRoot(_rootVideoCaptures), string.Empty);
            _pathMediaBackup = $@"{_deploymentsFolder}\{MediaFolder}";

            _mediaSourcePath = new MediaSourcePath();
        }

        public void DoBackup()
        {
            try
            {
                var logText = new StringBuilder();
                var pathLogs = $@"{_rootBackup}\{BackupLogsFolder}";

                var logFilename = InitializeLogFile(pathLogs);
#if DEBUG
                Console.WriteLine($"{Environment.NewLine}Examining files...");
#endif
                IMediaPathTypesClient mediaPathTypesClient = new MediaPathTypesClient();
                _mediaPathTypes = mediaPathTypesClient.Get().ToArray();

                IResidentsClient residentsClient = new ResidentsClient();
                _residentIds = residentsClient.Get().Select(r => r.Id).ToArray();

                // retrieve and flatten media models
                var mediaModels = GetMediaModels();
                _mediaFiles = GetFlattenedMedia(mediaModels);

                using (var w = File.AppendText(logFilename))
                {
                    var diBackup = new DirectoryInfo(_rootBackup);
                    if (!diBackup.Exists) return;

                    var rootBackup = Path.GetPathRoot(_rootBackup);
                    var driveBackup = new DriveInfo(rootBackup);

                    if (driveBackup.IsReady)
                    {
                        // backup deployment folders (except media)
                        logText.Append(BackupApplicationFiles(_rootDeployments, _rootBackup,
                            excludeFolders: new[] { Path.Combine(_rootDeployments, MediaFolder) }));

                        // backup the media
                        logText.Append(BackupMediaFiles());

                        // backup video captures
                        logText.Append(BackupApplicationFiles(_rootVideoCaptures, _rootBackup));

                        // delete obsolete deployment folders (ignore media)
                        var deploymentFolders = GetDeploymentFolders();
                        var rootFolder = Path.Combine(_rootBackup, _deploymentsFolder);
                        var ignoreRoots = new [] { Path.Combine(_rootBackup, _pathMediaBackup) };
                        logText.Append(DeleteFolders(rootFolder, foldersToKeep: deploymentFolders, ignoreRoots: ignoreRoots));

                        // delete obsolete media folders
                        var mediaFolders = GetMediaFolders(mediaModels);
                        rootFolder = Path.Combine(_rootBackup, _pathMediaBackup);
                        logText.Append(DeleteFolders(rootFolder, foldersToKeep: mediaFolders));

                        // delete obsolete video capture folders
                        //var videoCaptureFolders = GetVideoCaptureFolders();
                        //rootFolder = Path.Combine(_rootBackup, _videoCapturesFolder);
                        //logText.Append(DeleteFolders(rootFolder, videoCaptureFolders));

                        
                        // delete obsolete deployment files (except media and video captures)
                        var deploymentModels = GetDeploymentFileModels();
                        var excludeFolders = new []
                        {
                            Path.Combine(_rootBackup, _pathMediaBackup),
                            Path.Combine(_rootBackup, _videoCapturesFolder),
                            Path.Combine(_rootBackup, BackupLogsFolder)
                        };
                        logText.Append(DeleteFiles(_rootBackup, deploymentModels, recursive: true, excludeFolders: excludeFolders));

                        // delete obsolete media files
                        rootFolder = Path.Combine(_rootBackup, _pathMediaBackup);
                        logText.Append(DeleteFiles(rootFolder, _mediaFiles, recursive: true));

                        // delete obsolete video capture files
                        //var videoCaptureFiles = GetVideoCaptureFileModels();
                        //rootFolder = Path.Combine(_rootBackup, _videoCapturesFolder);
                        //logText.Append(DeleteFiles(rootFolder, videoCaptureFiles, recursive: true));

                        // create the database scripts
                        logText.Append(CreateScriptRestorePublicProfile($@"{_rootBackup}\{_deploymentsFolder}"));
                        logText.Append(CreateScriptRestoreResidents($@"{_rootBackup}\{_deploymentsFolder}"));
                        logText.Append(CreateScriptRestoreConfigurations($@"{_rootBackup}\{_deploymentsFolder}"));
                        logText.Append(CreateInstallPowerShellScript($@"{_rootBackup}\{_deploymentsFolder}"));
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
                    w.Close();
#if DEBUG
                    Console.WriteLine($"{Environment.NewLine}Backup Completed{Environment.NewLine}");
                    if (logText.Length <= 0)
                        Console.WriteLine(noChangesMessage);
                    Console.WriteLine($"{Environment.NewLine}Press any key to exit...");
                    Console.ReadKey();
#endif
                }
            }
#if DEBUG
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
#elif !DEBUG
            catch {}
#endif
        }

        private static string InitializeLogFile(string pathLog)
        {
            var directory = new DirectoryInfo(pathLog);
            if (!directory.Exists)
                directory.Create();

            var dateString = DateTime.Now.ToString("yyyy-MM-dd");
            var filePath = Path.Combine(pathLog, $"BackupLog_{dateString.Replace("-", "_")}.log");
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
                    return string.Empty;
                }

                if (!Directory.Exists(destination))
                {
                    var message = $"--- ERROR --- CreateBackupFolders: Destination folder {destination} does not exist{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
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
                    var pathDest = subPathSource.IndexOf(SqlFilestreamName, StringComparison.Ordinal) >= 0
                        ? Path.Combine(destination,
                            $@"Deployments\{subPathSource.Replace($@"\{SqlFilestreamName}\", string.Empty)}")
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

        private string BackupApplicationFiles(string source, string destination, string[] excludeFolders = null)
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
                    var message = $"--- WARNING --- BackupFiles: Source folder {source} does not exist{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return string.Empty;
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

                            var pathDest = pathSource.IndexOf(SqlFilestreamName, StringComparison.Ordinal) >= 0
                                ? Path.Combine(destination,
                                    $@"{_deploymentsFolder}\{pathSource.Replace($@"\{SqlFilestreamName}\", string.Empty)}")
                                : Path.Combine(destination, pathSource);

                            if (!Directory.Exists(pathDest))
                                Directory.CreateDirectory(pathDest);

                            var destFilePath = Path.Combine(pathDest, fiSource.Name);

                            if (fiSource.Name == InstallAbbyFilename)
                                continue;

                            if (File.Exists(destFilePath))
                            {
                                if (destFilePath.ToLower().EndsWith(".mp3") || destFilePath.ToLower().EndsWith(".mp4"))
                                {
                                    if (IsFileIdenticalMp3Mp4(fiSource.FullName, destFilePath))
                                        continue;
                                }
                                else
                                {
                                    if (IsFileIdentical(fiSource.FullName, destFilePath))
                                        continue;
                                }
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

        private string BackupMediaFiles()
        {
            var sb = new StringBuilder();

            try
            {
                foreach (var file in _mediaFiles)
                {
                    var sourcePath = file.IsLinked
                        ? Path.Combine(_mediaSourcePath.MediaRoot, _mediaSourcePath.SharedLibrary, file.MediaPath)
                        : Path.Combine(_mediaSourcePath.ProfileRoot, file.ResidentId.ToString(), file.MediaPath);
                    var sourceFile = Path.Combine(sourcePath, file.Filename);
                    var folder = file.FullPath.Replace($@"\{file.Filename}", string.Empty);
                    if (!Directory.Exists(folder))
                    {
                        sb.Append(CreateFolder(folder));
                        var message = $"Folder '{folder}' was created{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                    }

                    if (!File.Exists(file.FullPath))
                    {
                        sb.Append(CopyFile(sourceFile, file.FullPath));
                        var message = $"File '{sourceFile}' was copied to '{file.FullPath}'{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        sb.Append(message);
                    }
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

        private static string CopyFile(string source, string destination)
        {
            var logText = new StringBuilder();

            var cmd = $"copy \"{source}\" \"{destination}\"";
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runas",
                Arguments = $@"/C {cmd}"
            };

            try
            {
                using (var exeProcess = Process.Start(startInfo))
                {
                    exeProcess?.WaitForExit();
                }
            }
            catch (Exception e)
            {
                var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                logText.Append(message);
            }

            return logText.ToString();
        }

        private static string CreateFolder(string folder)
        {
            var logText = new StringBuilder();

            var cmd = $"New-Item -ItemType Directory -Force -Path '{folder}'";

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $@"/C powershell /C {cmd}"
            };

            try
            {
                using (var exeProcess = Process.Start(startInfo))
                {
                    exeProcess?.WaitForExit();
                }
            }
            catch (Exception e)
            {
                var message = $"--- ERROR --- BackupFiles: {e.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                logText.Append(message);
            }

            return logText.ToString();
        }

        private Media[] GetMediaModels()
        {
            var folderSharedLibrary = _mediaSourcePath.SharedLibrary.ToLower();
            var source = _mediaSourcePath.MediaRoot.ToLower();
            var validPaths = new[]                {
                Path.Combine(source, folderSharedLibrary),
                Path.Combine(source.ToLower(), ProfilesFolder.ToLower())
            };

            IMediaFilesClient mediaFilesClient = new MediaFilesClient();
            return mediaFilesClient.Get()
                .Where(lm =>
                {
                    var index1 = lm.Path.ToLower().IndexOf(validPaths[0], StringComparison.Ordinal);
                    var index2 = lm.Path.ToLower().IndexOf(validPaths[1], StringComparison.Ordinal);
                    return index1 >= 0 || index2 >= 0;
                })
                .ToArray();
        }

        private BackupFileModel[] GetFlattenedMedia(IEnumerable<Media> mediaModels)
        {
            var root = _mediaSourcePath.MediaRoot.ToLower();

            return mediaModels
                .SelectMany(lm =>
                {
                    var path = lm.Path.ToLower();
                    return lm.Files
                        .Select(f =>
                        {
                            var mediaPathFullWithSlash = path.Replace(root, string.Empty);
                            var mediaPathFull = mediaPathFullWithSlash.Remove(mediaPathFullWithSlash.Length - 1);

                            var isProfilesFolder = path.IndexOf($@"\{ProfilesFolder.ToLower()}\", StringComparison.Ordinal) >= 0;
                            var pathParts = mediaPathFull.Split('\\').Skip(1).ToArray();

                            var residentId = isProfilesFolder
                                ? int.Parse(pathParts[1])
                                : -1;

                            var mediaPathRoot = isProfilesFolder
                                ? Path.Combine(pathParts[0], pathParts[1])
                                : pathParts[0];

                            var mediaPath = mediaPathFull.Replace($@"\{mediaPathRoot}\", string.Empty);

                            var mediaPathTypeId = -1;
                            if (_mediaPathTypes != null)
                            {
                                mediaPathTypeId = _mediaPathTypes.FirstOrDefault(pt => pt.Path == mediaPath)?.Id ?? -1;
                            }

                            var destPath = !isProfilesFolder
                                ? Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, mediaPath)
                                : Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, residentId.ToString(), mediaPath);
                            var destFile = Path.Combine(destPath, f.Filename);

                            return new BackupFileModel
                            {
                                StreamId = f.StreamId,
                                ResidentId = residentId,
                                MediaPathTypeId = mediaPathTypeId,
                                MediaPath = mediaPath,
                                Filename = f.Filename,
                                FolderPath = destPath,
                                FullPath = destFile,
                                IsLinked = !isProfilesFolder
                            };
                        });
                }).ToArray();
        }

        private string[] GetMediaFolders(IEnumerable<Media> mediaModels)
        {
            const string activitiesPath = "activities";
            var matchingGamePath = Path.Combine("activities", "matching-game");
            const string audioPath = "audio";
            const string imagesPath = "images";
            const string videosPath = "videos";

            var folders = new List<string>
            {
                Path.Combine(_rootBackup, _pathMediaBackup),
                Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary)
            };

            var testPath = Path.Combine(_rootDeployments, MediaFolder, _mediaSourcePath.SharedLibrary, activitiesPath);
            if (Directory.Exists(testPath))
                folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, activitiesPath));

            testPath = Path.Combine(_rootDeployments, MediaFolder, _mediaSourcePath.SharedLibrary, matchingGamePath);
            if (Directory.Exists(testPath))
                folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, matchingGamePath));

            testPath = Path.Combine(_rootDeployments, MediaFolder, _mediaSourcePath.SharedLibrary, audioPath);
            if (Directory.Exists(testPath))
                folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, audioPath));

            testPath = Path.Combine(_rootDeployments, MediaFolder, _mediaSourcePath.SharedLibrary, imagesPath);
            if (Directory.Exists(testPath))
                folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, imagesPath));

            testPath = Path.Combine(_rootDeployments, MediaFolder, _mediaSourcePath.SharedLibrary, videosPath);
            if (Directory.Exists(testPath))
                folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, videosPath));

            foreach (var pathType in _mediaPathTypes)
            {
                testPath = Path.Combine(_rootDeployments, MediaFolder, _mediaSourcePath.SharedLibrary, pathType.Path);
                if (Directory.Exists(testPath))
                    folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, _mediaSourcePath.SharedLibrary, pathType.Path));
            }

            folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder));

            var profileIds = new[] {PublicProfileSource.Id.ToString()}.Union(_residentIds.Select(id => id.ToString()));
            foreach (var id in profileIds)
            {
                folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id));

                testPath = Path.Combine(_rootDeployments, MediaFolder, ProfilesFolder, id, activitiesPath);
                if (Directory.Exists(testPath))
                    folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id, activitiesPath));

                testPath = Path.Combine(_rootDeployments, MediaFolder, ProfilesFolder, id, matchingGamePath);
                if (Directory.Exists(testPath))
                    folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id, matchingGamePath));

                testPath = Path.Combine(_rootDeployments, MediaFolder, ProfilesFolder, id, audioPath);
                if (Directory.Exists(testPath))
                    folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id, audioPath));

                testPath = Path.Combine(_rootDeployments, MediaFolder, ProfilesFolder, id, imagesPath);
                if (Directory.Exists(testPath))
                    folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id, imagesPath));

                testPath = Path.Combine(_rootDeployments, MediaFolder, ProfilesFolder, id, videosPath);
                if (Directory.Exists(testPath))
                    folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id, videosPath));

                foreach (var pathType in _mediaPathTypes)
                {
                    testPath = Path.Combine(_rootDeployments, MediaFolder, ProfilesFolder, id, pathType.Path);
                    if (Directory.Exists(testPath))
                        folders.Add(Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, id, pathType.Path));
                }
            }

            var mediaRoot = _mediaSourcePath.MediaRoot.ToLower();
            var profileFolders = mediaModels
                .Where(m =>
                {
                    var isProfilesFolder = m.Path.IndexOf($@"\{ProfilesFolder}\", StringComparison.Ordinal) >= 0;
                    return isProfilesFolder;
                })
                .Select(m => 
            {
                var mediaPathFullWithSlash = m.Path.ToLower().Replace(mediaRoot, string.Empty);
                var mediaPathFull = mediaPathFullWithSlash.Remove(mediaPathFullWithSlash.Length - 1);
                var pathParts = mediaPathFull.Split('\\').Skip(1).ToArray();
                var residentId = int.Parse(pathParts[1]);
                var mediaPathRoot = Path.Combine(pathParts[0], pathParts[1]);
                var mediaPath = mediaPathFull.Replace($@"\{mediaPathRoot}\", string.Empty);
                var destPath = Path.Combine(_rootBackup, _pathMediaBackup, ProfilesFolder, residentId.ToString(), mediaPath);

                return destPath;
            }).ToArray();

            folders.AddRange(profileFolders);
            return folders.OrderBy(f => f).ToArray();
        }

        private static bool IsFileIdentical(string file1, string file2)
        {
            return new FileInfo(file1).Length == new FileInfo(file2).Length
            && (File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2)));
        }

        private static bool IsFileIdenticalMp3Mp4(string file1, string file2)
        {
            return new FileInfo(file1).Length == new FileInfo(file2).Length;
            //&& (File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2))); // causes OutOfMemory exception for large files
        }

        private BackupFileModel[] GetDeploymentFileModels()
        {
            var appRoots = new Collection<string>();
            foreach (var appRoot in _applicationRoots)
            {
                appRoots.Add(Path.Combine(_rootDeployments, appRoot));
            }

            var files = Directory.GetFiles(_rootDeployments, "*.*", SearchOption.AllDirectories);
            return files
                .Where(f => appRoots.Any(r => f.IndexOf(r, StringComparison.Ordinal) >= 0))
                .Select(f => new BackupFileModel
            {
                Filename = Path.GetFileName(f),
                FullPath = f.Replace(_rootDeployments, Path.Combine(_rootBackup, _deploymentsFolder))
            }).ToArray();
        }

        private BackupFileModel[] GetVideoCaptureFileModels()
        {
            var files = Directory.GetFiles(_rootVideoCaptures, "*.*", SearchOption.AllDirectories);
            return files.Select(f => new BackupFileModel
            {
                Filename = Path.GetFileName(f),
                FullPath = f.Replace(_rootVideoCaptures, Path.Combine(_rootBackup, _videoCapturesFolder))
            }).ToArray();
        }

        private string[] GetDeploymentFolders()
        {
            var folders = Directory.GetDirectories(_rootDeployments, "*", SearchOption.AllDirectories);
            return new [] { Path.Combine(_rootBackup, _deploymentsFolder) }
                .Union(folders
                .Where(f => f.IndexOf(_pathMediaBackup, StringComparison.Ordinal) < 0))
                .Select(f =>
                {
                    var newRoot = Path.Combine(_rootBackup, _deploymentsFolder);
                    var newFolder = f.Replace(_rootDeployments, newRoot);
                    return newFolder;
                })
                .ToArray();
        }

        private string[] GetVideoCaptureFolders()
        {
            var folders = Directory.GetDirectories(_rootVideoCaptures, "*", SearchOption.AllDirectories);
            var allFolders = new[] { Path.Combine(_rootBackup, _videoCapturesFolder) }
                .Union(folders
                .Select(f => f.Replace(_rootVideoCaptures,
                    Path.Combine(_rootBackup, _videoCapturesFolder))))
                .ToArray();

            return allFolders;
        }

        private static string DeleteFiles(string root, BackupFileModel[] models, bool recursive = false, string[] excludeFolders = null, string[] excludeRootFolders = null)
        {
            var logText = new StringBuilder();
            var fullName = string.Empty;
            if (excludeFolders == null) excludeFolders = new string[0];
            if (excludeRootFolders == null) excludeRootFolders = new string[0];

            try
            {
                if (!Directory.Exists(root)) return null;

                // data structure to hold names of subfolders to be examined for files
                var dirs = new Stack<string>(20);

                dirs.Push(root);
                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = recursive
                            ? Directory.GetDirectories(currentDir)
                                .Where(d => !excludeFolders.Contains(d))
                                .ToArray()
                            : new string[0];
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    string[] files;
                    try
                    {
                        files = Directory.GetFiles(currentDir).ToArray();
                    }

                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }

                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    // delete obsolete files
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(file);

                            if (models.Any(b => b.FullPath == fileInfo.FullName)) continue;
                            var isRootFolderToExclude = excludeRootFolders?.Any(f => currentDir.IndexOf(f, StringComparison.Ordinal) >= 0) ?? false;

                            if (isRootFolderToExclude) continue;

                            // don't delete the "restore" sql scripts which do not exist in the Deployments source location
                            var filename = Path.GetFileName(fileInfo.FullName);
                            if (filename == $"{RestorePublicProfileFilename}.sql"
                                || filename == $"{RestoreResidentsFilename}.sql"
                                || filename == $"{RestoreConfigurationsFilename}.sql")
                                continue;

                            fullName = fileInfo.FullName;
                            File.Delete(file);
                            var message = $"Obsolete file '{file}' was deleted{Environment.NewLine}";
#if DEBUG
                            Console.Write(message);
#endif
                            logText.Append(message);
                        }
                        catch (Exception ex)
                        {
                            var message = $"--- ERROR --- DeleteFiles: {ex.Message}{Environment.NewLine}";
#if DEBUG
                            Console.Write(message);
#endif
                            logText.Append(message);
                        }
                    }

                    foreach (var dir in subDirs.Select(x => x))
                        dirs.Push(dir);
                }
            }
            catch (Exception ex)
            {
                logText.Append($"ERROR - The following error occurred while deleting file '{fullName}':{ex.Message}");
            }

            return logText.ToString();
        }

        private static string DeleteFolders(string root, string[] foldersToKeep, string[] ignoreRoots = null)
        {
            var logText = new StringBuilder();
            var ignores = ignoreRoots ?? new string[0];

            try
            {
                if (!Directory.Exists(root)) return null;

                // data structure to hold names of subfolders
                var dirs = new Stack<string>(20);

                dirs.Push(root);
                while (dirs.Count > 0)
                {
                    var currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentDir)
                            .Where(f => !ignores.Contains(f))
                            .ToArray();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }

                    var directoryDest = new DirectoryInfo(currentDir);
                    var isFolderToKeep = foldersToKeep.Contains(currentDir);

                    if (!isFolderToKeep)
                    {
                        directoryDest.Delete(true);
                        var message = $"Obsolete folder '{directoryDest.FullName}' was deleted{Environment.NewLine}";
#if DEBUG
                        Console.Write(message);
#endif
                        logText.Append(message);
                    }
                    else
                    {
                        foreach (var dir in subDirs.Select(x => x))
                            dirs.Push(dir);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"--- ERROR --- DeleteFolders: {ex.Message}{Environment.NewLine}";
#if DEBUG
                Console.Write(message);
#endif
                logText.Append(message);
            }

            return logText.ToString();
        }

        private static string GetPublicProfileLinkedFiles(int responseTypeId, int mediaPathTypeId, ResponseTypePaths[] linked)
        {
            if (linked == null) return string.Empty;

            // get response type
            if (linked.All(x => x.ResponseType.Id != responseTypeId)) return string.Empty;
            var mediaResponseType = linked.Single(x => x.ResponseType.Id == responseTypeId);

            // get media path type
            var paths = mediaResponseType.Paths.ToArray();
            if (paths.All(x => x.MediaPathType.Id != mediaPathTypeId)) return string.Empty;

            var path = paths.Single(x => x.MediaPathType.Id == mediaPathTypeId);
            var files = path.Files.ToArray();

            var fileString = new StringBuilder();
            foreach (var f in files)
            {
                fileString.Append($"'{f.Filename.Replace("'", "''")}',");
            }

            return fileString.ToString().TrimEnd(',');
        }

        private static string GetResidentLinkedFiles(int residentId, int responseTypeId, int mediaPathTypeId, ResidentMedia[] linkedMedia)
        {
            if (linkedMedia == null) return string.Empty;

            // get resident
            if (linkedMedia.All(x => x.Resident.Id != residentId)) return string.Empty;
            var resident = linkedMedia.Single(x => x.Resident.Id == residentId);

            // get response type
            if (resident.ResponseTypePaths.All(x => x.ResponseType.Id != responseTypeId)) return string.Empty;
            var mediaResponseType = resident.ResponseTypePaths.Single(x => x.ResponseType.Id == responseTypeId);

            // get media path type
            var paths = mediaResponseType.Paths.ToArray();
            if (paths.All(x => x.MediaPathType.Id != mediaPathTypeId)) return string.Empty;

            var path = paths.Single(x => x.MediaPathType.Id == mediaPathTypeId);
            var files = path.Files.ToArray();

            var fileString = new StringBuilder();
            foreach (var f in files)
            {
                fileString.Append($"'{f.Filename.Replace("'", "''")}',");
            }

           return fileString.ToString().TrimEnd(',');
        }

        private static string CreateScriptRestorePublicProfile(string path)
        {
            var sb = new StringBuilder();

            try
            {
                IMediaPathTypesClient mediaPathTypesClient = new MediaPathTypesClient();
                var mediaPathTypes = mediaPathTypesClient.Get()
                    .Where(pt => pt.IsSharable)
                    .OrderBy(pt => pt.ResponseTypeId)
                    .ToArray();

                IResponseTypesClient responseTypesClient = new ResponseTypesClient();
                var responseTypes = responseTypesClient.Get().ToArray();

                IPublicMediaFilesClient publicMediaFilesClient = new PublicMediaFilesClient();
                var linkedMedia = publicMediaFilesClient.GetLinked().ToArray();

                var pathScript = $@"{path}\Install\Database\SQL Server\{RestorePublicProfileFilename}.sql";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                var mediaSourcePath = new MediaSourcePath();

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("DECLARE @pathProfiles varchar(100)");
                    sw.WriteLine("SET @pathProfiles = FileTableRootPath() + " + @"'\Media\Profiles\'");
                    sw.WriteLine("DECLARE @pathSharedLibrary varchar(100)");
                    sw.WriteLine("SET @pathSharedLibrary = FileTableRootPath() + " + $@"'\Media\{mediaSourcePath.SharedLibrary}\'");
                    sw.WriteLine("DECLARE @allowedExts varchar(100)");
                    sw.WriteLine();

                    foreach (var pathType in mediaPathTypes)
                    {
                        var responseType = responseTypes.Single(rt => rt.Id == pathType.ResponseTypeId);

                        // insert public profile media
                        sw.WriteLine($"--- MediaPathType '{pathType.Description}' - ResponseType '{responseType.Description}' ---");

                        // Owned
                        if (!pathType.IsSystem)
                        {
                            sw.WriteLine($"SET @allowedExts = '''' + REPLACE('{pathType.AllowedExts}', ', ', ''', ''') + ''''");
                            sw.WriteLine(
                                "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)");
                            sw.WriteLine(
                                $"SELECT 0, {responseType.Id}, {pathType.Id}, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                                $@"'{PublicProfileSource.Id}\{pathType.Path}\' AND (@allowedExts) LIKE '%' + [FileType] + '%'");
                            sw.WriteLine();
                        }

                        // From Shared Library
                        var filenames = GetPublicProfileLinkedFiles(responseType.Id, pathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine(
                                "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)");
                            sw.WriteLine(
                                $@"SELECT 1, {responseType.Id}, {pathType.Id}, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{pathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                            sw.WriteLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var message = $"--- ERROR --- CreateScriptRestorePublicProfile: {e.Message}{Environment.NewLine}";
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
                var mediaSourcePath = new MediaSourcePath();
                IResidentsClient residentsClient = new ResidentsClient();
                IMediaPathTypesClient mediaPathTypesClient = new MediaPathTypesClient();
                IResidentMediaFilesClient residentMediaFilesClient = new ResidentMediaFilesClient();
                IResponseTypesClient responseTypesClient = new ResponseTypesClient();

                var residents = residentsClient.Get().ToArray();            
                var mediaPathTypes = mediaPathTypesClient.Get(isSystem: false)
                    .OrderBy(pt => pt.ResponseTypeId).ToArray();          
                var linkedMedia = residentMediaFilesClient.GetLinked()?.ToArray();
                var responseTypes = responseTypesClient.Get().ToArray();

                _residentsExist = residents.Any();

                if (!_residentsExist)
                {
                    var message = $"--- WARNING --- CreateScriptRestoreResidents: No residents found{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return string.Empty;
                }

                var pathScript = $@"{path}\Install\Database\SQL Server\{RestoreResidentsFilename}.sql";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("DECLARE @pathProfiles varchar(100)");
                    sw.WriteLine("SET @pathProfiles = FileTableRootPath() + " + @"'\Media\Profiles\'");
                    sw.WriteLine("DECLARE @pathSharedLibrary varchar(100)");
                    sw.WriteLine("SET @pathSharedLibrary = FileTableRootPath() + " + $@"'\Media\{mediaSourcePath.SharedLibrary}\'");
                    sw.WriteLine("DECLARE @allowedExts varchar(100)");
                    sw.WriteLine();

                    // insert residents
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[Residents] ON");
                    foreach (var r in residents)
                    {
                        var allowVideoCapturing = r.AllowVideoCapturing ? 1 : 0;

                        var lastName = (r.LastName != null) 
                            ? $"'{r.LastName.Replace("'", "''")}'" 
                            : "null";

                        sw.WriteLine(
                            "INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated], [ProfilePicture]) " +
                            $"VALUES({r.Id}, '{r.FirstName.Replace("'", "''")}', {lastName}, '{r.Gender}', {r.GameDifficultyLevel}, {allowVideoCapturing}, GETDATE(), GETDATE(), CONVERT(VARBINARY(max), '0x{BitConverter.ToString(r.ProfilePicture).Replace("-", string.Empty)}', 1))");
                    }
                    sw.WriteLine("SET IDENTITY_INSERT [dbo].[Residents] OFF");

                    // insert resident media
                    foreach (var r in residents)
                    {
                        sw.WriteLine();
                        sw.WriteLine($"--- ResidentId {r.Id} ---");
                        sw.WriteLine();

                        foreach (var pathType in mediaPathTypes)
                        {
                            var responseType = responseTypes.Single(rt => rt.Id == pathType.ResponseTypeId);
                            sw.WriteLine($"--- MediaPathType '{pathType.Description}' - ResponseType '{responseType.Description}' ---");

                            // Owned
                            sw.WriteLine($"SET @allowedExts = '''' + REPLACE('{pathType.AllowedExts}', ', ', ''', ''') + ''''");
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)");
                            sw.WriteLine(
                                $"SELECT 0, {r.Id}, {responseType.Id}, {pathType.Id}, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                                $@"'{r.Id}\{pathType.Path}\' AND (@allowedExts) LIKE '%' + [FileType] + '%'");

                            // From Shared Library
                            var filenames = GetResidentLinkedFiles(r.Id, responseType.Id, pathType.Id, linkedMedia);
                            if (filenames.Length > 0)
                            {
                                sw.WriteLine();
                                sw.WriteLine(
                                    "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)");
                                sw.WriteLine(
                                    $@"SELECT 1, {r.Id}, {responseType.Id}, {pathType.Id}, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{pathType.Path}\'");
                                sw.WriteLine($"AND [Filename] IN ({filenames})");
                            }
                            sw.WriteLine();
                        }
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

            return sb.ToString();
        }

        private static string CreateScriptRestoreConfigurations(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var configsClient = new ConfigsClient();
                var configs = configsClient.Get().ToArray();

                if (!configs.Any())
                {
                    var message = $"--- WARNING --- CreateScriptRestoreConfigurations: No configurations found{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return string.Empty;
                }

                var pathScript = $@"{path}\Install\Database\SQL Server\{RestoreConfigurationsFilename}.sql";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("-- remove all existing confgs");
                    sw.WriteLine("DELETE FROM [dbo].[ConfigDetails]");
                    sw.WriteLine("GO");
                    sw.WriteLine("DELETE FROM [dbo].[Configs]");
                    sw.WriteLine("GO");

                    // iterate through configs
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
                        var configDetails = configsClient.GetWithDetails(c.Id).ConfigDetails;
                        sw.WriteLine();
                        sw.WriteLine("SET IDENTITY_INSERT [dbo].[ConfigDetails] ON");
                        foreach (var cd in configDetails)
                        {
                            var description = (cd.Description != null)
                                ? $"'{cd.Description.Replace("'", "''")}'"
                                : "null";

                            var location = (cd.Location != null)
                                ? $"'{cd.Location.Replace("'", "''")}'"
                                : "null";

                            sw.WriteLine(
                                "INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) " +
                                $"VALUES ({cd.Id}, {c.Id}, {cd.PhidgetType.Id}, {cd.PhidgetStyleType.Id}, {cd.ResponseType.Id}, {description}, {location})");
                        }
                        sw.WriteLine("SET IDENTITY_INSERT [dbo].[ConfigDetails] OFF");
                        sw.WriteLine();
                    }

                    IAmbientInvitationsClient ambientInvitationsClient = new AmbientInvitationsClient();
                    var invitations = ambientInvitationsClient.Get();

                    sw.WriteLine("--- ambient invitations");
                    sw.WriteLine("TRUNCATE TABLE [dbo].[AmbientInvitations]");
                    foreach (var i in invitations)
                    {
                        var isExecuteRandom = i.IsExecuteRandom ? 1 : 0;

                        sw.WriteLine(
                            "INSERT [dbo].[AmbientInvitations] ([Message], [IsExecuteRandom]) " +
                            $"VALUES ('{i.Message.Replace("'", "''")}', {isExecuteRandom})");
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

            return sb.ToString();
        }

        private string CreateInstallPowerShellScript(string path)
        {
            var sb = new StringBuilder();

            try
            {
                var pathScript = $@"{path}\Install\PowerShell\INSTALL_ABBY.ps1";
                const string pathPowerShell = @"C:\Deployments\Install\PowerShell";
                const string pathPowerShellData = @"C:\Deployments\Install\Database\PowerShell";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("Try");
                    sw.WriteLine("{");
                    sw.WriteLine("    Write-Host -ForegroundColor green " + "\"" + "`nInstalling ABBY...`n" + "\"");
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
                    sw.WriteLine(@"    invoke-expression -Command $installPathData\RestorePublicProfile.ps1");
                    if (_residentsExist)
                        sw.WriteLine(@"    invoke-expression -Command $installPathData\RestoreResidents.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPathData\RestoreConfigurations.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\CreateThumbnails.ps1");
                    sw.WriteLine(@"    invoke-expression -Command $installPath\InstallServices.ps1");
                    sw.WriteLine();
                    sw.WriteLine("    Write-Host -ForegroundColor green " + "\"" + "`nInstallation complete.`n" + "\"");
                    sw.WriteLine("    Write-Host -ForegroundColor DarkYellow " + "\"" + "The application will launch after the system is rebooted.`n" + "\"");
                    sw.WriteLine();
                    sw.WriteLine("    $confirmation = Read-Host " + "\"" + "Reboot now (Y/N)?" + "\"");
                    sw.WriteLine("    if ($confirmation.ToUpper() -eq 'Y')");
                    sw.WriteLine("    {");
                    sw.WriteLine("        Restart-Computer");
                    sw.WriteLine("    }");
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