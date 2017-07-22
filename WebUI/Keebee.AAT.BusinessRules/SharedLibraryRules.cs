using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.ThumbnailGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class SharedLibraryRules : RulesBase
    {
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly MediaSourcePath _mediaSourcePath;

        public SharedLibraryRules()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
            _mediaSourcePath = new MediaSourcePath();
        }

        public string AddFile(string filename, MediaPathType mediaPathType, out SharedMediaFileEdit mediaFileEdit)
        {
            string errMsg = null;
            mediaFileEdit = null;

            try
            {
                var mediaFile = GetMediaFile($@"{_mediaSourcePath.SharedLibrary}\{mediaPathType.Path}", filename);
                if (mediaFile == null) return $"Could not get MediaFile for file <b>{filename}</b>";

                // generate thumbnail if not 'sudio' media type
                byte[] thumb = null;
                var thumbnailGenerator = new ThumbnailGenerator();
                if (mediaPathType.Category != MediaPathTypeCategoryDescription.Audio)
                    thumb = thumbnailGenerator.Generate(mediaFile.StreamId, out errMsg);

                mediaFileEdit = GetMediaFileEdit(mediaFile, mediaPathType, thumb);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"SharedLibraryRules.AddFile: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
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

        public static MediaPathType GetMediaPathTypeFromRawPath(string path, IEnumerable<MediaPathType> mediaPathTypes)
        {
            var folderName = path.Split(Path.DirectorySeparatorChar)
                .GetValue((path.Split(Path.DirectorySeparatorChar).Length - 2)).ToString();

            var mediaPathType = mediaPathTypes.Single(x => x.Path.Contains(folderName));

            return mediaPathType;
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
                        LastName = x.Resident.LastName,
                        ProfilePicture = x.Resident.ProfilePicture
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

        private static SharedMediaFileEdit GetMediaFileEdit(
                MediaFile mediaFile,
                MediaPathType mediaPathType,
                byte[] thumb)
        {
            return new SharedMediaFileEdit
            {
                StreamId = mediaFile.StreamId,
                Filename = mediaFile.Filename.Replace($".{mediaFile.FileType}", string.Empty),
                FileSize = mediaFile.FileSize,
                FileType = mediaFile.FileType.ToUpper(),
                Path = mediaPathType.Path,
                MediaPathTypeId = mediaPathType.Id,
                NumLinkedProfiles = mediaFile.NumLinkedProfiles,
                Thumbnail = SharedLibraryRules.GetThumbnail(thumb)
            };
        }
    }
}
