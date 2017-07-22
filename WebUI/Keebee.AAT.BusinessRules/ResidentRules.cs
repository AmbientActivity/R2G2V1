using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class ResidentRules : RulesBase
    {
        private readonly IResidentsClient _residentsClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        public ResidentRules()
        {
            _residentsClient = new ResidentsClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
            _mediaFilesClient = new MediaFilesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        // validation
        public List<string> Validate(string firstName, string lastName, string gender, bool addnew)
        {
            var msgs = new List<string>();

            if (string.IsNullOrEmpty(firstName))
                msgs.Add("First Name is required");

            if (string.IsNullOrEmpty(gender))
                msgs.Add("Gender is required");

            if (!addnew) return msgs.Count > 0 ? msgs : null;

            var resident = _residentsClient.GetByNameGender(firstName, lastName, gender);
            if (resident == null) return msgs.Count > 0 ? msgs : null;
            if (resident.Id == 0) return msgs.Count > 0 ? msgs : null;

            var g = (gender == "M") ? "male" : "female";
            var residentName = lastName != null ? $"{firstName} {lastName}" : firstName;

            msgs.Add($"A {g} resident with the name '{residentName}' already exists");

            return msgs.Count > 0 ? msgs : null;
        }

        public string DeleteResident(int id)
        {
            string result;

            try
            {               
                // delete active resident event logs
                var activeResidentEventLogsClient = new ActiveResidentEventLogsClient();
                var activeResidentEventLogs = activeResidentEventLogsClient.GetForResident(id).ToArray();
                foreach (var eventLog in activeResidentEventLogs)
                {
                    result = activeResidentEventLogsClient.Delete(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Active Resident Event Logs{Environment.NewLine}{result}");
                }

                // delete activity event logs
                var activityEventLogsClient = new ActivityEventLogsClient();
                var activityEventLogs = activityEventLogsClient.GetForResident(id).ToArray();
                foreach (var eventLog in activityEventLogs)
                {
                    result = activityEventLogsClient.Delete(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Activity Event Logs{Environment.NewLine}{result}");
                }

                // delete interactive activity event logs
                var interactiveActivityEventLogsClient = new InteractiveActivityEventLogsClient();
                var interactiveActivityEventLogs = interactiveActivityEventLogsClient.GetForResident(id).ToArray();
                foreach (var eventLog in interactiveActivityEventLogs)
                {
                    result = interactiveActivityEventLogsClient.Delete(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Interactive Activity Event Logs{Environment.NewLine}{result}");
                }

                // delete resident
                var residentsClient = new ResidentsClient();
                result = residentsClient.Delete(id);

                if (result != null)
                    throw new Exception($"Error deleting Resident{Environment.NewLine}{result}");
            }

            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;
        }

        public static string GetProfilePicture(byte[] bytes)
        {
            string profilePicture = null;
            if (bytes != null)
            {
                profilePicture = (bytes.Length == 0)
                    ? null
                    : $"data:image/jpg;base64,{Convert.ToBase64String(bytes)}";
            }

            return profilePicture;
        }

        public static string GetProfilePicturePlaceholder()
        {
            return $"data:image/jpg;base64,{ImagesBase64.ProfilePicturePlaceholder}";
        }

        public static string GetThumbnail(byte[] binaryData)
        {
            return binaryData != null
                ? $"data:image/jpg;base64,{Convert.ToBase64String(binaryData)}"
                : null;
        }

        public string AddMediaFileFromFilename(string filename, int residentId, MediaPathType mediaPathType, ResponseType responseType, DateTime dateAdded, bool isLinked, out MediaFileEdit newFile)
        {
            string errMsg;
            newFile = null;

            try
            {
                var mediaFile = GetMediaFile($@"{residentId}\{mediaPathType.Path}", filename);
                if (mediaFile == null) return $"Could not get StreamId for file <b>{filename}</b>";

                errMsg = AddMediaFile(mediaFile.StreamId, residentId, mediaPathType, responseType, dateAdded, isLinked, out newFile);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"ResidentRules.AddMediaFileFromFilename: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
        }

        public string AddMediaFile(Guid streamId, int residentId, MediaPathType mediaPathType, ResponseType responseType, DateTime dateAdded, bool isLinked, out MediaFileEdit mediaFileEdit)
        {
            string errMsg;
            mediaFileEdit = null;

            try
            {
                var reesidentMediaFile = new ResidentMediaFileEdit
                {
                    StreamId = streamId,
                    ResidentId = residentId,
                    ResponseTypeId = responseType.Id,
                    MediaPathTypeId = mediaPathType.Id,
                    IsLinked = isLinked,
                    DateAdded = dateAdded
                };

                int newId;
                errMsg = _residentMediaFilesClient.Post(reesidentMediaFile, out newId);
                if (!string.IsNullOrEmpty(errMsg)) return errMsg;

                var mediaFile = _mediaFilesClient.Get(streamId);

                byte[] thumb = null;
                if (responseType.ResponseTypeCategory.Id != ResponseTypeCategoryId.Audio)
                {
                    thumb = _thumbnailsClient.Get(streamId)?.Image;
                }

                mediaFileEdit = GetMediaFileEdit(mediaFile, newId, mediaPathType, dateAdded, isLinked, thumb);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"ResidentRules.AddMediaFile: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
        }

        public string DeleteMediaFile(int id, ResponseType responseType)
        {
            string errMsg = null;

            try
            {
                var residentMediaFile = _residentMediaFilesClient.Get(id);
                if (residentMediaFile?.MediaFile == null) throw new Exception($"Could not find PublicMediaFile for id: {id}");

                if (residentMediaFile.MediaFile.IsLinked)
                {
                    _residentMediaFilesClient.Delete(id);
                }
                else
                {
                    var mediaFilePath = GetMediaFilePath(id);
                    if (mediaFilePath == null) throw new Exception($"Could not find MediaFilePath for id: {id}");

                    // delete the link
                    errMsg = _residentMediaFilesClient.Delete(id);
                    if (errMsg.Length > 0) throw new Exception(errMsg);

                    // delete the file
                    errMsg = DeleteFile($@"{mediaFilePath.Path}\{mediaFilePath.Filename}");
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    if (responseType.ResponseTypeCategory.Id != ResponseTypeCategoryId.Audio)
                    {
                        errMsg = _thumbnailsClient.Delete(residentMediaFile.MediaFile.StreamId);
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                EventLogger.WriteEntry($"ResidentRules.DeleteMediaFile: {errMsg}", EventLogEntryType.Error);
            }

            return errMsg;
        }

        private MediaFilePath GetMediaFilePath(int id)
        {
            var mediaFile = _residentMediaFilesClient.Get(id);

            return mediaFile == null
                ? null
                : _mediaFilesClient.Get(mediaFile.MediaFile.StreamId);
        }
    }
}
