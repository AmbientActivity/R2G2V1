using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/ActiveResidents")]
    public class ActiveResidentsController : ApiController
    {
        private readonly IActiveResidentService _activeResidentService;

        public ActiveResidentsController(IActiveResidentService activeResidentService)
        {
            _activeResidentService = activeResidentService;
        }

        // GET: api/ActiveResidents
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ActiveResident> activeResidents = new Collection<ActiveResident>();

            await Task.Run(() =>
            {
                activeResidents = _activeResidentService.Get();
            });

            if (activeResidents == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ActiveResidents = activeResidents
                .Select(x => new
                {
                    x.Id,
                    ResidentId = x.Resident?.Id
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActiveResidents/1
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var activeResident = new ActiveResident();

            await Task.Run(() =>
            {
                activeResident = _activeResidentService.Get(id);
            });

            if (activeResident == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Id = activeResident.Id;
            exObj.Resident = new
            {
                Id = activeResident.Resident?.Id ?? 0,
                FirstName = activeResident.Resident != null
                    ? activeResident.Resident.FirstName : "Public",
                activeResident.Resident?.LastName,
            };
            return new DynamicJsonObject(exObj);
        }

        // POST: api/ActiveResidents
        [HttpPost]
        public int Post([FromBody]ActiveResident activeResident)
        {
            return _activeResidentService.Post(activeResident);
        }

        // PATCH: api/ActiveResidents/1
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]ActiveResident activeResident)
        {
            _activeResidentService.Patch(id, activeResident);
        }

        // DELETE: api/ActiveResidents/1
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _activeResidentService.Delete(id);
        }
    }
}
