using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Keebee.AAT.SystemEventLogging;

namespace Keebee.AAT.Administrator.Controllers
{
    public class VideoCapturesController : Controller
    {
        private readonly SystemEventLogger _systemEventLogger;

        public VideoCapturesController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
        }

        // GET: VideoCaptures
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Export()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult DoExport(string date)
        {
            var rules = new VideoCaptureRules { EventLogger = _systemEventLogger };
            var filename = $"VideoCaptures_{date.Replace("/", "_")}.zip";
            var file = rules.GetZipFile(date);

            return File(file, "application/zip", filename);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            var vm = new
            {
                VideoCaptureList = GetVideoCaptureList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public FileResult Download(string filename)
        {
            var file = System.IO.File.ReadAllBytes($@"{VideoCaptures.Path}/{filename}");

            return File(file, "video/x-ms-wmv", filename);
        }

        private static IEnumerable<VideoCaptureViewModel> GetVideoCaptureList()
        {
            var root = new DirectoryInfo(VideoCaptures.Path);
            var folders = root.EnumerateDirectories().OrderBy(x => x.Name);

            var list = folders.SelectMany(x => x.EnumerateFiles())
                .Select(video => new VideoCaptureViewModel
                {
                    Filename = video.Name,
                    Path = video.DirectoryName,
                    FileSize = video.Length
                }).OrderBy(x => x.Filename);

            return list;
        }
    }
}