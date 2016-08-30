using Keebee.AAT.RESTClient;
using Keebee.AAT.EventLogging;
using Keebee.AAT.MessageQueuing;
#if DEBUG
using Keebee.AAT.Constants;
using System.Web.Script.Serialization;
#endif
using SkyeTek.Devices;
using SkyeTek.STPv3;
using SkyeTek.Tags;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace Keebee.AAT.RFIDReaderService
{
    partial class RFIDReaderService : ServiceBase
    {
#if DEBUG
        private readonly CustomMessageQueue _messageQueueRfidMonitor;
        private bool _readerMonitorIsActive;
#endif

        // operations REST client
        private readonly IOperationsClient _opsClient;

        // event logger
        private readonly EventLogger _eventLogger;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueRfid;

        // skyetek
        private Device _device;

        // thread 
        private readonly Thread _readTagThread;

        // read parameters
        private const int MaxReads = 15;
        private const int ReadInterval = 300;   // in milliseconds

        public RFIDReaderService()
        {
            InitializeComponent();
            _eventLogger = new EventLogger(EventLogType.RfidReaderService);
            _opsClient = new OperationsClient { EventLogger = _eventLogger };

            // message queue sender
            _messageQueueRfid = new CustomMessageQueue(new CustomMessageQueueArgs
                                                       {
                                                           QueueName = MessageQueueType.Rfid
                                                       }) {EventLogger = _eventLogger};
#if DEBUG
            _messageQueueRfidMonitor = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.RfidMonitor
            });

            var q = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.RfidMonitorState,
                MessageReceivedCallback = RFIDReaderMonitorMessageReceived
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
                _eventLogger.WriteEntry("No USB devices found", EventLogEntryType.Warning);
            }
            else
            {
                _device = devices[0];
                _device.Open();
            }
        }

        private void ReadTag()
        {
            while (true)
            {
                var residentId = GetResidentId();

                if (residentId < 0) continue;

                if ((residentId > 0) && _opsClient.ResidentProfileExists(residentId))
                {
#if DEBUG
                    if (_readerMonitorIsActive)
                        _messageQueueRfidMonitor.Send(CreateMessageBody(true, 1, residentId));
#endif
                    _messageQueueRfid.Send(Convert.ToString(residentId));
                }
                else
                {
#if DEBUG
                    if (_readerMonitorIsActive)
                        _messageQueueRfidMonitor.Send(CreateMessageBody(true, 1, 0));
#endif
                    _messageQueueRfid.Send("0");
                }
            }
        }

        private int GetResidentId()
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

            var nextId = nonZeroArray
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
                return -1;   // there's a tie - do nothing
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

            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"Read: {ex.Message}", EventLogEntryType.Error);
            }

            return residentId;
        }

#if DEBUG
        private static string CreateMessageBody(bool isFinal, int readCount, int residentId)
        {
            var message = new RfidMonitorMessage {IsFinal = isFinal, ReadCount = readCount, ResidentId = residentId};

            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(message);
            return messageBody;
        }

        private void RFIDReaderMonitorMessageReceived(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);

            var activeState = Convert.ToInt16(message);
            _readerMonitorIsActive = activeState > 0;
        }
#endif

        protected override void OnStart(string[] args)
        {
            _eventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _readTagThread.Abort();
            _eventLogger.WriteEntry("In OnStop");
        }
    }
}
