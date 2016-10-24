using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.Extensions;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.SystemEventLogging;
using CuteWebUI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using System;
using System.IO;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public ResidentsController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
        }

        // GET: Residen
        [Authorize]
        public ActionResult Index(int? id, string rfid, string firstname, string lastname, string sortcolumn,
            int? sortdescending)
        {
            return
                View(LoadResidentsViewModel(id ?? 0, null, true, rfid, firstname, lastname, sortcolumn, sortdescending));
        }

        [Authorize]
        public ActionResult Media(
            int id, 
            string rfid, 
            string firstname, 
            string lastname, 
            string sortcolumn, 
            int? mediaPathTypeId,
            string myuploader, 
            int? sortdescending)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Images;

            var vm = LoadResidentMediaViewModel(id, rfid, firstname, lastname, mediaPathTypeId, sortcolumn, sortdescending);

            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = ResidentRules.GetAllowedExtensions(mediaPathTypeId);
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                vm.UploaderHtml = uploader.Render();

                // GET:
                if (string.IsNullOrEmpty(myuploader))
                    return View(vm);

                // POST:
                var fileManager = new FileManager { EventLogger = _systemEventLogger };
                var rules = new ResidentRules {OperationsClient = _opsClient};

                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!ResidentRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var mediaPathType = rules.GetMediaPathType(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{id}\{mediaPathType}\{file.FileName}";

                    // delete it if it already exists
                    var msg = fileManager.DeleteFile(filePath);

                    if (msg.Length == 0)
                    {
                        file.MoveTo(filePath);
                        AddResidentMediaFile(id, file.FileName, (int) mediaPathTypeId, mediaPathType);
                    }
                }
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            var vm = new
            {
                ResidentList = GetResidentList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetDataMedia(int id, int mediaPathTypeId)
        {
            var mediaPathTypes = _opsClient.GetMediaPathTypes().OrderBy(p => p.Description); ;
            var fileList = GetFiles(id);

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    Description = x.Description.ToUppercaseFirst()
                })
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetUploaderHtml(int mediaPathTypeId)
        {
            string html;
            using (var uploader = new MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = ResidentRules.GetAllowedExtensions(mediaPathTypeId); ;
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                html = uploader.Render();
            }

            var rules = new ResidentRules { OperationsClient = _opsClient };
            return Json(new
            {
                UploaderHtml = html,
                AddButtonText = $"Upload {rules.GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public PartialViewResult GetResidentEditView(int id)
        {
            return PartialView("_ResidentEdit", LoadResidentEditViewModel(id));
        }

        [HttpPost]
        [Authorize]
        public JsonResult Save(string resident)
        {
            var r = JsonConvert.DeserializeObject<ResidentEditViewModel>(resident);
            var residentId = r.Id;
            var rules = new ResidentRules { OperationsClient = _opsClient };

            IEnumerable<string> msgs = rules.Validate(r.FirstName, r.LastName, r.Gender, residentId == 0);

            if (residentId > 0)
            {
                if (msgs == null)
                    UpdateResident(r);
            }
            else
            {
                if (msgs == null)
                    residentId = AddResident(r);
            }

            return Json(new
            {
                ResidentList = GetResidentList(),
                SelectedId = residentId,
                Success = (null == msgs) && (residentId > 0),
                ErrorMessages = msgs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id)
        {
            string errormessage;
            bool success;

            try
            {
                var activeResident = _opsClient.GetActiveResident();
                if (activeResident.Resident.Id == id)
                {
                    errormessage = "The resident is currently engaging with R2G2 and cannot be deleted at this time.";
                }
                else
                {
                    var rules = new ResidentRules {OperationsClient = _opsClient};
                    errormessage = rules.DeleteResident(id);

                    if (errormessage.Length == 0)
                    {
                        var fileManager = new FileManager {EventLogger = _systemEventLogger};
                        fileManager.DeleteFolders(id);
                    }
                }
                success = (errormessage.Length == 0);
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
                ResidentList = GetResidentList(),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult DeleteSelectedMediaFiles(int[] ids, int residentId, int mediaPathTypeId)
        {
            bool success;
            var errormessage = string.Empty;

            try
            {
                var activeResident = _opsClient.GetActiveResident();
                if (activeResident.Resident.Id == residentId)
                {
                    errormessage = "The resident is currently engaging with R2G2. Media cannot be deleted at this time.";
                }
                else
                {
                    if (ids.Length > 0)
                    {
                        var rules = new ResidentRules {OperationsClient = _opsClient};
                        foreach (var id in ids)
                        {
                            var file = rules.GetMediaFile(id);
                            if (file == null) continue;

                            var fileManager = new FileManager {EventLogger = _systemEventLogger};
                            errormessage = fileManager.DeleteFile($@"{file.Path}\{file.Filename}");

                            if (errormessage.Length == 0)
                                rules.DeleteResidentMediaFile(id);
                            else
                                break;
                        }
                    }
                }
                success = (errormessage.Length == 0);
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
                FileList = GetFiles(residentId)
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
        public FileResult GetFileStream(string filePath, string fileType)
        {
            var info = new FileInfo(filePath);
            return File(info.OpenRead(), $"image/{info}");
        }

        private static ResidentsViewModel LoadResidentsViewModel(
            int id, 
            List<string> msgs, 
            bool success, 
            string rfid, 
            string firstname, 
            string lastname, 
            string sortcolumn, 
            int? sortdescending)
        {
            var vm = new ResidentsViewModel
            {
                SelectedId = (int)id,
                ErrorMessages = msgs,
                Success = success,

                RfidSearch = rfid,
                FirstNameSearch = firstname,
                LastNameSearch = lastname,
                SortColumnName = sortcolumn,
                SortDescending = sortdescending
            };

            return vm;
        }

        private ResidentEditViewModel LoadResidentEditViewModel(int id)
        {
            Resident resident = null;

            if (id > 0)
            {
                resident = _opsClient.GetResident(id);
            }
            var vm = new ResidentEditViewModel
            {
                Id = resident?.Id ?? 0,
                FirstName = (resident != null) ? resident.FirstName : string.Empty,
                LastName = (resident != null) ? resident.LastName : string.Empty,
                Genders = new SelectList( new Collection<SelectListItem> {
                    new SelectListItem { Value = "M", Text = "M" },
                    new SelectListItem { Value = "F", Text = "F" }},
                    "Value", "Text", resident?.Gender),
                GameDifficultyLevels = new SelectList(new Collection<SelectListItem> {
                    new SelectListItem { Value = "1", Text = "1" },
                    new SelectListItem { Value = "2", Text = "2" },
                    new SelectListItem { Value = "3", Text = "3" },
                    new SelectListItem { Value = "4", Text = "4" },
                    new SelectListItem { Value = "5", Text = "5" }},
                    "Value", "Text", resident?.GameDifficultyLevel),
                AllowVideoCapturing = resident?.AllowVideoCapturing ?? false,
            };

            return vm;
        }

        private ResidentMediaViewModel LoadResidentMediaViewModel(
            int id, 
            string rfid, 
            string firstname, 
            string lastname, 
            int? mediaPathTypeId,
            string sortcolumn, 
            int? sortdescending)
        {
            var resident = _opsClient.GetResident(id);
            var fullName = (resident.LastName.Length > 0)
                ? $"{resident.FirstName} {resident.LastName}"
                : resident.FirstName;
            var rules = new ResidentRules {OperationsClient = _opsClient};

            var vm = new ResidentMediaViewModel
            {
                ResidentId = resident.Id,
                FullName = fullName,
                AddButtonText = $"Upload {rules.GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
                RfidSearch = rfid,
                FirstNameSearch = firstname,
                LastNameSearch = lastname,
                SortColumn = sortcolumn,
                SortDescending = sortdescending,
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Images
            };

            return vm;
        }

        private IEnumerable<ResidentViewModel> GetResidentList()
        {
            var residents = _opsClient.GetResidents();

            var list = residents
                .Select(resident => new ResidentViewModel
                {
                    Id = resident.Id,
                    FirstName = resident.FirstName,
                    LastName = resident.LastName,
                    Gender = resident.Gender,
                    GameDifficultyLevel = resident.GameDifficultyLevel,
                    AllowVideoCapturing = resident.AllowVideoCapturing,
                    DateCreated = resident.DateCreated,
                    DateUpdated = resident.DateUpdated,
                }).OrderBy(x => x.Id);

            return list;
        }

        private void UpdateResident(ResidentEditViewModel residentDetail)
        {
            var r = new ResidentEdit
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender,
                GameDifficultyLevel = residentDetail.GameDifficultyLevel,
                AllowVideoCapturing = residentDetail.AllowVideoCapturing
            };

            _opsClient.PatchResident(residentDetail.Id, r);
        }

        private int AddResident(ResidentEditViewModel residentDetail)
        {
            var r = new ResidentEdit
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender,
                GameDifficultyLevel = residentDetail.GameDifficultyLevel,
                AllowVideoCapturing = residentDetail.AllowVideoCapturing
            };

            var id = _opsClient.PostResident(r);

            if (id <= 0) return id;

            var fileManager = new FileManager {EventLogger = _systemEventLogger};
            fileManager.CreateFolders(id);

            return id;
        }

        private void AddResidentMediaFile(int residentId, string filename, int mediaPathTypeId, string mediaPathType)
        {
            var fileManager = new FileManager { EventLogger = _systemEventLogger };
            var streamId = fileManager.GetStreamId($@"{residentId}\{mediaPathType}", filename);
            var responseTypeId = ResidentRules.GetResponseTypeId(mediaPathTypeId);

            var mf = new ResidentMediaFileEdit
            {
                StreamId = streamId,
                ResidentId = residentId,
                ResponseTypeId = responseTypeId,
                MediaPathTypeId = mediaPathTypeId
            };

            _opsClient.PostResidentMediaFile(mf);
        }

        private IEnumerable<MediaFileViewModel> GetFiles(int id)
        {
            var list = new List<MediaFileViewModel>();
            var residentMedia = _opsClient.GetResidentMediaFilesForResident(id);

            if (residentMedia == null) return list;

            var mediaPaths = residentMedia.MediaFiles.SelectMany(x => x.Paths)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var pathRoot = $@"{_mediaPath.ProfileRoot}\{id}";

            foreach (var path in mediaPaths)
            {
                var files = path.Files.OrderBy(f => f.Filename);

                foreach (var file in files)
                {
                    var item = new MediaFileViewModel
                    {
                        Id = file.Id,
                        StreamId = file.StreamId,
                        Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                        FileType = file.FileType.ToUpper(),
                        FileSize = file.FileSize,
                        Path = $@"{pathRoot}\{path.MediaPathType.Description}",
                        MediaPathTypeId = path.MediaPathType.Id,
                    };

                    list.Add(item);
                }
            }

            return list;
        }
    }
}