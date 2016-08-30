using System;
using System.Collections.Generic;

namespace Keebee.AAT.RESTClient
{
    public class Profile
    {
        public int Id { get; set; }
        public int ResidentId { get; set; }
        public string Description { get; set; }
        public int GameDifficultyLevel { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual IList<ProfileDetail> ProfileDetails { get; set; }
    }
}
