using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Keebee.AAT.RESTClient
{
    public interface IOperationsClient
    {
        SystemEventLogger SystemEventLogger { set; }

        // GET
        IEnumerable<Response> GetAmbientResponses();
        IEnumerable<Resident> GetResidents();
        Resident GetResident(int residentId);
        IEnumerable<Resident> GetResidentsMedia();
        bool ResidentProfileExists(int residentId);
        Profile GetGenericProfile();
        Profile GetResidentProfile(int residentId);
        Config GetActiveConfigDetails();
        Config GetConfigDetails(int id);
        Profile GetProfileMedia(int profileId);
        IEnumerable<Response> GetProfileMediaForActivityResponseType(int profileId, int activityTypeId, int responseTypeId);

        IEnumerable<ActivityEventLog> GetActivityEventLogs(string date);
        IEnumerable<GameEventLog> GetGameEventLogs(string date);
        IEnumerable<RfidEventLog> GetRfidEventLogs(string date);

        // POST
        int PostResident(ResidentEdit resident);
        void PostActivityEventLog(ActivityEventLog activityEventLog);
        void PostGameEventLog(GameEventLog gameEventLog);
        void PostRfidEventLog(RfidEventLog rfidEventLog);

        // PATCH
        void PatchResident(int residentId, ResidentEdit resident);

        // DELETE
        void DeleteResident(int residentId);
    }

    public class OperationsClient : IOperationsClient
    {
        private const string UriBase = "http://localhost/Keebee.AAT.Operations/api/";

        private const string UrlResidents = "residents";
        private const string UrlResident = "residents/{0}";
        private const string UrlResidentProfile = "residents/{0}/profile";
        private const string UrlResidentsMedia = "residents/media";
        private const string UrlResidentMedia = "residents/{0}/media";
        private const string UrlProfiles = "profiles";
        private const string UrlProfile = "profiles/{0}";
        private const string UrlProfileDetails = "profiles/{0}/details";
        private const string UrlProfileMedia = "profiles/{0}/media";
        private const string UrlConfigDetails = "configs/{0}/details";
        private const string UrlActiveConfigDetails = "configs/active/details";
        private const string UrlProfileMediaForActivityResponseType = "profiles/{0}/media?activityTypeId={1}&responseTypeId={2}";
        private const string UrlAmbientResponses = "ambientresponses";
        private const string UrlActivityEventLogs = "activityeventlogs";
        private const string UrlGameEventLogs = "gameeventlogs";
        private const string UrlRfidEventLogs = "rfideventlogs";

        private const string UrlActivityEventLogsForDate = "activityeventlogs?date={0}";
        private const string UrlGameEventLogsForDate = "gameeventlogs?date={0}";
        private const string UrlRfidEventLogsForDate = "rfideventlogs?date={0}";

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        private readonly Uri _uriBase;

        public OperationsClient()
        {
            _uriBase = new Uri(UriBase);
        }

        // GET
        public IEnumerable<Response> GetAmbientResponses()
        {
            var data = Get(UrlAmbientResponses);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var ambientDetails = serializer.Deserialize<ResponseList>(data).AmbientResponses;

            return ambientDetails;
        } 

        public IEnumerable<Resident> GetResidents()
        {
            var data = Get(UrlResidents);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residents = serializer.Deserialize<ResidentList>(data).Residents.ToList();

            return residents;
        }

        public Resident GetResident(int residentId)
        {
            var data = Get(string.Format(UrlResident, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public IEnumerable<Resident> GetResidentsMedia()
        {
            var data = Get(UrlResidentsMedia);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residents = serializer.Deserialize<ResidentList>(data).Residents.ToList();

            return residents;
        }

        public Resident GetResidentMedia(int residentId)
        {
            var data = Get(string.Format(UrlResidentMedia, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public bool ResidentProfileExists(int residentId)
        {
            return Exists(string.Format(UrlResidentProfile, residentId));
        }

        public IEnumerable<Profile> GetProfiles()
        {
            var data = Get(UrlProfiles);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profiles = serializer.Deserialize<ProfileList>(data).Profiles;

            return profiles;
        }

        public Profile GetProfile(int profileId)
        {
            var data = Get(string.Format(UrlProfile, profileId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);

            return profile;
        }

        public Profile GetGenericProfile()
        {
            var data = Get(string.Format(UrlProfileDetails, ProfileId.Generic));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);
            profile.ResidentId = 0;

            return profile;
        }

        public Profile GetResidentProfile(int residentId)
        {
            var data = Get(string.Format(UrlResidentProfile, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);

            return profile;
        }

        public Config GetActiveConfigDetails()
        {
            var data = Get(UrlActiveConfigDetails);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Config GetConfigDetails(int id)
        {
            var data = Get(string.Format(UrlConfigDetails, id));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Profile GetProfileMedia(int profileId)
        {
            var data = Get(string.Format(UrlProfileMedia, profileId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);

            return profile;
        }

        public IEnumerable<Response> GetProfileMediaForActivityResponseType(int profileId, int activityTypeId, int responseTypeId)
        {
            var data = Get(string.Format(UrlProfileMediaForActivityResponseType, profileId, activityTypeId, responseTypeId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var details = serializer.Deserialize<ConfigDetailList>(data)
                .ConfigDetails.Single()
                .ResponseType.Responses;

            return details;
        }

        public IEnumerable<ActivityEventLog> GetActivityEventLogs(string date)
        {
            var data = Get(string.Format(UrlActivityEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<GameEventLog> GetGameEventLogs(string date)
        {
            var data = Get(string.Format(UrlGameEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var gameEventLogs = serializer.Deserialize<GameEventLogList>(data).GameEventLogs;

            return gameEventLogs;
        }

        public IEnumerable<RfidEventLog> GetRfidEventLogs(string date)
        {
            var data = Get(string.Format(UrlRfidEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var rfidEventLogs = serializer.Deserialize<RfidEventLogList>(data).RfidEventLogs;

            return rfidEventLogs;
        }

        // POST
        public int PostResident(ResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            return Post(UrlResidents, el);
        }

        public void PostActivityEventLog(ActivityEventLog activityEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(activityEventLog);

            Post(UrlActivityEventLogs, el);
        }

        public void PostGameEventLog(GameEventLog gameEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(gameEventLog);

            Post(UrlGameEventLogs, el);
        }

        public void PostRfidEventLog(RfidEventLog rfidEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(rfidEventLog);

            Post(UrlRfidEventLogs, el);
        }

        // PATCH
        public void PatchResident(int residentId, ResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            Patch(string.Format(UrlResident, residentId), el);
        }

        // DELETE
        public void DeleteResident(int residentId)
        {
            Delete(string.Format(UrlResident, residentId));
        }

        // private REST
        private string Get(string url)
        {
            string result = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uriBase;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        result = response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        throw new Exception(
                            $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                    }
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"RESTClient.Get: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }

            return result;
        }

        private bool Exists(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uriBase;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"RESTClient.Exists: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }

            return false;
        }

        private int Post(string url, string value)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // added additionally needed back slashes
                    value = $"\"{value.Replace("\"", "\\\"")}\"";

                    client.BaseAddress = _uriBase;

                    HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                    }

                    int newId;
                    var result = response.Content.ReadAsStringAsync().Result;
                    var isValid = int.TryParse(result, out newId);

                    if (isValid) return newId;
                    return -1;
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"RESTClient.Post: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }

            return -1;
        }

        private void Patch(string url, string value)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // added additionally needed back slashes
                    value = $"\"{value.Replace("\"", "\\\"")}\"";

                    client.BaseAddress = _uriBase;

                    var method = new HttpMethod("PATCH");
                    var uri = new Uri($@"{_uriBase}{url}");
                    var request = new HttpRequestMessage(method, uri)
                    {
                        Content = new StringContent(value, Encoding.UTF8, "application/json")
                    };

                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                    }
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"RESTClient.Patch: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }
        }

        private void Delete(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uriBase;

                    HttpResponseMessage response = client.DeleteAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"StatusCode: {response.StatusCode}{Environment.NewLine}Message: {response.Content}");
                    }
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"RESTClient.Delete: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }
        }
    }
}
