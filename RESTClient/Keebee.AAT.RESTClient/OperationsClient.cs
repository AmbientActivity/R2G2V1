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
        IEnumerable<Config> GetConfigs();
        Config GetConfig(int id);
        Config GetConfigByDescription(string desciption);
        ConfigDetail GetConfigDetail(int id);
        IEnumerable<ConfigDetail> GetConfigDetails();
        Config GetActiveConfigDetails();
        Config GetConfigWithDetails(int id);
        IEnumerable<PhidgetType> GetPhidgetTypes();
        IEnumerable<PhidgetStyleType> GetPhidgetStyleTypes();
        IEnumerable<ResponseType> GetResponseTypes();

        IEnumerable<Resident> GetResidents();
        Resident GetResident(int residentId);
        Profile GetResidentProfile(int residentId);
        Resident GetResidentByNameGender(string firstName, string lastName, string gender);
        bool ResidentProfileExists(int residentId);
        Profile GetGenericProfile();
        
        // media
        MediaFileSingle GetMediaFile(Guid streamId);
        IEnumerable<Media> GetMediaFilesForPath(string path);
        byte[] GetMediaFileStream(Guid streamId);

        IEnumerable<ActivityEventLog> GetActivityEventLogsForDate(string date);
        IEnumerable<GameEventLog> GetGameEventLogsForDate(string date);
        IEnumerable<RfidEventLog> GetRfidEventLogsForDate(string date);
        IEnumerable<ActivityEventLog> GetActivityEventLogsForConfig(int configId);
        IEnumerable<ActivityEventLog> GetActivityEventLogsForConfigDetail(int configDetailId);

        // POST
        int PostResident(ResidentEdit resident);
        int PostConfigDetail(ConfigDetailEdit configDetail);
        int PostConfig(ConfigEdit config);
        void PostActivityEventLog(ActivityEventLog activityEventLog);
        void PostGameEventLog(GameEventLog gameEventLog);
        void PostRfidEventLog(RfidEventLog rfidEventLog);
        void PostActivateConfig(int configId);

        // PATCH
        void PatchResident(int residentId, ResidentEdit resident);
        void PatchConfig(int configId, ConfigEdit configDetail);
        void PatchConfigDetail(int configDetailId, ConfigDetailEdit configDetail);

        // DELETE
        void DeleteConfig(int configId);
        void DeleteConfigDetail(int configDetailId);

        void DeleteResident(int residentId);
        void DeleteProfile(int profileId);
        void DeleteActivityEventLogsForResident(int residentId);
        void DeleteGameEventLogsForResident(int residentId);
        void DeleteRfidEventLogsForResident(int residentId);
    }

    public class OperationsClient : IOperationsClient
    {
        private const string UriBase = "http://localhost/Keebee.AAT.Operations/api/";

        #region urls

        // configurations
        private const string UrlConfigs = "configs";
        private const string UrlConfig = "configs/{0}";
        private const string UrlConfigByDescription = "configs?description={0}";
        private const string UrlConfigWithDetails = "configs/{0}/details";
        private const string UrlActiveConfigDetails = "configs/active/details";
        private const string UrlActivateConfig = "configs/{0}/activate";
        private const string UrlConfigDetail = "configdetails/{0}";
        private const string UrlConfigDetails = "configdetails";

        // phidget types
        private const string UrlPhidgetTypes = "phidgettypes";
        private const string UrlPhidgetStyleTypes = "phidgetstyletypes";

        // response types
        private const string UrlResponseTypes = "responsetypes";

        // residents
        private const string UrlResidents = "residents";
        private const string UrlResident = "residents/{0}";
        private const string UrlResidentDetails = "residents/{0}/details";
        private const string UrlResidentByNameGender = "residents?firstName={0}&lastName={1}&gender={2}";
        private const string UrlResidentProfile = "residents/{0}/profile";

        // profiles
        private const string UrlProfiles = "profiles";
        private const string UrlProfile = "profiles/{0}";
        private const string UrlProfileDetails = "profiles/{0}/details";

        // media files
        private const string UrlMediaFile = "mediafiles/{0}";
        private const string UrlMediaFilesForPath = "mediafiles?path={0}";
        private const string UrlMediaFileStream = "mediafilestreams/{0}";

        // activity event logs
        private const string UrlActivityEventLogs = "activityeventlogs";
        private const string UrlActivityEventLogsForResident = "activityeventlogs?residentId={0}";
        private const string UrlActivityEventLogsForDate = "activityeventlogs?date={0}";
        private const string UrlActivityEventLogsForConfig = "activityeventlogs?configId={0}";
        private const string UrlActivityEventLogsForConfigDetail = "activityeventlogs?configDetailId={0}";

        // game event logs
        private const string UrlGameEventLogs = "gameeventlogs";
        private const string UrlGameEventLogsForResident = "gameeventlogs?residentId={0}";
        private const string UrlGameEventLogsForDate = "gameeventlogs?date={0}";

        // rfid event logs
        private const string UrlRfidEventLogs = "rfideventlogs";
        private const string UrlRfidEventLogsForResident = "rfideventlogs?residentId={0}";
        private const string UrlRfidEventLogsForDate = "rfideventlogs?date={0}";

        #endregion

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

        // config
        public Config GetActiveConfigDetails()
        {
            var data = Get(UrlActiveConfigDetails);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public IEnumerable<Config> GetConfigs()
        {
            var data = Get(UrlConfigs);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var configs = serializer.Deserialize<ConfigList>(data).Configs;

            return configs;
        }
        
        public Config GetConfig(int configId)
        {
            var data = Get(string.Format(UrlConfig, configId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public Config GetConfigByDescription(string description)
        {
            var data = Get(string.Format(UrlConfigByDescription, description));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }

        public IEnumerable<ConfigDetail> GetConfigDetails()
        {
            var data = Get(UrlConfigDetails);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var configDetails = serializer.Deserialize<ConfigDetailList>(data).ConfigDetails;

            return configDetails;
        }

        public ConfigDetail GetConfigDetail(int configDetailId)
        {
            var data = Get(string.Format(UrlConfigDetail, configDetailId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var configDetail = serializer.Deserialize<ConfigDetail>(data);

            return configDetail;
        }

        public Config GetConfigWithDetails(int id)
        {
            var data = Get(string.Format(UrlConfigWithDetails, id));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<Config>(data);

            return config;
        }


        public IEnumerable<PhidgetType> GetPhidgetTypes()
        {
            var data = Get(UrlPhidgetTypes);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var phidgetTypes = serializer.Deserialize<PhidgetTypeList>(data).PhidgetTypes;

            return phidgetTypes;
        }

        public IEnumerable<PhidgetStyleType> GetPhidgetStyleTypes()
        {
            var data = Get(UrlPhidgetStyleTypes);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var phidgetTypes = serializer.Deserialize<PhidgetStyleTypeList>(data).PhidgetStyleTypes;

            return phidgetTypes;
        }

        public IEnumerable<ResponseType> GetResponseTypes()
        {
            var data = Get(UrlResponseTypes);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var responseTypes = serializer.Deserialize<ResponseTypeList>(data).ResponseTypes;

            return responseTypes;
        }


        // resident
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

        public Resident GetResidentDetails(int residentId)
        {
            var data = Get(string.Format(UrlResidentDetails, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public Resident GetResidentByNameGender(string firstName, string lastName, string gender)
        {
            var data = Get(string.Format(UrlResidentByNameGender, firstName, lastName, gender));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public Profile GetResidentProfile(int residentId)
        {
            var data = Get(string.Format(UrlResidentProfile, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var profile = serializer.Deserialize<Profile>(data);

            return profile;
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


        // media
        public IEnumerable<ActivityEventLog> GetActivityEventLogsForDate(string date)
        {
            var data = Get(string.Format(UrlActivityEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<GameEventLog> GetGameEventLogsForDate(string date)
        {
            var data = Get(string.Format(UrlGameEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var gameEventLogs = serializer.Deserialize<GameEventLogList>(data).GameEventLogs;

            return gameEventLogs;
        }

        public IEnumerable<RfidEventLog> GetRfidEventLogsForDate(string date)
        {
            var data = Get(string.Format(UrlRfidEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var rfidEventLogs = serializer.Deserialize<RfidEventLogList>(data).RfidEventLogs;

            return rfidEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetActivityEventLogsForConfig(int configId)
        {
            var data = Get(string.Format(UrlActivityEventLogsForConfig, configId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetActivityEventLogsForConfigDetail(int configDetailId)
        {
            var data = Get(string.Format(UrlActivityEventLogsForConfigDetail, configDetailId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public MediaFileSingle GetMediaFile(Guid streamId)
        {
            var data = Get(string.Format(UrlMediaFile, streamId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaFile = serializer.Deserialize<MediaFileSingle>(data);

            return mediaFile;
        }

        public IEnumerable<Media> GetMediaFilesForPath(string path)
        {
            var data = Get(string.Format(UrlMediaFilesForPath, path));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaList = serializer.Deserialize<MediaList>(data).Media;

            return mediaList;
        }

        public byte[] GetMediaFileStream(Guid streamId)
        {
            var data = GetBytes(string.Format(UrlMediaFileStream, streamId));

            return data;
        }

        // POST

        public int PostConfig(ConfigEdit config)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(config);

            return Post(UrlConfigs, el);
        }

        public int PostConfigDetail(ConfigDetailEdit configDetail)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(configDetail);

            return Post(UrlConfigDetails, el);
        }

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

        public void PostActivateConfig(int configId)
        {
            Post(string.Format(UrlActivateConfig, configId), string.Empty);
        }

        // PATCH
        public void PatchResident(int residentId, ResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            Patch(string.Format(UrlResident, residentId), el);
        }

        public void PatchConfig(int configId, ConfigEdit config)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(config);

            Patch(string.Format(UrlConfig, configId), el);
        }

        public void PatchConfigDetail(int configDetailId, ConfigDetailEdit configDetail)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(configDetail);

            Patch(string.Format(UrlConfigDetail, configDetailId), el);
        }

        // DELETE
        public void DeleteConfig(int configId)
        {
            Delete(string.Format(UrlConfig, configId));
        }

        public void DeleteConfigDetail(int configId)
        {
            Delete(string.Format(UrlConfigDetail, configId));
        }

        public void DeleteResident(int residentId)
        {
            Delete(string.Format(UrlResident, residentId));
        }

        public void DeleteProfile(int profileId)
        {
            Delete(string.Format(UrlProfile, profileId));
        }

        public void DeleteActivityEventLogsForResident(int residentId)
        {
            Delete(string.Format(UrlActivityEventLogsForResident, residentId));
        }

        public void DeleteGameEventLogsForResident(int residentId)
        {
            Delete(string.Format(UrlGameEventLogsForResident, residentId));
        }

        public void DeleteRfidEventLogsForResident(int residentId)
        {
            Delete(string.Format(UrlRfidEventLogsForResident, residentId));
        }

        // private
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

        private byte[] GetBytes(string url)
        {
            byte[] result = null;

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
                        result = response.Content.ReadAsByteArrayAsync().Result;
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
