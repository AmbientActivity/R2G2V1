using Keebee.AAT.ServiceModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Constants;
using Keebee.AAT.EventLogging;
using System.Net;
using System.Threading;
using System.Linq;
using System;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;

namespace Keebee.AAT.StateMachineService
{
    public partial class StateMachineService : ServiceBase
    {
        private const string UrlKeepAlive = "http://localhost/Keebee.AAT.Operations/api/status";

        // operations REST client
        private readonly IOperationsClient _opsClient;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueResponse;

        // event logger
        private readonly EventLogger _eventLogger;

        // active profile
        private Profile _activeProfile;

        // display state
        private bool _displayIsActive;

        public StateMachineService()
        {
            InitializeComponent();

            _eventLogger = new EventLogger(EventLogType.StateMachineService);
            _opsClient = new OperationsClient { EventLogger = _eventLogger };
 
            InitializeMessageQueueListeners();

            // message queue sender
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response
            }) {EventLogger = _eventLogger};

            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();

            //var activityLogExportThread = new Thread(ExportActivityLog);
            //activityLogExportThread.Start();
        }

        private void KeepAlive()
        {
            while (true)
            {
                // only execute if the display is active and the current active profile is "Generic"
                if (_displayIsActive && _activeProfile?.ResidentId == 0)
                {
                    var req = (HttpWebRequest)WebRequest.Create(UrlKeepAlive);
                    var response = (HttpWebResponse)req.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        _eventLogger.WriteEntry(
                            $"Error accessing web host.{Environment.NewLine}StatusCode: {response.StatusCode}");
                }

                try
                {
                    Thread.Sleep(60000);
                }

                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        private static void ExportActivityLog()
        {
            while (true)
            {
                var exporter = new ActivityLog.Exporter();

                exporter.Export(DateTime.Now.ToString("MM/dd/yyyy"));

                try
                {
                    var minutesToSleep = (int)(new DateTime(
                        DateTime.Now.AddDays(1).Year, 
                        DateTime.Now.AddDays(1).Month, 
                        DateTime.Now.AddDays(1).Day, 3, 0, 0) - DateTime.Now).TotalMinutes;

                    Thread.Sleep(minutesToSleep);
                }

                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget,
                MessageReceivedCallback = MessageReceivedPhidget
            }) { EventLogger = _eventLogger };

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Video,
                MessageReceivedCallback = MessageReceivedVideo
            }) { EventLogger = _eventLogger };

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Rfid,
                MessageReceivedCallback = MessageReceivedRfid
            }) { EventLogger = _eventLogger };

            var q4 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Display,
                MessageReceivedCallback = MessageReceivedDisplay
            }) { EventLogger = _eventLogger };
        }

        private void ExecuteUserResponse(int activityTypeId, int sensorValue)
        {
            if (_activeProfile == null) return;

            try
            {
                var profileDetail = _activeProfile.ProfileDetails
                    .FirstOrDefault(pd => pd.ActivityTypeId == activityTypeId);

                if (profileDetail == null) return;

                var responseMessage = new ResponseMessage
                {
                    ResidentId = _activeProfile.ResidentId,
                    ProfileDetailId = profileDetail.Id,
                    ActivityTypeId = profileDetail.ActivityTypeId,
                    ResponseTypeId = profileDetail.ResponseType.Id,
                    GameDifficultyLevel = _activeProfile.GameDifficultyLevel,
                    ResponseValue = sensorValue
                }; 

                _messageQueueResponse.Send(CreateResponseMessageBody(responseMessage));
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"ExecuteUserResponse: {ex.Message}", EventLogEntryType.Error); 
            }
        }

        private void ExecuteSystemResponse(int sensorValue)
        {
            try
            {
                var responseMessage = new ResponseMessage
                {
                    ProfileDetailId = sensorValue,
                    ResponseValue = 0
                };

                _messageQueueResponse.Send(CreateResponseMessageBody(responseMessage));
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"ExecuteSystemResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        #region message received event handlers

        private void MessageReceivedPhidget(object source, MessageEventArgs e)
        {
            try
            {
                // do nothing unless the display is active
                if (!_displayIsActive) return;

                var phidget = GetPhidgetFromMessageBody(e.MessageBody);
                if (phidget == null) return;

                var sensorValue = phidget.SensorValue;
                if (sensorValue >= 0) // user response
                {
                    // sensorId's are base 0 - convert to base 1 for ActivityTypeId
                    var activityTypeId = phidget.SensorId + 1;
                    ExecuteUserResponse(activityTypeId, sensorValue);
                }
                else // system response
                {
                    ExecuteSystemResponse(sensorValue);
                }
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"QueueMessageReceivedPhidget: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private static PhidgetMessage GetPhidgetFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var phidget = serializer.Deserialize<PhidgetMessage>(messageBody);
            return phidget;
        }

        private static string CreateResponseMessageBody(ResponseMessage responseMessage)
        {
            var serializer = new JavaScriptSerializer();
            var responseMessageBody = serializer.Serialize(responseMessage);
            return responseMessageBody;
        }

        private void MessageReceivedRfid(object source, MessageEventArgs e)
        {
            try
            {
                int residentId;
                var isValid = int.TryParse(e.MessageBody, out residentId);
                if (!isValid) return;

                if (residentId > 0)
                {
                    if (_activeProfile?.ResidentId != residentId)
                        _activeProfile = _opsClient.GetResidentProfile(residentId);
                }
                else
                {
                    if (_activeProfile?.ResidentId != 0)
                        _activeProfile = _opsClient.GetGenericProfile();
                }
            }

            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"MessageReceivedRfid{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedVideo(object source, MessageEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedDisplay(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayFromMessageBody(e.MessageBody);

                _displayIsActive = displayMessage.IsActive;

                if (!_displayIsActive) return;

                if (_activeProfile == null)
                    _activeProfile = _opsClient.GetGenericProfile();
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"MessageReceivedDisplay{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private static DisplayMessage GetDisplayFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var display = serializer.Deserialize<DisplayMessage>(messageBody);
            return display;
        }

        #endregion

        protected override void OnStart(string[] args)
        {
            _eventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _eventLogger.WriteEntry("In OnStop");
        }
    }
}
