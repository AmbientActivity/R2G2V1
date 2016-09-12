using Keebee.AAT.RESTClient;
using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.BusinessRules;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Keebee.AAT.Shared;
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
            return View();
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
            return PartialView("_ResidentEdit", LoadResidentEditViewModel(id));
        }

        [HttpPost]
        public JsonResult Save(string resident)
        {
            var r = JsonConvert.DeserializeObject<ResidentEditViewModel>(resident);
            IEnumerable<string> msgs;
            var residentId = r.Id;
            var residentRules = new ResidentRules { OperationsClient = _opsClient };

            if (residentId > 0)
            {
                msgs = residentRules.Validate(r.FirstName, r.LastName, r.Gender, false);
                if (msgs == null)
                    UpdateResident(r);
            }
            else
            {
                msgs = residentRules.Validate(r.FirstName, r.LastName, r.Gender, true);
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
            var profile = _opsClient.GetResidentProfile(id);
            if (profile.Id != ProfileId.Generic)
                _opsClient.DeleteProfile(profile.Id);

            _opsClient.DeleteResident(id);

            return Json(new
            {
                ResidentList = GetResidentList(),
                Success = true,
            }, JsonRequestBehavior.AllowGet);
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
                    "Value", "Text", resident?.Gender)
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
                    ProfileId = resident.Profile.Id > 0 ? resident.Profile.Id : 0,
                    FirstName = resident.FirstName,
                    LastName = resident.LastName,
                    Gender = resident.Gender,
                    HasProfile = resident.Profile.Id != ProfileId.Generic,
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
                Gender = residentDetail.Gender
            };

            _opsClient.PatchResident(residentDetail.Id, r);
        }

        private int AddResident(ResidentEditViewModel residentDetail)
        {
            var r = new ResidentEdit
            {
                FirstName = residentDetail.FirstName,
                LastName = residentDetail.LastName,
                Gender = residentDetail.Gender
            };

            var id = _opsClient.PostResident(r);

            return id;
        }
    }
}