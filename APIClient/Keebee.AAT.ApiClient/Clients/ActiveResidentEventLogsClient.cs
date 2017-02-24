using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IActiveResidentEventLogsClient
    {
        IEnumerable<ActiveResidentEventLog> GetForDate(string date);
        IEnumerable<ActiveResidentEventLog> GetForResident(int residentId);
        int Post(ActiveResidentEventLog activeResidentEventLog);
        string Delete(int id);
    }

    public class ActiveResidentEventLogsClient : IActiveResidentEventLogsClient
    {
        private readonly ClientBase _clientBase;

        public ActiveResidentEventLogsClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<ActiveResidentEventLog> GetForDate(string date)
        {
            var data = _clientBase.Get($"activeresidenteventlogs?date={date}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activeResidentEventLogs = serializer.Deserialize<ActiveResidentEventLogList>(data).ActiveResidentEventLogs;

            return activeResidentEventLogs;
        }

        public IEnumerable<ActiveResidentEventLog> GetForResident(int residentId)
        {
            var data = _clientBase.Get($"activeresidenteventlogs?residentId={residentId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var activeResidentEventLogs = serializer.Deserialize<ActiveResidentEventLogList>(data).ActiveResidentEventLogs;

            return activeResidentEventLogs;
        }

        public int Post(ActiveResidentEventLog activeResidentEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(activeResidentEventLog);

            return _clientBase.Post("activeresidenteventlogs", el);
        }

        public string Delete(int id)
        {
            return _clientBase.Delete($"activeresidenteventlogs/{id}");
        }
    }
}
