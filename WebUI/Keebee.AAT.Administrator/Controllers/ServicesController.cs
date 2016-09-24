using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
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
        public ActionResult Index()
        {
            return View();
        }

        public void RestartServices()
        {
            var rules = new ServicesRules{ EventLogger = _systemEventLogger };
            rules.RestartService("StateMachineService", 10000);
        }
    }
}