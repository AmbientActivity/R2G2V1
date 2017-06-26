using System;
using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class Thumbnail
    {
        [Key]
        public Guid StreamId { get; set; }

        [Required]
        public byte[] Image { get; set; }
    }
}