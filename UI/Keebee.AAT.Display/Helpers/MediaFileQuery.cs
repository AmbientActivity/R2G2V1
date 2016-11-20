using System.Collections.Generic;
using System.Linq;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;

namespace Keebee.AAT.Display.Helpers
{
    public class MediaFileQuery
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public string[] GetFilesForResponseType(int residentId, int responseTypeId, int mediaPatheTypeId = -1)
        {
            var files = new string[0];
            ICollection<MediaResponseType> mediaFiles = null;
            var numFiles = 0;

            // get media from public library for the response type
            if (residentId == PublicMediaSource.Id)
            {
                mediaFiles = _opsClient.GetPublicMediaFilesForResponseType(responseTypeId)
                    .MediaFiles.ToArray();

                numFiles += mediaFiles.Where(x => x.ResponseType.Id == responseTypeId)
                    .SelectMany(m => m.Paths)
                    .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                    .SelectMany(p => p.Files).Count();
            }
            else
            {
                // get media from resident's profile for the response type
                var media = _opsClient.GetResidentMediaFilesForResidentResponseType(residentId, responseTypeId);

                // get a count of files for the response type
                if (media != null)
                {
                    mediaFiles = media.MediaFiles.ToList();
                    numFiles = mediaFiles
                        .Where(x => x.ResponseType.Id == responseTypeId)
                        .SelectMany(m => m.Paths)
                        .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                        .SelectMany(p => p.Files).Count();
                }

                // if no files then look in public library
                if (numFiles == 0)
                {
                    residentId = PublicMediaSource.Id;
                    mediaFiles = _opsClient.GetPublicMediaFilesForResponseType(responseTypeId).MediaFiles.ToList();
                    numFiles = mediaFiles.Where(x => x.ResponseType.Id == responseTypeId)
                        .SelectMany(m => m.Paths)
                        .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                        .SelectMany(p => p.Files).Count();
                }
            }

            return (mediaFiles != null && numFiles > 0)
                ? GetAssembledFileList(mediaFiles, residentId, responseTypeId, mediaPatheTypeId)
                : files;
        }

        private string[] GetAssembledFileList(ICollection<MediaResponseType> mediaFiles, int residentId, int responseTypeId, int mediaPatheTypeId = -1)
        {
            var fileList = new List<string>();
            var pathRoot = $@"{_mediaPath.ProfileRoot}\{residentId}";

            var mediaPaths = mediaFiles
                .Single(x => x.ResponseType.Id == responseTypeId)
                .Paths.Where(p => mediaPatheTypeId < 0 ||  p.MediaPathType.Id == mediaPatheTypeId)
                .ToArray();

            foreach (var mediaPath in mediaPaths)
            {
                var list = mediaFiles
                    .Single(m => m.ResponseType.Id == responseTypeId)
                    .Paths
                    .Where(p => p.MediaPathType.Id == mediaPath.MediaPathType.Id)
                    .SelectMany(p => p.Files)
                    .OrderBy(f => f.Filename)
                    .Select(f => $@"{pathRoot}\{mediaPath.MediaPathType.Path}\{f.Filename}")
                    .ToList();

                fileList.AddRange(list);
            }

            return fileList.Any() ? fileList.ToArray() : new string[0];
        }
    }
}
