using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Main.Helpers
{
    public class MediaFileQuery
    {
        private readonly PublicMediaFilesClient _publicMediaFilesClient;
        private readonly ResidentMediaFilesClient _residentMediaFilesClient;

        public MediaFileQuery()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
        }

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public string[] GetFilesForResponseType(int residentId, int responseTypeId, int mediaPatheTypeId = -1)
        {
            var files = new string[0];
            var pathTypeFiles = new MediaPathTypeFiles[0];
            var numFiles = 0;

            // get media from public library for the response type
            if (residentId == PublicProfileSource.Id)
            {
                var paths = _publicMediaFilesClient.GetForResponseType(responseTypeId);

                if (paths != null)
                    pathTypeFiles = paths.ToArray();

                if (pathTypeFiles.Any())
                {
                    numFiles += pathTypeFiles
                        .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                        .SelectMany(p => p.Files).Count();
                }
            }
            else
            {
                // get media from resident's profile for the response type
                var paths = _residentMediaFilesClient.GetForResidentResponseType(residentId, responseTypeId);

                if (paths != null)
                    pathTypeFiles = paths.ToArray();

                // get a count of files for the response type
                if (pathTypeFiles.Any())
                {
                    numFiles = pathTypeFiles
                        .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                        .SelectMany(p => p.Files).Count();
                }

                // if no files then look in public library
                if (numFiles == 0)
                {
                    residentId = PublicProfileSource.Id;
                    var publicPaths = _publicMediaFilesClient.GetForResponseType(responseTypeId);
                        
                    if (publicPaths != null)
                    {
                        pathTypeFiles = publicPaths.ToArray();

                        numFiles = pathTypeFiles
                            .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                            .SelectMany(p => p.Files).Count();
                    }
                }
            }

            return (pathTypeFiles.Any() && numFiles > 0)
                ? GetAssembledFileList(pathTypeFiles, residentId, mediaPatheTypeId)
                : files;
        }

        private string[] GetAssembledFileList(MediaPathTypeFiles[] responseTypePaths, int residentId, int mediaPathTypeId = -1)
        {
            var fileList = new List<string>();
           
            var mediaPaths = responseTypePaths
                .Where(p => mediaPathTypeId < 0 ||  p.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            foreach (var mediaPath in mediaPaths)
            {
                var list = responseTypePaths
                    .Where(p => p.MediaPathType.Id == mediaPath.MediaPathType.Id)
                    .SelectMany(p => p.Files)
                    .OrderBy(f => f.Filename)
                    .Select(f =>
                    {
                        var pathRoot = (f.IsLinked)
                            ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                            : $@"{_mediaPath.ProfileRoot}\{residentId}";

                        return $@"{pathRoot}\{mediaPath.MediaPathType.Path}\{f.Filename}";
                    })
                    .ToList();

                fileList.AddRange(list);
            }

            return fileList.Any() ? fileList.ToArray() : new string[0];
        }
    }
}
