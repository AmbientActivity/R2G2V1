using System.Collections;
using Keebee.AAT.Administrator.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Keebee.AAT.RESTClient;
using ProfileDetail = Keebee.AAT.Administrator.ViewModels.ProfileDetail;

namespace Keebee.AAT.Administrator.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly OperationsClient _opsClient;

        public ProfilesController()
        {
            _opsClient = new OperationsClient();
        }

        // GET: Profile
        public ActionResult Index()
        {
            return View(LoadProfileViewModel(-1, null));
        }

        [HttpGet]
        public JsonResult GetData()
        {
            var vm = new
            {
                ProfileList = GetProfileList(),
                ResidentArrayList = GetResidentArrayList()
            };

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ProfileDetail> GetProfileList()
        {
            var profiles = _opsClient.GetProfiles();

            var list = profiles
                .Select(profile => new ProfileDetail
                                   {
                                       Id = profile.Id,
                                       Description = profile.Description,
                                       ResidentId = profile.ResidentId,
                                       GameDifficultyLevel = profile.GameDifficultyLevel,
                                       DateCreated = profile.DateCreated,
                                       DateUpdated = profile.DateUpdated,
                }).OrderBy(x => x.Description);

            return list;
        }

        public ArrayList GetResidentArrayList()
        {
            var residents = _opsClient.GetResidents();

            var al = new ArrayList { new { Value = 0, Display = "N/A" } };
            foreach (var r in residents)
                al.Add(new { Value = r.Id, Display = $"{r.LastName}, {r.FirstName}" });

            return al;
        }

        private static ProfileViewModel LoadProfileViewModel(int selectedId, List<string> msgs)
        {
            var model = new ProfileViewModel
            {
                SelectedId = selectedId,
                Success = (msgs == null),
                ErrorMessages = msgs
            };

            return model;
        }
    }
}