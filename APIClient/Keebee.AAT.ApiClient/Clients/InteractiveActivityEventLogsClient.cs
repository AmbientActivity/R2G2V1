using Keebee.AAT.ApiClient.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Keebee.AAT.ApiClient.Clients
{
    public interface IInteractiveActivityEventLogsClient
    {
        IEnumerable<InteractiveActivityEventLog> GetForDate(string date);
        IEnumerable<InteractiveActivityEventLog> GetForResident(int residentId);
        int Post(InteractiveActivityEventLog interactiveActivityEventLog);
        string Delete(int id);
    }

    public class InteractiveActivityEventLogsClient : IInteractiveActivityEventLogsClient
    {
        private readonly ClientBase _clientBase;

        public InteractiveActivityEventLogsClient()
        {
            _clientBase = new ClientBase();
        }

        public IEnumerable<InteractiveActivityEventLog> GetForDate(string date)
        {
            var data = _clientBase.Get($"interactiveactivityeventlogs?date={date}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var interactiveActivityEventLogs = serializer.Deserialize<InteractiveActivityEventLogList>(data).InteractiveActivityEventLogs;

            return interactiveActivityEventLogs;
        }

        public IEnumerable<InteractiveActivityEventLog> GetForResident(int residentId)
        {
            var data =_clientBase. Get($"interactiveactivityeventlogs?residentId={residentId}");
            if (data == null) return null;

            var serializer = new JavaScriptSerializer();
            var interactiveActivityEventLogs = serializer.Deserialize<InteractiveActivityEventLogList>(data).InteractiveActivityEventLogs;

            return interactiveActivityEventLogs;
        }

        public int Post(InteractiveActivityEventLog interactiveActivityEventLog)
        {
            var serializer = new JavaScriptSerializer();
            var el = serializer.Serialize(interactiveActivityEventLog);

            return _clientBase.Post("interactiveactivityeventlogs", el);
        }

        public string Delete(int id)
        {
            return _clientBase.Delete($"interactiveactivityeventlogs/{id}");
        }
    }
}
