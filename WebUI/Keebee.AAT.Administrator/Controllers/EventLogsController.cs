using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class EventLogsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Export()
        {
            return View();
        }

        [HttpGet]
        public FileResult DoExport(string date)
        {
            var exporter = new EventLogging.Exporter();
            var filename = $"EventLog_{date.Replace("/", "_")}.xls";
            var file = exporter.Export(date);

            return File(file, "application/vnd.ms-excel", filename);
        }
    }
}