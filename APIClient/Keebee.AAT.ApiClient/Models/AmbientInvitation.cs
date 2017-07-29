namespace Keebee.AAT.ApiClient.Models
{
    public class AmbientInvitation
    {
        public int Id { get; set; }

        public string Message { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class AmbientInvitationEdit
    {
        public int Id { get; set; }

        public string Message { get; set; }
        public int ResponseTypeId { get; set; }
    }
}
