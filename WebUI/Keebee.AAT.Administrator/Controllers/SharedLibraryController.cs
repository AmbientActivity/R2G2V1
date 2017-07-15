﻿using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.ThumbnailGeneration;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SharedLibraryController : Controller
    {
        // api client
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public SharedLibraryController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

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
            var fileList = new SharedLibraryFileViewModel[0];
            var mediaPathTypeList = new MediaPathType[0];

            try
            {
                var mediaPathTypes = _mediaPathTypesClient.Get().Where(mp => mp.IsSharable).ToArray();

                mediaPathTypeList = mediaPathTypes
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

                fileList = GetFiles(mediaPathTypes).ToArray();
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
        public JsonResult UploadFile(HttpPostedFileBase file, string mediaPath, int mediaPathTypeId, string mediaPathTypeCategory)
        {
            string errMsg = null;

            try
            {
                if (file != null)
                {
                    var filePath = $@"{_mediaSourcePath.MediaRoot}\{_mediaSourcePath.SharedLibrary}\{mediaPath}\{file.FileName}";

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
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(Guid[] streamIds, int mediaPathTypeId, bool isSharable)
        {
            var errMsg = string.Empty;
            var rules = new SharedLibraryRules();

            var mediaPathTypes = _mediaPathTypesClient.Get()
                .Where(mp => mp.Id != MediaPathTypeId.ImagesPersonal && mp.Id != MediaPathTypeId.HomeMovies).ToArray();
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
                        if (errMsg.Length > 0)
                            throw new Exception(errMsg);
                    }

                    errMsg = DeleteSharedLibraryFile(mediaFile, mediaPathTypeId);
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
                FileList = GetFiles(mediaPathTypes)
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
                FileType = m.FileType,
                Width = m.Width,
                Height = m.Height,
                Base64String = m.Base64String
            });
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

        private IEnumerable<SharedLibraryFileViewModel> GetFiles(IEnumerable<MediaPathType> mediaPathTypes)
        {
            var sharedMedia = _mediaFilesClient.GetWithLinkedData(_mediaSourcePath.SharedLibrary);
            var thumbnails = _thumbnailsClient.Get().ToArray();

            return sharedMedia.SelectMany(p =>
            {
                var mediaPathType = SharedLibraryRules.GetMediaPathTypeFromRawPath(p.Path, mediaPathTypes);

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
        }

        private string DeleteSharedLibraryFile(MediaFilePath mediaFile, int mediaPathTypeId)
        {
            string errorMessage = null;

            try
            {
                var fileManager = new FileManager { EventLogger = _systemEventLogger };
                fileManager.DeleteFile($@"{mediaFile.Path}\{mediaFile.Filename}");

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