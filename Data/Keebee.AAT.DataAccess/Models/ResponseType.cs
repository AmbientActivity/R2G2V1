using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ResponseType
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ResponseTypeCategory")]
        public int ResponseTypeCategoryId { get; set; }

        public virtual ResponseTypeCategory ResponseTypeCategory { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual InteractiveActivityType InteractiveActivityType { get; set; }
        public int? InteractiveActivityTypeId { get; set; }

        public bool IsRandom { get; set; }
        public bool IsRotational { get; set; }
        public bool IsUninterrupted { get; set; }
    }
}