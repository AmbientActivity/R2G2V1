namespace Keebee.AAT.Shared
{
    public class ResponseMessage
    {
        public int SensorValue { get; set; }
        public bool IsSystem { get; set; }
        public ActiveProfile ActiveProfile { get; set; }
        public ActiveConfigDetail ActiveConfigDetail { get; set; }
    }

    public class ActiveProfile
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public int ResidentId { get; set; }
        public int GameDifficultyLevel { get; set; }
    }

    public class ActiveConfigDetail
    {
        public int Id { get; set; }
        public int PhidgetTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public bool IsSystem { get; set; }
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
