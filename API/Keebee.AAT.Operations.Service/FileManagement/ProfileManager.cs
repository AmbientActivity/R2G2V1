using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.IO;

namespace Keebee.AAT.Operations.Service.FileManagement
{
    public class ProfileManager
    {
        private readonly SystemEventLogger _systemEventLogger;

        public ProfileManager()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.FileManager);
        }
      
        public void CreateFolders(int profileId)
        {
            try
            {
                var profilePath = $@"\\{Environment.MachineName}\{MediaPath.MediaRoot}\{MediaPath.Profiles}\{profileId}";

                if (!Directory.Exists(profilePath))
                {
                    var dirInfo = new DirectoryInfo(profilePath);

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

        public void DeleteFolders(int profileId)
        {
            try
            {
                var profilePath = $@"\\{Environment.MachineName}\{MediaPath.MediaRoot}\{MediaPath.Profiles}\{profileId}";

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

