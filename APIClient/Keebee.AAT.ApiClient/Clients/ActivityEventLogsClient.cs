using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IActivityEventLogsClient
    {
        IEnumerable<ActivityEventLog> GetForDate(string date);
        IEnumerable<ActivityEventLog> GetForConfig(int configId);
        IEnumerable<ActivityEventLog> GetForConfigDetail(int configDetailId);
        IEnumerable<ActivityEventLog> GetForResident(int residentId);
        int Post(ActivityEventLog activityEventLog);
        string Delete(int id);

    }

    public class ActivityEventLogsClient : IActivityEventLogsClient
    {
        private readonly ClientBase _clientBase;

        public ActivityEventLogsClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<ActivityEventLog> GetForDate(string date)
        {
            var data = _clientBase.Get($"activityeventlogs?date={date}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForConfig(int configId)
        {
            var data = _clientBase.Get($"activityeventlogs?configId={configId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForConfigDetail(int configDetailId)
        {
            var data = _clientBase.Get($"activityeventlogs?configDetailId={configDetailId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForResident(int residentId)
        {
            var data = _clientBase.Get($"activityeventlogs?residentId={residentId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activityEventLogs = serializer.Deserialize<ActivityEventLogList>(data).ActivityEventLogs;

            return activityEventLogs;
        }

        public int Post(ActivityEventLog activityEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(activityEventLog);

            return _clientBase.Post("activityeventlogs", el);
        }

        public string Delete(int id)
        {
            return _clientBase.Delete($"activityeventlogs/{id}");
        }
    }
}
