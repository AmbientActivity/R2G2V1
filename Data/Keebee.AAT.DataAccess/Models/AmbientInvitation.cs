using System.ComponentModel.DataAnnotations;

namespace Keebee.AAT.DataAccess.Models
{
    public class AmbientInvitation
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }

        public bool IsExecuteRandom { get; set; }
    }
}