using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class GameEventLog
    {
        public int Id { get; set; }

        // for insert
        public int? ResidentId { get; set; }
        public int GameTypeId { get; set; }
        public int DifficultyLevel { get; set; }
        public bool? IsSuccess { get; set; }
        public string Description { get; set; }

        // for export
        public string Date { get; set; }
        public string Time { get; set; }
        public string Resident { get; set; }
    }

    public class GameEventLogList
    {
        public IEnumerable<GameEventLog> GameEventLogs { get; set; }
    }
}
