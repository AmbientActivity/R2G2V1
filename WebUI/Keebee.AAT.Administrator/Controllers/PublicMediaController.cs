using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using CuteWebUI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using Keebee.AAT.Administrator.Extensions;
using Keebee.AAT.BusinessRules;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PublicMediaController : Controller
    {
        private readonly OperationsClient _opsClient;

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();
        private readonly FileManager _fileManager = new FileManager();

        public PublicMediaController()
        {
            _opsClient = new OperationsClient();
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
                uploader.AllowedFileExtensions = GetAllowedExtensions(mediaPathTypeId);
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                vm.UploaderHtml = uploader.Render();

                // GET:
                if (string.IsNullOrEmpty(myuploader))
                    return View(vm);

                // POST:

                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var mediaPathType = GetMediaPathType(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}\{mediaPathType}\{file.FileName}";

                    // delete it if it already exists
                    _fileManager.DeleteFile(filePath);
                    file.MoveTo(filePath);

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
        public JsonResult DeleteFile(Guid streamId, int mediaPathTypeId)
        {
            var file = _opsClient.GetMediaFile(streamId);

            if (file != null)
            {
                var fileManager = new FileManager();
                fileManager.DeleteFile($@"{file.Path}\{file.Filename}");
            }

            return Json(new
            {
                FileList = GetMediaFiles(mediaPathTypeId)
            }, JsonRequestBehavior.AllowGet);
        }

        private void AddPublicMediaFile(string filename, int responseTypeId, int mediaPathTypeId, string mediaPathType)
        {
            var streamId = _fileManager.GetStreamId($@"{PublicMediaSource.Id}\{mediaPathType}", filename);

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
            var publicMediaFiles = _opsClient.GetPublicMediaFiles();

            if (publicMediaFiles == null) return list;
 
            var mediaPaths = publicMediaFiles.MediaFiles.SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var mediaPathType = mediaPaths
                .First(x => x.MediaPathType.Id == mediaPathTypeId)
                .MediaPathType.Description;

            var pathRoot = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}";

            foreach (var mediafile in publicMediaFiles.MediaFiles)
            {
                foreach (var path in mediafile.Paths)
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
                            ResponseTypeId = mediafile.ResponseType.Id
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
            var vm = new PublicMediaViewModel
            {
                Title = PublicMediaSource.Description,
                AddButtonText = $"Upload {GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Images,
                SelectedResponseType = responseTypeId ?? ResponseTypeId.SlidShow
            };

            return vm;
        }

        private string GetMediaPathType(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).Description
                : _opsClient.GetMediaPathType(MediaPathTypeId.Images).Description;
        }

        private static string GetAllowedExtensions(int? mediaPathTypeId)
        {
            if (mediaPathTypeId == null) return string.Empty;

            var extensions = string.Empty;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    extensions = "*.jpg,*.jpeg,*.png,*.gif";
                    break;
                case MediaPathTypeId.Videos:
                    extensions = "*.mp4";
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.Sounds:
                    extensions = "*.mp3";
                    break;
                case MediaPathTypeId.Shapes:
                    extensions = "*.png";
                    break;
            }

            return extensions;
        }

        private static bool IsValidFile(string filename, int? mediaPathTypeId)
        {
            if (mediaPathTypeId == null) return false;

            var isValid = false;
            var name = filename.ToLower();

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    isValid = name.Contains("jpg") || name.Contains("jpeg") || name.Contains("png") || name.Contains("gif");
                    break;
                case MediaPathTypeId.Videos:
                    isValid = name.Contains("mp4");
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.Sounds:
                    isValid = name.Contains("mp3");
                    break;
                case MediaPathTypeId.Shapes:
                    isValid = name.Contains("png");
                    break;
            }

            return isValid;
        }
    }
}