namespace Keebee.AAT.Administrator.ViewModels
{
    public class ConfigDetailViewModel
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string PhidgetType { get; set; }
        public string ActivityType { get; set; }
        public string ResponseType { get; set; }
        public bool IsUserResponse { get; set; }
    }

    public class ConfigViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}