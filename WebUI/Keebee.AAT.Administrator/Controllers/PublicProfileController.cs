using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.ThumbnailGeneration;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Web;

namespace Keebee.AAT.Administrator.Controllers
{
    public class PublicProfileController : Controller
    {
        // api client
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IThumbnailsClient _thumbnailsClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public PublicProfileController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
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
            var fileList = new MediaFileViewModel[0];
            var mediaPathTypeList = new MediaPathType[0];

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
                        Category = x.Category,
                        Description = x.Description,
                        ShortDescription = x.ShortDescription,
                        Path = x.Path,
                        AllowedExts = x.AllowedExts.Replace(" ", string.Empty),
                        AllowedTypes = x.AllowedTypes.Replace(" ", string.Empty)
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
        public JsonResult AddFiles(string[] filenames, string mediaPath, int mediaPathTypeId)
        {
            string errMsg = null;

            try
            {
                foreach (var filename in filenames)
                {
                    errMsg = AddPublicMediaFile(filename, mediaPathTypeId, mediaPath);
                    if (!string.IsNullOrEmpty(errMsg))
                        throw new Exception(errMsg);
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
                FileList = GetFiles()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult UploadFile(HttpPostedFileBase file, string mediaPath, int mediaPathTypeId, string mediaPathTypeCategory)
        {
            string errMsg = null;
            string filename = null;

            try
            {
                if (file != null)
                {
                    filename = file.FileName;
                    var filePath = $@"{_mediaSourcePath.ProfileRoot}\{PublicProfileSource.Id}\{mediaPath}\{file.FileName}";

                    // delete it if it already exists
                    var fileManager = new FileManager {EventLogger = _systemEventLogger};
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
                Filename = filename,
                MediaPath = mediaPath,
                MediaPathTypeId = mediaPathTypeId,
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int mediaPathTypeId)
        {
            string errMsg;

            try
            {
                var rules = new PublicProfileRules();

                errMsg = rules.CanDeleteMultiple(ids.Length, mediaPathTypeId);
                if (errMsg.Length > 0)
                    throw new Exception(errMsg);

                foreach (var id in ids)
                {
                    var publicMediaFile = _publicMediaFilesClient.Get(id);
                    if (publicMediaFile == null) continue;

                    if (publicMediaFile.MediaFile.IsLinked)
                    {
                        _publicMediaFilesClient.Delete(id);
                    }
                    else
                    {
                        var file = rules.GetMediaFile(id);
                        if (file == null) continue;

                        // delete the link
                        errMsg = _publicMediaFilesClient.Delete(id);
                        if (errMsg.Length > 0)
                            throw new Exception(errMsg);

                        var fileManager = new FileManager { EventLogger = _systemEventLogger };
                        fileManager.DeleteFile($@"{file.Path}\{file.Filename}");

                        if (SharedLibraryRules.IsMediaTypeThumbnail(mediaPathTypeId))
                        {
                            errMsg = _thumbnailsClient.Delete(publicMediaFile.MediaFile.StreamId);
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
                FileList = GetFiles()
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
        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int mediaPathTypeId)
        {
            string errMsg = null;
            var newIds = new Collection<int>();

            try
            {
                if (streamIds != null)
                {
                    foreach (var streamId in streamIds)
                    {
                        var pmf = new PublicMediaFileEdit
                        {
                            StreamId = streamId,
                            ResponseTypeId = PublicProfileRules.GetResponseTypeId(mediaPathTypeId),
                            MediaPathTypeId = mediaPathTypeId,
                            IsLinked = true
                        };

                        int newId;
                        errMsg = _publicMediaFilesClient.Post(pmf, out newId);
                        if (errMsg != null) throw new Exception(errMsg);

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
                ErrorMessage =  errMsg,
                FileList = GetFiles(),
                NewIds = newIds
            }, JsonRequestBehavior.AllowGet);
        }

        private string AddPublicMediaFile(string filename, int mediaPathTypeId, string mediaPathType)
        {
            string errMsg = null;
            var fileManager = new FileManager { EventLogger = _systemEventLogger };

            var streamId = fileManager.GetStreamId($@"{PublicProfileSource.Id}\{mediaPathType}", filename);
            if (streamId == new Guid()) return null;

            var mf = new PublicMediaFileEdit
            {
                StreamId = streamId,
                ResponseTypeId = PublicProfileRules.GetResponseTypeId(mediaPathTypeId),
                MediaPathTypeId = mediaPathTypeId,
                IsLinked = false
            };

            int newId;
            errMsg = _publicMediaFilesClient.Post(mf, out newId);

            if (errMsg != null) return errMsg;

            if (!PublicProfileRules.IsMediaTypeThumbnail(mediaPathTypeId)) return null;
            var thumbnailGenerator = new ThumbnailGenerator();
            errMsg = thumbnailGenerator.Generate(streamId);
            return errMsg;
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