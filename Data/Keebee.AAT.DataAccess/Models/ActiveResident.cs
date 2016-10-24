using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Keebee.AAT.DataAccess.Models
{
    public class ActiveResident
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Resident")]
        public int? ResidentId { get; set; }
        public virtual Resident Resident { get; set; }
    }
}