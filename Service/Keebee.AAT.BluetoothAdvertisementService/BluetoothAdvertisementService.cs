using Keebee.AAT.SystemEventLogging;
using System;
using System.ServiceProcess;
using Windows.Devices.Bluetooth.Advertisement;

namespace Keebee.AAT.BluetoothAdvertisementService
{
    public partial class BluetoothAdvertisementService : ServiceBase
    {
        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        public BluetoothAdvertisementService()
        {
            InitializeComponent();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.BluetoothAdvertisementrService);

            InitializeWatcher();
        }

        private void InitializeWatcher()
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

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            var address = eventArgs.BluetoothAddress;
            var UIDs = eventArgs.Advertisement.ServiceUuids;

            _systemEventLogger.WriteEntry(
                $"ADDRESS: {address}{Environment.NewLine}" +
                $"UID: {UIDs}");
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
