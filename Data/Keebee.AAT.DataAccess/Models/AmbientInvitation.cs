using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class AmbientInvitation
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        [ForeignKey("ResponseType")]
        public int? ResponseTypeId { get; set; }
        public virtual ResponseType ResponseType { get; set; }
    }
}