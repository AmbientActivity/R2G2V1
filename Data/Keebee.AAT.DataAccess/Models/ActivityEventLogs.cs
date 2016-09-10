using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ActivityEventLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Resident")]
        public int? ResidentId { get; set; }
        public virtual Resident Resident { get; set; }

        [ForeignKey("ConfigDetail")]
        public int ConfigDetailId { get; set; }
        public virtual ConfigDetail ConfigDetail { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DateEntry { get; set; }
    }
}