using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class VideoCapturesController : Controller
    {
        [HttpGet]
        [Authorize]
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
            var rules = new VideoCaptureRules();
            var filename = $"VideoCaptures_{date.Replace("/", "_")}.zip";
            var file = rules.GetZipFile(date);

            return File(file, "application/zip", filename);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            string errMsg = null;
            VideoCaptureViewModel[] videoCaptureList = null;

            try
            {
                videoCaptureList = GetVideoCaptureList().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                VideoCaptureList = videoCaptureList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public FileResult Download(string path, string filename)
        {
            var file = System.IO.File.ReadAllBytes($@"{path}\{filename}");

            return File(file, "video/mp4", filename);
        }

        private static IEnumerable<VideoCaptureViewModel> GetVideoCaptureList()
        {
            var root = new DirectoryInfo(VideoCaptures.Path);

            if (!root.Exists) return new VideoCaptureViewModel[0];

            var folders = root.EnumerateDirectories().OrderBy(x => x.Name);

            var list = folders.SelectMany(x => x.EnumerateFiles()
                .Where(f => f.Length > 0))
                .Select(video => new VideoCaptureViewModel
                {
                    Filename = video.Name,
                    Path = video.DirectoryName,
                    FileSize = video.Length
                }).OrderByDescending(x => x.Filename);

            return list;
        }
    }
}