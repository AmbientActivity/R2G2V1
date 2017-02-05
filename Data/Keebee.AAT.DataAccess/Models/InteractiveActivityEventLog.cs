using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class InteractiveActivityEventLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Resident")]
        public int? ResidentId { get; set; }
        public virtual Resident Resident { get; set; }

        [ForeignKey("InteractiveActivityType")]
        public int InteractiveActivityTypeId { get; set; }
        public virtual InteractiveActivityType InteractiveActivityType { get; set; }

        public int? Difficultylevel{ get; set; }

        public bool? IsSuccess { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DateEntry { get; set; }
    }
}