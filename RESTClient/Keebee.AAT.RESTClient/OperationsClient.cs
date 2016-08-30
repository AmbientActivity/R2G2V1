using Keebee.AAT.Constants;
using Keebee.AAT.EventLogging;
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
        EventLogger EventLogger { set; }

        // GET
        IEnumerable<Response> GetAmbientResponses();
        IEnumerable<Resident> GetResidents();
        IEnumerable<Resident> GetResidentsMedia();
        bool ResidentProfileExists(int residentId);
        Profile GetGenericProfile();
        Profile GetResidentProfile(int residentId);
        Profile GetProfileMedia(int profileId);
        IEnumerable<Response> GetProfileResponses(int responseId);

        IEnumerable<EventLog> GetEventLogs(string date);
        IEnumerable<GamingEventLog> GetGamingEventLogs(string date);

        // POST
        void PostEventLog(EventLog eventLog);
    }

    public class OperationsClient : IOperationsClient
    {
        private const string UriBase = "http://localhost/Keebee.AAT.Operations/api/";

        private const string UrlResidents = "residents";
        private const string UrlResidentProfile = "residents/{0}/profile";
        private const string UrlResidentsMedia = "residents/media";
        private const string UrlResidentMedia = "residents/{0}/media";
        private const string UrlProfile = "profiles/{0}";
        private const string UrlProfileDetails = "profiles/{0}/details";
        private const string UrlProfileMedia = "profiles/{0}/media";
        private const string UrlResidentProfileDetails = "residents/{0}/profile/details";
        private const string UrlProfileResponses = "profiledetails/{0}/responses";
        private const string UrlAmbientResponses = "ambientresponses";
        private const string UrlEventLogs = "eventlogs";
        private const string UrlGamingEventLogs = "gamingeventlogs";

        private const string UrlEventLogsForDate = "eventlogs?date={0}";
        private const string UrlGamingEventLogsForDate = "gamingeventlogs?date={0}";

        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
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
            var ambientDetails = serializer.Deserialize<ProfileDetail>(data).AmbientResponses;

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

        public Profile GetProfile()
        {
            var data = Get(UrlProfile);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);

            return profile;
        }

        public Profile GetGenericProfile()
        {
            var data = Get(string.Format(UrlProfileDetails, UserProfile.Generic));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);
            profile.ResidentId = 0;

            return profile;
        }

        public Profile GetResidentProfile(int residentId)
        {
            var data = Get(string.Format(UrlResidentProfileDetails, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);
            profile.ResidentId = residentId;

            return profile;
        }

        public Profile GetProfileMedia(int profileId)
        {
            var data = Get(string.Format(UrlProfileMedia, profileId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);

            return profile;
        }

        public IEnumerable<Response> GetProfileResponses(int profileDetailId)
        {
            var data = Get(string.Format(UrlProfileResponses, profileDetailId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var details = serializer.Deserialize<ProfileDetail>(data).Responses;

            return details;
        }

        public IEnumerable<EventLog> GetEventLogs(string date)
        {
            var data = Get(string.Format(UrlEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var eventLogs = serializer.Deserialize<EventLogList>(data).EventLogs;

            return eventLogs;
        }

        public IEnumerable<GamingEventLog> GetGamingEventLogs(string date)
        {
            var data = Get(string.Format(UrlGamingEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var eventLogs = serializer.Deserialize<GaminingEventLogList>(data).GamingEventLogs;

            return eventLogs;
        }

        // POST
        public void PostEventLog(EventLog eventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(eventLog);

            Post(UrlEventLogs, el);
        }

        public void PostGamingEventLog(GamingEventLog gamingEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(gamingEventLog);

            Post(UrlGamingEventLogs, el);
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
                _eventLogger?.WriteEntry($"RESTClient.Get: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
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
                _eventLogger?.WriteEntry($"RESTClient.Exists: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }

            return false;
        }

        private void Post(string url, string value)
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
                }
            }

            catch (Exception ex)
            {
                _eventLogger?.WriteEntry($"RESTClient.Post: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }
        }
    }
}
