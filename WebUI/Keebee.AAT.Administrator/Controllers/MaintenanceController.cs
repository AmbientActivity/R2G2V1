﻿using System.Configuration;
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
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueConfigSms;
        private readonly CustomMessageQueue _messageQueueDisplaySms;
        private readonly CustomMessageQueue _messageQueueDisplayPhidget;
        private readonly CustomMessageQueue _messageQueueDisplayVideoCapture;
        private readonly CustomMessageQueue _messageQueuePhidget;

        public MaintenanceController()
        {
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };

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

            // phidget message queue sender
            _messageQueuePhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget
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
            var keepIISAlivePath = ConfigurationManager.AppSettings["KeepIISAliveServiceLocation"];

            // make them think the display is inactive (even if it isn't)
            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            // uninstall / reinstall the services
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.ReinstallServices(smsPath, phidgetPath, rfidPath, videoCapturePath, keepIISAlivePath);

            if (msg == null) return null;

            if (DisplayIsActive())
            {
                // alert them that the display is active (if it is)
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
        public string UninstallServices()
        {
            var smsPath = ConfigurationManager.AppSettings["StateMachineServiceLocation"];
            var phidgetPath = ConfigurationManager.AppSettings["PhidgetServiceLocation"];
            var rfidPath = ConfigurationManager.AppSettings["RfidReaderServiceLocation"];
            var videoCapturePath = ConfigurationManager.AppSettings["VideoCaptureServiceLocation"];
            var keepIISAlivePath = ConfigurationManager.AppSettings["KeepIISAliveServiceLocation"];

            // make them think the display is inactive (even if it isn't)
            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            // uninstall the services
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.UninstallServices(smsPath, phidgetPath, rfidPath, videoCapturePath, keepIISAlivePath);

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

            if (msg == null) return null;

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
        public string KillDisplay()
        {
            var rules = new MaintenanceRules
            {
                OperationsClient = _opsClient,
                MessageQueuePhidget = _messageQueuePhidget,
                EventLogger = _systemEventLogger
            };

            return rules.KillDisplay();
        }

        [Authorize]
        public string ClearServiceLogs()
        {
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            return rules.ClearServiceLogs();
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