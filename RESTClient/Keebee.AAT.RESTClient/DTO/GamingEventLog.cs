using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class GamingEventLog
    {
        // for insert
        public int? ResidentId { get; set; }
        public int EventLogEntryTypeId { get; set; }
        public int DifficultyLevel { get; set; }
        public bool IsSuccess { get; set; }
        public string Description { get; set; }

        // for export
        public string Date { get; set; }
        public string Time { get; set; }
        public string Resident { get; set; }
    }

    public class GaminingEventLogList
    {
        public IEnumerable<GamingEventLog> GamingEventLogs { get; set; }
    }
}
