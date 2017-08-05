using System;
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
        public PhidgetStyleTypeMessage PhidgetStyleType { get; set; }
        public ResponseTypeMessage ResponseType { get; set; }
    }

    public class ResponseTypeMessage
    {
        public int Id { get; set; }
        public int ResponseTypeCategoryId { get; set; }
        public bool IsRandom { get; set; }
        public bool IsRotational { get; set; }
        public bool IsUninterrupted { get; set; }
        public int InteractiveActivityTypeId { get; set; }
        public string SwfFile { get; set; }
    }

    public class PhidgetStyleTypeMessage
    {
        public int Id { get; set; }
        public bool IsIncremental { get; set; }
    }

    public class ResidentBluetoothMessage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
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
        public bool IsActiveEventLog { get; set; }
        public int SensorValue { get; set; }
        public ResidentMessage Resident { get; set; }
        public ConfigDetailMessage ConfigDetail { get; set; }
    }

    public class DisplayMessage
    {
        public bool IsActive { get; set; }
    }

    public class BeaconMonitorResidentMessage
    {
        public int ResidentId { get; set; }
        public string ResidentName { get; set; }
        public int Rssi { get; set; }
    }

    public class BeaconMonitorMessage
    {
        public string BeaconType { get; set; }
        public ulong Address { get; set; }
        public short[] Rssi { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Payload { get; set; }
    }
}
