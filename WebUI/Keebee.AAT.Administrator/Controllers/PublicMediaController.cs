using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.Administrator.Extensions;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using CuteWebUI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PublicMediaController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public PublicMediaController()
        {
            _opsClient = new OperationsClient();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
        }

        // GET: PublicMediaFile
        public ActionResult Index(
            int? mediaPathTypeId,
            int? responseTypeId,
            string myuploader)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Images;
            if (responseTypeId == null) responseTypeId = ResponseTypeId.SlidShow;

            var vm = LoadPublicMediaViewModel(mediaPathTypeId, responseTypeId);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = PublicMediaRules.GetAllowedExtensions(mediaPathTypeId);
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

                    if (!PublicMediaRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var rules = new PublicMediaRules { OperationsClient = _opsClient };
                    var mediaPathType = rules.GetMediaPathType(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}\{mediaPathType}\{file.FileName}";

                    if (rules.IsRemovable(filePath, (int)mediaPathTypeId, (int)responseTypeId))
                    {
                        fileManager.DeleteFile(filePath);
                    }

                    if (!rules.FileExists($@"{PublicMediaSource.Id}\{mediaPathType}", file.FileName))
                    {
                        file.MoveTo(filePath);
                    }

                    AddPublicMediaFile(file.FileName, (int)responseTypeId, (int)mediaPathTypeId, mediaPathType);
                }
            }

            return View(vm);
        }

        [HttpGet]
        public JsonResult GetData(int mediaPathTypeId)
        {
            var rules = new PublicMediaRules { OperationsClient =  _opsClient };
            var mediaPathTypes = _opsClient.GetMediaPathTypes().Where(x => x.Id != MediaPathTypeId.Pictures);
            var responseTypes = rules.GetValidResponseTypes(mediaPathTypeId);
            var fileList = GetMediaFiles(mediaPathTypeId);

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    Description = x.Description.ToUppercaseFirst()
                }),
                ResponseTypeList = responseTypes.Select(x => new
                {
                    x.Id,
                    x.Description
                })
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSelected(Guid[] streamIds, int mediaPathTypeId, int responseTypeId)
        {
            bool success;
            string errormessage;

            try
            {
                var rules = new PublicMediaRules {OperationsClient = _opsClient};

                errormessage = rules.CanDeleteMultiple(streamIds.Length, mediaPathTypeId, responseTypeId);
                if (errormessage.Length > 0)
                    throw new Exception(errormessage);

                foreach (var id in streamIds)
                {
                    var file = _opsClient.GetMediaFile(id);
                    if (file == null) continue;

                    // if the file is used in multiple response types
                    if (rules.IsMultipleReponseTypes(id))
                    {
                        // delete the link only
                        errormessage = rules.DeletePublicMediaFile(id, responseTypeId);
                        if (errormessage.Length > 0)
                            throw new Exception(errormessage);
                    }
                    else
                    {
                        // otherwise delete the file (link will get deleted automatically)
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
                        FileList = GetMediaFiles(mediaPathTypeId)
                    }, JsonRequestBehavior.AllowGet);
        }

        private void AddPublicMediaFile(string filename, int responseTypeId, int mediaPathTypeId, string mediaPathType)
        {
            var fileManager = new FileManager { EventLogger = _systemEventLogger };
            var streamId = fileManager.GetStreamId($@"{PublicMediaSource.Id}\{mediaPathType}", filename);

            var mf = new PublicMediaFileEdit
            {
                StreamId = streamId,
                ResponseTypeId = responseTypeId,
                MediaPathTypeId = mediaPathTypeId
            };

            _opsClient.PostPublicMediaFile(mf);
        }

        private IEnumerable<PublicMediaFileViewModel> GetMediaFiles(int mediaPathTypeId)
        {
            var list = new List<PublicMediaFileViewModel>();
            var publicMedia= _opsClient.GetPublicMediaFiles();

            if (publicMedia == null) return list;
 
            var mediaPaths = publicMedia.MediaFiles.SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var mediaPathType = mediaPaths
                .First(x => x.MediaPathType.Id == mediaPathTypeId)
                .MediaPathType.Description;

            var pathRoot = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}";

            foreach (var media in publicMedia.MediaFiles)
            {
                foreach (var path in media.Paths.Where(x => x.MediaPathType.Id == mediaPathTypeId))
                {
                    foreach (var file in path.Files.OrderBy(x => x.Filename))
                    {
                        var vm = new PublicMediaFileViewModel
                        {
                            StreamId = file.StreamId,
                            Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                            FileType = file.FileType.ToUpper(),
                            FileSize = file.FileSize,
                            Path = $@"{pathRoot}\{mediaPathType}",
                            ResponseTypeId = media.ResponseType.Id
                        };

                        list.Add(vm);
                    }
                }         
            }

            return list;
        }

        private PublicMediaViewModel LoadPublicMediaViewModel(
                int? mediaPathTypeId,
                int? responseTypeId)
        {
            var rules = new PublicMediaRules {OperationsClient = _opsClient};
            var vm = new PublicMediaViewModel
            {
                Title = PublicMediaSource.Description,
                AddButtonText = $"Upload {rules.GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Images,
                SelectedResponseType = responseTypeId ?? ResponseTypeId.SlidShow
            };

            return vm;
        }
    }
}