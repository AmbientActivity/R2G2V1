using System;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Keebee.AAT.Exporting;
using System.Web.Mvc;
using Keebee.AAT.Shared;

namespace Keebee.AAT.Administrator.Controllers
{
    public class EventLogsController : Controller
    {
        private readonly OperationsClient _opsClient;

        public EventLogsController()
        {
            _opsClient = new OperationsClient();
        }

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
            var exporter = new EventLogExporter();
            var filename = $"EventLog_{date.Replace("/", "_")}.xls";
            var file = exporter.Export(date);

            return File(file, "application/vnd.ms-excel", filename);
        }

        [HttpGet]
        public FileResult Download(string streamId)
        {
            var mediaFileStream = _opsClient.GetMediaFileStream(new Guid(streamId));
            var filename = mediaFileStream.Filename;
            var file = Encoding.ASCII.GetBytes(mediaFileStream.Stream);

            return File(file, "application/vnd.ms-excel", filename);
        }

        [HttpGet]
        public JsonResult GetData()
        {
            var vm = new
            {
                EventLogList = GetEventLogList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<EventLogViewModel> GetEventLogList()
        {
            var mediaFiles = _opsClient.GetMediaFilesForPath(Exports.EventLogPath);

            var list = mediaFiles
                .Select(mediaFile => new EventLogViewModel
                {
                    StreamId = mediaFile.StreamId,
                    Filename = mediaFile.Filename,
                    FileType = mediaFile.FileType,
                    FileSize = mediaFile.FileSize,
                    Path = Exports.EventLogPath
                }).OrderBy(x => x.Filename);

            return list;
        }
    }
}