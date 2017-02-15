﻿using Keebee.AAT.ApiClient;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class SharedLibraryRules
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
                ? _opsClient.GetMediaPathType((int) mediaPathTypeId) 
                : _opsClient.GetMediaPathType(MediaPathTypeId.ImagesGeneral);

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
            var file = _opsClient.GetMediaFileFromPath(path, filename);

            return (file != null);
        }

        public string DeleteSharedMediaFileLinks(Guid streamId)
        {
            try
            {
                var residentMediaFileIds = _opsClient.GetResidentMediaFileIdsForStreamId(streamId);
                foreach (var id in residentMediaFileIds)
                {
                    _opsClient.DeleteResidentMediaFile(id);
                }

                var publicMediaFileIds = _opsClient.GetPublicMediaFileIdsForStreamId(streamId);
                foreach (var id in publicMediaFileIds)
                {
                    _opsClient.DeletePublicMediaFile(id);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
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

        private static MediaPathType GetMediaPathTypeFromRawPath(string path, IEnumerable<MediaPathType> mediaPathTypes)
        {
            var folderName = path.Split(Path.DirectorySeparatorChar)
                .GetValue((path.Split(Path.DirectorySeparatorChar).Length - 2)).ToString();

            var mediaPathType = mediaPathTypes.Single(x => x.Path.Contains(folderName));

            return mediaPathType;
        }

        public IEnumerable<object> GetFileList(IEnumerable<MediaPathType> mediaPathTypes)
        {
            var mediaSourcePath = new MediaSourcePath();
            
            // public linked media (for tooltip total linked)
            var publicFiles = new LinkedMediaFile[0];
            var publicMedia = _opsClient.GetLinkedPublicMedia();
            if (publicMedia != null)
            {
                publicFiles = publicMedia.MediaFiles
                    .SelectMany(x => x.Paths)
                    .SelectMany(x => x.Files).ToArray();
            }

            // resident linked media (for tooltip total linked)
            var residentFilesLinked = new LinkedMediaFile[0];
            var residentMediaLinked = _opsClient.GetLinkedResidentMedia();
            if (residentMediaLinked != null)
            {
                residentFilesLinked = residentMediaLinked
                    .SelectMany(x => x.MediaResponseTypes)
                    .SelectMany(x => x.Paths)
                    .SelectMany(x => x.Files).ToArray();
            }

            // shared library
            var sharedMedia = _opsClient.GetMediaFilesForPath(mediaSourcePath.SharedLibrary);   
            var sharedFileList = sharedMedia.SelectMany(p =>
            {
                var mediaPathType = GetMediaPathTypeFromRawPath(p.Path, mediaPathTypes);

                return p.Files.Select(f =>
                {
                    var numLinkedResidentProfiles = residentFilesLinked.Count(x => x.StreamId == f.StreamId);
                    var isLinkedToPublicProfile = publicFiles.Any(x => x.StreamId == f.StreamId);

                    return new
                    {
                        f.StreamId,
                        f.Filename,
                        f.FileSize,
                        f.FileType,
                        mediaPathType.Path,
                        MediaPathTypeId = mediaPathType.Id,
                        NumLinkedProfiles = numLinkedResidentProfiles + (isLinkedToPublicProfile ? 1 : 0)
                    };
                });
            });

            return sharedFileList;
        }

        public IEnumerable<object> GetMediaPathTypeList(IEnumerable<MediaPathType> mediaPathTypes)
        {
            return mediaPathTypes.Select(x => new
            {
                x.Id,
                x.Description,
                x.ShortDescription,
                x.IsPreviewable,
                IsSharable = true
            }).OrderBy(x => x.Description);
        }

        public IEnumerable<Resident> GetLinkedProfiles(Guid streamId)
        {
            var publicMedia = _opsClient.GetPublicMediaFilesForStreamId(streamId);
            var publicProfile = new List<Resident>();

            if (publicMedia != null)
            {
                publicProfile.Add(new Resident
                {
                    Id = PublicProfileSource.Id,
                    FirstName = PublicProfileSource.Name
                });
            }

            var residentMedia = _opsClient.GetLinkedResidentMediaForStreamId(streamId);
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
    }
}
