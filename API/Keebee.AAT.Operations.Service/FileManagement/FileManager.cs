using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.IO;

namespace Keebee.AAT.Operations.Service.FileManagement
{
    public class FileManager
    {
        private readonly SystemEventLogger _systemEventLogger;

        public FileManager()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.FileManager);
        }
      
        public void CreateFolders(int residentId)
        {
            try
            {
                var mediaPath = $@"\\{Environment.MachineName}\{MediaPath.MediaRoot}\{MediaPath.Profiles}\{residentId}";

                if (!Directory.Exists(mediaPath))
                {
                    var dirInfo = new DirectoryInfo(mediaPath);

                    dirInfo.CreateSubdirectory(MediaPath.Images);
                    dirInfo.CreateSubdirectory(MediaPath.Videos);
                    dirInfo.CreateSubdirectory(MediaPath.Music);
                    dirInfo.CreateSubdirectory(MediaPath.Pictures);
                    dirInfo.CreateSubdirectory(MediaPath.Shapes);
                    dirInfo.CreateSubdirectory(MediaPath.Sounds);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"FileManagement.ProfileManager.CreateFolders :{ex.Message}", EventLogEntryType.Error);
            }
        }

        public void DeleteFolders(int residentId)
        {
            try
            {
                var profilePath = $@"\\{Environment.MachineName}\{MediaPath.MediaRoot}\{MediaPath.Profiles}\{residentId}";

                if (Directory.Exists(profilePath))
                {
                    var dirInfo = new DirectoryInfo(profilePath);
                    
                    dirInfo.Delete(true);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"FileManagement.ProfileManager.CreateFolders :{ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}

