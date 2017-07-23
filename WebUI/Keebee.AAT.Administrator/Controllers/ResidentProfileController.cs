using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.BusinessRules.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ResidentProfileController : Controller
    {
        // api client
        private readonly IResidentsClient _residentsClient;
        private readonly IActiveResidentClient _activeResidentClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IResponseTypesClient _responseTypesClient;
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
            _responseTypesClient = new ResponseTypesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        // GET: ResidentProfile
        [Authorize]
        public ActionResult Index(int id, string idsearch, string firstname, string lastname, string sortcolumn, int? sortdescending)
        {
            return View(LoadResidentProfileViewModel(id, idsearch, firstname, lastname, sortcolumn, sortdescending));
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int id, int mediaPathTypeId)
        {
            string errMsg = null;
            MediaFileViewModel[] fileList = null;
            MediaPathType[] mediaPathTypeList = null;

            try
            {
                fileList = GetFiles(id).ToArray();

                mediaPathTypeList = _mediaPathTypesClient.Get()
                    .Where(x => !x.IsSystem)
                    .OrderBy(p => p.Description)
                    .Select(x => new MediaPathType
                    {
                        Id = x.Id,
                        ResponseTypeId = x.ResponseTypeId,
                        Category = x.Category,
                        Description = x.Description,
                        ShortDescription = x.ShortDescription,
                        IsSharable = x.IsSharable,
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
        public JsonResult AddFiles(string[] filenames, int residentId, int mediaPathTypeId, int responseTypeId)
        {
            string errMsg = null;
            var newFiles = new Collection<MediaFileModel>();
            const bool isLinked = false;

            try
            {
                var dateAdded = DateTime.Now;
                var rules = new ResidentRules { EventLogger = _systemEventLogger };
                var responseType = _responseTypesClient.Get(responseTypeId);
                var mediaPathType = _mediaPathTypesClient.Get(mediaPathTypeId);

                foreach (var filename in filenames.OrderByDescending(x => x))
                {
                    MediaFileModel newFile;
                    errMsg = rules.AddMediaFileFromFilename(filename, residentId, mediaPathType, responseType, dateAdded, isLinked, out newFile);

                    if (!string.IsNullOrEmpty(errMsg))
                        throw new Exception(errMsg);

                    newFiles.Add(newFile);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
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
        public JsonResult UploadFile(HttpPostedFileBase file, int residentId, int mediaPathTypeId, string mediaPath, string mediaPathTypeCategory)
        {
            string errMsg = null;
            string filename = null;

            try
            {
                if (file != null)
                {
                    filename = file.FileName;
                    var rules = new ResidentRules { EventLogger = _systemEventLogger };

                    errMsg = rules.SaveUploadedFile(
                        filename,
                        $@"{_mediaSourcePath.ProfileRoot}\{residentId}\{mediaPath}",
                        file.InputStream,
                        mediaPathTypeCategory);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                _systemEventLogger.WriteEntry($"ResidentProfile.UploadFile: {errMsg}", EventLogEntryType.Error);
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
        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int residentId, string mediaPath, int mediaPathTypeId, int responseTypeId)
        {
            string errMsg = null;
            var newFiles = new Collection<MediaFileModel>();
            const bool isLinked = true;

            try
            {
                if (streamIds != null)
                {
                    var rules = new ResidentRules();
                    var dateAdded = DateTime.Now;
                    var responseType = _responseTypesClient.Get(responseTypeId);
                    var mediaPathType = _mediaPathTypesClient.Get(mediaPathTypeId);

                    foreach (var streamId in streamIds)
                    {
                        MediaFileModel newFile;
                        errMsg = rules.AddMediaFile(streamId, residentId, mediaPathType, responseType, dateAdded, isLinked, out newFile);

                        if (!string.IsNullOrEmpty(errMsg))
                            throw new Exception(errMsg);

                        newFiles.Add(newFile);
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                _systemEventLogger.WriteEntry($"ResidentProfile.AddSharedMediaFiles: {errMsg}", EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                FileList = newFiles
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

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int residentId, int responseTypeId)
        {
            var errMsg = string.Empty;
            var deletedIds = new Collection<int>();

            try
            {
                var activeResident = _activeResidentClient.Get();
                if (activeResident.Resident.Id == residentId)
                {
                    errMsg = "The resident is currently engaging with R2G2. Media cannot be deleted at this time.";
                }
                else
                {
                    var rules = new ResidentRules { EventLogger = _systemEventLogger };
                    var responseType = _responseTypesClient.Get(responseTypeId);

                    foreach (var id in ids)
                    {
                        errMsg = rules.DeleteMediaFile(id, responseType);
                        if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                        deletedIds.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                _systemEventLogger.WriteEntry($"ResidentProfile.DeleteSelected: {errMsg}", EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage =  errMsg,
                DeletedIds = deletedIds
            }, JsonRequestBehavior.AllowGet);
        }

        private ResidentProfileViewModel LoadResidentProfileViewModel(int id, string idsearch, string firstname, string lastname, string sortcolumn, int? sortdescending)
        {
            var resident = _residentsClient.Get(id);
            var fullName = (resident.LastName != null)
                ? $"{resident.FirstName} {resident.LastName}"
                : resident.FirstName;

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
                SelectedMediaPathTypeId = MediaPathTypeId.Music
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
                        DateAdded = file.DateAdded,
                        Thumbnail = ResidentRules.GetThumbnail(thumb?.Image)
                    };

                    list.Add(item);
                }
            }

            return list.OrderBy(x => x.Filename);
        }
    }
}