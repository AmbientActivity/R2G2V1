using Keebee.AAT.ServiceModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Constants;
using Keebee.AAT.SystemEventLogging;
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
        private readonly SystemEventLogger _systemEventLogger;

        // active profile
        private Profile _activeProfile;

        // display state
        private bool _displayIsActive;

        private readonly System.Timers.Timer _autoLogTimer;
        private DateTime _scheduleAutoLogTime;

        public StateMachineService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.StateMachineService);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
 
            InitializeMessageQueueListeners();

            // message queue sender
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response
            }) {SystemEventLogger = _systemEventLogger};

            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();

            _autoLogTimer = new System.Timers.Timer();
            _scheduleAutoLogTime = DateTime.Today.AddDays(1).AddHours(1);
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
                        _systemEventLogger.WriteEntry(
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

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget,
                MessageReceivedCallback = MessageReceivedPhidget
            }) { SystemEventLogger = _systemEventLogger };

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Video,
                MessageReceivedCallback = MessageReceivedVideo
            }) { SystemEventLogger = _systemEventLogger };

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Rfid,
                MessageReceivedCallback = MessageReceivedRfid
            }) { SystemEventLogger = _systemEventLogger };

            var q4 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Display,
                MessageReceivedCallback = MessageReceivedDisplay
            }) { SystemEventLogger = _systemEventLogger };
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
                _systemEventLogger.WriteEntry($"ExecuteUserResponse: {ex.Message}", EventLogEntryType.Error); 
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
                _systemEventLogger.WriteEntry($"ExecuteSystemResponse: {ex.Message}", EventLogEntryType.Error);
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
                _systemEventLogger.WriteEntry($"QueueMessageReceivedPhidget: {ex.Message}", EventLogEntryType.Error);
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
                _systemEventLogger.WriteEntry($"MessageReceivedRfid{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedVideo(object source, MessageEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry(ex.Message, EventLogEntryType.Error);
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
                _systemEventLogger.WriteEntry($"MessageReceivedDisplay{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
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
            _systemEventLogger.WriteEntry("In OnStart");

            // For first time, set amount of seconds between current time and schedule time
            _autoLogTimer.Enabled = true;
            _autoLogTimer.Interval = _scheduleAutoLogTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            _autoLogTimer.Elapsed += ExportEventLog;
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }

        // This method is called by the timer delegate.
        public void ExportEventLog(object sender, System.Timers.ElapsedEventArgs e)
        {
            var exporter = new EventLogging.Exporter();

            var date = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
            exporter.ExportAndSave(date);

            _systemEventLogger.WriteEntry($"Log automatically exported for date: {date}");

            // If tick for the first time, reset next run to every 24 hours
            if (_autoLogTimer.Interval != 24 * 60 * 60 * 1000)
            {
                _autoLogTimer.Interval = 24 * 60 * 60 * 1000;
            }
        }
    }
}
