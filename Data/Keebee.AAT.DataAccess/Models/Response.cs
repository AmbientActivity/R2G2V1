using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class Response
    {
        [Key]
        public int Id { get; set; }

        public int ProfileId { get; set; }
        public int ResponseTypeId { get; set; }

        [ForeignKey("MediaFile")]
        [Required]
        public Guid StreamId { get; set; }
        public virtual MediaFile MediaFile { get; set; }
    }
}