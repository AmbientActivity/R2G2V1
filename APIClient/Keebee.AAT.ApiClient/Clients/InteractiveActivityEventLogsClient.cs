using System;
using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IInteractiveActivityEventLogsClient
    {
        IEnumerable<InteractiveActivityEventLog> GetForDate(string date);
        IEnumerable<InteractiveActivityEventLog> GetForResident(int residentId);
        int Post(InteractiveActivityEventLog interactiveActivityEventLog);
        string Delete(int id);
    }

    public class InteractiveActivityEventLogsClient : BaseClient, IInteractiveActivityEventLogsClient
    {
        public IEnumerable<InteractiveActivityEventLog> GetForDate(string date)
        {
            var request = new RestRequest($"interactiveactivityeventlogs?date={date}", Method.GET);
            var data = Execute(request);
            var interactiveActivityEventLogs = JsonConvert.DeserializeObject<IEnumerable<InteractiveActivityEventLog>>(data.Content);

            return interactiveActivityEventLogs;
        }

        public IEnumerable<InteractiveActivityEventLog> GetForResident(int residentId)
        {
            var request = new RestRequest($"interactiveactivityeventlogs?residentId={residentId}", Method.GET);
            var data = Execute(request);
            var interactiveActivityEventLogs = JsonConvert.DeserializeObject<IEnumerable<InteractiveActivityEventLog>>(data.Content);

            return interactiveActivityEventLogs;
        }

        public int Post(InteractiveActivityEventLog interactiveActivityEventLog)
        {
            var request = new RestRequest("interactiveactivityeventlogs", Method.POST);
            var json = request.JsonSerializer.Serialize(interactiveActivityEventLog);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"interactiveactivityeventlogs/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
