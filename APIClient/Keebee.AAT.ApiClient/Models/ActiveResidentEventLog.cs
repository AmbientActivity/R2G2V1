﻿namespace Keebee.AAT.ApiClient.Models
{
    public class ActiveResidentEventLog
    {
        public int Id { get; set; }

        // for insert
        public int? ResidentId { get; set; }
        public string Description { get; set; }

        // for export
        public string Date { get; set; }
        public string Time { get; set; }
        public string Resident { get; set; }
    }
}
