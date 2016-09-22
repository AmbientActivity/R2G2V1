//using Keebee.AAT.Shared;
//using Keebee.AAT.SystemEventLogging;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using Keebee.AAT.RESTClient;

//namespace Keebee.AAT.FileManagement
//{
//    public class FileManager
//    {
//        private readonly SystemEventLogger _systemEventLogger;
//        private readonly OperationsClient _opsClient;

//        // media path
//        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

//        public FileManager()
//        {
//            _systemEventLogger = new SystemEventLogger(SystemEventLogType.FileManager);
//            _opsClient = new OperationsClient();
//        }

//        public void DeleteFile(string path)
//        {
//            try
//            {
//                if (File.Exists(path))
//                {
//                    File.Delete(path);
//                }
//            }
//            catch (Exception ex)
//            {
//                _systemEventLogger.WriteEntry($"FileManagement.ProfileManager.CreateFolders :{ex.Message}", EventLogEntryType.Error);
//            }
//        }

//        public void CreateFolders(int residentId)
//        {
//            try
//            {
//                var mediaRoot = $@"{_mediaPath.ProfileRoot}\{residentId}";
//                var paths = _opsClient.GetMediaPathTypes();

//                if (Directory.Exists(mediaRoot)) return;

//                var dirInfo = new DirectoryInfo(mediaRoot);
//                foreach (var path in paths)
//                {
//                    dirInfo.CreateSubdirectory(path.Description);
//                }
//            }
//            catch (Exception ex)
//            {
//                _systemEventLogger.WriteEntry($"FileManagement.ProfileManager.CreateFolders :{ex.Message}", EventLogEntryType.Error);
//            }
//        }

//        public void DeleteFolders(int residentId)
//        {
//            try
//            {
//                var profilePath = $@"{_mediaPath.ProfileRoot}\{residentId}";

//                if (Directory.Exists(profilePath))
//                {
//                    var dirInfo = new DirectoryInfo(profilePath);

//                    dirInfo.Delete(true);
//                }
//            }
//            catch (Exception ex)
//            {
//                _systemEventLogger.WriteEntry($"FileManagement.ProfileManager.CreateFolders :{ex.Message}", EventLogEntryType.Error);
//            }
//        }

//        public Guid GetStreamId(string path, string filename)
//        {
//            var file = _opsClient.GetMediaFileFromPath(path, filename);
//            return file.StreamId;
//        }
//    }
//}

