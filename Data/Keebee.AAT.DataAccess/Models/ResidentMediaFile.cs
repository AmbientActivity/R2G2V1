using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ResidentMediaFile
    {
        [Key]
        public int Id { get; set; }
        public bool IsPublic { get; set; }

        [ForeignKey("Resident")]
        public int ResidentId { get; set; }
        public virtual Resident Resident { get; set; }

        [ForeignKey("MediaFile")]
        public Guid StreamId { get; set; }
        public virtual MediaFile MediaFile { get; set; }

        [ForeignKey("ResponseType")]
        public int ResponseTypeId { get; set; }
        public virtual ResponseType ResponseType { get; set; }

        [ForeignKey("MediaPathType")]
        public int MediaPathTypeId { get; set; }
        public virtual MediaPathType MediaPathType { get; set; }
    }
}