using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using CuteWebUI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.IO;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SystemLibrariesController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public SystemLibrariesController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
        }

        // GET: SystemLibraries
        [Authorize]
        public ActionResult Index(
            int? mediaPathTypeId,
            string myuploader)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Music;

            var vm = LoadSystemLibrariesViewModel(mediaPathTypeId);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = SystemLibrariesRules.GetAllowedExtensions(mediaPathTypeId);
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                vm.UploaderHtml = uploader.Render();

                // GET:
                if (string.IsNullOrEmpty(myuploader))
                    return View(vm);

                // POST:
                var fileManager = new FileManager { EventLogger = _systemEventLogger };

                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!SystemLibrariesRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var rules = new SystemLibrariesRules { OperationsClient = _opsClient };
                    var mediaPath = rules.GetMediaPath(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.SharedLibrary}\{mediaPath}\{file.FileName}";

                    // delete it if it already exists
                    var msg = fileManager.DeleteFile(filePath);

                    if (msg.Length == 0)
                        file.MoveTo(filePath);
                }
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int mediaPathTypeId)
        {
            var rules = new SystemLibrariesRules { OperationsClient = _opsClient };
            var mediaPathTypes = _opsClient.GetMediaPathTypes()
                    .Where(mp => mp.Id != MediaPathTypeId.PersonalImages && mp.Id != MediaPathTypeId.HomeMovies).ToArray();

            var vm = new
            {
                FileList = rules.GetFileList(mediaPathTypes),
                MediaPathTypeList = rules.GetMediaPathTypeList(mediaPathTypes)
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetUploaderHtml(int mediaPathTypeId)
        {
            string html;
            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = SystemLibrariesRules.GetAllowedExtensions(mediaPathTypeId); ;
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                html = uploader.Render();
            }

            var rules = new SystemLibrariesRules { OperationsClient = _opsClient };
            var responseTypes = rules.GetValidResponseTypes(mediaPathTypeId)
                .OrderBy(r => r.Description);
            ;
            return Json(new
            {
                UploaderHtml = html,
                ResponseTypeList = responseTypes.Select(x => new
                {
                    x.Id,
                    x.Description
                }),
                AddButtonText = $"Upload {rules.GetMediaPathShortDescription(mediaPathTypeId)}",
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(Guid[] streamIds, int mediaPathTypeId, bool isSharable)
        {
            bool success;
            var errormessage = string.Empty;
            var rules = new SystemLibrariesRules { OperationsClient = _opsClient };
            var mediaPathTypes = _opsClient.GetMediaPathTypes()
                    .Where(mp => mp.Id != MediaPathTypeId.PersonalImages && mp.Id != MediaPathTypeId.HomeMovies).ToArray();

            try
            {
                foreach (var streamId in streamIds)
                {
                    var file = _opsClient.GetMediaFile(streamId);
                    if (file == null) continue;

                    // delete all the linkage if it's sharable media (non-system)
                    if (isSharable)
                    {
                        errormessage = rules.DeleteSharedMediaFileLinks(streamId);
                        if (errormessage.Length > 0)
                            throw new Exception(errormessage);
                    }

                    var fileManager = new FileManager { EventLogger = _systemEventLogger };
                    fileManager.DeleteFile($@"{file.Path}\{file.Filename}");
                }

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                errormessage = ex.Message;
            }

            return Json(new
            {
                Success = success,
                ErrorMessage = errormessage,
                FileList = rules.GetFileList(mediaPathTypes)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetImageViewerView(Guid streamId, string fileType)
        {
            var rules = new ImageViewerRules { OperationsClient = _opsClient };
            var m = rules.GetImageViewerModel(streamId, fileType);

            return PartialView("_ImageViewer", new ImageViewerViewModel
            {
                FilePath = m.FilePath,
                FileType = m.FileType,
                Width = m.Width,
                Height = m.Height,
                PaddingLeft = m.PaddingLeft
            });
        }

        [HttpGet]
        [Authorize]
        public FileResult GetFileStream(string filePath, string fileType)
        {
            var info = new FileInfo(filePath);
            return File(info.OpenRead(), $"image/{info}");
        }

        private SystemLibrariesViewModel LoadSystemLibrariesViewModel(int? mediaPathTypeId)
        {
            var rules = new SystemLibrariesRules { OperationsClient = _opsClient };
            var vm = new SystemLibrariesViewModel
            {
                Title = "Shared/System Libraries",
                AddButtonText = $"Upload {rules.GetMediaPathShortDescription(mediaPathTypeId)}",
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Music
            };

            return vm;
        }
    }
}