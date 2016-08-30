using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class EventLog
    {
        // for insert
        public int? ResidentId { get; set; }
        public int EventLogEntryTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public int ActivityTypeId { get; set; }
        public string Description { get; set; }

        // for export
        public string Date { get; set; }
        public string Time { get; set; }
        public string Resident { get; set; }
        public string ActivityType { get; set; }
        public string ResponseType { get; set; }
        public string ResponseTypeCategory { get; set; }
    }

    public class EventLogList
    {
        public IEnumerable<EventLog> EventLogs { get; set; }
    }
}
