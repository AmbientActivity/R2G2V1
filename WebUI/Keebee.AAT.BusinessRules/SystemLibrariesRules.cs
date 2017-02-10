using System;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class SystemLibrariesRules
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

        public MediaPathType GetMediaPathType(int? mediaPathTypeId)
        {
            MediaPathType mediaPathType = null;
            var mediaSourcePath = new MediaSourcePath();

            if (mediaPathTypeId != null)
            {
                if (mediaPathTypeId > 0)
                    mediaPathType = _opsClient.GetMediaPathType((int) mediaPathTypeId);
                else
                {
                    switch (mediaPathTypeId)
                    {
                        case MediaPathTypeId.AmbientVideos:
                            mediaPathType = new MediaPathType
                            {
                                Id = MediaPathTypeId.AmbientVideos,
                                Description = SystemMediaPathType.AmbientDescription,
                                ShortDescription = SystemMediaPathType.AmbientShortDescription,
                                Path = mediaSourcePath.Ambient
                            };
                            break;
                        case MediaPathTypeId.CatsVideos:
                            mediaPathType = new MediaPathType
                            {
                                Id = MediaPathTypeId.CatsVideos,
                                Description = SystemMediaPathType.CatsDescription,
                                ShortDescription = SystemMediaPathType.CatsShortDescription,
                                Path = mediaSourcePath.Cats
                            };
                            break;
                    }
                }
            }
            else
            {
                mediaPathType = _opsClient.GetMediaPathType(MediaPathTypeId.GeneralImages);
            }

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
                case MediaPathTypeId.AmbientVideos:
                case MediaPathTypeId.CatsVideos:
                    isValid = name.Contains("mp4");
                    break;
            }

            return isValid;
        }

        public static bool IsMediaTypePreviewable(int mediaPathTypeId)
        {
            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.GeneralImages:
                case MediaPathTypeId.PersonalImages:
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

            var sharedMedia = _opsClient.GetMediaFilesForPath(mediaSourcePath.SharedLibrary);
            var ambientMedia = _opsClient.GetMediaFilesForPath(mediaSourcePath.Ambient);
            var catsMedia = _opsClient.GetMediaFilesForPath(mediaSourcePath.Cats);

            var sharedFileList = sharedMedia.SelectMany(p =>
            {
                var mediaPathType = GetMediaPathTypeFromRawPath(p.Path, mediaPathTypes);

                return p.Files.Select(f =>
                {
                    var numLinkedResidentProfiles = _opsClient.GetResidentMediaFileIdsForStreamId(f.StreamId).Length;
                    var numLinkedPublicProfiles = _opsClient.GetPublicMediaFileIdsForStreamId(f.StreamId).Length;
                    return new
                    {
                        f.StreamId,
                        f.Filename,
                        f.FileSize,
                        f.FileType,
                        mediaPathType.Path,
                        MediaPathTypeId = mediaPathType.Id,
                        NumLinkedProfiles = numLinkedResidentProfiles + numLinkedPublicProfiles
                    };
                });
            });

            var ambientFileList = ambientMedia.SelectMany(p => p.Files.Select(f => new
            {
                f.StreamId,
                f.Filename,
                f.FileSize,
                f.FileType,
                Path = mediaSourcePath.Ambient,
                MediaPathTypeId = MediaPathTypeId.AmbientVideos,
                NumLinkedProfiles = 0
            }));

            var catsFileList = catsMedia.SelectMany(p => p.Files.Select(f => new
            {
                f.StreamId,
                f.Filename,
                f.FileSize,
                f.FileType,
                Path = mediaSourcePath.Ambient,
                MediaPathTypeId = MediaPathTypeId.CatsVideos,
                NumLinkedProfiles = 0
            }));

            return sharedFileList.Union(ambientFileList).Union(catsFileList);
        }

        public IEnumerable<object> GetMediaPathTypeList(IEnumerable<MediaPathType> mediaPathTypes)
        {
            var mediaSourcePath = new MediaSourcePath();

            return mediaPathTypes.Select(x => new
            {
                x.Id,
                x.Description,
                x.ShortDescription,
                IsPreviewable = PublicProfileRules.IsMediaTypePreviewable(x.Id),
                IsSharable = true
            }).Union(new List<object>
            {
                new
                {
                    Id = MediaPathTypeId.AmbientVideos,
                    Description = SystemMediaPathType.AmbientDescription,
                    ShortDescription = SystemMediaPathType.AmbientShortDescription,
                    Path = mediaSourcePath.Ambient,
                    IsPreviewable = false,
                    IsSharable = false
                },
                new
                {
                    Id = MediaPathTypeId.CatsVideos,
                    Description = SystemMediaPathType.CatsDescription,
                    ShortDescription = SystemMediaPathType.CatsShortDescription,
                    Path = mediaSourcePath.Cats,
                    IsPreviewable = false,
                    IsSharable = false
                }
            });
        }
    }
}
