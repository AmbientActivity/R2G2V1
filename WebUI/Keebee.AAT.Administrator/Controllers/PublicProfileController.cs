using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.ThumbnailGeneration;
using CuteWebUI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PublicProfileController : Controller
    {
        // api client
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public PublicProfileController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
            _thumbnailsClient = new ThumbnailsClient();
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

                    var rules = new PublicProfileRules();
                    var mediaPath = rules.GetMediaPath(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{PublicProfileSource.Id}\{mediaPath}\{file.FileName}";

                    // delete it if it already exists
                    var msg = fileManager.DeleteFile(filePath);

                    if (msg.Length == 0)
                    {
                        file.MoveTo(filePath);
                        msg = AddPublicMediaFile(file.FileName, (int)mediaPathTypeId, mediaPath);
                        vm.ErrorMessage = msg;
                    }
                }
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int mediaPathTypeId)
        {
            var mediaPathTypes = _mediaPathTypesClient.Get()
                .Where(x => x.IsSharable)
                .Where(x => !x.IsSystem)
                .OrderBy(p => p.Description);
            var fileList = GetMediaFiles();

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    x.Category,
                    x.Description,
                    x.ShortDescription
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

            var rules = new PublicProfileRules();
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
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int mediaPathTypeId)
        {
            bool success;
            string errMsg;

            try
            {
                var rules = new PublicProfileRules();

                errMsg = rules.CanDeleteMultiple(ids.Length, mediaPathTypeId);
                if (errMsg.Length > 0)
                    throw new Exception(errMsg);

                foreach (var id in ids)
                {
                    var publicMediaFile = _publicMediaFilesClient.Get(id);
                    if (publicMediaFile == null) continue;

                    if (publicMediaFile.MediaFile.IsLinked)
                    {
                        _publicMediaFilesClient.Delete(id);
                    }
                    else
                    {
                        var file = rules.GetMediaFile(id);
                        if (file == null) continue;

                        // delete the link
                        errMsg = _publicMediaFilesClient.Delete(id);
                        if (errMsg.Length > 0)
                            throw new Exception(errMsg);

                        var fileManager = new FileManager { EventLogger = _systemEventLogger };
                        fileManager.DeleteFile($@"{file.Path}\{file.Filename}");

                        if (SharedLibraryRules.IsMediaTypeThumbnail(mediaPathTypeId))
                        {
                            errMsg = _thumbnailsClient.Delete(publicMediaFile.MediaFile.StreamId)
                                ?? string.Empty;
                        }
                    }
                }

                success = string.IsNullOrEmpty(errMsg);
            }
            catch (Exception ex)
            {
                success = false;
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = success,
                ErrorMessage = !success ? errMsg : null,
                FileList = GetMediaFiles()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetImageViewerView(Guid streamId, string fileType)
        {
            var rules = new ImageViewerRules();
            var m = rules.GetImageViewerModel(streamId, fileType);

            return PartialView("_ImageViewer", new ImageViewerViewModel
            {
                FileType = m.FileType,
                Width = m.Width,
                Height = m.Height,
                Base64String = m.Base64String
            });
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetSharedLibarayLinkView(int mediaPathTypeId)
        {
            var vm = LoadSharedLibaryAddViewModel(mediaPathTypeId);

            return vm.SharedFiles.Any()
                ? PartialView("_SharedLibraryLink", vm) 
                : null;
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int mediaPathTypeId)
        {
            bool success;
            string errMsg = null;

            try
            {
                if (streamIds != null)
                {
                    foreach (var streamId in streamIds)
                    {
                        var pmf = new PublicMediaFileEdit
                        {
                            StreamId = streamId,
                            ResponseTypeId = PublicProfileRules.GetResponseTypeId(mediaPathTypeId),
                            MediaPathTypeId = mediaPathTypeId,
                            IsLinked = true
                        };

                        int newId;
                        errMsg = _publicMediaFilesClient.Post(pmf, out newId);
                    }
                }
                success = string.IsNullOrEmpty(errMsg);
            }
            catch (Exception ex)
            {
                success = false;
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = success,
                ErrorMessage = !success ? errMsg : null,
                FileList = GetMediaFiles()
            }, JsonRequestBehavior.AllowGet);
        }

        private string AddPublicMediaFile(string filename, int mediaPathTypeId, string mediaPathType)
        {
            var fileManager = new FileManager { EventLogger = _systemEventLogger };
            var streamId = fileManager.GetStreamId($@"{PublicProfileSource.Id}\{mediaPathType}", filename);

            var mf = new PublicMediaFileEdit
            {
                StreamId = streamId,
                ResponseTypeId = PublicProfileRules.GetResponseTypeId(mediaPathTypeId),
                MediaPathTypeId = mediaPathTypeId,
                IsLinked = false
            };

            int newId;
            var errMsg = _publicMediaFilesClient.Post(mf, out newId);

            if (errMsg != null) return errMsg;

            if (PublicProfileRules.IsMediaTypeThumbnail(mediaPathTypeId))
            {
                var thumbnailGenerator = new ThumbnailGenerator();
                errMsg = thumbnailGenerator.Generate(streamId);
            }

            return errMsg;
        }

        private static PublicProfileViewModel LoadPublicProfileViewModel(int? mediaPathTypeId)
        {
            var vm = new PublicProfileViewModel
            {
                Title = PublicProfileSource.Description,
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Music
            };

            return vm;
        }

        private static SharedLibraryLinkViewModel LoadSharedLibaryAddViewModel(int mediaPathTypeId)
        {
            var rules = new PublicProfileRules();
            var files = rules.GetAvailableSharedMediaFiles(mediaPathTypeId);

            var vm = new SharedLibraryLinkViewModel
            {
                SharedFiles = files
                .Select(f => new SharedLibraryFileViewModel
                {
                    StreamId = f.StreamId,
                    Filename = f.Filename
                })
            };

            return vm;
        }

        private IEnumerable<MediaFileViewModel> GetMediaFiles()
        {
            var list = new List<MediaFileViewModel>();
            var mediaResponseTypes = _publicMediaFilesClient.Get(isSystem: false).ToArray();
            var thumbnails = _thumbnailsClient.Get().ToArray();
            var mediaPaths = mediaResponseTypes.SelectMany(x => x.Paths).ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = $@"{_mediaPath.ProfileRoot}\{PublicProfileSource.Id}";

            foreach (var media in mediaResponseTypes)
            {
                foreach (var path in media.Paths)
                {
                    var files = path.Files.OrderBy(f => f.Filename);

                    foreach (var file in files)
                    {
                        var thumb = thumbnails.FirstOrDefault(x => x.StreamId == file.StreamId);

                        var vm = new MediaFileViewModel
                        {
                            Id = file.Id,
                            StreamId = file.StreamId,
                            Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                            FileType = file.FileType.ToUpper(),
                            FileSize = file.FileSize,
                            IsLinked = file.IsLinked,
                            Path = $@"{pathRoot}\{path.MediaPathType.Path}",
                            MediaPathTypeId = path.MediaPathType.Id,
                            Thumbnail = thumb?.Image != null 
                                ? $"data:image/jpg;base64,{ Convert.ToBase64String(thumb.Image)}" 
                                : string.Empty
                        };

                        list.Add(vm);
                    }
                }
            }

            return list.OrderBy(x => x.Filename);
        }
    }
}