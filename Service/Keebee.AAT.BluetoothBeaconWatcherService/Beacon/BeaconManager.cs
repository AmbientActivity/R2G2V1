using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Bluetooth.Advertisement;

namespace Keebee.AAT.BluetoothBeaconWatcherService.Beacon
{
    /// <summary>
    /// Manages multiple beacons and distributes received Bluetooth LE
    /// Advertisements based on unique Bluetooth beacons.
    /// 
    /// Whenever your app gets a callback that it has received a new Bluetooth LE
    /// advertisement, send it to the ReceivedAdvertisement() method of this class,
    /// which will handle the data and either add a new Bluetooth beacon to the list
    /// of beacons observed so far, or update a previously known beacon with the
    /// new advertisement information.
    /// </summary>
    public class BeaconManager
    {
        /// <summary>
        /// List of known beacons so far, which all have a unique Bluetooth MAC address
        /// and can have multiple data frames.
        /// </summary>
        public ObservableCollection<Beacon> BluetoothBeacons { get; set; } = new ObservableCollection<Beacon>();

        /// <summary>
        /// Analyze the received Bluetooth LE advertisement, and either add a new unique
        /// beacon to the list of known beacons, or update a previously known beacon
        /// with the new information.
        /// </summary>
        /// <param name="btAdv">Bluetooth advertisement to parse, as received from
        /// the Windows Bluetooth LE API.</param>
        public void ReceivedAdvertisement(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {

            if (btAdv == null) return;
            var beacons = BluetoothBeacons.ToList();

            foreach (var bluetoothBeacon in beacons)
            {
                // check if we already know this bluetooth address
                if (bluetoothBeacon.BluetoothAddress != btAdv.BluetoothAddress) continue;

                // update it
                bluetoothBeacon.UpdateBeacon(btAdv);

                return;
            }

            // make sure a duplicate doesn't get added (just for good measure)
            if (beacons.Any(x => x.BluetoothAddress == btAdv.BluetoothAddress)) return;

            // create a new beacon
            var newBeacon = new Beacon(btAdv);

            // make sure it is an iBeacon, and verify that it has a payload
            if ((newBeacon.BeaconType != Beacon.BeaconTypeEnum.iBeacon) ||
                !newBeacon.BeaconFrames.Any()) return;

            BluetoothBeacons.Add(newBeacon);
        }
    }
}
