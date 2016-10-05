using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.Administrator.Extensions;
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
    public class PublicMediaController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public PublicMediaController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
        }

        // GET: PublicMediaFile
        [Authorize]
        public ActionResult Index(
            int? mediaPathTypeId,
            int? responseTypeId,
            string myuploader)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Images;
            if (responseTypeId == null) responseTypeId = ResponseTypeId.SlidShow;

            var vm = LoadPublicMediaViewModel(mediaPathTypeId, responseTypeId);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = PublicMediaRules.GetAllowedExtensions(mediaPathTypeId);
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

                    if (!PublicMediaRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var rules = new PublicMediaRules { OperationsClient = _opsClient };
                    var mediaPathType = rules.GetMediaPathType(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}\{mediaPathType}\{file.FileName}";

                    if (rules.IsRemovable(filePath, (int)mediaPathTypeId, (int)responseTypeId))
                    {
                        fileManager.DeleteFile(filePath);
                    }

                    if (!rules.FileExists($@"{PublicMediaSource.Id}\{mediaPathType}", file.FileName))
                    {
                        file.MoveTo(filePath);
                    }

                    AddPublicMediaFile(file.FileName, (int)responseTypeId, (int)mediaPathTypeId, mediaPathType);
                }
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int mediaPathTypeId)
        {
            var rules = new PublicMediaRules { OperationsClient =  _opsClient };
            var mediaPathTypes = _opsClient.GetMediaPathTypes()
                .Where(x => x.Id != MediaPathTypeId.Pictures)
                .OrderBy(p => p.Description);
            var responseTypes = rules.GetValidResponseTypes(mediaPathTypeId)
                .OrderBy(r => r.Description);
            var fileList = GetMediaFiles();

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    Description = x.Description.ToUppercaseFirst()
                }),
                ResponseTypeList = responseTypes.Select(x => new
                {
                    x.Id,
                    x.Description
                })
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
                uploader.AllowedFileExtensions = ResidentRules.GetAllowedExtensions(mediaPathTypeId); ;
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                html = uploader.Render();
            }

            var rules = new PublicMediaRules { OperationsClient = _opsClient };
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
                AddButtonText = $"Upload {rules.GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int mediaPathTypeId, int responseTypeId)
        {
            bool success;
            string errormessage;

            try
            {
                var rules = new PublicMediaRules {OperationsClient = _opsClient};

                errormessage = rules.CanDeleteMultiple(ids.Length, mediaPathTypeId, responseTypeId);
                if (errormessage.Length > 0)
                    throw new Exception(errormessage);

                foreach (var id in ids)
                {
                    var file = rules.GetMediaFile(id);
                    if (file == null) continue;

                    // if the file is used in multiple response types
                    if (rules.IsMultipleReponseTypes(id))
                    {
                        // delete the link only
                        errormessage = rules.DeletePublicMediaFile(id);
                        if (errormessage.Length > 0)
                            throw new Exception(errormessage);
                    }
                    else
                    {
                        // otherwise delete the file (link will get deleted automatically)
                        var fileManager = new FileManager {EventLogger = _systemEventLogger};
                        fileManager.DeleteFile($@"{file.Path}\{file.Filename}");
                    }
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
                        FileList = GetMediaFiles()
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

        private void AddPublicMediaFile(string filename, int responseTypeId, int mediaPathTypeId, string mediaPathType)
        {
            var fileManager = new FileManager { EventLogger = _systemEventLogger };
            var streamId = fileManager.GetStreamId($@"{PublicMediaSource.Id}\{mediaPathType}", filename);

            var mf = new PublicMediaFileEdit
            {
                StreamId = streamId,
                ResponseTypeId = responseTypeId,
                MediaPathTypeId = mediaPathTypeId
            };

            _opsClient.PostPublicMediaFile(mf);
        }

        private PublicMediaViewModel LoadPublicMediaViewModel(
                int? mediaPathTypeId,
                int? responseTypeId)
        {
            var rules = new PublicMediaRules {OperationsClient = _opsClient};
            var vm = new PublicMediaViewModel
            {
                Title = PublicMediaSource.Description,
                AddButtonText = $"Upload {rules.GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Images,
                SelectedResponseType = responseTypeId ?? ResponseTypeId.SlidShow
            };

            return vm;
        }

        private IEnumerable<PublicMediaFileViewModel> GetMediaFiles()
        {
            var list = new List<PublicMediaFileViewModel>();
            var publicMedia = _opsClient.GetPublicMediaFiles();

            if (publicMedia == null) return list;

            var mediaPaths = publicMedia.MediaFiles.SelectMany(x => x.Paths)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}";

            foreach (var media in publicMedia.MediaFiles)
            {
                foreach (var path in media.Paths)
                {
                    var files = path.Files.OrderBy(f => f.Filename);

                    foreach (var file in files)
                    {
                        var vm = new PublicMediaFileViewModel
                        {
                            Id = file.Id,
                            StreamId = file.StreamId,
                            Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                            FileType = file.FileType.ToUpper(),
                            FileSize = file.FileSize,
                            Path = $@"{pathRoot}\{path.MediaPathType.Description}",
                            MediaPathTypeId = path.MediaPathType.Id,
                            ResponseTypeId = media.ResponseType.Id
                        };

                        list.Add(vm);
                    }
                }
            }

            return list;
        }

    }
}