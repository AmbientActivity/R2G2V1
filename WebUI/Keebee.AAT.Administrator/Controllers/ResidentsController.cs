using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

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
        public ActionResult Index()
        {
            return View(LoadResidentViewModel(-1, null));
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
        public PartialViewResult GetResidentEditView(int id)
        {
            return PartialView("_ResidentEdit", LoadMemberEditViewModel(id));
        }

        [HttpPost]
        public JsonResult Save(string resident)
        {
            var r = JsonConvert.DeserializeObject<ResidentDetail>(resident);
            IEnumerable<string> msgs = null;
            var residentId = r.Id;

            if (residentId > 0)
            {
                //msgs = ValidateBand(b.Name, false);
                //if (msgs == null)
                    UpdateResident(r);
            }
            else
            {
                //msgs = ValidateBand(b.Name, true);
                //if (msgs == null)
                    AddResident(r);
            }

            return Json(new
            {
                ResidentList = GetResidentList(),
                SelectedId = residentId,
                Success = (null == msgs),
                ErrorMessages = msgs
            }, JsonRequestBehavior.AllowGet);
        }

        private ResidentEditViewModel LoadMemberEditViewModel(int id)
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
                    "Value", "Text", resident?.Gender)
            };

            return vm;
        }

        private IEnumerable<ResidentDetail> GetResidentList()
        {
            var residents = _opsClient.GetResidents();

            var list = residents
                .Select(resident => new ResidentDetail
                {
                    Id = resident.Id,
                    ProfileId = resident.Profile?.Id > 0 ? resident.Profile?.Id : 0,
                    FirstName = resident.FirstName,
                    LastName = resident.LastName,
                    Gender = resident.Gender,
                    DateCreated = resident.DateCreated,
                    DateUpdated = resident.DateUpdated,
                }).OrderBy(x => x.Id);

            return list;
        }

        private static ResidentViewModel LoadResidentViewModel(int selectedId, List<string> msgs)
        {
            var model = new ResidentViewModel
            {
                SelectedId = selectedId,
                Success = (msgs == null),
                ErrorMessages = msgs
            };

            return model;
        }

        private void UpdateResident(ResidentDetail residentDetail)
        {
            var r = new ResidentUpdate
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender
            };

            _opsClient.PatchResident(residentDetail.Id, r);
        }

        private void AddResident(ResidentDetail residentDetail)
        {
            var r = new Resident
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender
            };

            _opsClient.PostResident(r);
            //if (id <= 0) return id;

            //return id;
        }

        private IEnumerable<string> ValidateResident(string firstname, string lastname, bool addNew)
        {
            //return _validationRules.ValidateBand(name, addNew);
            return null;
        }
    }
}