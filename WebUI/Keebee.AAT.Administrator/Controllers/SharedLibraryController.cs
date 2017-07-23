using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SharedLibraryController : Controller
    {
        // api client
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IResponseTypesClient _responseTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public SharedLibraryController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

            _mediaFilesClient = new MediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
            _responseTypesClient = new ResponseTypesClient(); 
            _thumbnailsClient = new ThumbnailsClient();
        }

        [Authorize]
        public ActionResult Index()
        {
            return View(new SharedLibraryViewModel
            {
                Title = "Shared Library",
                SelectedMediaPathTypeId = MediaPathTypeId.Music
            });
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData(int mediaPathTypeId)
        {
            string errMsg = null;
            SharedLibraryFileViewModel[] fileList = null;
            MediaPathType[] mediaPathTypeList = null;

            try
            {
                mediaPathTypeList = GetMediaPathTypeList();
                fileList = GetFiles().ToArray();
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
        public JsonResult UploadFile(HttpPostedFileBase file, int mediaPathTypeId, string mediaPath, string mediaPathTypeCategory)
        {
            string errMsg = null;
            string filename = null;

            try
            {
                if (file != null)
                {
                    filename = file.FileName;
                    var rules = new SharedLibraryRules { EventLogger = _systemEventLogger };
                    var path = $@"{_mediaSourcePath.MediaRoot}\{_mediaSourcePath.SharedLibrary}\{mediaPath}";

                    errMsg = rules.SaveUploadedFile(filename, path, file.InputStream, mediaPathTypeCategory);
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
                Filename = filename
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult AddFiles(string[] filenames, int mediaPathTypeId)
        {
            string errMsg = null;
            var newFiles = new Collection<SharedMediaFileModel>();

            try
            {
                var rules = new SharedLibraryRules { EventLogger = _systemEventLogger };

                var mediaPathType = _mediaPathTypesClient.Get(mediaPathTypeId);

                foreach (var filename in filenames)
                {
                    SharedMediaFileModel newFile;
                    errMsg = rules.AddFile(filename, mediaPathType, out newFile);

                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

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
        public JsonResult DeleteSelected(Guid[] streamIds, int mediaPathTypeId, bool isSharable)
        {
            var errMsg = string.Empty;
            var rules = new SharedLibraryRules();
            var deletedIds = new Collection<Guid>();

            try
            {
                foreach (var streamId in streamIds)
                {
                    var mediaFile = _mediaFilesClient.Get(streamId);
                    if (mediaFile == null) continue;

                    // delete all the linkage if it's sharable media (non-system)
                    if (isSharable)
                    {
                        errMsg = rules.DeleteSharedMediaFileLinks(streamId);
                        if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);                        
                    }

                    errMsg = DeleteSharedLibraryFile(mediaFile, mediaPathTypeId);
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    deletedIds.Add(streamId);
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

        [HttpGet]
        [Authorize]
        public PartialViewResult GetLinkedProfilesView(Guid streamId)
        {
            return PartialView("_LinkedProfiles", new LinkedProfilesViewModel{ StreamId = streamId });
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetDataLinkedProfiles(Guid streamId)
        {
            string errMsg = null;
            ResidentViewModel[] profiles = null;

            try
            {
                var rules = new SharedLibraryRules();
                profiles = rules.GetLinkedProfiles(streamId)
                    .Select(r => new
                    ResidentViewModel
                    {
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        ProfilePicture = ResidentRules.GetProfilePicture(r?.ProfilePicture),
                        ProfilePicturePlaceholder = ResidentRules.GetProfilePicturePlaceholder()
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
                Profiles = profiles

            }, JsonRequestBehavior.AllowGet);
        }

        private MediaPathType[] GetMediaPathTypeList()
        {
            var mediaPathTypes = _mediaPathTypesClient.Get().Where(mp => mp.IsSharable).ToArray();

            return mediaPathTypes
                .Select(x => new MediaPathType
                {
                    Id = x.Id,
                    Category = x.Category,
                    Description = x.Description,
                    ShortDescription = x.ShortDescription,
                    IsSharable = true,
                    Path = x.Path,
                    AllowedExts = x.AllowedExts.Replace(" ", string.Empty),
                    AllowedTypes = x.AllowedTypes.Replace(" ", string.Empty),
                    MaxFileBytes = x.MaxFileBytes,
                    MaxFileUploads = x.MaxFileUploads
                }).OrderBy(x => x.Description).ToArray();
        }

        private IEnumerable<SharedLibraryFileViewModel> GetFiles(IEnumerable<MediaPathType> mediaPathTypes = null)
        {
            var sharedMedia = _mediaFilesClient.GetWithLinkedData(_mediaSourcePath.SharedLibrary);
            var thumbnails = _thumbnailsClient.Get().ToArray();
            var pathTypes = mediaPathTypes ?? _mediaPathTypesClient.Get().Where(mp => mp.IsSharable).ToArray();

            var media = sharedMedia.SelectMany(p =>
            {
                var mediaPathType = SharedLibraryRules.GetMediaPathTypeFromRawPath(p.Path, pathTypes);

                return p.Files.Select(f =>
                {
                    var thumb = thumbnails.FirstOrDefault(x => x.StreamId == f.StreamId);
                    return new SharedLibraryFileViewModel
                    {
                        StreamId = f.StreamId,
                        Filename = f.Filename.Replace($".{f.FileType}", string.Empty),
                        FileSize = f.FileSize,
                        FileType = f.FileType.ToUpper(),
                        Path = mediaPathType.Path,
                        MediaPathTypeId = mediaPathType.Id,
                        NumLinkedProfiles = f.NumLinkedProfiles,
                        Thumbnail = SharedLibraryRules.GetThumbnail(thumb?.Image)
                    };
                }).OrderBy(x => x.Filename);
            });

            return media;
        }

        private string DeleteSharedLibraryFile(MediaFilePath mediaFile, int mediaPathTypeId)
        {
            string errorMessage = null;

            try
            {
                var rules = new SharedLibraryRules { EventLogger = _systemEventLogger };
                rules.DeleteFile($@"{mediaFile.Path}\{mediaFile.Filename}");

                if (SharedLibraryRules.IsMediaTypeThumbnail(mediaPathTypeId))
                {
                    errorMessage = _thumbnailsClient.Delete(mediaFile.StreamId);
                }
                
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}