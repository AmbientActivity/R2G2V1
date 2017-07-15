using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class MediaPathType
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MediaPathTypeCategory")]
        public int MediaPathTypeCategoryId { get; set; }

        public virtual MediaPathTypeCategory MediaPathTypeCategory { get; set; }

        [Required]
        public string Path { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        [Required]
        public string AllowedExts { get; set; }
        [Required]
        public string AllowedTypes { get; set; }
        public bool IsSystem { get; set; }
        public bool IsSharable { get; set; }
    }
}