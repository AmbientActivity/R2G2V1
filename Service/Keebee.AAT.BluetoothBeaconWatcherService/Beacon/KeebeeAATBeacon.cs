using System;

namespace Keebee.AAT.BluetoothBeaconWatcherService.Beacon
{
    public class KeebeeBeacon
    {
        public Beacon.BeaconTypeEnum BeaconType { get; set; }
        public string CompanyUuid { get; set; }
        public int FacilityId { get; set; }
        public int Rssi { get; set; }
        public int ResidentId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
