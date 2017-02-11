using System;
using Keebee.AAT.ApiClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class PublicProfileRules
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
                case MediaPathTypeId.GeneralImages:
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
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).Path
                : _opsClient.GetMediaPathType(MediaPathTypeId.GeneralImages).Path;
        }

        public string GetMediaPathShortDescription(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).ShortDescription
                : _opsClient.GetMediaPathType(MediaPathTypeId.GeneralImages).ShortDescription;
        }

        public bool FileExists(string path, string filename)
        {
            var file = _opsClient.GetMediaFileFromPath(path, filename);

            return (file != null);
        }

        // when doing a bulk delete, ensure there is at least one media file remaining per response type
        public string CanDeleteMultiple(int numSelected, int mediaPathTypeId)
        {
            var result = string.Empty;
            var responseTypeId = GetResponseTypeId(mediaPathTypeId);
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

        public string DeletePublicMediaFile(int id)
        {
            return _opsClient.DeletePublicMediaFile(id); 
        }

        public MediaFileSingle GetMediaFile(int id)
        {
            var mediaFile = _opsClient.GetPublicMediaFile(id);

            return mediaFile == null 
                ? null 
                : _opsClient.GetMediaFile(mediaFile.MediaFile.StreamId);
        }

        public static string GetAllowedExtensions(int? mediaPathTypeId)
        {
            if (mediaPathTypeId == null) return string.Empty;

            var extensions = string.Empty;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.GeneralImages:
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
                case MediaPathTypeId.PersonalImages:
                case MediaPathTypeId.GeneralImages:
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

        public static int GetResponseTypeId(int mediaPathTypeId)
        {
            var responseTypeId = -1;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.PersonalImages:
                case MediaPathTypeId.GeneralImages:
                    responseTypeId = ResponseTypeId.SlideShow;
                    break;
                case MediaPathTypeId.HomeMovies:
                case MediaPathTypeId.TVShows:
                    responseTypeId = ResponseTypeId.Television;
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.RadioShows:
                    responseTypeId = ResponseTypeId.Radio;
                    break;
                case MediaPathTypeId.MatchingGameShapes:
                case MediaPathTypeId.MatchingGameSounds:
                    responseTypeId = ResponseTypeId.MatchingGame;
                    break;
            }

            return responseTypeId;
        }

        public IEnumerable<MediaFile> GetAvailableSharedMediaFiles(int mediaPathTypeId)
        {
            var mediaSource = new MediaSourcePath();
            var mediaPath = GetMediaPath(mediaPathTypeId);
            var sharedPaths = _opsClient.GetMediaFilesForPath($@"{mediaSource.SharedLibrary}\{mediaPath}").ToArray();
            var existingSharedMediaPaths = _opsClient.GetPublicMediaFilesForMediaPathType(mediaPathTypeId);
            IEnumerable<Guid> existingStreamIds = new List<Guid>();

            if (existingSharedMediaPaths != null)
            {
                if (existingSharedMediaPaths.MediaFiles.Any())
                {
                    existingStreamIds = existingSharedMediaPaths.MediaFiles.SelectMany(p => p.Paths)
                        .SelectMany(f => f.Files)
                        .Where(f => f.IsLinked)
                        .Select(f => f.StreamId);
                }
            }

            var availableFiles = sharedPaths
                .SelectMany(f => f.Files)
                .Where(f => !existingStreamIds.Contains(f.StreamId));

            return availableFiles;
        }

        public string GetNoAvailableSharedMediaMessage(int mediaPathTypeId)
        {
            var mediaPathType = GetMediaPathShortDescription(mediaPathTypeId);

            var hasHave = mediaPathType.EndsWith("s") ? "have" : "has";
            var message = $"All available {mediaPathType} {hasHave} already been included in this profile.";

            return message;
        }
    }
}
