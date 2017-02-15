using Keebee.AAT.ApiClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.Shared;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.SystemEventLogging;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.IO;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SystemProfileController : Controller
    {
        private readonly OperationsClient _opsClient;

        private readonly MediaSourcePath _mediaSourcePath = new MediaSourcePath();

        public SystemProfileController()
        {
            var systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
            _opsClient = new OperationsClient { SystemEventLogger = systemEventLogger };
        }

        // GET: SystemProfile
        [Authorize]
        public ActionResult Index()
        {
            return View(LoadPublicProfileViewModel());
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            var mediaPathTypes = _opsClient.GetMediaPathTypes()
                .Where(x => x.IsSystem)
                .OrderBy(p => p.Description);
            var fileList = GetMediaFiles();

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.ShortDescription,
                    x.IsPreviewable
                })
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelected(int[] ids, int mediaPathTypeId)
        {
            bool success;
            string errormessage;

            try
            {
                var rules = new PublicProfileRules { OperationsClient = _opsClient };

                errormessage = rules.CanDeleteMultiple(ids.Length, mediaPathTypeId);
                if (errormessage.Length > 0)
                    throw new Exception(errormessage);

                foreach (var id in ids)
                {
                    var publicMediaFile = _opsClient.GetPublicMediaFile(id);
                    if (publicMediaFile == null) continue;

                    if (publicMediaFile.MediaFile.IsLinked)
                    {
                        rules.DeletePublicMediaFile(id);
                    }
                    else
                    {
                        var file = rules.GetMediaFile(id);
                        if (file == null) continue;

                        // delete the link
                        errormessage = rules.DeletePublicMediaFile(id);
                        if (errormessage.Length > 0)
                            throw new Exception(errormessage);
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
                FileList = GetMediaFiles()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetImageViewerView(Guid streamId, string fileType)
        {
            var rules = new ImageViewerRules { OperationsClient = _opsClient };
            var m = rules.GetImageViewerModel(streamId, fileType);

            return PartialView("_ImageViewer", new ImageViewerViewModel
            {
                FilePath = m.FilePath,
                FileType = m.FileType,
                Width = m.Width,
                Height = m.Height,
                PaddingLeft = m.PaddingLeft
            });
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetSharedLibarayLinkView(int mediaPathTypeId)
        {
            var vm = LoadSharedLibaryAddViewModel(mediaPathTypeId);

            return vm.SharedFiles.Any()
                ? PartialView("_SharedLibraryLink", vm)
                : null;
        }

        [HttpGet]
        [Authorize]
        public FileResult GetFileStream(string filePath, string fileType)
        {
            var info = new FileInfo(filePath);
            return File(info.OpenRead(), $"image/{info}");
        }

        public JsonResult AddSharedMediaFiles(Guid[] streamIds, int mediaPathTypeId)
        {
            bool success;
            var errormessage = string.Empty;

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

                        _opsClient.PostPublicMediaFile(pmf);
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
                FileList = GetMediaFiles()
            }, JsonRequestBehavior.AllowGet);
        }

        private static PublicProfileViewModel LoadPublicProfileViewModel()
        {
            var vm = new PublicProfileViewModel
            {
                Title = PublicProfileSource.DescriptionSystem,
                AddButtonText = "Add"
            };

            return vm;
        }

        private SharedLibraryLinkViewModel LoadSharedLibaryAddViewModel(int mediaPathTypeId)
        {
            var rules = new PublicProfileRules { OperationsClient = _opsClient };
            var files = rules.GetAvailableSharedMediaFiles(mediaPathTypeId);

            var vm = new SharedLibraryLinkViewModel
            {
                SharedFiles = files
                .Select(f => new SharedLibraryFileViewModel
                {
                    StreamId = f.StreamId,
                    Filename = f.Filename
                })
            };

            return vm;
        }

        private IEnumerable<PublicMediaFileViewModel> GetMediaFiles()
        {
            var list = new List<PublicMediaFileViewModel>();
            var publicMedia = _opsClient.GetPublicMediaFiles(isSystem: true);

            if (publicMedia == null) return list;

            var mediaPaths = publicMedia.MediaFiles.SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.IsSystem)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = _mediaSourcePath.MediaRoot;

            foreach (var media in publicMedia.MediaFiles)
            {
                foreach (var path in media.Paths.Where(x => x.MediaPathType.IsSystem))
                {
                    var files = path.Files.OrderBy(f => f.Filename);

                    foreach (var file in files)
                    {
                        var vm = new PublicMediaFileViewModel
                        {
                            Id = file.Id,
                            StreamId = file.StreamId,
                            Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                            FileType = file.FileType.ToUpper(),
                            FileSize = file.FileSize,
                            Path = $@"{pathRoot}\{path.MediaPathType.Path}",
                            IsLinked = file.IsLinked,
                            MediaPathTypeId = path.MediaPathType.Id
                        };

                        list.Add(vm);
                    }
                }
            }

            return list;
        }
    }
}