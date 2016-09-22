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
        public int GameDifficultyLevel { get; set; }
        public IEnumerable<ConfigDetail> ConfigDetails { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public IEnumerable<MediaResponseType> MediaFiles;
    }

    public class ResidentEdit
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int GameDifficultyLevel { get; set; }
    }

    public class ResidentList
    {
        public IEnumerable<Resident> Residents;
    }
}
