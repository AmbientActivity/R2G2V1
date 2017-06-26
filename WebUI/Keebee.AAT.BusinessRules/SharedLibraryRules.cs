using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class SharedLibraryRules
    {
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IMediaFilesClient _mediaFilesClient;

        public SharedLibraryRules()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
            _mediaFilesClient = new MediaFilesClient();
        }

        public IEnumerable<ResponseType> GetValidResponseTypes(int mediaPathTypeId)
        {
            var responseTypesClient = new ResponseTypesClient();
            var responseTypes = responseTypesClient.Get();

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

        public MediaPathType GetMediaPathType(int? mediaPathTypeId)
        {
            var mediaPathType = mediaPathTypeId != null 
                ? _mediaPathTypesClient.Get((int) mediaPathTypeId) 
                : _mediaPathTypesClient.Get(MediaPathTypeId.ImagesGeneral);

            return mediaPathType;
        }

        public string GetMediaPath(int? mediaPathTypeId)
        {
            return GetMediaPathType(mediaPathTypeId).Path;
        }

        public string GetMediaPathShortDescription(int? mediaPathTypeId)
        {
            return GetMediaPathType(mediaPathTypeId).ShortDescription;
        }

        public bool FileExists(string path, string filename)
        {
            var file = _mediaFilesClient.GetFromPath(path, filename);

            return (file != null);
        }

        public string DeleteSharedMediaFileLinks(Guid streamId)
        {
            try
            {
                var residentMediaFileIds = _residentMediaFilesClient.GetIdsForStreamId(streamId);
                foreach (var id in residentMediaFileIds)
                {
                    _residentMediaFilesClient.Delete(id);
                }

                var publicMediaFileIds = _publicMediaFilesClient.GetIdsForStreamId(streamId);
                foreach (var id in publicMediaFileIds)
                {
                    _publicMediaFilesClient.Delete(id);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
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
                case MediaPathTypeId.Ambient:
                case MediaPathTypeId.Cats:
                    isValid = name.Contains("mp4");
                    break;
            }

            return isValid;
        }

        public static bool IsMediaTypePreviewable(int mediaPathTypeId)
        {
            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.ImagesGeneral:
                case MediaPathTypeId.ImagesPersonal:
                case MediaPathTypeId.MatchingGameShapes:
                    return true;
                default:
                    return false;
            }
        }

        public static MediaPathType GetMediaPathTypeFromRawPath(string path, IEnumerable<MediaPathType> mediaPathTypes)
        {
            var folderName = path.Split(Path.DirectorySeparatorChar)
                .GetValue((path.Split(Path.DirectorySeparatorChar).Length - 2)).ToString();

            var mediaPathType = mediaPathTypes.Single(x => x.Path.Contains(folderName));

            return mediaPathType;
        }

        public IEnumerable<object> GetMediaPathTypeList(IEnumerable<MediaPathType> mediaPathTypes)
        {
            return mediaPathTypes.Select(x => new
            {
                x.Id,
                x.Category,
                x.Description,
                x.ShortDescription,
                IsSharable = true
            }).OrderBy(x => x.Description);
        }

        public IEnumerable<Resident> GetLinkedProfiles(Guid streamId)
        {
            var mediaResponseTypes = _publicMediaFilesClient.GetLinkedForStreamId(streamId).ToArray();
            var publicProfile = new List<Resident>();

            if (mediaResponseTypes.Any())
            {
                publicProfile.Add(new Resident
                {
                    Id = PublicProfileSource.Id,
                    FirstName = PublicProfileSource.Name
                });
            }

            var residentMedia = _residentMediaFilesClient.GetLinkedForStreamId(streamId);
            var residentProfiles = new List<Resident>();

            if (residentMedia != null)
            {
                var residents = residentMedia
                    .Select(x => new Resident
                    {
                        Id = x.Resident.Id,
                        FirstName = x.Resident.FirstName,
                        LastName = x.Resident.LastName
                    }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
                residentProfiles.AddRange(residents);
            }

            return publicProfile.Union(residentProfiles);
        }

        public static string GetThumbnail(byte[] binaryData)
        {
            return binaryData != null
                ? $"data:image/jpg;base64,{Convert.ToBase64String(binaryData)}"
                : null;
        }
    }
}
