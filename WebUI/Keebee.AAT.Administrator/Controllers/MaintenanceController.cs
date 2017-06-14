using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Configuration;
using Keebee.AAT.BusinessRules.Shared;

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
        private readonly CustomMessageQueue _messageQueueDisplayBluetoothBeaconWatcher;
        private readonly CustomMessageQueue _messageQueueResponse;

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

            // display-video-capture message queue sender
            _messageQueueDisplayVideoCapture = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayVideoCapture
            });

            // display-bluetooth-beacon-watcher message queue sender
            _messageQueueDisplayBluetoothBeaconWatcher = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayBluetoothBeaconWatcher
            });

            // response message queue sender
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response
            });
        }

        // GET: Services
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public JsonResult ReinstallServices()
        {
            var smsPath = ConfigurationManager.AppSettings["StateMachineServiceLocation"];
            var phidgetPath = ConfigurationManager.AppSettings["PhidgetServiceLocation"];
            var bluetoothBeaconWatcherPath = ConfigurationManager.AppSettings["BluetoothBeaconWatcherServiceLocation"];
            var videoCapturePath = ConfigurationManager.AppSettings["VideoCaptureServiceLocation"];
            var keepIISAlivePath = ConfigurationManager.AppSettings["KeepIISAliveServiceLocation"];

            // make them think the display is inactive (even if it isn't)
            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            // uninstall / reinstall the services
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.ReinstallServices(smsPath, phidgetPath, bluetoothBeaconWatcherPath, videoCapturePath, keepIISAlivePath);

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
                var configsClient = new ConfigsClient();
                var activeConfigId = configsClient.GetActive().Id;
                var configRules = new PhidgetConfigRules();
                var message = configRules.GetMessageBody(activeConfigId);
                _messageQueueConfigSms.Send(message);
            }

            return Json(new
            {
                ErrorMessage = msg
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult UninstallServices()
        {
            var smsPath = ConfigurationManager.AppSettings["StateMachineServiceLocation"];
            var phidgetPath = ConfigurationManager.AppSettings["PhidgetServiceLocation"];
            var bluetoothBeaconWatcherPath = ConfigurationManager.AppSettings["BluetoothBeaconWatcherServiceLocation"];
            var videoCapturePath = ConfigurationManager.AppSettings["VideoCaptureServiceLocation"];
            var keepIISAlivePath = ConfigurationManager.AppSettings["KeepIISAliveServiceLocation"];

            // make them think the display is inactive (even if it isn't)
            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            // uninstall the services
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.UninstallServices(smsPath, phidgetPath, bluetoothBeaconWatcherPath, videoCapturePath, keepIISAlivePath);

            return Json(new
            {
                ErrorMessage = msg
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult RestartServices()
        {
            var isInstalledBeaconWatcher = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher);
            var isInstalledVideoCapture = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.VideoCapture);

            // gather the current config info (will need to send it to the SMS after it is restarted)
            var configsClient = new ConfigsClient();
            var activeConfigId = configsClient.GetActive().Id;
            var configRules = new PhidgetConfigRules();
            var configMessage = configRules.GetMessageBody(activeConfigId);

            // temporarily inform the services that the display is currently not active
            _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
            _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));

            if (isInstalledVideoCapture)
                _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));

            if (isInstalledBeaconWatcher)
                _messageQueueDisplayBluetoothBeaconWatcher.Send(CreateDisplayMessageBody(false));

            // restart the services
            var rules = new MaintenanceRules { EventLogger = _systemEventLogger };
            var msg = rules.RestartServices();

            if (msg == null)
            {

                // inform the services if the display is currently active
                if (DisplayIsActive())
                {
                    _messageQueueDisplaySms.Send(CreateDisplayMessageBody(true));
                    _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(true));

                    if (isInstalledVideoCapture)
                        _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(true));

                    if (isInstalledBeaconWatcher)
                        _messageQueueDisplayBluetoothBeaconWatcher.Send(CreateDisplayMessageBody(true));
                }
                else
                {
                    // inform the state machine to reload its current config
                    _messageQueueConfigSms.Send(configMessage);
                }
            }

            return Json(new
            {
                ErrorMessage = msg
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public string KillDisplay()
        {
            var rules = new MaintenanceRules
            {
                MessageQueueResponse = _messageQueueResponse,
                EventLogger = _systemEventLogger
            };

            return rules.KillDisplay();
        }

        [Authorize]
        [HttpGet]
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