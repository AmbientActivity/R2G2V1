using System.Collections.Generic;
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
        public bool IsInteractive { get; set; }
        public bool IsSystem { get; set; }

        public virtual IList<Response> Responses { get; set; }
    }
}