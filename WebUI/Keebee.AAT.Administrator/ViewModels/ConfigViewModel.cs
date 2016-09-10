namespace Keebee.AAT.Administrator.ViewModels
{
    public class ConfigDetailViewModel
    {
        public int Id { get; set; }
        public string ActivityType { get; set; }
        public string ResponseType { get; set; }
        public string Phidget { get; set; }
        public bool IsSystem { get; set; }
    }

    public class ConfigViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}