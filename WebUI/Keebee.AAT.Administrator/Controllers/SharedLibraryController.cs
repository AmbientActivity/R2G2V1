using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.Administrator.Extensions;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Web;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SharedLibraryController : Controller
    {
        // api client
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public SharedLibraryController()
        {
            _mediaFilesClient = new MediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
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
                SystemEventLogger.WriteEntry($"SharedLibrary.GetData: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            var jsonResult = Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                FileList = fileList,
                MediaPathTypeList = mediaPathTypeList
            }, JsonRequestBehavior.AllowGet);

            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
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
                    var rules = new SharedLibraryRules();
                    var path = $@"{_mediaSourcePath.MediaRoot}\{_mediaSourcePath.SharedLibrary}\{mediaPath}";

                    errMsg = rules.SaveUploadedFile(filename, path, file.InputStream, mediaPathTypeCategory);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"SharedLibrary.UploadFile: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
                var rules = new SharedLibraryRules();

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
                SystemEventLogger.WriteEntry($"SharedLibrary.AddFiles: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
        public JsonResult DeleteSelected(Guid[] streamIds, string mediaPathTypeCategory, bool isSharable)
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

                    errMsg = DeleteSharedLibraryFile(mediaFile, mediaPathTypeCategory);
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    deletedIds.Add(streamId);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"SharedLibrary.DeleteSelected: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
        public JsonResult GetImageViewerView(Guid streamId, string fileType)
        {
            string errMsg;
            string html = null;

            try
            {
                var rules = new ImageViewerRules();
                var model = rules.GetImageViewerModel(streamId, fileType, out errMsg);
                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                html = this.RenderPartialViewToString("_ImageViewer", new ImageViewerViewModel
                {
                    FileType = model.FileType,
                    Width = model.Width,
                    Height = model.Height,
                    Base64String = model.Base64String
                });
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                Html = html
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetLinkedProfilesView(Guid streamId)
        {
            string errMsg = null;
            string html = null;

            try
            {
                html = this.RenderPartialViewToString("_LinkedProfiles", 
                    new LinkedProfilesViewModel { StreamId = streamId });
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                Html = html,
            }, JsonRequestBehavior.AllowGet);
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
                SystemEventLogger.WriteEntry($"SharedLibrary.GetDataLinkedProfiles: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
            var pathTypes = mediaPathTypes ?? _mediaPathTypesClient
                .Get().Where(mp => mp.IsSharable);

            var media = sharedMedia.SelectMany(p =>
            {
                var mediaPathType = SharedLibraryRules.GetMediaPathTypeFromRawPath(p.Path, pathTypes.ToArray());
                return p.Files.Where(x => mediaPathType != null).Select(f =>
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
                })
                
                .OrderBy(x => x.Filename);
            });

            return media;
        }

        private string DeleteSharedLibraryFile(MediaFilePath mediaFile, string mediaPathTypeCategory)
        {
            string errMsg = null;

            try
            {
                var rules = new SharedLibraryRules();
                rules.DeleteFile($@"{mediaFile.Path}\{mediaFile.Filename}");

                if (mediaPathTypeCategory != MediaPathTypeCategoryDescription.Audio)
                {
                    errMsg = _thumbnailsClient.Delete(mediaFile.StreamId);
                }
                
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }
    }
}