﻿using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
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
        private const string RestorePublicProfileFilename = "RestorePublicProfile";
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
                        logText.Append(CreateScriptRestorePublicProfile($@"{_pathBackup}\{_deploymentsFolder}"));
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
                    var message = $"--- WARNING --- RemoveObsoleteFolders: {pathDestination} does not exist{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return string.Empty;
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

                if (!Directory.Exists(source))
                {
                    return string.Empty;
                }

                if (!Directory.Exists(pathDest))
                {
                    var message = $"--- ERROR --- RemoveObsoleteFiles: Destination folder {pathDest} does not exist{Environment.NewLine}";
#if DEBUG
                    Console.Write(message);
#endif
                    return message;
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
                            .Where(x => !x.Contains(RestorePublicProfileFilename))
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

        private static string GetPublicProfileLinkedFilenames(int responseTypeId, int mediaPathTypeId, ResponseTypePaths[] linked)
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

        private static string GetResidentLinkedFilenames(int residentId, int responseTypeId, int mediaPathTypeId, ResidentMedia[] linkedMedia)
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
                var mediaPathTypesClient = new MediaPathTypesClient();
                var mediaPathTypes = mediaPathTypesClient.Get().ToArray();

                var publicMediaFilesClient = new PublicMediaFilesClient();
                var linkedMedia = publicMediaFilesClient.GetLinked().ToArray();

                var pathScript = $@"{path}\Install\Database\SQL Server\{RestorePublicProfileFilename}.sql";

                if (File.Exists(pathScript))
                    File.Delete(pathScript);

                var mediaSourcePath = new MediaSourcePath();

                using (var sw = new StreamWriter(pathScript))
                {
                    sw.WriteLine("DECLARE @pathProfiles varchar(max)");
                    sw.WriteLine("SET @pathProfiles = FileTableRootPath() + " + @"'\Media\Profiles\'");
                    sw.WriteLine("DECLARE @pathSharedLibrary varchar(max)");
                    sw.WriteLine("SET @pathSharedLibrary = FileTableRootPath() + " + $@"'\Media\{mediaSourcePath.SharedLibrary}\'");
                    sw.WriteLine();

                    // insert public profile media
                    sw.WriteLine();
                    sw.WriteLine("--- Activity 1 - ResponseType 'SlideShow' ---");

                    // Images General
                    var mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.ImagesGeneral);
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 0, {ResponseTypeId.SlideShow}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        $@"'{PublicProfileSource.Id}\{mediaPathType.Path}\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");

                    // Images General Linked
                    var filenames = GetPublicProfileLinkedFilenames(ResponseTypeId.SlideShow, mediaPathType.Id, linkedMedia);
                    if (filenames.Length > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine(
                            "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $@"SELECT 1, {ResponseTypeId.SlideShow}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                        sw.WriteLine($"AND [Filename] IN ({filenames})");
                    }

                    sw.WriteLine();
                    // Matching Game Shapes
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.MatchingGameShapes);
                    sw.WriteLine("--- Activity 2 - ResponseType 'MatchingGame' ---");
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 0, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        $@"'{PublicProfileSource.Id}\{mediaPathType.Path}\' AND [FileType] = 'png'");

                    // Matching Game Shapes Linked
                    filenames = GetPublicProfileLinkedFilenames(ResponseTypeId.MatchingGame, mediaPathType.Id, linkedMedia);
                    if (filenames.Length > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine(
                            "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $@"SELECT 1, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                        sw.WriteLine($"AND [Filename] IN ({filenames})");
                    }

                    sw.WriteLine();
                    // Matching Game Sounds
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.MatchingGameSounds);
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 0, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        $@"'{PublicProfileSource.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp3'");

                    // Matching Game Sounds Linked
                    filenames = GetPublicProfileLinkedFilenames(ResponseTypeId.MatchingGame, mediaPathType.Id, linkedMedia);
                    if (filenames.Length > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine(
                            "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $@"SELECT 1, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                        sw.WriteLine($"AND [Filename] IN ({filenames})");
                    }

                    sw.WriteLine();
                    // Cats Videos Linked
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.Cats);
                    sw.WriteLine("--- Activity 3 - ResponseType 'Cats' ---");
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 1, {ResponseTypeId.Cats}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + " +
                        $@"'{mediaPathType.Path}\' AND [FileType] = 'mp4'");

                    sw.WriteLine();
                    // Music
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.Music);
                    sw.WriteLine("--- Activity 5 - ResponseType 'Radio' ---");
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 0, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        $@"'{PublicProfileSource.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp3'");

                    // Music Linked
                    filenames = GetPublicProfileLinkedFilenames(ResponseTypeId.Radio, mediaPathType.Id, linkedMedia);
                    if (filenames.Length > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine(
                            "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $@"SELECT 1, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                        sw.WriteLine($"AND [Filename] IN ({filenames})");
                    }

                    sw.WriteLine();
                    // Radio Shows
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.RadioShows);
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 0, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        $@"'{PublicProfileSource.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp3'");

                    // Radio Shows Linked
                    filenames = GetPublicProfileLinkedFilenames(ResponseTypeId.Radio, mediaPathType.Id, linkedMedia);
                    if (filenames.Length > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine(
                            "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $@"SELECT 1, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                        sw.WriteLine($"AND [Filename] IN ({filenames})");
                    }

                    sw.WriteLine();
                    // TV Shows
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.TVShows);
                    sw.WriteLine("--- Activity 6 - ResponseType 'Television' ---");
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 0, {ResponseTypeId.Television}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                        $@"'{PublicProfileSource.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp4'");

                    // TV Shows Linked
                    filenames = GetPublicProfileLinkedFilenames(ResponseTypeId.Television, mediaPathType.Id, linkedMedia);
                    if (filenames.Length > 0)
                    {
                        sw.WriteLine();
                        sw.WriteLine(
                            "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $@"SELECT 1, {ResponseTypeId.Television}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                        sw.WriteLine($"AND [Filename] IN ({filenames})");
                    }

                    sw.WriteLine();
                    // Ambient Videos Linked
                    mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.Ambient);
                    sw.WriteLine("--- Activity 7 - ResponseType 'Ambient' ---");
                    sw.WriteLine(
                        "INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)");
                    sw.WriteLine(
                        $"SELECT 1, {ResponseTypeId.Ambient}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + " +
                        $@"'{mediaPathType.Path}\' AND [FileType] = 'mp4'");

                    sw.WriteLine();
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
                var residentsClient = new ResidentsClient();
                var mediaPathTypesClient = new MediaPathTypesClient();
                var residentMediaFilesClient = new ResidentMediaFilesClient();

                var residents = residentsClient.Get().ToArray();            
                var mediaPathTypes = mediaPathTypesClient.Get().ToArray();          
                var linkedMedia = residentMediaFilesClient.GetLinked()?.ToArray();

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
                    sw.WriteLine("DECLARE @pathProfiles varchar(max)");
                    sw.WriteLine("SET @pathProfiles = FileTableRootPath() + " + @"'\Media\Profiles\'");
                    sw.WriteLine("DECLARE @pathSharedLibrary varchar(max)");
                    sw.WriteLine("SET @pathSharedLibrary = FileTableRootPath() + " + $@"'\Media\{mediaSourcePath.SharedLibrary}\'");
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
                        sw.WriteLine("--- Activity 1 - ResponseType 'SlideShow' ---");

                        // Images General
                        var mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.ImagesGeneral);
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.SlideShow}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");

                        // Images General Linked
                        var filenames = GetResidentLinkedFilenames(r.Id, ResponseTypeId.SlideShow, mediaPathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine();
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                            sw.WriteLine(
                                $@"SELECT 1, {r.Id}, {ResponseTypeId.SlideShow}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                        }

                        sw.WriteLine();
                        // Images Personal
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.ImagesPersonal);
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.SlideShow}, {MediaPathTypeId.ImagesPersonal}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')");

                        sw.WriteLine();
                        // Matching Game Shapes
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.MatchingGameShapes);
                        sw.WriteLine("--- Activity 2 - ResponseType 'MatchingGame' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] = 'png'");

                        // Matching Game Shapes Linked
                        filenames = GetResidentLinkedFilenames(r.Id, ResponseTypeId.MatchingGame, mediaPathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine();
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                            sw.WriteLine(
                                $@"SELECT 1, {r.Id}, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                        }

                        sw.WriteLine();
                        // Matching Game Sounds
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.MatchingGameSounds);
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp3'");

                        // Matching Game Sounds Linked
                        filenames = GetResidentLinkedFilenames(r.Id, ResponseTypeId.MatchingGame, mediaPathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine();
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                            sw.WriteLine(
                                $@"SELECT 1, {r.Id}, {ResponseTypeId.MatchingGame}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                        }

                        sw.WriteLine();
                        // Music
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.Music);
                        sw.WriteLine("--- Activity 5 - ResponseType 'Radio' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp3'");

                        // Music Linked
                        filenames = GetResidentLinkedFilenames(r.Id, ResponseTypeId.Radio, mediaPathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine();
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                            sw.WriteLine(
                                $@"SELECT 1, {r.Id}, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                        }

                        sw.WriteLine();
                        // Radio Shows
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.RadioShows);
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp3'");

                        // Radio Shows Linked
                        filenames = GetResidentLinkedFilenames(r.Id, ResponseTypeId.Radio, mediaPathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine();
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                            sw.WriteLine(
                                $@"SELECT 1, {r.Id}, {ResponseTypeId.Radio}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                        }

                        sw.WriteLine();
                        // TV Shows
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.TVShows);
                        sw.WriteLine("--- Activity 5 - ResponseType 'Television' ---");
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.Television}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp4'");

                        // TV Shows Linked
                        filenames = GetResidentLinkedFilenames(r.Id, ResponseTypeId.Television, mediaPathType.Id, linkedMedia);
                        if (filenames.Length > 0)
                        {
                            sw.WriteLine();
                            sw.WriteLine(
                                "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                            sw.WriteLine(
                                $@"SELECT 1, {r.Id}, {ResponseTypeId.Television}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + '{mediaPathType.Path}\'");
                            sw.WriteLine($"AND [Filename] IN ({filenames})");
                        }

                        sw.WriteLine();
                        // Home Movies
                        mediaPathType = mediaPathTypes.Single(x => x.Id == MediaPathTypeId.HomeMovies);
                        sw.WriteLine(
                            "INSERT INTO ResidentMediaFiles (IsLinked, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)");
                        sw.WriteLine(
                            $"SELECT 0, {r.Id}, {ResponseTypeId.Television}, {mediaPathType.Id}, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + " +
                            $@"'{r.Id}\{mediaPathType.Path}\' AND [FileType] = 'mp4'");

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