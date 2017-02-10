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
    public class PublicProfileController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public PublicProfileController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
        }

        // GET: PublicProfile
        [Authorize]
        public ActionResult Index(
            int? mediaPathTypeId,
            string myuploader)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Music;

            var vm = LoadPublicProfileViewModel(mediaPathTypeId);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = PublicProfileRules.GetAllowedExtensions(mediaPathTypeId);
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

                    if (!PublicProfileRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var rules = new PublicProfileRules { OperationsClient = _opsClient };
                    var mediaPath = rules.GetMediaPath(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}\{mediaPath}\{file.FileName}";

                    // delete it if it already exists
                    var msg = fileManager.DeleteFile(filePath);

                    if (msg.Length == 0)
                    {
                        file.MoveTo(filePath);
                        AddPublicMediaFile(file.FileName, (int)mediaPathTypeId, mediaPath);
                    }       
                }
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int mediaPathTypeId)
        {
            var mediaPathTypes = _opsClient.GetMediaPathTypes()
                .Where(x => x.Id != MediaPathTypeId.PersonalImages && x.Id != MediaPathTypeId.HomeMovies)
                .OrderBy(p => p.Description);
            var fileList = GetMediaFiles();

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.ShortDescription,
                    IsPreviewable = PublicProfileRules.IsMediaTypePreviewable(x.Id)
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
                uploader.AllowedFileExtensions = PublicProfileRules.GetAllowedExtensions(mediaPathTypeId); ;
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                html = uploader.Render();
            }

            var rules = new PublicProfileRules { OperationsClient = _opsClient };
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
        public JsonResult DeleteSelected(int[] ids, int mediaPathTypeId)
        {
            bool success;
            string errormessage;

            try
            {
                var rules = new PublicProfileRules { OperationsClient = _opsClient };

                errormessage = rules.CanDeleteMultiple(ids.Length, mediaPathTypeId);
                if (errormessage.Length > 0)
                    throw new Exception(errormessage);

                foreach (var id in ids)
                {
                    var publicMediaFile = _opsClient.GetPublicMediaFile(id);
                    if (publicMediaFile == null) continue;

                    if (publicMediaFile.MediaFile.IsShared)
                    {
                        rules.DeletePublicMediaFile(id);
                    }
                    else
                    {
                        var file = rules.GetMediaFile(id);
                        if (file == null) continue;

                        // delete the link
                        errormessage = rules.DeletePublicMediaFile(id);
                        if (errormessage.Length > 0)
                            throw new Exception(errormessage);

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
        public PartialViewResult GetSharedLibarayAddView(int mediaPathTypeId)
        {
            return PartialView("_SharedLibraryAdd", LoadSharedLibaryAddViewModel(mediaPathTypeId));
        }

        [HttpGet]
        [Authorize]
        public FileResult GetFileStream(string filePath, string fileType)
        {
            var info = new FileInfo(filePath);
            return File(info.OpenRead(), $"image/{info}");
        }

        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int mediaPathTypeId)
        {
            bool success;
            var errormessage = string.Empty;

            try
            {
                foreach (var streamId in streamIds)
                {
                    var pmf = new PublicMediaFileEdit
                    {
                        StreamId = streamId,
                        ResponseTypeId = PublicProfileRules.GetResponseTypeId(mediaPathTypeId),
                        MediaPathTypeId = mediaPathTypeId,
                        IsShared = true
                    };

                    _opsClient.PostPublicMediaFile(pmf);
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

        private void AddPublicMediaFile(string filename, int mediaPathTypeId, string mediaPathType)
        {
            var fileManager = new FileManager { EventLogger = _systemEventLogger };
            var streamId = fileManager.GetStreamId($@"{PublicMediaSource.Id}\{mediaPathType}", filename);

            var mf = new PublicMediaFileEdit
            {
                StreamId = streamId,
                ResponseTypeId = PublicProfileRules.GetResponseTypeId(mediaPathTypeId),
                MediaPathTypeId = mediaPathTypeId,
                IsShared = false
            };

            _opsClient.PostPublicMediaFile(mf);
        }

        private PublicProfileViewModel LoadPublicProfileViewModel(
                int? mediaPathTypeId)
        {
            var rules = new PublicProfileRules { OperationsClient = _opsClient };
            var vm = new PublicProfileViewModel
            {
                Title = PublicMediaSource.Description,
                AddButtonText = $"Upload {rules.GetMediaPathShortDescription(mediaPathTypeId)}",
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Music
            };

            return vm;
        }

        private SharedLibraryAddViewModel LoadSharedLibaryAddViewModel(int mediaPathTypeId)
        {
            var rules = new PublicProfileRules { OperationsClient = _opsClient };
            var files = rules.GetAvailableSharedMediaFiles(mediaPathTypeId);

            var vm = new SharedLibraryAddViewModel
            {
                SharedFiles = files
                .Select(f => new SharedLibraryFileViewModel
                {
                    StreamId = f.StreamId,
                    Filename = f.Filename
                }),
                NoAvailableMediaMessage = rules.GetNoAvailableSharedMediaMessage(mediaPathTypeId)
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
                            IsShared = file.IsShared,
                            Path = $@"{pathRoot}\{path.MediaPathType.Path}",
                            MediaPathTypeId = path.MediaPathType.Id
                        };

                        list.Add(vm);
                    }
                }
            }

            return list;
        }

    }
}