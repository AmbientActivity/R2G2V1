﻿namespace Keebee.AAT.ApiClient.Models
{
    public class ActiveResident
    {
        public int Id { get; set; }

        public Resident Resident { get; set; }
    }

    public class ActiveResidentEdit
    {
        public int Id { get; set; }

        public int? ResidentId { get; set; }
    }
}
