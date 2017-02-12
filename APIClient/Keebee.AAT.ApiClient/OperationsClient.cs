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

namespace Keebee.AAT.ApiClient
{
    public interface IApiClient
    {
        SystemEventLogger SystemEventLogger { set; }

        // GET
        // config
        IEnumerable<Config> GetConfigs();
        Config GetConfig(int id);
        Config GetActiveConfig();
        Config GetConfigByDescription(string desciption);
        ConfigDetail GetConfigDetail(int id);
        IEnumerable<ConfigDetail> GetConfigDetails();
        Config GetActiveConfigDetails();
        Config GetConfigWithDetails(int id);
        IEnumerable<PhidgetType> GetPhidgetTypes();
        IEnumerable<PhidgetStyleType> GetPhidgetStyleTypes();
        IEnumerable<ResponseType> GetResponseTypes();


        // user
        IEnumerable<User> GetUsers();
        User GetUser(int userId);
        User GetUserByUsername(string username);


        // user roles
        IEnumerable<UserRole> GetUserRoles();
        UserRole GetUserRole(int userRoleId);
        IEnumerable<UserRoleSingle> GetRolesByUser(int userId);


        // resident
        IEnumerable<Resident> GetResidents();
        Resident GetResident(int residentId);
        Resident GetResidentWithMedia(int residentId);
        Resident GetResidentByNameGender(string firstName, string lastName, string gender);
        bool ResidentExists(int residentId);
        ActiveResident GetActiveResident();


        // media files
        MediaFileSingle GetMediaFile(Guid streamId);
        IEnumerable<Media> GetMediaFilesForPath(string path);
        MediaFileSingle GetMediaFileFromPath(string path, string filename);
        byte[] GetMediaFileStream(Guid streamId);
        byte[] GetMediaFileStreamFromPath(string path, string filename);


        // media path types
        IEnumerable<MediaPathType> GetMediaPathTypes();
        MediaPathType GetMediaPathType(int mediaPathTypeId);
        IEnumerable<MediaPathType> GetMediaPathTypes(bool isSystem);


        // public media files
        PublicMediaFile GetPublicMediaFile(int id);
        PublicMedia GetPublicMediaFiles();
        PublicMediaResponseType GetPublicMediaFilesForResponseType(int responseTypeId);
        PublicMedia GetPublicMediaFilesForMediaPathType(int mediaPathTypeId);
        IEnumerable<PublicMediaFile> GetPublicMediaFilesForStreamId(Guid streamId);
        PublicMediaFile GetPublicMediaFileForResponseTypeFilename(int responseTypeId, string filename);
        int[] GetPublicMediaFileIdsForStreamId(Guid streamId);


        // resident media files
        IEnumerable<ResidentMedia> GetResidentMediaFiles();
        ResidentMediaFile GetResidentMediaFile(int id);
        ResidentMedia GetResidentMediaFilesForResident(int residentId);
        ResidentMediaResponseType GetResidentMediaFilesForResidentResponseType(int residentId, int responseTypeId);
        IEnumerable<ResidentMediaResponseType> GetLinkedResidentMedia();
        IEnumerable<ResidentMediaResponseType> GetLinkedResidentMediaForStreamId(Guid streamId);
        int[] GetResidentMediaFileIdsForStreamId(Guid streamId);


        // event logs
        IEnumerable<ActivityEventLog> GetActivityEventLogsForDate(string date);
        IEnumerable<ActiveResidentEventLog> GetActiveResidentEventLogsForDate(string date);
        IEnumerable<InteractiveActivityEventLog> GetInteractiveActivityEventLogsForDate(string date);
        IEnumerable<ActivityEventLog> GetActivityEventLogsForConfig(int configId);
        IEnumerable<ActivityEventLog> GetActivityEventLogsForConfigDetail(int configDetailId);
        IEnumerable<ActivityEventLog> GetActivityEventLogsForResident(int residentId);
        IEnumerable<ActiveResidentEventLog> GetActiveResidentEventLogsForResident(int residentId);
        IEnumerable<InteractiveActivityEventLog> GetInteractiveActivityEventLogsForResident(int residentId);

        // POST
        int PostUser(User user);
        int PostResident(ResidentEdit resident);
        int PostConfigDetail(ConfigDetailEdit configDetail);
        int PostConfig(ConfigEdit config);
        int PostActivityEventLog(ActivityEventLog activityEventLog);
        int PostInteractiveActivityEventLog(InteractiveActivityEventLog interactiveActivityEventLog);
        int PostActiveResidentEventLog(ActiveResidentEventLog activeResidentEventLog);
        int PostActivateConfig(int configId);
        int PostResidentMediaFile(ResidentMediaFileEdit residentMediaFile);
        int PostPublicMediaFile(PublicMediaFileEdit publicMediaFile);

        // PATCH
        void PatchResident(int residentId, ResidentEdit resident);
        void PatchActiveResident(ActiveResidentEdit resident);
        void PatchConfig(int configId, ConfigEdit configDetail);
        void PatchConfigDetail(int configDetailId, ConfigDetailEdit configDetail);
        void PatchUser(int userId, User user);

        // DELETE
        string DeleteUser(int userId);
        string DeleteResident(int residentId);
        void DeleteConfig(int configId);
        void DeleteConfigDetail(int configDetailId);
        string DeletePublicMediaFile(int mediaFileId);
        string DeleteActivityEventLog(int activityLogId);
        string DeleteActiveResidentEventLog(int activeResidentEventLogId);
        string DeleteInteractiveActivityEventLog(int interactiveActivityEventLogId);
    }

    public class OperationsClient : IApiClient
    {
        private const string UriBase = "http://localhost/Keebee.AAT.Operations/api/";

        #region urls

        // configurations
        private const string UrlConfigs = "configs";
        private const string UrlConfig = "configs/{0}";
        private const string UrlActiveConfig = "configs/active";
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

        // users
        private const string UrlUsers = "users";
        private const string UrlUser = "users/{0}";
        private const string UrlUserByUsername = "users?username={0}";

        // user roles
        private const string UrlUserRoles = "userroles";
        private const string UrlUserRole = "userroles/{0}";
        private const string UrlUserRolesByUser = "userroles?userId={0}";

        // residents
        private const string UrlResidents = "residents";
        private const string UrlResident = "residents/{0}";
        private const string UrlResidentByNameGender = "residents?firstName={0}&lastName={1}&gender={2}";
        private const string UrlResidentWithMedia = "residents/{0}/media";
        private const string UrlActiveResident = "activeresidents/{0}";

        // media files
        private const string UrlMediaFile = "mediafiles/{0}";
        private const string UrlMediaFilesForPath = "mediafiles?path={0}";
        private const string UrlMediaFileFromPath = "mediafiles?path={0}&filename={1}";
        private const string UrlMediaFileStream = "mediafilestreams/{0}";
        private const string UrlMediaFileStreamFromPath = "mediafilestreams?path={0}&filename={1}";

        // media path types
        private const string UrlMediaPathTypes = "mediapathtypes";
        private const string UrlMediaPathType = "mediapathtypes/{0}";
        private const string UrlMediaPathTypesSystem = "mediapathtypes?isSystem={0}";

        // public media files
        private const string UrlPublicMediaFiles = "publicmediafiles";
        private const string UrlPublicMediaFile = "publicmediafiles/{0}";
        private const string UrlPublicMediaFilesForResponseType = "publicmediafiles?responseTypeId={0}";
        private const string UrlPublicMediaFilesForMediaPathType = "publicmediafiles?mediaPathTypeId={0}";
        private const string UrlPublicMediaFilesForMediaStreamId = "publicmediafiles?streamId={0}";
        private const string UrlPublicMediaFilesForResponseTypeIdFilename = "publicmediafiles?responseTypeId={0}&filename={1}";
        private const string UrlPublicMediaFileIdsForStreamId = "publicmediafiles/ids?streamId={0}";

        // resident media files
        private const string UrlResidentMediaFiles = "residentmediafiles";
        private const string UrlResidentMediaFile = "residentmediafiles/{0}";
        private const string UrlResidentMediaFilesForResident = "residentmediafiles?residentId={0}";
        private const string UrlResidentMediaFilesForResidentResponseType = "residentmediafiles?residentId={0}&responseTypeId={1}";
        private const string UrlLinkedResidentMedia = "residentmediafiles/linked";
        private const string UrlLinkedResidentMediaForStreamId = "residentmediafiles/linked?streamId={0}";
        private const string UrlResidentMediaFileIdsForStreamId = "residentmediafiles/ids?streamId={0}";

        // active resident event logs
        private const string UrlActiveResidentEventLogs = "activeresidenteventlogs";
        private const string UrlActiveResidentEventLog = "activeresidenteventlogs/{0}";
        private const string UrlActiveResidentEventLogsForResident = "activeresidenteventlogs?residentId={0}";
        private const string UrlActiveResidentEventLogsForDate = "activeresidenteventlogs?date={0}";

        // activity event logs
        private const string UrlActivityEventLogs = "activityeventlogs";
        private const string UrlActivityEventLog = "activityeventlogs/{0}";
        private const string UrlActivityEventLogsForDate = "activityeventlogs?date={0}";
        private const string UrlActivityEventLogsForConfig = "activityeventlogs?configId={0}";
        private const string UrlActivityEventLogsForConfigDetail = "activityeventlogs?configDetailId={0}";
        private const string UrlActivityEventLogsForResident = "activityeventlogs?residentId={0}";

        // interactive activity event logs
        private const string UrlInteractiveActivityEventLogs = "interactiveactivityeventlogs";
        private const string UrlInteractiveActivityEventLog = "interactiveactivityeventlogs/{0}";
        private const string UrlInteractiveActivityEventLogsForResident = "interactiveactivityeventlogs?residentId={0}";
        private const string UrlInteractiveActivityEventLogsForDate = "interactiveactivityeventlogs?date={0}";

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

        public Config GetActiveConfig()
        {
            var data = Get(UrlActiveConfig);
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


        // types
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


        // user
        public IEnumerable<User> GetUsers()
        {
            var data = Get(UrlUsers);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var users = serializer.Deserialize<UserList>(data).Users.ToList();

            return users;
        }

        public User GetUser(int userId)
        {
            var data = Get(string.Format(UrlUser, userId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var user = serializer.Deserialize<User>(data);

            return user;
        }

        public User GetUserByUsername(string username)
        {
            var data = Get(string.Format(UrlUserByUsername, username));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var user = serializer.Deserialize<User>(data);

            return user;
        }


        // user role
        public IEnumerable<UserRole> GetUserRoles()
        {
            var data = Get(UrlUserRoles);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var userroles = serializer.Deserialize<UserRoleList>(data).UserRoles.ToList();

            return userroles;
        }

        public UserRole GetUserRole(int userRoleId)
        {
            var data = Get(string.Format(UrlUserRole, userRoleId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var userRole = serializer.Deserialize<UserRole>(data);

            return userRole;
        }

        public IEnumerable<UserRoleSingle> GetRolesByUser(int userId)
        {
            var data = Get(string.Format(UrlUserRolesByUser, userId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var userRoles = serializer.Deserialize<UserRolesList>(data).UserRoles;

            return userRoles;
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

        public Resident GetResidentByNameGender(string firstName, string lastName, string gender)
        {
            var data = Get(string.Format(UrlResidentByNameGender, firstName, lastName, gender));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public Resident GetResidentWithMedia(int residentId)
        {
            var data = Get(string.Format(UrlResidentWithMedia, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<Resident>(data);

            return resident;
        }

        public bool ResidentExists(int residentId)
        {
            return Exists(string.Format(UrlResident, residentId));
        }

        public ActiveResident GetActiveResident()
        {
            var data = Get(string.Format(UrlActiveResident, 1));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<ActiveResident>(data);

            return resident;
        }


        // event log
        public IEnumerable<ActiveResidentEventLog> GetActiveResidentEventLogsForDate(string date)
        {
            var data = Get(string.Format(UrlActiveResidentEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activeResidentEventLogs = serializer.Deserialize<ActiveResidentEventLogList>(data).ActiveResidentEventLogs;

            return activeResidentEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetActivityEventLogsForDate(string date)
        {
            var data = Get(string.Format(UrlActivityEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<InteractiveActivityEventLog> GetInteractiveActivityEventLogsForDate(string date)
        {
            var data = Get(string.Format(UrlInteractiveActivityEventLogsForDate, date));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var interactiveActivityEventLogs = serializer.Deserialize<InteractiveActivityEventLogList>(data).InteractiveActivityEventLogs;

            return interactiveActivityEventLogs;
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

        public IEnumerable<ActivityEventLog> GetActivityEventLogsForResident(int residentId)
        {
            var data = Get(string.Format(UrlActivityEventLogsForResident, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<ActiveResidentEventLog> GetActiveResidentEventLogsForResident(int residentId)
        {
            var data = Get(string.Format(UrlActiveResidentEventLogsForResident, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activeResidentEventLogs = serializer.Deserialize<ActiveResidentEventLogList>(data).ActiveResidentEventLogs;

            return activeResidentEventLogs;
        }

        public IEnumerable<InteractiveActivityEventLog> GetInteractiveActivityEventLogsForResident(int residentId)
        {
            var data = Get(string.Format(UrlInteractiveActivityEventLogsForResident, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var interactiveActivityEventLogs = serializer.Deserialize<InteractiveActivityEventLogList>(data).InteractiveActivityEventLogs;

            return interactiveActivityEventLogs;
        }


        // media
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

        public MediaFileSingle GetMediaFileFromPath(string path, string filename)
        {
            var data = Get(string.Format(UrlMediaFileFromPath, path, filename));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var media = serializer.Deserialize<MediaFileSingle>(data);

            return media;
        }

        public byte[] GetMediaFileStream(Guid streamId)
        {
            var data = GetBytes(string.Format(UrlMediaFileStream, streamId));

            return data;
        }

        public byte[] GetMediaFileStreamFromPath(string path, string filename)
        {
            var data = Get(string.Format(UrlMediaFileStreamFromPath, path, filename));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var media = serializer.Deserialize<MediaFileStreamSingle>(data).Stream;

            return media;
        }


        // media path types
        public IEnumerable<MediaPathType> GetMediaPathTypes()
        {
            var data = Get(UrlMediaPathTypes);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var pathTypes = serializer.Deserialize<MediaPathTypeList>(data).MediaPathTypes;

            return pathTypes;
        }

        public MediaPathType GetMediaPathType(int mediaPathTypeId)
        {
            var data = Get(string.Format(UrlMediaPathType, mediaPathTypeId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var pathType = serializer.Deserialize<MediaPathType>(data);

            return pathType;
        }

        public IEnumerable<MediaPathType> GetMediaPathTypes(bool isSystem)
        {
            var data = Get(string.Format(UrlMediaPathTypesSystem, isSystem));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var pathTypes = serializer.Deserialize<MediaPathTypeList>(data).MediaPathTypes;

            return pathTypes;
        }


        // public media files
        public PublicMediaFile GetPublicMediaFile(int id)
        {
            var data = Get(string.Format(UrlPublicMediaFile, id));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaFile = serializer.Deserialize<PublicMediaFile>(data);

            return mediaFile;
        }

        public PublicMedia GetPublicMediaFiles()
        {
            var data = Get(UrlPublicMediaFiles);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMedia>(data);

            return publicMedia;
        }

        public PublicMediaResponseType GetPublicMediaFilesForResponseType(int responseTypeId)
        {
            var data = Get(string.Format(UrlPublicMediaFilesForResponseType, responseTypeId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMediaResponseType>(data);

            return publicMedia;
        }

        public PublicMedia GetPublicMediaFilesForMediaPathType(int mediaPathTypeId)
        {
            var data = Get(string.Format(UrlPublicMediaFilesForMediaPathType, mediaPathTypeId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var publicMedia = serializer.Deserialize<PublicMedia>(data);

            return publicMedia;
        }

        public IEnumerable<PublicMediaFile> GetPublicMediaFilesForStreamId(Guid streamId)
        {
            var data = Get(string.Format(UrlPublicMediaFilesForMediaStreamId, streamId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaStreamIds = serializer.Deserialize<PublicMediaStreamIdList>(data).MediaFiles;

            return mediaStreamIds;
        }

        public PublicMediaFile GetPublicMediaFileForResponseTypeFilename(int responseTypeId, string filename)
        {
            var data = Get(string.Format(UrlPublicMediaFilesForResponseTypeIdFilename, responseTypeId, filename));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaFile = serializer.Deserialize<PublicMediaFile>(data);

            return mediaFile;
        }

        public int[] GetPublicMediaFileIdsForStreamId(Guid streamId)
        {
            var data = Get(string.Format(UrlPublicMediaFileIdsForStreamId, streamId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var ids = serializer.Deserialize<int[]>(data);

            return ids;
        }


        // resident media files
        public IEnumerable<ResidentMedia> GetResidentMediaFiles()
        {
            var data = Get(UrlResidentMediaFiles);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residentMedia = serializer.Deserialize<ResidentMediaList>(data).Media;

            return residentMedia;
        }

        public ResidentMediaFile GetResidentMediaFile(int id)
        {
            var data = Get(string.Format(UrlResidentMediaFile, id));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residentMediaFile = serializer.Deserialize<ResidentMediaFile>(data);

            return residentMediaFile;
        }

        public ResidentMedia GetResidentMediaFilesForResident(int residentId)
        {
            var data = Get(string.Format(UrlResidentMediaFilesForResident, residentId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var residentMedia = serializer.Deserialize<ResidentMediaSingle>(data).ResidentMedia;

            return residentMedia;
        }

        public ResidentMediaResponseType GetResidentMediaFilesForResidentResponseType(int residentId, int responseTypeId)
        {
            var data = Get(string.Format(UrlResidentMediaFilesForResidentResponseType, residentId, responseTypeId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaResponseType = serializer.Deserialize<ResidentMediaResponseTypeSingle>(data).ResidentMedia;

            return mediaResponseType;
        }

        public IEnumerable<ResidentMediaResponseType> GetLinkedResidentMedia()
        {
            var data = Get(UrlLinkedResidentMedia);
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaResponseTypeList = serializer.Deserialize<ResidentMediaResponseTypeList>(data).ResidentMediaList;

            return mediaResponseTypeList;
        }

        public IEnumerable<ResidentMediaResponseType> GetLinkedResidentMediaForStreamId(Guid streamId)
        {
            var data = Get(string.Format(UrlLinkedResidentMediaForStreamId, streamId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var mediaResponseTypeList = serializer.Deserialize<ResidentMediaResponseTypeList>(data).ResidentMediaList;

            return mediaResponseTypeList;
        }

        public int[] GetResidentMediaFileIdsForStreamId(Guid streamId)
        {
            var data = Get(string.Format(UrlResidentMediaFileIdsForStreamId, streamId));
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var ids = serializer.Deserialize<int[]>(data);

            return ids;
        }

        //
        // POST

        public int PostUser(User user)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(user);

            return Post(UrlUsers, el);
        }

        public int PostResident(ResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            return Post(UrlResidents, el);
        }

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

        public int PostActivityEventLog(ActivityEventLog activityEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(activityEventLog);

            return Post(UrlActivityEventLogs, el);
        }

        public int PostInteractiveActivityEventLog(InteractiveActivityEventLog interactiveActivityEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(interactiveActivityEventLog);

            return Post(UrlInteractiveActivityEventLogs, el);
        }

        public int PostActiveResidentEventLog(ActiveResidentEventLog activeResidentEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(activeResidentEventLog);

            return Post(UrlActiveResidentEventLogs, el);
        }

        public int PostActivateConfig(int configId)
        {
            return Post(string.Format(UrlActivateConfig, configId), string.Empty);
        }

        public int PostResidentMediaFile(ResidentMediaFileEdit residenttMediaFile)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(residenttMediaFile);

            return Post(UrlResidentMediaFiles, el);
        }

        public int PostPublicMediaFile(PublicMediaFileEdit publicMediaFile)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(publicMediaFile);

            return Post(UrlPublicMediaFiles, el);
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

        public void PatchUser(int userId, User user)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(user);

            Patch(string.Format(UrlUser, userId), el);
        }

        public void PatchActiveResident(ActiveResidentEdit resident)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(resident);

            Patch(string.Format(UrlActiveResident, 1), el);
        }

        // DELETE
        public string DeleteUser(int userId)
        {
            return Delete(string.Format(UrlUser, userId));
        }

        public string DeleteResident(int residentId)
        {
            return Delete(string.Format(UrlResident, residentId));
        }

        public string DeleteResidentMediaFile(int mediaFileId)
        {
            return Delete(string.Format(UrlResidentMediaFile, mediaFileId));
        }

        public void DeleteConfig(int configId)
        {
            Delete(string.Format(UrlConfig, configId));
        }

        public void DeleteConfigDetail(int configId)
        {
            Delete(string.Format(UrlConfigDetail, configId));
        }

        public string DeletePublicMediaFile(int mediaFileId)
        {
           return Delete(string.Format(UrlPublicMediaFile, mediaFileId));
        }

        public string DeleteActivityEventLog(int activityEventLogId)
        {
            return Delete(string.Format(UrlActivityEventLog, activityEventLogId));
        }

        public string DeleteActiveResidentEventLog(int activeResidentEventLogId)
        {
            return Delete(string.Format(UrlActiveResidentEventLog, activeResidentEventLogId));
        }

        public string DeleteInteractiveActivityEventLog(int interactiveActivityEventLogId)
        {
            return Delete(string.Format(UrlInteractiveActivityEventLog, interactiveActivityEventLogId));
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
                _systemEventLogger?.WriteEntry($"ApiClient.Get: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }

            return result;
        }

        // TODO: Figure out why this doesn't work (returns too many bytes)
        private byte[] GetBytes(string url)
        {
            byte[] result = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = _uriBase;
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
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
                _systemEventLogger?.WriteEntry($"ApiClient.Get: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
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
                _systemEventLogger?.WriteEntry($"ApiClient.Exists: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
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
                _systemEventLogger?.WriteEntry($"ApiClient.Post: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
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
                _systemEventLogger?.WriteEntry($"ApiClient.Patch: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }
        }

        private string Delete(string url)
        {
            var result = string.Empty;

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
                result = ex.Message;
                _systemEventLogger?.WriteEntry($"ApiClient.Delete: {ex.Message}{Environment.NewLine}url:{url}", EventLogEntryType.Error);
            }

            return result;
        }
    }
}
