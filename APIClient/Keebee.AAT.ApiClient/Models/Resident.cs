using System;

namespace Keebee.AAT.ApiClient.Models
{

    public class Resident
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int GameDifficultyLevel { get; set; }
        public bool AllowVideoCapturing { get; set; }
        public byte[] ProfilePicture { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
