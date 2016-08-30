using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class EventLogController : Controller
    {
        // GET: EventLog
        public ActionResult Export()
        {
            return View();
        }

        public void DoExport(string date)
        {
            var exporter = new ActivityLog.Exporter();

            exporter.Export(date);
        }
    }
}