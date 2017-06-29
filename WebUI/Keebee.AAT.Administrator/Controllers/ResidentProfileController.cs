using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CuteWebUI;
using Keebee.AAT.ThumbnailGeneration;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ResidentProfileController : Controller
    {
        // api client
        private readonly IResidentsClient _residentsClient;
        private readonly IActiveResidentClient _activeResidentClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public ResidentProfileController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

            _residentsClient = new ResidentsClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
            _activeResidentClient = new ActiveResidentClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        // GET: ResidentProfile
        [Authorize]
        public ActionResult Index(
            int id,
            string idsearch,
            string firstname,
            string lastname,
            string sortcolumn,
            int? mediaPathTypeId,
            string myuploader,
            int? sortdescending)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Music;

            var vm = LoadResidentProfileViewModel(id, idsearch, firstname, lastname, mediaPathTypeId, sortcolumn, sortdescending);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = ResidentRules.GetAllowedExtensions(mediaPathTypeId);
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                vm.UploaderHtml = uploader.Render();

                // GET:
                if (string.IsNullOrEmpty(myuploader))
                    return View(vm);

                // POST:
                var fileManager = new FileManager { EventLogger = _systemEventLogger };
                var rules = new ResidentRules();

                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!ResidentRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var mediaPath = rules.GetMediaPath(mediaPathTypeId);
                    var filePath = $@"{_mediaSourcePath.ProfileRoot}\{id}\{mediaPath}\{file.FileName}";

                    // delete it if it already exists
                    var msg = fileManager.DeleteFile(filePath);

                    if (msg.Length == 0)
                    {
                        file.MoveTo(filePath);
                        msg = AddResidentMediaFile(id, file.FileName, (int)mediaPathTypeId, mediaPath);
                        vm.ErrorMessage = msg;
                    }
                }
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int id, int mediaPathTypeId)
        {
            var mediaPathTypes = _mediaPathTypesClient.Get()
                .Where(x => !x.IsSystem)
                .OrderBy(p => p.Description);

            var fileList = GetFiles(id);

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    x.Category,
                    x.Description,
                    x.ShortDescription,
                    x.IsSharable
                })
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddFromSharedLibrary(Guid[] streamIds, int residentId, int mediaPathTypeId)
        {
            bool success;
            string errorMessage = null;

            try
            {
                var responseTypeId = ResidentRules.GetResponseTypeId(mediaPathTypeId);

                if (streamIds != null)
                {
                    foreach (var streamId in streamIds)
                    {
                        var mf = new ResidentMediaFileEdit
                        {
                            StreamId = streamId,
                            ResidentId = residentId,
                            ResponseTypeId = responseTypeId,
                            MediaPathTypeId = mediaPathTypeId,
                            IsLinked = true
                        };

                        int newId;
                        errorMessage = _residentMediaFilesClient.Post(mf, out newId);
                    }
                }
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
            }

            return Json(new
            {
                Success = success,
                ErrorMessage = errorMessage,
                FileList = GetFiles(residentId)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetSharedLibarayLinkView(int residentId, int mediaPathTypeId)
        {
            var vm = LoadSharedLibaryAddViewModel(residentId, mediaPathTypeId);

            return vm.SharedFiles.Any()
                ? PartialView("_SharedLibraryLink", vm)
                : null;
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

            var rules = new ResidentRules();
            return Json(new
            {
                UploaderHtml = html,
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
                PaddingLeft = m.PaddingLeft,
                Base64String = m.Base64String
            });
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int residentId, int mediaPathTypeId)
        {
            bool success;
            var errormessage = string.Empty;

            try
            {
                var activeResident = _activeResidentClient.Get();
                if (activeResident.Resident.Id == residentId)
                {
                    errormessage = "The resident is currently engaging with R2G2. Media cannot be deleted at this time.";
                }
                else
                {
                    if (ids.Length > 0)
                    {
                        var rules = new ResidentRules();
                        foreach (var id in ids)
                        {
                            var resdientMediaFile = _residentMediaFilesClient.Get(id);
                            if (resdientMediaFile == null) continue;

                            if (resdientMediaFile.MediaFile.IsLinked)
                            {
                                _residentMediaFilesClient.Delete(id);
                            }
                            else
                            {
                                var file = rules.GetMediaFile(id);
                                if (file == null) continue;

                                var fileManager = new FileManager { EventLogger = _systemEventLogger };
                                errormessage = fileManager.DeleteFile($@"{file.Path}\{file.Filename}");

                                if (errormessage.Length == 0)
                                    _residentMediaFilesClient.Delete(id);
                                else
                                    break;
                            }
                        }
                    }
                }
                success = (errormessage.Length == 0);
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
                FileList = GetFiles(residentId)
            }, JsonRequestBehavior.AllowGet);
        }

        private ResidentProfileViewModel LoadResidentProfileViewModel(
            int id,
            string idsearch,
            string firstname,
            string lastname,
            int? mediaPathTypeId,
            string sortcolumn,
            int? sortdescending)
        {
            var resident = _residentsClient.Get(id);
            var fullName = (resident.LastName != null)
                ? $"{resident.FirstName} {resident.LastName}"
                : resident.FirstName;
            var rules = new ResidentRules();

            var vm = new ResidentProfileViewModel
            {
                ResidentId = resident.Id,
                FullName = fullName,
                ProfilePicture = ResidentRules.GetProfilePicture(resident.ProfilePicture) ?? ResidentRules.GetProfilePicturePlaceholder(),
                IdSearch = idsearch,
                FirstNameSearch = firstname,
                LastNameSearch = lastname,
                SortColumn = sortcolumn,
                SortDescending = sortdescending,
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Music
            };

            return vm;
        }

        private string AddResidentMediaFile(int residentId, string filename, int mediaPathTypeId, string mediaPathType)
        {
            var fileManager = new FileManager { EventLogger = _systemEventLogger };
            var streamId = fileManager.GetStreamId($@"{residentId}\{mediaPathType}", filename);
            var responseTypeId = ResidentRules.GetResponseTypeId(mediaPathTypeId);

            var mf = new ResidentMediaFileEdit
            {
                StreamId = streamId,
                ResidentId = residentId,
                ResponseTypeId = responseTypeId,
                MediaPathTypeId = mediaPathTypeId,
                IsLinked = false
            };

            int newId;
            var errorMessage = _residentMediaFilesClient.Post(mf, out newId);

            if (errorMessage != null) return errorMessage;

            var thumbnailGenerator = new ThumbnailGenerator();
            errorMessage = thumbnailGenerator.Generate(streamId);

            return errorMessage;
        }

        private static SharedLibraryLinkViewModel LoadSharedLibaryAddViewModel(int residentId, int mediaPathTypeId)
        {
            var rules = new ResidentRules();
            var files = rules.GetAvailableSharedMediaFiles(residentId, mediaPathTypeId).ToArray();

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

        private IEnumerable<MediaFileViewModel> GetFiles(int id)
        {
            var list = new List<MediaFileViewModel>();
            var paths = _residentMediaFilesClient.GetForResident(id);
            var thumbnails = _thumbnailsClient.Get().ToArray();

            if (paths == null) return list;

            var mediaPaths = paths.SelectMany(x => x.Paths).ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = $@"{_mediaSourcePath.ProfileRoot}\{id}";

            foreach (var path in mediaPaths)
            {
                var files = path.Files.OrderBy(f => f.Filename);

                foreach (var file in files)
                {
                    var thumb = thumbnails.FirstOrDefault(x => x.StreamId == file.StreamId);
                    var item = new MediaFileViewModel
                    {
                        Id = file.Id,
                        StreamId = file.StreamId,
                        Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                        FileType = file.FileType.ToUpper(),
                        FileSize = file.FileSize,
                        IsLinked = file.IsLinked,
                        Path = $@"{pathRoot}\{path.MediaPathType.Description}",
                        MediaPathTypeId = path.MediaPathType.Id,
                        Thumbnail = ResidentRules.GetThumbnail(thumb?.Image)
                    };

                    list.Add(item);
                }
            }

            return list.OrderBy(x => x.Filename);
        }
    }
}