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

namespace Keebee.AAT.Administrator.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly OperationsClient _opsClient;
        private readonly SystemEventLogger _systemEventLogger;

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public ResidentsController()
        {
            _opsClient = new OperationsClient();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);
        }

        // GET: Resident
        public ActionResult Index(int? id, string rfid, string firstname, string lastname, string sortcolumn,
            int? sortdescending)
        {
            return
                View(LoadResidentsViewModel(id ?? 0, null, true, rfid, firstname, lastname, sortcolumn, sortdescending));
        }

        public ActionResult Media(
            int id, 
            string rfid, 
            string firstname, 
            string lastname, 
            string sortcolumn, 
            int? mediaPathTypeId,

            //TODO: for when 'choose from public library' gets implemented
            //int? mediaSourceTypeId,
            string myuploader, 
            int? sortdescending)
        {
            // first time loading
            if (mediaPathTypeId == null) mediaPathTypeId = MediaPathTypeId.Images;

            //TODO: for when 'choose from public library' gets implemented
            //if (mediaSourceTypeId == null) mediaSourceTypeId = MediaSourceTypeId.Personal;

            var vm = LoadResidentMediaViewModel(id, rfid, firstname, lastname, mediaPathTypeId,

                //TODO: for when 'choose from public library' gets implemented
                //mediaSourceTypeId, 
                sortcolumn, sortdescending);

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

                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!ResidentRules.IsValidFile(file.FileName, mediaPathTypeId)) continue;

                    var mediaPathType = GetMediaPathType(mediaPathTypeId);
                    var filePath = $@"{_mediaPath.ProfileRoot}\{id}\{mediaPathType}\{file.FileName}";

                    // delete it if it already exists
                    fileManager.DeleteFile(filePath);
                    file.MoveTo(filePath);

                    AddResidentMediaFile(id, file.FileName, (int)mediaPathTypeId, mediaPathType);   
                }
            }

            return View(vm);
        }

        [HttpGet]
        public JsonResult GetData()
        {
            var vm = new
            {
                ResidentList = GetResidentList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDataMedia(int id, int mediaPathTypeId)
        {
            var mediaPathTypes = _opsClient.GetMediaPathTypes();
            var fileList = GetMediaFiles(id, mediaPathTypeId);

            var vm = new
            {
                FileList = fileList,
                MediaPathTypeList = mediaPathTypes.Select(x => new
                {
                    x.Id,
                    Description = x.Description.ToUppercaseFirst()
                })//,
                //MediaSourceTypeList = new Collection<object>
                //{
                //    new { PublicMediaSource.Id, Description = MediaSourceType.Public },
                //    new { Id = 1, Description = MediaSourceType.Personal } 
                //}
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult GetResidentEditView(int id)
        {
            return PartialView("_ResidentEdit", LoadResidentEditViewModel(id));
        }

        [HttpPost]
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
        public JsonResult Delete(int id)
        {
            var rules = new ResidentRules {OperationsClient = _opsClient};
            var result = rules.DeleteResident(id);

            if (result.Length == 0)
            {
                var fileManager = new FileManager { EventLogger = _systemEventLogger };
                fileManager.DeleteFolders(id);
            }

            return Json(new
            {
                Success = (result.Length == 0),
                ErrorMessage = result,
                ResidentList = GetResidentList(),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteSelectedMediaFiles(Guid[] streamIds, int residentId, int mediaPathTypeId)
        {
            bool success;
            var errormessage = string.Empty;

            try
            {
                foreach (var id in streamIds)
                {
                    var file = _opsClient.GetMediaFile(id);

                    if (file == null) continue;

                    var fileManager = new FileManager { EventLogger = _systemEventLogger };
                    fileManager.DeleteFile($@"{file.Path}\{file.Filename}");
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
                FileList = GetMediaFiles(residentId, mediaPathTypeId)
            }, JsonRequestBehavior.AllowGet);
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
                    "Value", "Text", resident?.GameDifficultyLevel)
            };

            return vm;
        }

        private ResidentMediaViewModel LoadResidentMediaViewModel(
            int id, 
            string rfid, 
            string firstname, 
            string lastname, 
            int? mediaPathTypeId,

            //TODO: for when 'choose from public library' gets implemented
            //int? mediaSourceTypeId,
            string sortcolumn, 
            int? sortdescending)
        {
            var resident = _opsClient.GetResident(id);
            var fullName = (resident.LastName.Length > 0)
                ? $"{resident.FirstName} {resident.LastName}"
                : resident.FirstName;

            var vm = new ResidentMediaViewModel
            {
                ResidentId = resident.Id,
                FullName = fullName,
                AddButtonText = $"Upload {GetMediaPathType(mediaPathTypeId).ToUppercaseFirst()}",
                RfidSearch = rfid,
                FirstNameSearch = firstname,
                LastNameSearch = lastname,
                SortColumn = sortcolumn,
                SortDescending = sortdescending,
                SelectedMediaPathType = mediaPathTypeId ?? MediaPathTypeId.Images//,

                //TODO: for when 'choose from public library' gets implemented
                //SelectedMediaSourceType = mediaSourceTypeId ?? MediaSourceTypeId.Personal
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
                GameDifficultyLevel = residentDetail.GameDifficultyLevel
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
                GameDifficultyLevel = residentDetail.GameDifficultyLevel
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
            var responseTypeId = GetResponseTypeId(mediaPathTypeId);

            var mf = new ResidentMediaFileEdit
            {
                StreamId = streamId,
                ResidentId = residentId,
                ResponseTypeId = responseTypeId,
                MediaPathTypeId = mediaPathTypeId,
                IsPublic = false
            };

            _opsClient.PostResidentMediaFile(mf);
        }

        private IEnumerable<MediaFileViewModel> GetMediaFiles(int id, int mediaPathTypeId)
        {
            var list = new List<MediaFileViewModel>();
            var residentMedia = _opsClient.GetResidentMediaFilesForResident(id);

            if (residentMedia == null) return list;

            var mediaPaths = residentMedia.MediaFiles.SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            if (!mediaPaths.Any()) return list;

            var mediaPathType = mediaPaths
                .Single(x => x.MediaPathType.Id == mediaPathTypeId)
                .MediaPathType.Description;

            var pathRoot = $@"{_mediaPath.ProfileRoot}\{id}";

            list = mediaPaths
                .SelectMany(x => x.Files)
                .OrderBy(x => x.Filename)
                .Select(file => new MediaFileViewModel
                {
                    StreamId = file.StreamId,
                    Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                    FileType = file.FileType.ToUpper(),
                    FileSize = file.FileSize,
                    Path = $@"{pathRoot}\{mediaPathType}",
                    IsPublic = file.IsPublic
                }).ToList();

            return list;
        }

        private string GetMediaPathType(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _opsClient.GetMediaPathType((int)mediaPathTypeId).Description
                : _opsClient.GetMediaPathType(MediaPathTypeId.Images).Description;
        }

        private static int GetResponseTypeId(int mediaPathTypeId)
        {
            int responseTypeId = -1;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.Images:
                case MediaPathTypeId.Pictures:
                    responseTypeId = ResponseTypeId.SlidShow;
                    break;
                case MediaPathTypeId.Videos:
                    responseTypeId = ResponseTypeId.Television;
                    break;
                case MediaPathTypeId.Music:
                    responseTypeId = ResponseTypeId.Radio;
                    break;
                case MediaPathTypeId.Shapes:
                case MediaPathTypeId.Sounds:
                    responseTypeId = ResponseTypeId.MatchingGame;
                    break;
            }

            return responseTypeId;
        }
    }
}