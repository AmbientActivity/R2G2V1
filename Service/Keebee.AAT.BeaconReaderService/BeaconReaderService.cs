using Keebee.AAT.SystemEventLogging;
using System;
using System.ServiceProcess;
using Windows.Devices.Bluetooth.Advertisement;

namespace Keebee.AAT.BeaconReaderService
{
    public partial class BeaconReaderService : ServiceBase
    {
        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        public BeaconReaderService()
        {
            InitializeComponent();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.BeaconReaderService);

            InitializeWatcher();
        }

        private static void InitializeWatcher()
        {
            var watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active,
                SignalStrengthFilter =
                {
                    // Only activate the watcher when we're recieving values >= -80
                    InRangeThresholdInDBm = -80,
                    // Stop watching if the value drops below -90 (user walked away)
                    OutOfRangeThresholdInDBm = -90
                }
            };

            // Register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // Wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // Starting watching for advertisements
            watcher.Start();
        }

        private static void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            var address = eventArgs.BluetoothAddress;
            var name = eventArgs.Advertisement.LocalName;
        }

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }
    }
}
