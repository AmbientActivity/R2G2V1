using Keebee.AAT.RESTClient;
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
                // delete rfid event logs
                var rfidEventLogs = _opsClient.GetRfidEventLogsForResident(id).ToArray();
                foreach (var eventLog in rfidEventLogs)
                {
                    result = _opsClient.DeleteRfidEventLog(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting RFID Event Logs{Environment.NewLine}{result}");
                }

                // delete activity event logs
                var activityEventLogs = _opsClient.GetActivityEventLogsForResident(id).ToArray();
                foreach (var eventLog in activityEventLogs)
                {
                    result = _opsClient.DeleteActivityEventLog(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Activity Event Logs{Environment.NewLine}{result}");
                }

                // delete game event logs
                var gameEventLogs = _opsClient.GetGameEventLogsForResident(id).ToArray();
                foreach (var eventLog in gameEventLogs)
                {
                    result = _opsClient.DeleteGameEventLog(eventLog.Id);

                    if (result.Length > 0)
                        throw new Exception($"Error deleting Game Event Logs{Environment.NewLine}{result}");
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
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    isValid = name.Contains("jpg") || name.Contains("jpeg") || name.Contains("png") || name.Contains("gif");
                    break;
                case MediaPathTypeId.Videos:
                    isValid = name.Contains("mp4");
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.Sounds:
                    isValid = name.Contains("mp3");
                    break;
                case MediaPathTypeId.Shapes:
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
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    extensions = "*.jpg,*.png,*.gif";
                    break;
                case MediaPathTypeId.Videos:
                    extensions = "*.mp4";
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.Sounds:
                    extensions = "*.mp3";
                    break;
                case MediaPathTypeId.Shapes:
                    extensions = "*.png";
                    break;
            }

            return extensions;
        }

        public string GetMediaPathType(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).Description
                : _opsClient.GetMediaPathType(MediaPathTypeId.Images).Description;
        }

        public static int GetResponseTypeId(int mediaPathTypeId)
        {
            var responseTypeId = -1;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    responseTypeId = ResponseTypeId.SlidShow;
                    break;
                case MediaPathTypeId.Videos:
                    responseTypeId = ResponseTypeId.Television;
                    break;
                case MediaPathTypeId.Music:
                    responseTypeId = ResponseTypeId.Radio;
                    break;
                case MediaPathTypeId.Shapes:
                case MediaPathTypeId.Sounds:
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
    }
}
