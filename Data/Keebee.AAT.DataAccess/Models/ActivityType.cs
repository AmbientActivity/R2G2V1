using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class ActivityType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string PhidgetType { get; set; }
    }
}