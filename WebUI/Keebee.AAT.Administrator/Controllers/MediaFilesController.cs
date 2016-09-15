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
                Media = GetMedia(path)
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<MediaFileViewModel> GetMedia(string path)
        {
            var mediaList = _opsClient.GetMediaFilesForPath(path);

            var media = mediaList.First();

            var list = media.Files
                .Select(mediaFile => new MediaFileViewModel
                {
                    StreamId = mediaFile.StreamId,
                    IsFolder = mediaFile.IsFolder,
                    Filename = mediaFile.Filename,
                    FileType = mediaFile.FileType,
                    FileSize = mediaFile.FileSize,
                    Path = media.Path
                }).OrderBy(x => x.Filename);

            return list;
        }
    }
}