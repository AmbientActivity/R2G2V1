using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Keebee.AAT.ApiClient;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;

namespace Keebee.AAT.Administrator.FileManagement
{
    public class FileManager
    {
        private readonly OperationsClient _opsClient;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        // media path
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public FileManager()
        {
            _opsClient = new OperationsClient();
        }

        public string DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                //_systemEventLogger.WriteEntry($"FileManagement.DeleteFile :{ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
            return string.Empty;
        }

        public void CreateFolders(int residentId)
        {
            try
            {
                var mediaRoot = $@"{_mediaPath.ProfileRoot}\{residentId}";
                var paths = _opsClient.GetMediaPathTypes();

                if (Directory.Exists(mediaRoot)) return;

                var dirInfo = new DirectoryInfo(mediaRoot);
                foreach (var path in paths)
                {
                    dirInfo.CreateSubdirectory(path.Path);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"FileManagement.CreateFolders :{ex.Message}", EventLogEntryType.Error);
            }
        }

        public void DeleteFolders(int residentId)
        {
            try
            {
                var profilePath = $@"{_mediaPath.ProfileRoot}\{residentId}";

                if (Directory.Exists(profilePath))
                {
                    var dirInfo = new DirectoryInfo(profilePath);

                    dirInfo.Delete(true);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"FileManagement.DeleteFolders :{ex.Message}", EventLogEntryType.Error);
            }
        }

        public Guid GetStreamId(string path, string filename)
        {
            var file = _opsClient.GetMediaFileFromPath(path, filename.Replace("&", "%26"));
            return file.StreamId;
        }
    }
}

