using System;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Shared;
using System.Collections.Generic;
using System.Linq;
using Keebee.AAT.Exporting;
using System.Web.Mvc;

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
        public FileResult DoExport(string date)
        {
            var exporter = new EventLogExporter();
            var dateTime = DateTime.Parse(date);
            var dateString = dateTime.ToString("yyyy-MM-dd");
            var filename = $"EventLog_{dateString.Replace("-", "_")}.xls";
            var file = exporter.Export(date);

            return File(file, "application/vnd.ms-excel", filename);
        }

        [HttpGet]
        [Authorize]
        public FileResult Download(string streamId)
        {
            var mediaFile = _opsClient.GetMediaFile(new Guid(streamId));
            var file = System.IO.File.ReadAllBytes($@"{mediaFile.Path}/{mediaFile.Filename}");

            //TODO: figure out why this doesn't return the same thing as above
            //var file = _opsClient.GetMediaFileStream(new Guid(streamId));

            return File(file, "application/vnd.ms-excel", mediaFile.Filename);
        }

        [HttpGet]
        [Authorize]
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
            var media = _opsClient.GetMediaFilesForPath(Exports.EventLogPath).Single();

            var list = media.Files
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