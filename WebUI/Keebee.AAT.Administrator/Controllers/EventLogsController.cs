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
        public FileResult Download(Guid streamId)
        {
            var mediaFilesCLient = new MediaFilesClient();
            var mediaFile = mediaFilesCLient.Get(streamId);
            var file = System.IO.File.ReadAllBytes($@"{mediaFile.Path}/{mediaFile.Filename}");

            return File(file, "application/vnd.ms-excel", mediaFile.Filename);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            string errMsg = null;
            var eventLogList = new EventLogViewModel[0];

            try
            {
                eventLogList = GetEventLogList().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                EventLogList = eventLogList
            }, JsonRequestBehavior.AllowGet);
        }

        private static IEnumerable<EventLogViewModel> GetEventLogList()
        {
            var mediaFilesClient = new MediaFilesClient();
            var paths = mediaFilesClient.GetForPath(Exports.EventLogPath).ToArray();

            if (!paths.Any()) return new Collection<EventLogViewModel>();

            var media = paths.Single();

            var list = media.Files
                .Select(file =>
                {
                    var filename = file.Filename.Replace($".{file.FileType}", string.Empty);
                    //TODO: might be good to parse the date from the filename and display it in a separate column
                    return new EventLogViewModel
                    {
                        StreamId = file.StreamId,
                        Filename = filename,
                        FileType = file.FileType.ToUpper(),
                        FileSize = file.FileSize,
                        Path = Exports.EventLogPath
                    };
                }).OrderByDescending(x => x.Filename);

            return list;
        }
    }
}