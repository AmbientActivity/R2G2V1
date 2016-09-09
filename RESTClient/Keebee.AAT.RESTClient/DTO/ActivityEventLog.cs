using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class ActivityEventLog
    {
        // for insert
        public int? ResidentId { get; set; }
        public int ResponseTypeId { get; set; }
        public int ActivityTypeId { get; set; }
        public string Description { get; set; }

        // for export
        public string Date { get; set; }
        public string Time { get; set; }
        public string Resident { get; set; }
        public string ActivityTypeDesc { get; set; }
        public string ResponseType { get; set; }
        public string ResponseTypeCategory { get; set; }
    }

    public class ActivityEventLogList
    {
        public IEnumerable<ActivityEventLog> ActivityEventLogs { get; set; }
    }
}
