using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PublicProfileController : Controller
    {
        // api client
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IResponseTypesClient _responseTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public PublicProfileController()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
            _responseTypesClient = new ResponseTypesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        // GET: PublicProfile
        [Authorize]
        public ActionResult Index()
        {
            return View(new PublicProfileViewModel
            {
                Title = PublicProfileSource.Description,
                SelectedMediaPathTypeId = MediaPathTypeId.Music
            });
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int mediaPathTypeId)
        {
            string errMsg = null;
            MediaFileViewModel[] fileList = null;
            MediaPathType[] mediaPathTypeList = null;

            try
            {
                fileList = GetFiles().ToArray();

                mediaPathTypeList = _mediaPathTypesClient.Get()
                    .Where(x => x.IsSharable)
                    .Where(x => !x.IsSystem)
                    .OrderBy(p => p.Description)
                    .Select(x => new MediaPathType
                    {
                        Id = x.Id,
                        ResponseTypeId = x.ResponseTypeId,
                        Category = x.Category,
                        Description = x.Description,
                        ShortDescription = x.ShortDescription,
                        Path = x.Path,
                        AllowedExts = x.AllowedExts.Replace(" ", string.Empty),
                        AllowedTypes = x.AllowedTypes.Replace(" ", string.Empty),
                        MaxFileBytes = x.MaxFileBytes,
                        MaxFileUploads = x.MaxFileUploads
                    }).ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"PublicProfile.GetData: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                FileList = fileList,
                MediaPathTypeList = mediaPathTypeList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UploadFile(HttpPostedFileBase file, int mediaPathTypeId, string mediaPath, string mediaPathTypeCategory)
        {
            string errMsg = null;
            string filename = null;

            try
            {
                if (file != null)
                {
                    filename = file.FileName;
                    var rules = new PublicProfileRules();
                    var path = $@"{_mediaSourcePath.ProfileRoot}\{PublicProfileSource.Id}\{mediaPath}";

                    errMsg = rules.SaveUploadedFile(filename, path, file.InputStream, mediaPathTypeCategory);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"PublicProfile.UploadFile: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Filename = filename,
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddFiles(string[] filenames, int mediaPathTypeId, int responseTypeId)
        {
            string errMsg = null;
            var newFiles = new Collection<MediaFileModel>();
            const bool isLinked = false;

            try
            {
                var dateAdded = DateTime.Now;
                var rules = new PublicProfileRules();
                var responseType = _responseTypesClient.Get(responseTypeId);
                var mediaPathType = _mediaPathTypesClient.Get(mediaPathTypeId);

                foreach (var filename in filenames.OrderByDescending(x => x))
                {
                    MediaFileModel newFile;
                    errMsg = rules.AddMediaFileFromFilename(filename, mediaPathType, responseType, dateAdded, isLinked, out newFile);

                    if (!string.IsNullOrEmpty(errMsg))
                        throw new Exception(errMsg);

                    newFiles.Add(newFile);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"PublicProfile.AddFiles: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                FileList = newFiles
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int mediaPathTypeId, int responseTypeId)
        {
            string errMsg = null;
            var newFiles = new Collection<MediaFileModel>();
            const bool isLinked = true;

            try
            {
                if (streamIds != null)
                {
                    var rules = new PublicProfileRules();
                    var dateAdded = DateTime.Now;
                    var responseType = _responseTypesClient.Get(responseTypeId);
                    var mediaPathType = _mediaPathTypesClient.Get(mediaPathTypeId);

                    foreach (var streamId in streamIds)
                    {
                        MediaFileModel newFile;
                        errMsg = rules.AddMediaFile(streamId, mediaPathType, responseType, dateAdded, isLinked, out newFile);

                        if (!string.IsNullOrEmpty(errMsg))
                            throw new Exception(errMsg);

                        newFiles.Add(newFile);
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"PublicProfile.AddSharedMediaFiles: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                FileList = newFiles
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int mediaPathTypeId, int responseTypeId)
        {
            string errMsg;
            var deletedIds = new Collection<int>();

            try
            {
                var rules = new PublicProfileRules();

                errMsg = rules.CanDeleteMultiple(ids.Length, mediaPathTypeId, responseTypeId);
                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                var responseType = _responseTypesClient.Get(responseTypeId);
                foreach (var id in ids)
                {
                    errMsg = rules.DeleteMediaFile(id, responseType);
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    deletedIds.Add(id);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"PublicProfile.DeleteSelected: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                DeletedIds = deletedIds
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

        private IEnumerable<MediaFileViewModel> GetFiles()
        {
            var list = new List<MediaFileViewModel>();
            var mediaResponseTypes = _publicMediaFilesClient.Get(isSystem: false).ToArray();
            var thumbnails = _thumbnailsClient.Get().ToArray();
            var mediaPaths = mediaResponseTypes.SelectMany(x => x.Paths).ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = $@"{_mediaSourcePath.ProfileRoot}\{PublicProfileSource.Id}";

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
                            DateAdded = file.DateAdded,
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