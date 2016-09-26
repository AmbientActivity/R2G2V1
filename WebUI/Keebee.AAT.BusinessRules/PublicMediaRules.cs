using System;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class PublicMediaRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        public IEnumerable<ResponseType> GetValidResponseTypes(int mediaPathTypeId)
        {
            var responseTypes = _opsClient.GetResponseTypes();

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.SlidShow);

                case MediaPathTypeId.Videos:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Television 
                        || x.Id == ResponseTypeId.Cats
                        || x.Id == ResponseTypeId.Ambient);

                case MediaPathTypeId.Music:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Radio);

                case MediaPathTypeId.Shapes:
                case MediaPathTypeId.Sounds:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.MatchingGame);

                default:
                    return new List<ResponseType>();
            }
        }

        public string GetMediaPathType(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).Description
                : _opsClient.GetMediaPathType(MediaPathTypeId.Images).Description;
        }

        public bool FileExists(string path, string filename)
        {
            var file = _opsClient.GetMediaFileFromPath(path, filename);

            return (file != null);
        }

        // don't delete the file if already it exists in another response type
        public bool IsRemovable(string filePath, int mediaPathTypeId, int responseTypeId)
        {
            var publicMedia = _opsClient.GetPublicMediaFilesForMediaPathType(mediaPathTypeId);

            if (publicMedia == null) return true;
            if (!publicMedia.MediaFiles.Any()) return true;

            var paths = publicMedia.MediaFiles.Select(x => x.ResponseType)
                .Where(x => x.Id != responseTypeId);

            return (paths.Count() == 1);
        }

        // when doing a bulk delete, ensure there is at least one media file remaining per response type
        public string CanDeleteMultiple(int numSelected, int mediaPathTypeId, int responseTypeId)
        {
            var result = string.Empty;

            var publicMedia = _opsClient.GetPublicMediaFilesForMediaPathType(mediaPathTypeId);

            if (publicMedia == null) return result;
            if (!publicMedia.MediaFiles.Any()) return result;

            var files = publicMedia.MediaFiles
                .Where(x => x.ResponseType.Id == responseTypeId)
                .SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .SelectMany(x => x.Files);

            if (files.Count() == numSelected)
                result = "Must have at least 1 media file for each respone type";

            return result;
        }

        // when doing a single delete, don't delete the file, just the link if it exists in another response type
        public bool IsMultipleReponseTypes(Guid streamId)
        {
            var streamIds = _opsClient.GetPublicMediaFilesForStreamId(streamId).ToArray();

            return streamIds.Count() > 1;
        }

        public string DeletePublicMediaFile(Guid streamId, int reponseTypeId)
        {
            var id = _opsClient.GetPublicMediaFilesForStreamId(streamId)
                .Single(x => x.ResponseType.Id == reponseTypeId).Id;

            return _opsClient.DeletePublicMediaFile(id);
        }

        public static string GetAllowedExtensions(int? mediaPathTypeId)
        {
            if (mediaPathTypeId == null) return string.Empty;

            var extensions = string.Empty;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    extensions = "*.jpg,*.jpeg,*.png,*.gif";
                    break;
                case MediaPathTypeId.Videos:
                    extensions = "*.mp4";
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.Sounds:
                    extensions = "*.mp3";
                    break;
                case MediaPathTypeId.Shapes:
                    extensions = "*.png";
                    break;
            }

            return extensions;
        }

        public static bool IsValidFile(string filename, int? mediaPathTypeId)
        {
            if (mediaPathTypeId == null) return false;

            var isValid = false;
            var name = filename.ToLower();

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    isValid = name.Contains("jpg") || name.Contains("jpeg") || name.Contains("png") || name.Contains("gif");
                    break;
                case MediaPathTypeId.Videos:
                    isValid = name.Contains("mp4");
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.Sounds:
                    isValid = name.Contains("mp3");
                    break;
                case MediaPathTypeId.Shapes:
                    isValid = name.Contains("png");
                    break;
            }

            return isValid;
        }
    }
}
