using System.Configuration;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Administrator.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly SystemEventLogger _systemEventLogger;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueConfigSms;
        private readonly CustomMessageQueue _messageQueueDisplaySms;
        private readonly CustomMessageQueue _messageQueueDisplayPhidget;
        private readonly CustomMessageQueue _messageQueueDisplayVideoCapture;

        public MaintenanceController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

            // config-sms message queue sender
            _messageQueueConfigSms = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms
            });

            // display-sms message queue sender
            _messageQueueDisplaySms = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplaySms
            });

            // display-phidget message queue sender
            _messageQueueDisplayPhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayPhidget
            });

            // display-phidget message queue sender
            _messageQueueDisplayVideoCapture = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayVideoCapture
            });
        }

        // GET: Services
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public string ReinstallServices()
        {
            var smsPath = ConfigurationManager.AppSettings["StateMachineServiceLocation"];
            var phidgetPath = ConfigurationManager.AppSettings["PhidgetServiceLocation"];
            var rfidPath = ConfigurationManager.AppSettings["RfidReaderServiceLocation"];
            var videoCapturePath = ConfigurationManager.AppSettings["VideoCaptureServiceLocation"];

            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            // uninstall / reinstall the services
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.ReinstallServices(smsPath, phidgetPath, rfidPath, videoCapturePath);

            if (msg.Length != 0) return msg;

            if (DisplayIsActive())
            {
                _messageQueueDisplaySms.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(true));
            }
            else
            {
                // send active config update
                var opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
                var activeConfigId = opsClient.GetActiveConfig().Id;
                var configRules = new ConfigRules { OperationsClient = opsClient };
                var message = configRules.GetMessageBody(activeConfigId);
                _messageQueueConfigSms.Send(message);
            }
            return msg;
        }

        [Authorize]
        public string RestartServices()
        {
            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.RestartServices();

            if (msg.Length != 0) return msg;

            if (DisplayIsActive())
            {
                _messageQueueDisplaySms.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(true));
            }
            else
            {
                // send active config update
                var opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
                var activeConfigId = opsClient.GetActiveConfig().Id;
                var configRules = new ConfigRules { OperationsClient = opsClient };
                var message = configRules.GetMessageBody(activeConfigId);
                _messageQueueConfigSms.Send(message);
            }

            return msg;
        }

        private static bool DisplayIsActive()
        {
            var processes = Process.GetProcessesByName("Keebee.AAT.Display");
            return (processes.Any());
        }

        private static string CreateDisplayMessageBody(bool isActive)
        {
            var displayMessage = new DisplayMessage
            {
                IsActive = isActive
            };

            var serializer = new JavaScriptSerializer();
            var displayMessageBody = serializer.Serialize(displayMessage);
            return displayMessageBody;
        }
    }
}