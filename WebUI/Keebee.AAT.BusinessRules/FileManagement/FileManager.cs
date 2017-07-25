using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace Keebee.AAT.BusinessRules
{
    public class FileManager
    {
        private readonly MediaPathTypesClient _mediaPathTypesClient;
        private readonly MediaFilesClient _mediaFilesClient;

        // media path
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public FileManager()
        {
            _mediaPathTypesClient = new MediaPathTypesClient();
            _mediaFilesClient = new MediaFilesClient();
        }

        public string DeleteFile(string path)
        {
            string errMsg = null;

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"FileManagement.DeleteFile :{ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }

            return errMsg;
        }

        public string CreateFolders(int profileId)
        {
            string errMsg = null;
            try
            {
                var mediaRoot = $@"{_mediaPath.ProfileRoot}\{profileId}";
                var paths = _mediaPathTypesClient.Get();

                if (Directory.Exists(mediaRoot)) return null;

                var dirInfo = new DirectoryInfo(mediaRoot);
                foreach (var path in paths)
                {
                    dirInfo.CreateSubdirectory(path.Path);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"FileManagement.CreateFolders :{ex.Message}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return errMsg;
        }

        public string DeleteFolders(int profileId)
        {
            string errMsg = null;

            try
            {
                var profilePath = $@"{_mediaPath.ProfileRoot}\{profileId}";

                if (Directory.Exists(profilePath))
                {
                    var dirInfo = new DirectoryInfo(profilePath);

                    dirInfo.Delete(true);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"FileManagement.DeleteFolders :{ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }

            return errMsg;
        }

        public Guid GetStreamId(string path, string filename)
        {
            var file = _mediaFilesClient.GetFromPath(path, filename.Replace("&", "%26"));
            return file?.StreamId ?? new Guid();
        }

        public MediaFile GetMediaFile(string path, string filename)
        {
            return _mediaFilesClient.GetFromPath(path, filename.Replace("&", "%26"));
        }
    }
}

