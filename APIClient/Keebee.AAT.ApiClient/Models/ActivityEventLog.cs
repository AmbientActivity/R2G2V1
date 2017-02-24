using System.Collections.Generic;

namespace Keebee.AAT.ApiClient.Models
{
    public class ActivityEventLog
    {
        public int Id { get; set; }

        // for insert
        public int ConfigId { get; set; }
        public int? ResidentId { get; set; }
        public int ConfigDetailId { get; set; }
        public string Description { get; set; }

        // for export
        public string Date { get; set; }
        public string Time { get; set; }
        public string Resident { get; set; }
        public string ActivityType { get; set; }
        public string ResponseType { get; set; }
        public string ResponseTypeCategory { get; set; }
    }

    public class ActivityEventLogList
    {
        public IEnumerable<ActivityEventLog> ActivityEventLogs { get; set; }
    }
}
