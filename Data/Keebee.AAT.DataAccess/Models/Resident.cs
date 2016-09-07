using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class Resident
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [ForeignKey("Profile")]
        public int ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public virtual IList<PersonalPicture> PersonalPictures { get; set; }
    }
}