using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.Exporting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class EventLogsController : Controller
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
            var mediaFilesCLient = new MediaFilesClient();
            var mediaFile = mediaFilesCLient.Get(new Guid(streamId));
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

        private static IEnumerable<EventLogViewModel> GetEventLogList()
        {
            var mediaFilesClient = new MediaFilesClient();
            var paths = mediaFilesClient.GetForPath(Exports.EventLogPath).ToArray();

            if (!paths.Any()) return new Collection<EventLogViewModel>();

            var media = paths.Single();

            var list = media.Files
                .Select(mediaFile => new EventLogViewModel
                {
                    StreamId = mediaFile.StreamId,
                    Filename = mediaFile.Filename,
                    FileType = mediaFile.FileType,
                    FileSize = mediaFile.FileSize,
                    Path = Exports.EventLogPath
                }).OrderByDescending(x => x.Filename);

            return list;
        }
    }
}