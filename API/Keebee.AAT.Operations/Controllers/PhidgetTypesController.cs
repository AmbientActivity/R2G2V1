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
    [RoutePrefix("api/PhidgetTypes")]
    public class PhidgetTypesController : ApiController
    {
        private readonly IPhidgetTypeService _phidgetTypeService;

        public PhidgetTypesController(IPhidgetTypeService phidgetTypeService)
        {
            _phidgetTypeService = phidgetTypeService;
        }

        // GET: api/PhidgetTypes
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<PhidgetType> phidgetTypes = new Collection<PhidgetType>();

            await Task.Run(() =>
            {
                phidgetTypes = _phidgetTypeService.Get();
            });

            if (phidgetTypes == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.PhidgetTypes = phidgetTypes
                .Select(x => new
                {
                    x.Id,
                    x.Description
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/PhidgetTypes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var phidgetType = new PhidgetType();

            await Task.Run(() =>
            {
                phidgetType = _phidgetTypeService.Get(id);
            });

            if (phidgetType == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = phidgetType.Id;
            exObj.Description = phidgetType.Description;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/PhidgetTypes
        [HttpPost]
        public void Post([FromBody]PhidgetType phidgetType)
        {
            _phidgetTypeService.Post(phidgetType);
        }

        // PATCH: api/PhidgetTypes/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]PhidgetType phidgetType)
        {
            _phidgetTypeService.Patch(id, phidgetType);
        }

        // DELETE: api/PhidgetTypes/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _phidgetTypeService.Delete(id);
        }
    }
}
