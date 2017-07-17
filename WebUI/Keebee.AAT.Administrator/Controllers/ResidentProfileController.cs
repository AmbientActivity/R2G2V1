using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ThumbnailGeneration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
        public JsonResult AddFiles(string[] filenames, int residentId, string mediaPath, int mediaPathTypeId, string dateAdded)
        {
            string errMsg = null;
            var newIds = new Collection<int>();

            try
            {
                foreach (var filename in filenames)
                {
                    int newId;
                    errMsg = AddResidentMediaFileFromFilename(filename, residentId, mediaPathTypeId, mediaPath, dateAdded, out newId);

                    if (!string.IsNullOrEmpty(errMsg))
                        throw new Exception(errMsg);

                    newIds.Add(newId);
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
                FileList = GetFiles(residentId),
                NewIds = newIds
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UploadFile(HttpPostedFileBase file, string mediaPath, int mediaPathTypeId, string mediaPathTypeCategory, int residentId)
        {
            string errMsg = null;
            string filename = null;

            try
            {
                if (file != null)
                {
                    filename = file.FileName;
                    var filePath = $@"{_mediaSourcePath.ProfileRoot}\{residentId}\{mediaPath}\{file.FileName}";

                    // delete it if it already exists
                    var fileManager = new FileManager { EventLogger = _systemEventLogger };
                    var msg = fileManager.DeleteFile(filePath);

                    if (msg.Length == 0)
                    {
                        if (mediaPathTypeCategory == MediaPathTypeCategoryDescription.Image)
                        {
                            var image = Image.FromStream(file.InputStream);
                            var orientedImage = image.Orient();
                            var scaled = orientedImage.Scale();
                            scaled.Save(filePath);
                        }
                        else
                        {
                            file.SaveAs(filePath);
                        }
                    }
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
                Filename = filename,
                MediaPath = mediaPath,
                MediaPathTypeId = mediaPathTypeId
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int residentId, int mediaPathTypeId, string dateAdded)
        {
            string errMsg = null;
            var newIds = new Collection<int>();

            try
            {
                var rules = new ResidentRules();
                if (streamIds != null)
                {
                    foreach (var streamId in streamIds)
                    {
                        int newId;
                        errMsg = rules.AddMediaFile(streamId, residentId, mediaPathTypeId, dateAdded, true, out newId);

                        if (!string.IsNullOrEmpty(errMsg))
                            throw new Exception(errMsg);

                        newIds.Add(newId);
                    }
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
                FileList = GetFiles(residentId),
                NewIds = newIds,
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
        public JsonResult DeleteSelected(int[] ids, int residentId, int mediaPathTypeId)
        {
            var errMsg = string.Empty;

            try
            {
                var activeResident = _activeResidentClient.Get();
                if (activeResident.Resident.Id == residentId)
                {
                    errMsg = "The resident is currently engaging with R2G2. Media cannot be deleted at this time.";
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
                                errMsg = fileManager.DeleteFile($@"{file.Path}\{file.Filename}");

                                if (errMsg.Length == 0)
                                    _residentMediaFilesClient.Delete(id);
                                else
                                    break;

                                if (ResidentRules.IsMediaTypeThumbnail(mediaPathTypeId))
                                {
                                    errMsg = _thumbnailsClient.Delete(resdientMediaFile.MediaFile.StreamId) 
                                        ?? string.Empty;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage =  errMsg,
                FileList = GetFiles(residentId)
            }, JsonRequestBehavior.AllowGet);
        }

        private string AddResidentMediaFileFromFilename(string filename, int residentId, int mediaPathTypeId, string mediaPathType, string dateAdded, out int newId)
        {
            newId = -1;
            var fileManager = new FileManager { EventLogger = _systemEventLogger };

            var streamId = fileManager.GetStreamId($@"{residentId}\{mediaPathType}", filename);
            if (streamId == new Guid()) return $"Could not get StreamId for file <b>{filename}</b>";

            var rules = new ResidentRules();
            var errMsg = rules.AddMediaFile(streamId, residentId, mediaPathTypeId, dateAdded, false, out newId);

            if (!string.IsNullOrEmpty(errMsg)) return errMsg;

            if (!ResidentRules.IsMediaTypeThumbnail(mediaPathTypeId)) return errMsg;

            var thumbnailGenerator = new ThumbnailGenerator();
            errMsg = thumbnailGenerator.Generate(streamId);

            return errMsg;
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