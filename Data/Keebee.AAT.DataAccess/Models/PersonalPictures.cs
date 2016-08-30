using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class PersonalPicture
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Resident")]
        public int ResidentId { get; set; }
        public virtual Resident Resident { get; set; }

        [ForeignKey("MediaFile")]
        [Required]
        public Guid StreamId { get; set; }
        public virtual MediaFile MediaFile { get; set; }
    }
}