using System.Collections.Generic;

namespace Keebee.AAT.Shared
{
    public class ConfigMessage
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsActiveEventLog { get; set; }
        public bool IsDisplayActive { get; set; }
        public IEnumerable<ConfigDetailMessage> ConfigDetails { get; set; }
    }

    public class ConfigDetailMessage
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public int PhidgetTypeId { get; set; }
        public int PhidgetStyleTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public bool IsSystemReponseType { get; set; }
    }

    public class ResidentMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GameDifficultyLevel { get; set; }
        public bool AllowVideoCapturing { get; set; }
    }

    public class ResponseMessage
    {
        public int SensorValue { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActiveEventLog { get; set; }
        public ResidentMessage Resident { get; set; }
        public ConfigDetailMessage ConfigDetail { get; set; }
        public int[] ResponseTypeIds { get; set; }
    }

    public class DisplayMessage
    {
        public bool IsActive { get; set; }
    }

    public class RfidMonitorMessage
    {
        public bool IsFinal { get; set; }
        public int ReadCount { get; set; }
        public int ResidentId { get; set; }
    }
}
