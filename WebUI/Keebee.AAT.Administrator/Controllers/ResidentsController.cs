using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.FileManagement;
using Keebee.AAT.Administrator.Extensions;
using Keebee.AAT.Shared;
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

        public ResidentsController()
        {
            _opsClient = new OperationsClient();
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
            string mediaType, 
            string myuploader, 
            int? sortdescending)
        {
            // first time loading
            if (mediaType == null)
                mediaType = MediaPath.Images.ToUppercaseFirst();

            var vm = LoadResidentMediaViewModel(id, rfid, firstname, lastname, mediaType, sortcolumn, sortdescending);

            using (var uploader = new CuteWebUI.MvcUploader(System.Web.HttpContext.Current))
            {
                uploader.UploadUrl = Response.ApplyAppPathModifier("~/UploadHandler.ashx");
                uploader.Name = "myuploader";
                uploader.AllowedFileExtensions = GetAllowedExtensions(mediaType);
                uploader.MultipleFilesUpload = true;
                uploader.InsertButtonID = "uploadbutton";
                vm.UploaderHtml = uploader.Render();

                // GET:
                if (string.IsNullOrEmpty(myuploader))
                    return View(vm);

                // POST:
                var fileManager = new FileManager();
                // for multiple files the value is string : guid/guid/guid 
                foreach (var strguid in myuploader.Split('/'))
                {
                    var fileguid = new Guid(strguid);
                    var file = uploader.GetUploadedFile(fileguid);
                    if (file?.FileName == null) continue;

                    if (!IsValidFile(file.FileName, mediaType)) continue;

                    var filePath = fileManager.GetFilePath(id, mediaType, file.FileName);
                    // delete it if it already exists
                    fileManager.DeleteFile(filePath);
                    file.MoveTo(filePath);
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
        public JsonResult GetDataMedia(int id, string mediaType)
        {
            var vm = new
            {
                FileList = GetFileList(id, mediaType),
                MediaTypeList = new Collection<string>
                {
                    MediaPath.Images.ToUppercaseFirst(),
                    MediaPath.Videos.ToUppercaseFirst(),
                    MediaPath.Music.ToUppercaseFirst(),
                    MediaPath.Pictures.ToUppercaseFirst(),
                    MediaPath.Shapes.ToUppercaseFirst(),
                    MediaPath.Sounds.ToUppercaseFirst()
                }
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
            var residentRules = new ResidentRules { OperationsClient = _opsClient };

            IEnumerable<string> msgs = residentRules.Validate(r.FirstName, r.LastName, r.Gender, residentId == 0);

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
                Success = (null == msgs),
                ErrorMessages = msgs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            _opsClient.DeleteResident(id);

            return Json(new
            {
                ResidentList = GetResidentList(),
                Success = true,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFile(Guid streamId, int residentId, string mediaType)
        {
            var file = _opsClient.GetMediaFile(streamId);

            if (file != null)
            {
                var fileManager = new FileManager();
                fileManager.DeleteFile($@"{file.Path}\{file.Filename}");
            }

            return Json(new
            {
                FileList = GetFileList(residentId, mediaType)
            }, JsonRequestBehavior.AllowGet);
        }

        private static ResidentsViewModel LoadResidentsViewModel(int id, List<string> msgs, bool success, 
            string rfid, string firstname, string lastname, string sortcolumn, int? sortdescending)
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

        private ResidentMediaViewModel LoadResidentMediaViewModel(int id, string rfid, string firstname, string lastname, string mediaType, string sortcolumn, int? sortdescending)
        {
            var resident = _opsClient.GetResident(id);
            var fullName = (resident.LastName.Length > 0)
                ? $"{resident.FirstName} {resident.LastName}"
                : resident.FirstName;

            var vm = new ResidentMediaViewModel
            {
                ResidentId = resident.Id,
                FullName = fullName,
                UploadButtonText = $"Upload {mediaType}",
                RfidSearch = rfid,
                FirstNameSearch = firstname,
                LastNameSearch = lastname,
                SortColumn = sortcolumn,
                SortDescending = sortdescending,
                SelectedMediaType = mediaType
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

            return id;
        }

        private IEnumerable<MediaFileViewModel> GetFileList(int id, string mediaType)
        {
            var list = new List<MediaFileViewModel>();
            var media = _opsClient.GetMediaFilesForPath($@"{id}\{mediaType}");

            if (media == null) return list;

            foreach (var m in media)
            {
                foreach (var file in m.Files.OrderBy(o => o.Filename))
                {
                    list.Add(new MediaFileViewModel
                    {
                        StreamId = file.StreamId,
                        IsFolder = file.IsFolder,
                        Filename = file.Filename.Replace($".{file.FileType}", string.Empty),
                        FileType = file.FileType.ToUpper(),
                        FileSize = file.FileSize,
                        Path = m.Path
                    });
                }
            }

            return list;
        }

        private static string GetAllowedExtensions(string mediaType)
        {
            var extensions = string.Empty;

            switch (mediaType.ToLower())
            {
                case MediaPath.Images:
                case MediaPath.Pictures:
                    extensions = "*.jpg,*.jpeg,*.png,*.gif";
                    break;
                case MediaPath.Videos:
                    extensions = "*.mp4";
                    break;
                case MediaPath.Music:
                case MediaPath.Sounds:
                    extensions = "*.mp3";
                    break;
                case MediaPath.Shapes:
                    extensions = "*.png";
                    break;
            }

            return extensions;
        }

        private static bool IsValidFile(string filename, string mediaType)
        {
            var isValid = false;
            var name = filename.ToLower();

            switch (mediaType.ToLower())
            {
                case MediaPath.Images:
                case MediaPath.Pictures:
                    isValid = name.Contains("jpg") || name.Contains("jpeg") || name.Contains("png") || name.Contains("gif");
                    break;
                case MediaPath.Videos:
                    isValid = name.Contains("mp4");
                    break;
                case MediaPath.Music:
                case MediaPath.Sounds:
                    isValid = name.Contains("mp3");
                    break;
                case MediaPath.Shapes:
                    isValid = name.Contains("png");
                    break;
            }

            return isValid;
        }
    }
}