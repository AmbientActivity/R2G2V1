namespace Keebee.AAT.ApiClient.Models
{
    public class ConfigDetailBase
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsEventLogs { get; set; }
    }

    public class ConfigDetail : ConfigDetailBase
    {
        public PhidgetType PhidgetType { get; set; }
        public PhidgetStyleType PhidgetStyleType { get; set; }
        public ResponseType ResponseType { get; set; }
    }

    public class ConfigDetailEdit : ConfigDetailBase
    {
        public int PhidgetTypeId { get; set; }
        public int PhidgetStyleTypeId { get; set; }
        public int ResponseTypeId { get; set; }
    }
}
