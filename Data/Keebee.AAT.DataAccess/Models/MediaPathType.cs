using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class MediaPathType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        public bool IsPreviewable { get; set; }
        public bool IsSystem { get; set; }
        public bool IsSharable { get; set; }
    }
}