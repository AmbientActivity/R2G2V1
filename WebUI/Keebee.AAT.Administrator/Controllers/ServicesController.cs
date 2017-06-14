using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules.Shared;
using System.Configuration;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ServicesController : Controller
    {
        private readonly SystemEventLogger _systemEventLogger;
        private readonly ServicesRules _rules;

        public ServicesController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
            _rules = new ServicesRules { EventLogger = _systemEventLogger };
        }

        // GET: Services
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return View(new ServicesViewModel
            {
                IsInstalledBeaconWatcherService = ServicesRules.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher) ? 1 : 0,
                IsInstalledVideoCaptureService = ServicesRules.IsInstalled(ServiceUtilities.ServiceType.VideoCapture) ? 1 : 0
            });
        }

        [Authorize]
        [HttpGet]
        public JsonResult SaveSettings(bool activateBeaconWatcher, bool activateVideoCapture)
        {
            var beaconWatcherPath = ConfigurationManager.AppSettings["BluetoothBeaconWatcherServiceLocation"];
            var videoCapturePath = ConfigurationManager.AppSettings["VideoCaptureServiceLocation"];

            var msg =  _rules.Install(ServiceUtilities.ServiceType.BluetoothBeaconWatcher, beaconWatcherPath, activateBeaconWatcher) ??
                       _rules.Install(ServiceUtilities.ServiceType.VideoCapture, videoCapturePath, activateVideoCapture);

            return Json(new
            {
                ErrorMessage = msg,
                ServiceSettings = new ServicesViewModel
                {
                    IsInstalledBeaconWatcherService = ServicesRules.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher) ? 1 : 0,
                    IsInstalledVideoCaptureService = ServicesRules.IsInstalled(ServiceUtilities.ServiceType.VideoCapture) ? 1 : 0
                }

            }, JsonRequestBehavior.AllowGet);
        }
    }
}