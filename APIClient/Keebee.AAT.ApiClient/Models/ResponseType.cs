namespace Keebee.AAT.ApiClient.Models
{
    public class ResponseType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public bool IsRandom { get; set; }
        public ResponseTypeCategory ResponseTypeCategory { get; set; }
        public InteractiveActivityType InteractiveActivityType { get; set; }
    }
}
