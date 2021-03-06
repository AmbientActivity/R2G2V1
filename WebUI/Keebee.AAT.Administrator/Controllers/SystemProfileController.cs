﻿using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.BusinessRules.Models;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Administrator.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SystemProfileController : Controller
    {
        // api client
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IResponseTypesClient _responseTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public SystemProfileController()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
            _responseTypesClient = new ResponseTypesClient();
            _thumbnailsClient = new ThumbnailsClient();
        }

        // GET: SystemProfile
        [Authorize]
        public ActionResult Index()
        {
            return View(new PublicProfileViewModel
            {
                Title = PublicProfileSource.DescriptionSystem,
                SelectedMediaPathTypeId = MediaPathTypeId.Ambient
            });
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            string errMsg = null;
            MediaFileViewModel[] fileList = null;
            MediaPathType[] mediaPathTypeList = null;

            try
            {
                fileList = GetFiles().ToArray();

                mediaPathTypeList = _mediaPathTypesClient.Get()
                    .Where(x => x.IsSystem)
                    .OrderBy(p => p.Description)
                    .Select(x => new MediaPathType
                    {
                        Id = x.Id,
                        ResponseTypeId = x.ResponseTypeId,
                        Category = x.Category,
                        Description = x.Description,
                        ShortDescription = x.ShortDescription
                    }).ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"SystemProfile.GetData: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"SystemProfile.AddSharedMediaFiles: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"SystemProfile.DeleteSelected: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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

        private IEnumerable<MediaFileViewModel> GetFiles()
        {
            var list = new List<MediaFileViewModel>();
            var mediaResponseTypes = _publicMediaFilesClient.Get(isSystem: true).ToArray(); // system only
            var thumbnails = _thumbnailsClient.Get().ToArray();

            var mediaPaths = mediaResponseTypes.SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.IsSystem)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = _mediaSourcePath.MediaRoot;

            foreach (var media in mediaResponseTypes)
            {
                foreach (var path in media.Paths.Where(x => x.MediaPathType.IsSystem))
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
                            Path = $@"{pathRoot}\{path.MediaPathType.Path}",
                            IsLinked = file.IsLinked,
                            MediaPathTypeId = path.MediaPathType.Id,
                            DateAdded = file.DateAdded,
                            Thumbnail = SystemProfileRules.GetThumbnail(thumb?.Image)
                        };

                        list.Add(vm);
                    }
                }
            }

            return list;
        }
    }
}