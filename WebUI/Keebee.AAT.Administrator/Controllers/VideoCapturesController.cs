using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class VideoCapturesController : Controller
    {
        private readonly OperationsClient _opsClient;

        public VideoCapturesController()
        {
            _opsClient = new OperationsClient();
        }

        // GET: VideoCaptures
        public ActionResult Index()
        {
            return View();
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

        private IEnumerable<VideoCaptureViewModel> GetVideoCaptureList()
        {
            var dir = new DirectoryInfo(VideoCaptures.Path);
            var videos = dir.GetFiles();

            var list = videos
                .Select(video => new VideoCaptureViewModel
                {
                    Filename = video.Name,
                    Path = VideoCaptures.Path,
                    FileSize = video.Length
                }).OrderBy(x => x.Filename);

            return list;
        }
    }
}