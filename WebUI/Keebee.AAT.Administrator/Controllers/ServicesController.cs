using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using System.Configuration;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ServicesController : Controller
    {
        private readonly SystemEventLogger _systemEventLogger;

        public ServicesController()
        {
           _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
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
            var rules = new ServicesRules { EventLogger = _systemEventLogger };
            var beaconWatcherPath = ConfigurationManager.AppSettings["BluetoothBeaconWatcherServiceLocation"];
            var videoCapturePath = ConfigurationManager.AppSettings["VideoCaptureServiceLocation"];

            var errMsg =  rules.Install(ServiceUtilities.ServiceType.BluetoothBeaconWatcher, beaconWatcherPath, activateBeaconWatcher) ??
                       rules.Install(ServiceUtilities.ServiceType.VideoCapture, videoCapturePath, activateVideoCapture);

            if (string.IsNullOrEmpty(errMsg))
            {
                if (Request.IsAuthenticated)
                    Session["IsBeaconWatcherServiceInstalled"] = activateBeaconWatcher ? "true" : "false";
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                ServiceSettings = new ServicesViewModel
                {
                    IsInstalledBeaconWatcherService = ServicesRules.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher) ? 1 : 0,
                    IsInstalledVideoCaptureService = ServicesRules.IsInstalled(ServiceUtilities.ServiceType.VideoCapture) ? 1 : 0
                }

            }, JsonRequestBehavior.AllowGet);
        }
    }
}