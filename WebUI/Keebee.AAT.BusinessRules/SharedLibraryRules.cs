using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.ThumbnailGeneration;
using Keebee.AAT.SystemEventLogging;
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

        public string AddFile(string filename, MediaPathType mediaPathType, out SharedMediaFileModel mediaFileModel)
        {
            string errMsg = null;
            mediaFileModel = null;

            try
            {
                var mediaFile = GetMediaFile($@"{_mediaSourcePath.SharedLibrary}\{mediaPathType.Path}", filename);
                if (mediaFile == null) return $"Could not get MediaFile for file <b>{filename}</b>";

                // generate thumbnail if not 'sudio' media type
                byte[] thumb = null;
                if (mediaPathType.Category != MediaPathTypeCategoryDescription.Audio)
                {
                    var thumbnailGenerator = new ThumbnailGenerator();
                    thumb = thumbnailGenerator.Generate(mediaFile.StreamId, out errMsg);
                }

                mediaFileModel = GetMediaFileModel(mediaFile, mediaPathType, thumb);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"SharedLibraryRules.AddFile: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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

        public static MediaPathType GetMediaPathTypeFromRawPath(string path, MediaPathType[] mediaPathTypes)
        {
            var folderName = path.Split(Path.DirectorySeparatorChar)
                .GetValue((path.Split(Path.DirectorySeparatorChar).Length - 2)).ToString();

            MediaPathType mediaPathType = null;
            if (mediaPathTypes.Any(x => x.Path.Contains(folderName)))
                mediaPathType = mediaPathTypes.Single(x => x.Path.Contains(folderName));

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

        private static SharedMediaFileModel GetMediaFileModel(
                MediaFile mediaFile,
                MediaPathType mediaPathType,
                byte[] thumb)
        {
            return new SharedMediaFileModel
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
