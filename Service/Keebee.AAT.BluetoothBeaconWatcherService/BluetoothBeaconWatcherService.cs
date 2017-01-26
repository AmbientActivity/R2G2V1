using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.BluetoothBeaconWatcherService.Beacon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceProcess;
using System.Timers;
using System.Web.Script.Serialization;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace Keebee.AAT.BluetoothBeaconWatcherService
{
    public partial class BluetoothBeaconWatcherService : ServiceBase
    {
        // operations api
        private readonly IOperationsClient _opsClient;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // message queue
        private readonly CustomMessageQueue _messageQueueBeaconWatcher;

        // beacon manager
        private readonly BeaconManager _beaconManager;

        // beacon keys
        private readonly string _companyUuid;
        private readonly int _facilityId;

        // beacon watcher
        private readonly BluetoothLEAdvertisementWatcher _watcher;

        // display state
        private bool _displayIsActive;

        // active resident
        private int _activeResidentId;

        // residents
        private ResidentMessage[] _residents;

        private readonly ResidentMessage _publicResident = new ResidentMessage
        {
            Id = PublicMediaSource.Id,
            Name = PublicMediaSource.Name,
            GameDifficultyLevel = 1,
            AllowVideoCapturing = false
        };

        // timer
        private readonly Timer _timer;

        public BluetoothBeaconWatcherService()
        {
            InitializeComponent();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.BluetoothBeaconWatcherService);
            _opsClient = new OperationsClient();

            // app settings
            _companyUuid = ConfigurationManager.AppSettings["CompanyUUID"];
            _facilityId = Convert.ToInt32(ConfigurationManager.AppSettings["FacilityId"]);
            var inRangeThresholdInDb = Convert.ToInt16(ConfigurationManager.AppSettings["InRangeThresholdInDB"]);
            var outOfRangeThresholdInDBb = Convert.ToInt16(ConfigurationManager.AppSettings["OutOfRangeThresholdInDB"]);
            var outOfRangeTimeout = Convert.ToInt16(ConfigurationManager.AppSettings["OutOfRangeTimeout"]);
            var readInterval = Convert.ToInt32(ConfigurationManager.AppSettings["BeaconReadInterval"]);

            // message queue sender
            _messageQueueBeaconWatcher = new CustomMessageQueue(new CustomMessageQueueArgs
                {
                    QueueName = MessageQueueType.BluetoothBeaconWatcher
                })
                {SystemEventLogger = _systemEventLogger};

            // message queue listener
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
                {
                    QueueName = MessageQueueType.BluetoothBeaconWatcherReload,
                    MessageReceivedCallback = MessageReceivedBluetoothBeaconWatcherReload
                })
                {SystemEventLogger = _systemEventLogger};

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
                {
                    QueueName = MessageQueueType.DisplayBluetoothBeaconWatcher,
                    MessageReceivedCallback = MessageReceivedDisplayBluetoothBeaconWatcher
                })
                {SystemEventLogger = _systemEventLogger};

            _beaconManager = new BeaconManager();

            // initialize watcher
            _watcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active,
                SignalStrengthFilter = new BluetoothSignalStrengthFilter
                {
                    InRangeThresholdInDBm = inRangeThresholdInDb,
                    OutOfRangeThresholdInDBm = outOfRangeThresholdInDBb,
                    OutOfRangeTimeout = TimeSpan.FromMilliseconds(outOfRangeTimeout)
                }
            };

            _watcher.Stopped += WatcherOnStopped;
            StartWatching();

            // set the timer for timed beacon reads
            _timer = new Timer(readInterval);
            _timer.Elapsed += TimerElapsed;
        }

        private void StartWatching()
        {
            if (_watcher == null) return;

            try
            {
                _watcher.Received += WatcherOnReceived;
                _watcher.Start();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StartWatching{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StoptWatching()
        {
            if (_watcher == null) return;

            try
            {
                _watcher.Stop();
                _watcher.Received -= WatcherOnReceived;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StoptWatching{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ValidateBeacons();

                if (!_beaconManager.BluetoothBeacons.Any())
                {
                    if (_activeResidentId == PublicMediaSource.Id) return;

                    _messageQueueBeaconWatcher.Send(GetSerializedResident(_publicResident));
                    _activeResidentId = PublicMediaSource.Id;
                }

                else if (LoadResidents())
                {
                    var closestBeacon = GetClosestKeebeeBeacon(_beaconManager.BluetoothBeacons);
                    var residentId = closestBeacon?.ResidentId ?? 0;

                    if (residentId == _activeResidentId) return;

                    var resident = GetResident(residentId) ?? _publicResident;
                    _messageQueueBeaconWatcher.Send(GetSerializedResident(resident));
                    _activeResidentId = residentId;
                }
            }
            catch(InvalidOperationException)
            { 
                // occurs when a beacon is removed from the beacon manager's list - ignore it
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"TimerElapsed{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ValidateBeacons()
        {
            try
            {
                var beacons = _beaconManager.BluetoothBeacons;
                var tenSeconds = new TimeSpan(0, 0, 0, 10, 0);

                foreach (var beacon in beacons)
                {
                    // if the beacon has stopped advertising remove it
                    if (DateTimeOffset.Now - beacon.Timestamp >= tenSeconds)
                    {
                        _beaconManager.BluetoothBeacons.Remove(beacon);
                    }
                    else
                    {
                        if (!beacon.Rssi.Any()) continue;

                        if (beacon.Rssi.Last() <= -127)
                            _beaconManager.BluetoothBeacons.Remove(beacon);
                    }
                }
            }
            catch (ArgumentException e)
            {
                _systemEventLogger.WriteEntry($"ValidateBeacons{Environment.NewLine}{e}", EventLogEntryType.Error);
            }
        }

        private KeebeeBeacon GetClosestKeebeeBeacon(IEnumerable<Beacon.Beacon> allBeacons)
        {
            try
            {
                var keebeeBeacons = allBeacons
                    .Where(x => x.BeaconFrames.Count > 0)
                    .Select(x =>
                    {
                        var beaconFrame = x.BeaconFrames.First();
                        return new KeebeeBeacon
                        {
                            BeaconType = x.BeaconType,
                            Rssi = x.Rssi.Sum(y => y) / x.Rssi.Count,  // calculate average signal strength
                            CompanyUuid = GetCompanyUuid(x),
                            FacilityId =
                                GetIntFromByteArray(new byte[] { 0, 0, beaconFrame.Payload[18], beaconFrame.Payload[19] }),
                            ResidentId =
                                GetIntFromByteArray(new byte[] { 0, 0, beaconFrame.Payload[20], beaconFrame.Payload[21] })
                        };
                    })
                    .Where(x => x.CompanyUuid == _companyUuid && x.FacilityId == _facilityId)
                    .OrderByDescending(x => x.Rssi);

                return !keebeeBeacons.Any() ? null : keebeeBeacons.First();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"GetClosestKeebeeBeacon{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
                return null;
            }
        }

        private void WatcherOnReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            try
            {
                _beaconManager.ReceivedAdvertisement(eventArgs);
            }
            catch (ArgumentException e)
            {
                _systemEventLogger.WriteEntry($"WatcherOnReceived{Environment.NewLine}{e}", EventLogEntryType.Error);
            }
        }

        private void WatcherOnStopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            string errorMsg = null;
            if (args != null)
            {
                switch (args.Error)
                {
                    case BluetoothError.Success:
                        errorMsg = "WatchingSuccessfullyStopped";
                        break;
                    case BluetoothError.RadioNotAvailable:
                        errorMsg = "ErrorNoRadioAvailable";
                        break;
                    case BluetoothError.ResourceInUse:
                        errorMsg = "ErrorResourceInUse";
                        break;
                    case BluetoothError.DeviceNotConnected:
                        errorMsg = "ErrorDeviceNotConnected";
                        break;
                    case BluetoothError.DisabledByPolicy:
                        errorMsg = "ErrorDisabledByPolicy";
                        break;
                    case BluetoothError.NotSupported:
                        errorMsg = "ErrorNotSupported";
                        break;
                }
            }

            if (errorMsg != null) return;

            _systemEventLogger.WriteEntry($"WatcherOnStopped{Environment.NewLine}Failed to restart Bluetooth Watch", EventLogEntryType.Error);
        }

        private bool LoadResidents()
        {
            try
            {
                if (_residents != null) return true;

                var residents = _opsClient.GetResidents().ToArray();

                _residents = residents.Select(r => new ResidentMessage
                {
                    Id = r.Id,
                    Name = $"{r.FirstName} {r.LastName}".Trim(),
                    GameDifficultyLevel = r.GameDifficultyLevel,
                    AllowVideoCapturing = r.AllowVideoCapturing
                }).ToArray();

                _activeResidentId = PublicMediaSource.Id;
                return _residents.Any();

            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"LoadResidents: {ex.Message}", EventLogEntryType.Error);
            }
            return false;
        }

        private ResidentMessage GetResident(int id)
        {
            try
            {
                return _residents?.SingleOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"GetResident: {ex.Message}", EventLogEntryType.Error);
                return null;
            }
        }

        private static string GetSerializedResident(ResidentMessage resident)
        {
            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(resident);

            return messageBody;
        }

        private void MessageReceivedBluetoothBeaconWatcherReload(object source, MessageEventArgs e)
        {
            IEnumerable<ResidentMessage> residents = null;
            try
            {
                var serializer = new JavaScriptSerializer();
                residents = serializer.Deserialize<IEnumerable<ResidentMessage>>(e.MessageBody);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedBluetoothBeaconWatcherReload: {ex.Message}", EventLogEntryType.Error);
            }

            if (residents != null)
                _residents = residents.ToArray();
        }

        private void MessageReceivedDisplayBluetoothBeaconWatcher(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _displayIsActive = displayMessage.IsActive;

                if (_displayIsActive)
                {
                    StartWatching();
                    _timer?.Start();
                }
                else
                {
                    _timer?.Stop();
                    StoptWatching();
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplaydBluetoothBeaconWatcher{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private static DisplayMessage GetDisplayStateFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var display = serializer.Deserialize<DisplayMessage>(messageBody);
            return display;
        }

        #region Tools

        private static string GetCompanyUuid(Beacon.Beacon bluetoothBeacon)
        {

            var payload = string.Empty;
            var uuid = string.Empty;

            var beaconFrame = bluetoothBeacon.BeaconFrames.First();

            if (beaconFrame is UidEddystoneFrame)
            {
                payload = ((UidEddystoneFrame)beaconFrame).NamespaceIdAsNumber.ToString("X") +
                          " / " +
                          ((UidEddystoneFrame)beaconFrame).InstanceIdAsNumber.ToString("X");
            }
            else
            {
                if (!(beaconFrame is UrlEddystoneFrame) && !(beaconFrame is TlmEddystoneFrame))
                    payload = BitConverter.ToString(((UnknownBeaconFrame)beaconFrame).Payload);
            }

            var payloadBytes = HexStringToByteArray(payload);
            if (payloadBytes.Length <= 17) return uuid;

            var uUidBytes = new[]
            {
                payloadBytes[2], payloadBytes[3], payloadBytes[4], payloadBytes[5],
                payloadBytes[6], payloadBytes[7], payloadBytes[8], payloadBytes[9],
                payloadBytes[10], payloadBytes[11], payloadBytes[12], payloadBytes[13],
                payloadBytes[14], payloadBytes[15], payloadBytes[16], payloadBytes[17],
            };

            uuid = BitConverter.ToString(uUidBytes);
            return uuid;
        }

        private static int GetIntFromByteArray(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Convert minus-separated hex string to a byte array. Format example: "4E-66-63"
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hex)
        {
            // Remove all space characters
            var hexPure = hex.Replace("-", "");
            if (hexPure.Length % 2 != 0)
            {
                // No even length of the string
                throw new Exception("No valid hex string");
            }
            var numberChars = hexPure.Length / 2;
            var bytes = new byte[numberChars];
            var sr = new StringReader(hexPure);
            try
            {
                for (var i = 0; i < numberChars; i++)
                {
                    bytes[i] = Convert.ToByte(new string(new[] { (char)sr.Read(), (char)sr.Read() }), 16);
                }
            }
            catch (Exception)
            {
                throw new Exception("No valid hex string");
            }
            finally
            {
                sr.Dispose();
            }
            return bytes;
        }
        #endregion

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
            _timer.Stop();
            _timer.Dispose();
        }
    }
}