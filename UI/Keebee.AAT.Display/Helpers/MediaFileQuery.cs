using Keebee.AAT.ApiClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Linq;

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

        public string[] GetFilesForSystemResponseType(int responseTypeId, int mediaPathTypeId)
        {
            var fileList = new List<string>();
            MediaResponseType mediaResponseType = null;

            var media = _opsClient.GetPublicMediaFilesForResponseType(responseTypeId);

            if (media != null)
                mediaResponseType = media.MediaResponseType;

            if (mediaResponseType == null) return fileList.ToArray();

            var mediaPaths = mediaResponseType
                .Paths.Where(p => p.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            foreach (var mediaPath in mediaPaths)
            {
                var list = mediaResponseType
                    .Paths
                    .Where(p => p.MediaPathType.Id == mediaPath.MediaPathType.Id)
                    .SelectMany(p => p.Files)
                    .OrderBy(f => f.Filename)
                    .Select(f => $@"{_mediaPath.MediaRoot}\{_mediaPath.SystemLibrary}\{mediaPath.MediaPathType.Path}\{f.Filename}")
                    .ToList();

                fileList.AddRange(list);
            }

            return fileList.ToArray();
        }

        public string[] GetFilesForResponseType(int residentId, int responseTypeId, int mediaPatheTypeId = -1)
        {
            var files = new string[0];
            MediaResponseType mediaResponseType = null;
            var numFiles = 0;

            // get media from public library for the response type
            if (residentId == PublicMediaSource.Id)
            {
                var media = _opsClient.GetPublicMediaFilesForResponseType(responseTypeId);

                if (media != null)
                    mediaResponseType = media.MediaResponseType;

                if (mediaResponseType != null)
                {
                    numFiles += mediaResponseType.Paths
                        .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                        .SelectMany(p => p.Files).Count();
                }
            }
            else
            {
                // get media from resident's profile for the response type
                var media = _opsClient.GetResidentMediaFilesForResidentResponseType(residentId, responseTypeId);

                if (media != null)
                    mediaResponseType = media.MediaResponseType;

                // get a count of files for the response type
                if (mediaResponseType != null)
                {
                    numFiles = mediaResponseType.Paths
                        .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                        .SelectMany(p => p.Files).Count();
                }

                // if no files then look in public library
                if (numFiles == 0)
                {
                    residentId = PublicMediaSource.Id;
                    var publicMedia = _opsClient.GetPublicMediaFilesForResponseType(responseTypeId);

                    if (publicMedia != null)
                        mediaResponseType = publicMedia.MediaResponseType;

                    if (publicMedia != null)
                    {
                        numFiles = mediaResponseType.Paths
                            .Where(p => mediaPatheTypeId < 0 || p.MediaPathType.Id == mediaPatheTypeId)
                            .SelectMany(p => p.Files).Count();
                    }
                }
            }

            return (mediaResponseType != null && numFiles > 0)
                ? GetAssembledFileList(mediaResponseType, residentId, mediaPatheTypeId)
                : files;
        }

        private string[] GetAssembledFileList(MediaResponseType mediaResponseType, int residentId, int mediaPathTypeId = -1)
        {
            var fileList = new List<string>();
           
            var mediaPaths = mediaResponseType
                .Paths.Where(p => mediaPathTypeId < 0 ||  p.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            foreach (var mediaPath in mediaPaths)
            {
                var list = mediaResponseType
                    .Paths
                    .Where(p => p.MediaPathType.Id == mediaPath.MediaPathType.Id)
                    .SelectMany(p => p.Files)
                    .OrderBy(f => f.Filename)
                    .Select(f =>
                    {
                        var pathRoot = f.IsLinked
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
