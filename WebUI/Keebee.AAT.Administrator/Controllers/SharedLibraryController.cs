using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using CuteWebUI;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SharedLibraryController : Controller
    {
        // api client
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public SharedLibraryController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

            _mediaFilesClient = new MediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
        }

        [Authorize]
        public ActionResult Index(
            int? mediaPathTypeId,
            string myuploader)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Music;

            var vm = LoadSharedLibraryViewModel(mediaPathTypeId);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = SharedLibraryRules.GetAllowedExtensions(mediaPathTypeId);
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                vm.UploaderHtml = uploader.Render();

                // GET:
                if (string.IsNullOrEmpty(myuploader))
                    return View(vm);

                // POST:
                var fileManager = new FileManager {EventLogger = _systemEventLogger};

                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!SharedLibraryRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var rules = new SharedLibraryRules();
                    var mediaPath = rules.GetMediaPathType(mediaPathTypeId);
                    var path = $@"{_mediaSourcePath.MediaRoot}\{_mediaSourcePath.SharedLibrary}\{mediaPath.Path}\";
                    var filePath = $@"{path}\{file.FileName}";

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
            var rules = new SharedLibraryRules();
            var mediaPathTypes = _mediaPathTypesClient.Get().Where(mp => mp.IsSharable).ToArray();

            var vm = new
            {
                FileList = GetFileList(mediaPathTypes),
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
                uploader.AllowedFileExtensions = SharedLibraryRules.GetAllowedExtensions(mediaPathTypeId);
                ;
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                html = uploader.Render();
            }

            var rules = new SharedLibraryRules();
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
            var rules = new SharedLibraryRules();

            var mediaPathTypes = _mediaPathTypesClient.Get()
                .Where(mp => mp.Id != MediaPathTypeId.ImagesPersonal && mp.Id != MediaPathTypeId.HomeMovies).ToArray();
            try
            {
                foreach (var streamId in streamIds)
                {
                    var file = _mediaFilesClient.Get(streamId);
                    if (file == null) continue;

                    // delete all the linkage if it's sharable media (non-system)
                    if (isSharable)
                    {
                        errormessage = rules.DeleteSharedMediaFileLinks(streamId);
                        if (errormessage.Length > 0)
                            throw new Exception(errormessage);
                    }

                    var fileManager = new FileManager {EventLogger = _systemEventLogger};
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
                FileList = GetFileList(mediaPathTypes)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetLinkedResidentsView(Guid streamId)
        {
            return PartialView("_LinkedResidentMedia", LoadLinkedProfilesViewModel(streamId));
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetImageViewerView(Guid streamId, string fileType)
        {
            var rules = new ImageViewerRules();
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

        private static SharedLibraryViewModel LoadSharedLibraryViewModel(int? mediaPathTypeId)
        {
            var rules = new SharedLibraryRules();
            var vm = new SharedLibraryViewModel
            {
                Title = "Shared Library",
                AddButtonText = $"Upload {rules.GetMediaPathShortDescription(mediaPathTypeId)}",
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Music
            };

            return vm;
        }

        private static LinkedProfilesViewModel LoadLinkedProfilesViewModel(Guid streamId)
        {
            var rules = new SharedLibraryRules();
            var linkedProfiles = rules.GetLinkedProfiles(streamId);

            var vm = new LinkedProfilesViewModel
            {
                Profiles = linkedProfiles.Select(r => new
                    ResidentViewModel
                    {
                        FirstName = r.FirstName,
                        LastName = r.LastName
                    }),
                NoAvailableMediaMessage = "No profiles are linked to this file."
            };

            return vm;
        }

        private IEnumerable<object> GetFileList(IEnumerable<MediaPathType> mediaPathTypes)
        {
            var sharedMedia = _mediaFilesClient.GetWithLinkedData(_mediaSourcePath.SharedLibrary);

            return sharedMedia.SelectMany(p =>
            {
                var mediaPathType = SharedLibraryRules.GetMediaPathTypeFromRawPath(p.Path, mediaPathTypes);

                return p.Files.Select(f =>
                    new
                    {
                        f.StreamId,
                        f.Filename,
                        f.FileSize,
                        f.FileType,
                        mediaPathType.Path,
                        MediaPathTypeId = mediaPathType.Id,
                        f.NumLinkedProfiles
                    });
            });
        }
    }
}