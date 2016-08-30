using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class AmbientResponse
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ResponseType")]
        public int ResponseTypeId { get; set; }
        public virtual ResponseType ResponseType { get; set; }

        [ForeignKey("MediaFile")]
        [Required]
        public Guid StreamId { get; set; }
        public virtual MediaFile MediaFile { get; set; }
    }
}