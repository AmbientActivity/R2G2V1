using System.IO;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ImageViewerController : Controller
    {
        [HttpGet]
        public FileResult GetFileStream(string filePath, string fileType)
        {
            var info = new FileInfo(filePath);
            return File(info.OpenRead(), $"image/{info}");
        }
    }
}