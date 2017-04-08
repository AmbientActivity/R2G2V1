using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IActivityEventLogsClient
    {
        IEnumerable<ActivityEventLog> GetForDate(string date);
        IEnumerable<ActivityEventLog> GetForConfig(int configId);
        IEnumerable<ActivityEventLog> GetForConfigDetail(int configDetailId);
        IEnumerable<ActivityEventLog> GetForResident(int residentId);
        IEnumerable<ActivityEventLog> GetIds();
        int Post(ActivityEventLog activityEventLog);
        string Delete(int id);

    }

    public class ActivityEventLogsClient : BaseClient, IActivityEventLogsClient
    {
        public IEnumerable<ActivityEventLog> GetForDate(string date)
        {
            var request = new RestRequest($"activityeventlogs?date={date}", Method.GET);
            var data = Execute(request);
            var activityEventLogs = JsonConvert.DeserializeObject<IEnumerable<ActivityEventLog>>(data.Content);

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForConfig(int configId)
        {
            var request = new RestRequest($"activityeventlogs?configId={configId}", Method.GET);
            var data = Execute(request);
            var activityEventLogs = JsonConvert.DeserializeObject<IEnumerable<ActivityEventLog>>(data.Content);

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForConfigDetail(int configDetailId)
        {
            var request = new RestRequest($"activityeventlogs?configDetailId={configDetailId}", Method.GET);
            var data = Execute(request);
            var activityEventLogs = JsonConvert.DeserializeObject<IEnumerable<ActivityEventLog>>(data.Content);

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForResident(int residentId)
        {
            var request = new RestRequest($"activityeventlogs?residentId={residentId}", Method.GET);
            var data = Execute(request);
            var activityEventLogs = JsonConvert.DeserializeObject<IEnumerable<ActivityEventLog>>(data.Content);

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetIds()
        {
            var request = new RestRequest("activityeventlogs/ids", Method.GET);
            var data = Execute(request);
            var activityEventLogs = JsonConvert.DeserializeObject<IEnumerable<ActivityEventLog>>(data.Content);

            return activityEventLogs;
        }

        public int Post(ActivityEventLog activityEventLog)
        {
            var request = new RestRequest("activityeventlogs", Method.POST);
            var json = request.JsonSerializer.Serialize(activityEventLog);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"activityeventlogs/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
