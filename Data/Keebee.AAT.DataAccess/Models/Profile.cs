using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public int GameDifficultyLevel { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public virtual IList<ProfileDetail> ProfileDetails { get; set; }
    }
}