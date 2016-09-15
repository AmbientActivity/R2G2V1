using System;
using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{

    public class Resident
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Profile Profile { get; set; }
    }

    public class ResidentEdit
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
    }

    public class ResidentList
    {
        public IEnumerable<Resident> Residents;
    }
}
