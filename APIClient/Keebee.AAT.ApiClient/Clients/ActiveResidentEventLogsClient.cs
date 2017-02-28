using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IActiveResidentEventLogsClient
    {
        IEnumerable<ActiveResidentEventLog> GetForDate(string date);
        IEnumerable<ActiveResidentEventLog> GetForResident(int residentId);
        int Post(ActiveResidentEventLog activeResidentEventLog);
        string Delete(int id);
    }

    public class ActiveResidentEventLogsClient : BaseClient, IActiveResidentEventLogsClient
    {
        public IEnumerable<ActiveResidentEventLog> GetForDate(string date)
        {
            var request = new RestRequest($"activeresidenteventlogs?date={date}", Method.GET);
            var data = Execute(request);
            var activeResidentEventLogs = JsonConvert.DeserializeObject<ActiveResidentEventLogList>(data.Content).ActiveResidentEventLogs;

            return activeResidentEventLogs;
        }

        public IEnumerable<ActiveResidentEventLog> GetForResident(int residentId)
        {
            var request = new RestRequest($"activeresidenteventlogs?residentId={residentId}", Method.GET);
            var data = Execute(request);
            var activeResidentEventLogs = JsonConvert.DeserializeObject<ActiveResidentEventLogList>(data.Content).ActiveResidentEventLogs;

            return activeResidentEventLogs;
        }

        public int Post(ActiveResidentEventLog activeResidentEventLog)
        {
            var request = new RestRequest("activeresidenteventlogs", Method.POST);
            var json = request.JsonSerializer.Serialize(activeResidentEventLog);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = Execute(request);

            var newId = Convert.ToInt32(response.Content);
            return newId;
        }

        public string Delete(int id)
        {
            var request = new RestRequest($"activeresidenteventlogs/{id}", Method.DELETE);
            return Execute(request).Content;
        }
    }
}
