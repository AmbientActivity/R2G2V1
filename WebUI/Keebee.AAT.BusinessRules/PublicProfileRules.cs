using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class PublicProfileRules : RulesBase
    {
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        public PublicProfileRules()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaFilesClient = new MediaFilesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        public string AddMediaFileFromFilename(string filename, MediaPathType mediaPathType, ResponseType responseType, DateTime dateAdded, bool isLinked, out MediaFileModel newFile)
        {
            string errMsg;
            newFile = null;

            try
            {
                var mediaFile = GetMediaFile($@"{PublicProfileSource.Id}\{mediaPathType.Path}", filename);
                if (mediaFile == null) return $"Could not get StreamId for file <b>{filename}</b>";

                errMsg = AddMediaFile(mediaFile.StreamId, mediaPathType, responseType, dateAdded, isLinked, out newFile);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"PublicProfileRules.AddMediaFileFromFilename: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
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

        public string AddMediaFile(Guid streamId, MediaPathType mediaPathType, ResponseType responseType, DateTime dateAdded, bool isLinked, out MediaFileModel mediaFileModel)
        {
            string errMsg;
            mediaFileModel = null;

            try
            {
                var publicMediaFile = new PublicMediaFileEdit
                {
                    StreamId = streamId,
                    ResponseTypeId = responseType.Id,
                    MediaPathTypeId = mediaPathType.Id,
                    IsLinked = isLinked,
                    DateAdded = dateAdded
                };

                int newId;
                errMsg = _publicMediaFilesClient.Post(publicMediaFile, out newId);
                if (!string.IsNullOrEmpty(errMsg)) return errMsg;

                var mediaFile = _mediaFilesClient.Get(streamId);

                byte[] thumb = null;
                if (responseType.ResponseTypeCategory.Id != ResponseTypeCategoryId.Audio)
                {
                    thumb = _thumbnailsClient.Get(streamId)?.Image;
                }

                mediaFileModel = GetMediaFileModel(mediaFile, newId, mediaPathType, dateAdded, isLinked, thumb);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"PublicProfileRules.AddMediaFile: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
        }

        public string DeleteMediaFile(int id, ResponseType responseType)
        {
            string errMsg = null;

            try
            {
                var publicMediaFile = _publicMediaFilesClient.Get(id);
                if (publicMediaFile?.MediaFile == null) throw new Exception($"Could not find PublicMediaFile for id: {id}");

                if (publicMediaFile.MediaFile.IsLinked)
                {
                    _publicMediaFilesClient.Delete(id);
                }
                else
                {
                    var mediaFilePath = GetMediaFilePath(id);
                    if (mediaFilePath == null) throw new Exception($"Could not find MediaFilePath for id: {id}");

                    // delete the link
                    errMsg = _publicMediaFilesClient.Delete(id);
                    if (errMsg.Length > 0) throw new Exception(errMsg);

                    // delete the file
                    errMsg = DeleteFile($@"{mediaFilePath.Path}\{mediaFilePath.Filename}");
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    if (responseType.ResponseTypeCategory.Id != ResponseTypeCategoryId.Audio)
                    {
                        errMsg = _thumbnailsClient.Delete(publicMediaFile.MediaFile.StreamId);
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"PublicProfileRules.DeleteMediaFile: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
        }

        private MediaFilePath GetMediaFilePath(int id)
        {
            var mediaFile = _publicMediaFilesClient.Get(id);

            return mediaFile == null
                ? null
                : _mediaFilesClient.Get(mediaFile.MediaFile.StreamId);
        }
    }
}
