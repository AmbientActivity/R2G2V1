using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.BluetoothBeaconWatcherService.Beacon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;

namespace Keebee.AAT.BluetoothBeaconWatcherService
{
    public partial class BluetoothBeaconWatcherService : ServiceBase
    {
        #region declaration

        // message queue
        private readonly CustomMessageQueue _messageQueueBeaconWatcher;
        private readonly CustomMessageQueue _messageQueueBeaconMonitor;
        private readonly CustomMessageQueue _messageQueueBeaconMonitorResident;

        private bool _beaconMonitorIsActive;

        private const int BeaconReadInterval = 1000;   // 1 second

        // api client
        private readonly IResidentsClient _residentsClient;

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
        private IList<ResidentBluetoothMessage> _residents;

        private readonly ResidentBluetoothMessage _publicResident = new ResidentBluetoothMessage
        {
            Id = PublicProfileSource.Id,
            Name = PublicProfileSource.Name
        };

        // timer
        private readonly Timer _timer;

        #endregion

        public BluetoothBeaconWatcherService()
        {
            InitializeComponent();
            _residentsClient = new ResidentsClient();

            // app settings
            _companyUuid = ConfigurationManager.AppSettings["CompanyUUID"];
            _facilityId = Convert.ToInt32(ConfigurationManager.AppSettings["FacilityId"]);
            var inRangeThresholdInDb = Convert.ToInt16(ConfigurationManager.AppSettings["InRangeThresholdInDB"]);
            var outOfRangeThresholdInDBb = Convert.ToInt16(ConfigurationManager.AppSettings["OutOfRangeThresholdInDB"]);
            var outOfRangeTimeout = Convert.ToInt16(ConfigurationManager.AppSettings["OutOfRangeTimeout"]);

            // message queue sender
            _messageQueueBeaconWatcher = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BluetoothBeaconWatcher
            });

            _messageQueueBeaconMonitor = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BeaconMonitor
            });

            _messageQueueBeaconMonitorResident = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BeaconMonitorResident
            });

            // message queue listener
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BluetoothBeaconWatcherReload,
                MessageReceivedCallback = MessageReceivedBluetoothBeaconWatcherReload
            });

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayBluetoothBeaconWatcher,
                MessageReceivedCallback = MessageReceivedDisplayBluetoothBeaconWatcher
            });

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BeaconMonitorState,
                MessageReceivedCallback = BeaconMonitorMessageReceived
            });

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
            _watcher.Received += WatcherOnReceived;

            // set the timer for timed beacon reads
            _timer = new Timer(BeaconReadInterval);
            _timer.Elapsed += TimerElapsed;

            _watcher.Start();
            _timer.Start();
        }

        #region initialization

        private void StopWatcher()
        {
            if (_watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
                _watcher.Stop();
        }

        private void StartWatcher()
        {
            if (_watcher.Status == BluetoothLEAdvertisementWatcherStatus.Stopped)
                _watcher.Start();
        }

        #endregion

        #region core logic

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_displayIsActive) return;

                _timer.Stop();
                StopWatcher();

                ValidateBeacons();

                if (!_beaconManager.BluetoothBeacons.Any())
                {
                    if (_activeResidentId == PublicProfileSource.Id)
                    {
                        _timer.Start();
                        StartWatcher();
                        return;
                    }

                    _messageQueueBeaconWatcher.Send(JsonConvert.SerializeObject(_publicResident));
                    _activeResidentId = PublicProfileSource.Id;

                    if (_beaconMonitorIsActive)
                        _messageQueueBeaconMonitorResident.Send(GetSerializedBeaconWatcherMonitorResidentMessage(_activeResidentId, PublicProfileSource.Name, 0));
                }

                else if (LoadResidents())
                {
                    var closestBeacon = GetClosestKeebeeBeacon(_beaconManager.BluetoothBeacons);
                    var residentId = closestBeacon?.ResidentId ?? 0;

                    if (_beaconMonitorIsActive)
                    {
                        var r = GetResident(residentId) ?? _publicResident;
                        _messageQueueBeaconMonitorResident.Send(GetSerializedBeaconWatcherMonitorResidentMessage(r.Id, r.Name, closestBeacon?.Rssi));
                    }

                    if (residentId == _activeResidentId)
                    {
                        _timer.Start();
                        StartWatcher();
                        return;
                    }

                    var resident = GetResident(residentId) ?? _publicResident;

                    _messageQueueBeaconWatcher.Send(JsonConvert.SerializeObject(resident));
                    _activeResidentId = residentId;
                }
            }
            catch (InvalidOperationException)
            {
                // occurs when a beacon is removed from the beacon manager's list - ignore it
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"TimerElapsed{Environment.NewLine}{ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
            }

            _timer.Start();
            StartWatcher();
        }

        private void ValidateBeacons()
        {
            try
            {
                var beacons = _beaconManager.BluetoothBeacons.ToArray();
                var fiveSeconds = new TimeSpan(0, 0, 0, 5, 0);

                foreach (var beacon in beacons)
                {
                    // if the beacon has stopped advertising for more than 5 seconds then remove it
                    if (DateTimeOffset.Now - beacon.Timestamp < fiveSeconds) continue;
                    _beaconManager.BluetoothBeacons.Remove(beacon);

                    // if the beacon monitor is active then send it list of beacons
                    if (!_beaconMonitorIsActive) continue;
                    _messageQueueBeaconMonitor.Send(GetSerializedBeaconWatcherMonitorMessage(_beaconManager.BluetoothBeacons));
                }
            }
            catch (ArgumentException e)
            {
                SystemEventLogger.WriteEntry($"ValidateBeacons{Environment.NewLine}{e}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"GetClosestKeebeeBeacon{Environment.NewLine}{ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
                return null;
            }
        }

        private bool LoadResidents()
        {
            try
            {
                if (_residents != null) return true;
                var residents = _residentsClient.Get().ToArray();
                if (!residents.Any()) return false;

                _residents = residents.Select(r => new ResidentBluetoothMessage
                {
                    Id = r.Id,
                    Name = $"{r.FirstName} {r.LastName}".Trim()
                }).ToArray();

                _activeResidentId = PublicProfileSource.Id;
                return _residents.Any();

            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"LoadResidents: {ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
            }
            return false;
        }

        private ResidentBluetoothMessage GetResident(int id)
        {
            try
            {
                return _residents?.SingleOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"GetResident: {ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
                return null;
            }
        }

        #endregion

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

        #region message send/receive

        private void MessageReceivedBluetoothBeaconWatcherReload(object source, MessageEventArgs e)
        {
            if (_residents == null) return;

            ResidentBluetoothMessage resident = null;
            try
            {
                resident = JsonConvert.DeserializeObject<ResidentBluetoothMessage>(e.MessageBody);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceivedBluetoothBeaconWatcherReload: {ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
            }

            if (resident == null) return;
            if (resident.Id <= 0) return;

            if (resident.IsDeleted)
            {
                _residents = _residents.Where(x => x.Id != resident.Id).ToList();
            }
            else
            {
                if (_residents.Any(x => x.Id == resident.Id))
                    _residents.Single(x => x.Id == resident.Id).Name = resident.Name;
                else
                    _residents.Add(new ResidentBluetoothMessage { Id = resident.Id, Name = resident.Name });
            }
        }

        private void MessageReceivedDisplayBluetoothBeaconWatcher(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _displayIsActive = displayMessage.IsActive;

                if (_displayIsActive) return;

                _messageQueueBeaconWatcher.Send(JsonConvert.SerializeObject(_publicResident));
                _activeResidentId = PublicProfileSource.Id;

                if (!_beaconMonitorIsActive) return;
                _messageQueueBeaconMonitorResident.Send(JsonConvert.SerializeObject(_publicResident));
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceivedDisplaydBluetoothBeaconWatcher{Environment.NewLine}{ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
            }
        }

        private static DisplayMessage GetDisplayStateFromMessageBody(string messageBody)
        {
            var display = JsonConvert.DeserializeObject<DisplayMessage>(messageBody);
            return display;
        }

        private static string GetSerializedBeaconWatcherMonitorMessage(IEnumerable<Beacon.Beacon> beacons)
        {
            var message = beacons
                .Where(b => b.BeaconFrames.Count > 0)
                .Select(b =>
                {
                    var beaconFrame = b.BeaconFrames.First();
                    return new BeaconMonitorMessage
                    {
                        BeaconType = b.BeaconType.ToString(),
                        Address = b.BluetoothAddress,
                        Rssi = b.Rssi.ToArray(),
                        TimeStamp = b.Timestamp,
                        Payload = $"{GetCompanyUuid(b)}-" +
                                  $"{GetIntFromByteArray(new byte[] { 0, 0, beaconFrame.Payload[18], beaconFrame.Payload[19] })}" +
                                  $"-{GetIntFromByteArray(new byte[] { 0, 0, beaconFrame.Payload[20], beaconFrame.Payload[21] })}"

                    };
                }).ToArray();

            return JsonConvert.SerializeObject(message);
        }

        private static string GetSerializedBeaconWatcherMonitorResidentMessage(int residentId, string residentName, int? rssi)
        {
            var localRssi = rssi ?? 0;
            var message = new BeaconMonitorResidentMessage
            {
                ResidentId = residentId,
                ResidentName = residentName,
                Rssi = localRssi
            };

            return JsonConvert.SerializeObject(message);
        }

        private void BeaconMonitorMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                var message = (e.MessageBody);

                var activeState = Convert.ToInt16(message);
                _beaconMonitorIsActive = activeState > 0;

                if (!_beaconMonitorIsActive) return;

                var resident = _activeResidentId > 0
                    ? GetResident(_activeResidentId)
                    : _publicResident;

                KeebeeBeacon closestBeacon = null;
                if (_activeResidentId > 0)
                {
                    if (_beaconManager.BluetoothBeacons.Any())
                    {
                        closestBeacon = GetClosestKeebeeBeacon(_beaconManager.BluetoothBeacons);
                    }
                }

                _messageQueueBeaconMonitorResident.Send(
                    GetSerializedBeaconWatcherMonitorResidentMessage(_activeResidentId, resident.Name,
                        closestBeacon?.Rssi));
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"BeaconMonitorMessageReceived: {ex.Message}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
            }
        }

        #endregion

        #region event handlers

        private void WatcherOnReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            try
            {
                if (!_displayIsActive) return;

                _beaconManager.ReceivedAdvertisement(eventArgs);

                if (!_beaconMonitorIsActive) return;
                _messageQueueBeaconMonitor.Send(GetSerializedBeaconWatcherMonitorMessage(_beaconManager.BluetoothBeacons));
            }
            catch (ArgumentException e)
            {
                SystemEventLogger.WriteEntry($"WatcherOnReceived{Environment.NewLine}{e}", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
            }
        }

        private static void WatcherOnStopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
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

            SystemEventLogger.WriteEntry($"WatcherOnStopped{Environment.NewLine}Failed to restart Bluetooth Watch", SystemEventLogType.BluetoothBeaconWatcherService, EventLogEntryType.Error);
        }

        protected override void OnStart(string[] args)
        {
            SystemEventLogger.WriteEntry("In OnStart", SystemEventLogType.BluetoothBeaconWatcherService);
        }

        protected override void OnStop()
        {
            SystemEventLogger.WriteEntry("In OnStop", SystemEventLogType.BluetoothBeaconWatcherService);
            _timer.Stop();
            _timer.Dispose();
        }

        #endregion
    }
}