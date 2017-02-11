using Keebee.AAT.ApiClient;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class ResidentRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
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

            var resident = _opsClient.GetResidentByNameGender(firstName, lastName, gender);
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
                var activeResidentEventLogs = _opsClient.GetActiveResidentEventLogsForResident(id).ToArray();
                foreach (var eventLog in activeResidentEventLogs)
                {
                    result = _opsClient.DeleteActiveResidentEventLog(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Active Resident Event Logs{Environment.NewLine}{result}");
                }

                // delete activity event logs
                var activityEventLogs = _opsClient.GetActivityEventLogsForResident(id).ToArray();
                foreach (var eventLog in activityEventLogs)
                {
                    result = _opsClient.DeleteActivityEventLog(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Activity Event Logs{Environment.NewLine}{result}");
                }

                // delete interactive activity event logs
                var interactiveActivityEventLogs = _opsClient.GetInteractiveActivityEventLogsForResident(id).ToArray();
                foreach (var eventLog in interactiveActivityEventLogs)
                {
                    result = _opsClient.DeleteInteractiveActivityEventLog(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Interactive Activity Event Logs{Environment.NewLine}{result}");
                }

                // delete resident
                result = _opsClient.DeleteResident(id);

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
                case MediaPathTypeId.PersonalImages:
                case MediaPathTypeId.GeneralImages:
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

        public MediaFileSingle GetMediaFile(int id)
        {
            var mediaFile = _opsClient.GetResidentMediaFile(id);

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
                case MediaPathTypeId.PersonalImages:
                case MediaPathTypeId.GeneralImages:
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
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).Path
                : _opsClient.GetMediaPathType(MediaPathTypeId.GeneralImages).Path;
        }

        public string GetMediaPathShortDescription(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).ShortDescription
                : _opsClient.GetMediaPathType(MediaPathTypeId.GeneralImages).ShortDescription;
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

        public byte[] GetFile(string path, string filename)
        {
            var file = _opsClient.GetMediaFileStreamFromPath(path, filename);

            return file;
        }

        public string DeleteResidentMediaFile(int id)
        {
            return _opsClient.DeleteResidentMediaFile(id);
        }

        public IEnumerable<MediaFile> GetAvailableSharedMediaFiles(int residentId, int mediaPathTypeId)
        {
            var mediaSource = new MediaSourcePath();
            var mediaPath = GetMediaPath(mediaPathTypeId);
            var sharedPaths = _opsClient.GetMediaFilesForPath($@"{mediaSource.SharedLibrary}\{mediaPath}").ToArray();
            var responseTypeId = GetResponseTypeId(mediaPathTypeId);
            var existingLinkedMediaPaths = _opsClient.GetResidentMediaFilesForResidentResponseType(residentId, responseTypeId);
            IEnumerable<Guid> existingStreamIds = new List<Guid>();

            if (existingLinkedMediaPaths?.MediaResponseType != null)
            {
                if (existingLinkedMediaPaths.MediaResponseType.Paths.Any())
                {
                    existingStreamIds = existingLinkedMediaPaths
                        .MediaResponseType.Paths
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
