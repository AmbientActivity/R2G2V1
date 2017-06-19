using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class Resident
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        public int GameDifficultyLevel { get; set; }
        public bool AllowVideoCapturing { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public byte[] ProfilePicture { get; set; }
        public virtual IList<ResidentMediaFile> MediaFiles { get; set; }
    }
}