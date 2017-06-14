using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Shared;
using Keebee.AAT.Administrator.FileManagement;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ResidentsController : Controller
    {
        // api client
        private readonly IResidentsClient _residentsClient;
        private readonly IActiveResidentClient _activeResidentClient;

        private readonly SystemEventLogger _systemEventLogger;
        private readonly CustomMessageQueue _messageQueueBluetoothBeaconWatcherReload;

        public ResidentsController()
        {
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.AdminInterface);

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
        public ActionResult Index(int? id, string idsearch, string firstname, string lastname, string sortcolumn,
            int? sortdescending)
        {
            return
                View(LoadResidentsViewModel(id ?? 0, null, true, idsearch, firstname, lastname, sortcolumn, sortdescending));
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
            var rules = new ResidentRules();
            IEnumerable<ResidentViewModel> residentList = new Collection<ResidentViewModel>();

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

            residentList = GetResidentList();

            var success = (null == msgs) && (residentId > 0);
            if (success)
            {
                if (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher))
                {
                    // alert the bluetooth beacon watcher to reload its residents
                    _messageQueueBluetoothBeaconWatcherReload.Send(CreateMessageBodyFromResidents(residentList));
                }
            }

            return Json(new
            {
                ResidentList = residentList,
                SelectedId = residentId,
                Success = success,
                ErrorMessages = msgs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public JsonResult Delete(int id)
        {
            string errormessage;
            bool success;
            IEnumerable<ResidentViewModel> residentList = new Collection<ResidentViewModel>();
            
            try
            {
                var activeResident = _activeResidentClient.Get();
                if (activeResident.Resident.Id == id)
                {
                    errormessage = "The resident is currently engaging with R2G2 and cannot be deleted at this time.";
                }
                else
                {
                    var rules = new ResidentRules();
                    errormessage = rules.DeleteResident(id);

                    if (errormessage.Length == 0)
                    {
                        var fileManager = new FileManager {EventLogger = _systemEventLogger};
                        fileManager.DeleteFolders(id);
                    }
                }

                residentList = GetResidentList();

                success = (errormessage.Length == 0);
                if (success)
                {
                    if (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher))
                    {
                        // alert the bluetooth beacon watcher to reload its residents
                        _messageQueueBluetoothBeaconWatcherReload.Send(CreateMessageBodyFromResidents(residentList));
                    }
                }
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
                ResidentList = residentList,
            }, JsonRequestBehavior.AllowGet);
        }

        private static ResidentsViewModel LoadResidentsViewModel(
            int id, 
            List<string> msgs, 
            bool success, 
            string idsearch, 
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

                IdSearch = idsearch,
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
                resident = _residentsClient.Get(id);
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

        private IEnumerable<ResidentViewModel> GetResidentList()
        {
            var residents = _residentsClient.Get();

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

        private void UpdateResident(ResidentViewModel residentDetail)
        {
            var r = new ResidentEdit
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender,
                GameDifficultyLevel = residentDetail.GameDifficultyLevel,
                AllowVideoCapturing = residentDetail.AllowVideoCapturing
            };

            _residentsClient.Patch(residentDetail.Id, r);
        }

        private int AddResident(ResidentViewModel residentDetail)
        {
            var r = new ResidentEdit
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender,
                GameDifficultyLevel = residentDetail.GameDifficultyLevel,
                AllowVideoCapturing = residentDetail.AllowVideoCapturing
            };

            var id = _residentsClient.Post(r);

            if (id <= 0) return id;

            var fileManager = new FileManager {EventLogger = _systemEventLogger};
            fileManager.CreateFolders(id);

            return id;
        }

        private static string CreateMessageBodyFromResidents(IEnumerable<ResidentViewModel> residents)
        {
            var residentMessages = residents.Select(r => new ResidentMessage
            {
                Id = r.Id,
                Name = $"{r.FirstName} {r.LastName}".Trim(),
                GameDifficultyLevel = r.GameDifficultyLevel,
                AllowVideoCapturing = r.AllowVideoCapturing
            }).ToArray();

            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(residentMessages);

            return messageBody;
        }
    }
}