using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using SkyeTek.Devices;
using SkyeTek.STPv3;
using SkyeTek.Tags;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Web.Script.Serialization;

namespace Keebee.AAT.RfidReaderService
{
    internal partial class RfidReaderService : ServiceBase
    {
#if DEBUG
        private readonly CustomMessageQueue _messageQueueRfidMonitor;
        private bool _readerMonitorIsActive;
#endif

        // residents
        private Resident[] _residents;

        // operations REST client
        private readonly IOperationsClient _opsClient;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueRfid;

        // skyetek
        private Device _device;

        // thread 
        private readonly Thread _readTagThread;

        // read parameters
        private const int MaxReads = 15;
        private const int ReadInterval = 300;   // in milliseconds

        public RfidReaderService()
        {
            InitializeComponent();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.RfidReaderService);
            _opsClient = new OperationsClient();

            // message queue sender
            _messageQueueRfid = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Rfid
            })
            { SystemEventLogger = _systemEventLogger };
#if DEBUG
            _messageQueueRfidMonitor = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.RfidMonitor
            });

            var q = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.RfidMonitorState,
                MessageReceivedCallback = RfidReaderMonitorMessageReceived
            });
#endif
            InitializeReader();
            _readTagThread = new Thread(ReadTag);
            _readTagThread.Start();
        }

        private void InitializeReader()
        {
            Device[] devices = USBDeviceFactory.Enumerate();
            if (devices.Length == 0)
            {
                _systemEventLogger.WriteEntry("No RFID Reader devices found", EventLogEntryType.Warning);
            }
            else
            {
                _device = devices[0];
                _device.Open();
                _systemEventLogger.WriteEntry("Device initialized successfully");
            }
        }

        private void ReadTag()
        {
            try
            {
                if (_residents == null)
                    LoadResidents();

                while (true)
                {
                    var residentId = GetResidentId();
                    if (residentId < 0) continue;

                    var resident = residentId == 0
                        ? new Resident { Id = PublicMediaSource.Id, FirstName = PublicMediaSource.Name, GameDifficultyLevel = 1, AllowVideoCapturing = false }
                        : GetResident(residentId);

                    if (resident == null) continue;
#if DEBUG
                    if (_readerMonitorIsActive)
                        _messageQueueRfidMonitor.Send(CreateMessageBody(true, 1, residentId));
#endif
                    _messageQueueRfid.Send(CreateMessageBodyFromResident(resident));
                }
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ReadTag: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private int GetResidentId()
        {
            int nextId = -1;

            try
            {
                var idArray = new int[MaxReads];

                for (var i = 0; i < MaxReads; i++)
                {
                    var id = Read();
                    if (id >= 0)
                        idArray[i] = id;
#if DEBUG
                if (_readerMonitorIsActive)
                    _messageQueueRfidMonitor.Send(CreateMessageBody(false, i + 1, idArray[i]));
#endif
                    try
                    {
                        Thread.Sleep(ReadInterval);
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                }

                if (!idArray.Any(x => x > 0)) return 0;

                var nonZeroArray = idArray.Where(x => x > 0).ToArray();

                nextId = nonZeroArray
                    .GroupBy(x => x)
                    .OrderByDescending(x => x.Count())
                    .First().Key;

                var distinctIds = nonZeroArray.Distinct();

                var maxCount = nonZeroArray
                    .Select(x => x)
                    .Count(x => x == nextId);

                if (distinctIds.Any(id => nonZeroArray.Select(x => x)
                                              .Where(x => x != nextId)
                                              .Count(x => x == id) == maxCount))
                {
                    return -1; // there's a tie - do nothing
                }
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"GetResidentId: {ex.Message}", EventLogEntryType.Error);
            }

            return nextId;
        }

        private int Read()
        {
            if (_device == null) return -1;

            var residentId = -1;
            try
            {
                var tag = new Tag { Type = TagType.AUTO_DETECT };

                var request = new STPv3Request
                {
                    Command = STPv3Commands.SELECT_TAG,
                    Tag = tag,
                    Inventory = true
                };
                request.Issue(_device);

                STPv3Response response;
                while (((response = request.GetResponse()) != null) && (response.Success))
                {
                    if (response.ResponseCode == STPv3ResponseCode.SELECT_TAG_PASS)
                    {
                        var hex = string.Join(string.Empty,
                            Array.ConvertAll(response.TID,
                                value => $"{value:X2}"));

                        int parsedInt;
                        var isValid = int.TryParse(hex, out parsedInt);

                        residentId = isValid
                            ? parsedInt
                            : 0;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                // do nothing
            }

            return residentId;
        }

        private static string CreateMessageBodyFromResident(Resident resident)
        {
            var residentMessage = new ResidentMessage
            {
                Id = resident.Id,
                Name = $"{resident.FirstName} {resident.LastName}".Trim(),
                GameDifficultyLevel = resident.GameDifficultyLevel,
                AllowVideoCapturing = resident.AllowVideoCapturing
            };

            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(residentMessage);

            return messageBody;
        }

        private void LoadResidents()
        {
            try
            {
                _residents = _opsClient.GetResidents().ToArray();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"LoadResidents: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private Resident GetResident(int id)
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

#if DEBUG
        private static string CreateMessageBody(bool isFinal, int readCount, int residentId)
        {
            var message = new RfidMonitorMessage { IsFinal = isFinal, ReadCount = readCount, ResidentId = residentId };

            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(message);
            return messageBody;
        }

        private void RfidReaderMonitorMessageReceived(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);

            var activeState = Convert.ToInt16(message);
            _readerMonitorIsActive = activeState > 0;
        }
#endif

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
            _readTagThread.Abort();
        }
    }
}
