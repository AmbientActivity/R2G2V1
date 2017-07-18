using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class PublicProfileRules
    {
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IResponseTypesClient _responseTypesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;

        public PublicProfileRules()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaFilesClient = new MediaFilesClient();
            _responseTypesClient = new ResponseTypesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
        }

        public string AddMediaFile(Guid streamId, int mediaPathTypeId, int responseTypeId, DateTime dateAdded, bool isLinked, out int newId)
        {
            var mf = new PublicMediaFileEdit
            {
                StreamId = streamId,
                ResponseTypeId = responseTypeId,
                MediaPathTypeId = mediaPathTypeId,
                IsLinked = isLinked,
                DateAdded = dateAdded
            };

            return _publicMediaFilesClient.Post(mf, out newId);
        }

        public IEnumerable<ResponseType> GetValidResponseTypes(int mediaPathTypeId)
        {
            var responseTypes = _responseTypesClient.Get();

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.ImagesGeneral:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.SlideShow);

                case MediaPathTypeId.TVShows:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Television);
                case MediaPathTypeId.Music:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Radio);
                case MediaPathTypeId.RadioShows:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.Radio);
                case MediaPathTypeId.MatchingGameShapes:
                case MediaPathTypeId.MatchingGameSounds:
                    return responseTypes.Where(x => x.Id == ResponseTypeId.MatchingGame);

                default:
                    return new List<ResponseType>();
            }
        }

        public string GetMediaPath(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _mediaPathTypesClient.Get((int)mediaPathTypeId).Path
                : _mediaPathTypesClient.Get(MediaPathTypeId.ImagesGeneral).Path;
        }

        public string GetMediaPathShortDescription(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _mediaPathTypesClient.Get((int)mediaPathTypeId).ShortDescription
                : _mediaPathTypesClient.Get(MediaPathTypeId.ImagesGeneral).ShortDescription;
        }

        public bool FileExists(string path, string filename)
        {
            var file = _mediaFilesClient.GetFromPath(path, filename);

            return (file != null);
        }

        // when doing a bulk delete, ensure there is at least one media file remaining per response type
        public string CanDeleteMultiple(int numSelected, int mediaPathTypeId, int responseTypeId)
        {
            var result = string.Empty;
            var mediaResponseTypes = _publicMediaFilesClient.GetForMediaPathType(mediaPathTypeId).ToArray();

            if (!mediaResponseTypes.Any()) return result;

            var files = mediaResponseTypes
                .Where(x => x.ResponseType.Id == responseTypeId)
                .SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .SelectMany(x => x.Files);

            if (files.Count() == numSelected)
                result = "Must have at least 1 media file for each respone type";

            return result;
        }

        public MediaFilePath GetMediaFile(int id)
        {
            var mediaFile = _publicMediaFilesClient.Get(id);

            return mediaFile == null
                ? null
                : _mediaFilesClient.Get(mediaFile.MediaFile.StreamId);
        }

        public static string GetAllowedExtensions(int? mediaPathTypeId)
        {
            if (mediaPathTypeId == null) return string.Empty;

            var extensions = string.Empty;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.ImagesGeneral:
                    extensions = "*.jpg,*.jpeg,*.png,*.gif";
                    break;
                case MediaPathTypeId.TVShows:
                    extensions = "*.mp4";
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.RadioShows:
                case MediaPathTypeId.MatchingGameSounds:
                    extensions = "*.mp3";
                    break;
                case MediaPathTypeId.MatchingGameShapes:
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
                case MediaPathTypeId.ImagesPersonal:
                case MediaPathTypeId.ImagesGeneral:
                    isValid = name.Contains("jpg") || name.Contains("jpeg") || name.Contains("png") || name.Contains("gif");
                    break;
                case MediaPathTypeId.TVShows:
                    isValid = name.Contains("mp4");
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.RadioShows:
                case MediaPathTypeId.MatchingGameSounds:
                    isValid = name.Contains("mp3");
                    break;
                case MediaPathTypeId.MatchingGameShapes:
                    isValid = name.Contains("png");
                    break;
            }

            return isValid;
        }

        public static bool IsMediaTypeThumbnail(int mediaPathTypeId)
        {
            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Music:
                case MediaPathTypeId.RadioShows:
                case MediaPathTypeId.MatchingGameSounds:
                    return false;
                default:
                    return true;
            }
        }
    }
}
