using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ActivityLogController : Controller
    {
        [HttpGet]
        public ActionResult Export()
        {
            return View();
        }

        [HttpGet]
        public FileResult DoExport(string date)
        {
            var exporter = new ActivityLog.Exporter();

            var filename = $"ActivityLog_{date.Replace("/", "_")}.xls";
            var file = exporter.Export(date);

            return File(file, "application/vnd.ms-excel", filename);
        }
    }
}