using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class ResidentRules
    {
        private readonly IResidentsClient _residentsClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;

        public ResidentRules()
        {
            _residentsClient = new ResidentsClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
            _mediaFilesClient = new MediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
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
            var residentName = (lastName.Length > 0) ? $"{firstName} {lastName}" : firstName;

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

                if (result.Length > 0)
                    throw new Exception($"Error deleting Resident{Environment.NewLine}{result}");
            }

            catch (Exception ex)
            {
                return ex.Message;
            }

            return result;
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
                case MediaPathTypeId.HomeMovies:
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

        public MediaFilePath GetMediaFile(int id)
        {
            var mediaFile = _residentMediaFilesClient.Get(id);

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
                case MediaPathTypeId.ImagesPersonal:
                case MediaPathTypeId.ImagesGeneral:
                    extensions = "*.jpg,*.png,*.gif";
                    break;
                case MediaPathTypeId.HomeMovies:
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

        public static int GetResponseTypeId(int mediaPathTypeId)
        {
            var responseTypeId = -1;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.ImagesPersonal:
                case MediaPathTypeId.ImagesGeneral:
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

        public byte[] GetFile(string path, string filename)
        {
            var file = _mediaFilesClient.GetFileStreamFromPath(path, filename);

            return file;
        }

        public IEnumerable<MediaFile> GetAvailableSharedMediaFiles(int residentId, int mediaPathTypeId)
        {
            var mediaSource = new MediaSourcePath();
            var mediaPath = GetMediaPath(mediaPathTypeId);

            var sharedPaths = _mediaFilesClient.GetForPath($@"{mediaSource.SharedLibrary}\{mediaPath}").ToArray();
            var responseTypeId = GetResponseTypeId(mediaPathTypeId);

            var mediaResponseType = _residentMediaFilesClient.GetForResidentResponseType(residentId, responseTypeId);
            IEnumerable<Guid> existingStreamIds = new List<Guid>();

            if (mediaResponseType.Paths.Any())
            {
                existingStreamIds = mediaResponseType
                    .Paths
                    .SelectMany(f => f.Files)
                    .Where(f => f.IsLinked)
                    .Select(f => f.StreamId);
            }

            var availableFiles = sharedPaths
                .SelectMany(f => f.Files)
                .Where(f => !existingStreamIds.Contains(f.StreamId));

            return availableFiles;
        }

        public string GetNoAvailableSharedMediaMessage(int mediaPathTypeId)
        {
            var mediaPathType = GetMediaPathShortDescription(mediaPathTypeId);

            var isAre = mediaPathType.EndsWith("s") ? "are" : "is";
            var message = $"All available {mediaPathType} {isAre} already included in this profile.";

            return message;
        }
    }
}
