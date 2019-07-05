namespace Keebee.AAT.ApiClient.Models
{
    public class ResponseType
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsRandom { get; set; }
        public bool IsRotational { get; set; }
        public bool IsUninterrupted { get; set; }
        public ResponseTypeCategory ResponseTypeCategory { get; set; }
        public InteractiveActivityType InteractiveActivityType { get; set; }
    }
}
