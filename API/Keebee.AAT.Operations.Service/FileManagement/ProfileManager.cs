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
       
        internal static class FolderName
        {
            public const string Images = "images";
            public const string Videos = "videos";
            public const string Music = "music";
            public const string Pictures = "pictures";
            public const string Shapes = "shapes";
            public const string Sounds = "sounds";
        }

        public void CreateFolders(int profileId)
        {
            try
            {
                var profilePath = $@"\\{Environment.MachineName}\{MediaPath.ProfileRoot}\{profileId}";

                if (!Directory.Exists(profilePath))
                {
                    var dirInfo = new DirectoryInfo(profilePath);

                    dirInfo.CreateSubdirectory(FolderName.Images);
                    dirInfo.CreateSubdirectory(FolderName.Videos);
                    dirInfo.CreateSubdirectory(FolderName.Music);
                    dirInfo.CreateSubdirectory(FolderName.Pictures);
                    dirInfo.CreateSubdirectory(FolderName.Shapes);
                    dirInfo.CreateSubdirectory(FolderName.Sounds);
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
                var profilePath = $@"\\{Environment.MachineName}\{MediaPath.ProfileRoot}\{profileId}";

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

