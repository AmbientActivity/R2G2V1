using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Administrator.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ResidentsController : Controller
    {
        // api client
        private readonly IResidentsClient _residentsClient;
        private readonly IActiveResidentClient _activeResidentClient;

        private readonly CustomMessageQueue _messageQueueBluetoothBeaconWatcherReload;

        public ResidentsController()
        {
            _residentsClient = new ResidentsClient();
            _activeResidentClient = new ActiveResidentClient();

            // bluetooth beacon watcher reload message queue sender
            _messageQueueBluetoothBeaconWatcherReload = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BluetoothBeaconWatcherReload
            });
        }

        // GET: Resident
        [Authorize]
        public ActionResult Index(
            int? id, string 
            firstname, 
            string lastname, 
            string idsearch, 
            string sortcolumn,
            int? sortdescending)
        {
            return
                View(LoadResidentsViewModel(
                    id ?? 0,
                    idsearch, 
                    firstname, 
                    lastname, 
                    sortcolumn,
                    sortdescending));
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetData()
        {
            string errMsg = null;
            ResidentViewModel[] residentList = null;

            try
            {
                residentList = GetResidentList().ToArray();
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"Residents.GetData: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                EerrorMessage = errMsg,
                ResidentList = residentList,
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
        public JsonResult Validate(ResidentEditViewModel resident)
        {
            IEnumerable<string> validateMsgs = null;
            string errMsg = null;

            try
            {
                var rules = new ResidentRules();
                validateMsgs = rules.Validate(resident.FirstName, resident.LastName, resident.Gender, resident.Id == 0);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"Residents.Validate: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ValidationMessages = validateMsgs,
                ErrorMessage = errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Save(ResidentEditViewModel resident)
        {
            string errMsg;

            try
            {
                var residentId = resident.Id;
                var date = DateTime.Now;

                if (residentId > 0)
                {
                    resident.DateUpdated = date;
                    errMsg = UpdateResident(resident);
                }
                else
                {
                    resident.DateCreated = date;
                    resident.DateUpdated = date;
                    errMsg = AddResident(resident, out residentId);
                    resident.Id = residentId;
                }

                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                resident.LastName = resident.LastName ?? string.Empty;
                resident.ProfilePicture = ResidentRules.GetProfilePicture(resident.ProfilePicture);
                resident.ProfilePicturePlaceholder = ResidentRules.GetProfilePicturePlaceholder();

                // send the bluetooth beacon watcher service the updated resident
                if (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher))
                    _messageQueueBluetoothBeaconWatcherReload.Send(CreateMessageBodyFromResident(resident));
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"Residents.Save: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                ResidentList = new ResidentViewModel[] { resident }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id)
        {
            string errMsg;
            var deletedId = 0;

            try
            {
                var activeResident = _activeResidentClient.Get();
                if (activeResident.Resident.Id == id)
                {
                    errMsg = "The resident is currently engaging with R2G2 and cannot be deleted at this time.";
                }
                else
                {
                    var rules = new ResidentRules();

                    errMsg = rules.DeleteResident(id);
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    errMsg = rules.DeleteFolders(id);
                    if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                    deletedId = id;
                }

                if (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher))
                {
                    // send the bluetooth beacon watcher the deleted resident
                    _messageQueueBluetoothBeaconWatcherReload.Send(CreateMessageBodyFromResident(
                        new ResidentViewModel{ Id = id, FirstName = string.Empty }, isDeleted: true));
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                SystemEventLogger.WriteEntry($"Residents.Delete: {errMsg}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                DeletedId = deletedId
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public string UploadProfilePicture(HttpPostedFileBase file)
        {
            try
            {
                // convert to image and orient correctly
                var image = Image.FromStream(file.InputStream);
                var orientedImg = image.Orient();

                // convert back to stream
                var stream = new MemoryStream();
                orientedImg.Save(stream, ImageFormat.Jpeg);

                // convert to web image and resize
                var webImg = new WebImage(stream);

                var croppedImage = webImg.CustomCrop(1);
                webImg.Resize(96, 96, true, true).Crop(1, 1);

                return Convert.ToBase64String(croppedImage.GetBytes());

            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry(ex.Message, SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return null;
        }

        private static ResidentsViewModel LoadResidentsViewModel(
            int id, 
            string idsearch, 
            string firstname, 
            string lastname, 
            string sortcolumn, 
            int? sortdescending)
        {
            var vm = new ResidentsViewModel
            {
                SelectedId = (int)id,
                IdSearch = idsearch,
                FirstNameSearch = firstname,
                LastNameSearch = lastname,
                SortColumnName = sortcolumn,
                SortDescending = sortdescending,
                IsVideoCaptureServiceInstalled = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.VideoCapture) ? 1 : 0
            };

            return vm;
        }

        private ResidentEditViewModel LoadResidentEditViewModel(int id)
        {
            Resident resident = null;

            if (id > 0)
            {
                resident = _residentsClient.Get(id);
            }

            var vm = new ResidentEditViewModel
            {
                Id = resident?.Id ?? 0,
                FirstName = (resident != null) ? resident.FirstName : string.Empty,
                LastName = (resident != null) ? resident.LastName : string.Empty,
                Gender = resident?.Gender ?? string.Empty,
                GameDifficultyLevels = new SelectList(new Collection<SelectListItem> {
                    new SelectListItem { Value = "1", Text = "1" },
                    new SelectListItem { Value = "2", Text = "2" },
                    new SelectListItem { Value = "3", Text = "3" },
                    new SelectListItem { Value = "4", Text = "4" },
                    new SelectListItem { Value = "5", Text = "5" }},
                    "Value", "Text", resident?.GameDifficultyLevel),
                AllowVideoCapturing = resident?.AllowVideoCapturing ?? false,
                ProfilePicture = ResidentRules.GetProfilePicture(resident?.ProfilePicture),
                ProfilePicturePlaceholder = ResidentRules.GetProfilePicturePlaceholder(),
                IsVideoCaptureServiceInstalled = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.VideoCapture)
            };

            return vm;
        }

        private IEnumerable<ResidentViewModel> GetResidentList()
        {
            var residents = _residentsClient.Get();

            var list = residents
                .Select(resident => new ResidentViewModel
                {
                    Id = resident.Id,
                    FirstName = resident.FirstName,
                    LastName = resident.LastName ?? string.Empty,
                    Gender = resident.Gender,
                    GameDifficultyLevel = resident.GameDifficultyLevel,
                    AllowVideoCapturing = resident.AllowVideoCapturing,
                    ProfilePicture = ResidentRules.GetProfilePicture(resident?.ProfilePicture),
                    ProfilePicturePlaceholder = ResidentRules.GetProfilePicturePlaceholder(),
                    DateCreated = resident.DateCreated,
                    DateUpdated = resident.DateUpdated
                }).OrderBy(x => x.FirstName);

            return list;
        }

        private string UpdateResident(ResidentViewModel residentDetail)
        {
            var r = new Resident
            {
                FirstName = residentDetail.FirstName,
                LastName = !string.IsNullOrEmpty(residentDetail.LastName) ? residentDetail.LastName : null,
                Gender = residentDetail.Gender,
                GameDifficultyLevel = residentDetail.GameDifficultyLevel,
                AllowVideoCapturing = residentDetail.AllowVideoCapturing,
                ProfilePicture = residentDetail.ProfilePicture != null 
                    ? Convert.FromBase64String(residentDetail.ProfilePicture) 
                    : null,
                DateUpdated = residentDetail.DateUpdated
            };

            return _residentsClient.Patch(residentDetail.Id, r);
        }

        private string AddResident(ResidentViewModel residentDetail, out int residentId)
        {
            string errMsg = null;
            residentId = -1;

            try
            {
                errMsg = _residentsClient.Post(new Resident
                {
                    FirstName = residentDetail.FirstName,
                    LastName = !string.IsNullOrEmpty(residentDetail.LastName) ? residentDetail.LastName : null,
                    Gender = residentDetail.Gender,
                    GameDifficultyLevel = residentDetail.GameDifficultyLevel,
                    AllowVideoCapturing = residentDetail.AllowVideoCapturing,
                    ProfilePicture = residentDetail.ProfilePicture != null ? Convert.FromBase64String(residentDetail.ProfilePicture) : null,
                    DateCreated = residentDetail.DateCreated,
                    DateUpdated = residentDetail.DateUpdated
                }, out residentId);

                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);

                var rules = new ResidentRules();
                errMsg = rules.CreateFolders(residentId);
                if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return errMsg;
        }

        private static string CreateMessageBodyFromResident(ResidentViewModel resident, bool isDeleted = false)
        {
            var residentMessage = new ResidentBluetoothMessage
            {
                Id = resident.Id,
                Name = $"{resident.FirstName} {resident.LastName}".Trim(),
                IsDeleted = isDeleted
            };

            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(residentMessage);

            return messageBody;
        }
    }
}