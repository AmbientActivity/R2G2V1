using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class MediaFilesController : Controller
    {
        private readonly OperationsClient _opsClient;

        public MediaFilesController()
        {
            _opsClient = new OperationsClient();
        }

        // GET: MediaFile
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData(string path)
        {
            var vm = new
            {
                MediaFileList = GetMediaFileList(path)
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<MediaFileViewModel> GetMediaFileList(string path)
        {
            var mediaFiles = _opsClient.GetMediaFilesForPath(path);

            var list = mediaFiles
                .Select(mediaFile => new MediaFileViewModel
                {
                    StreamId = mediaFile.StreamId,
                    IsFolder = mediaFile.IsFolder,
                    Filename = mediaFile.Filename,
                    FileType = mediaFile.FileType,
                    FileSize = mediaFile.FileSize,
                    Path = mediaFile.Path
                }).OrderBy(x => x.Filename);

            return list;
        }
    }
}