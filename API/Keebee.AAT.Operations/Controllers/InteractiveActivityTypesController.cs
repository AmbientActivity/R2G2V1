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
    [RoutePrefix("api/InteractiveActivityTypes")]
    public class InteractiveActivityTypesController : ApiController
    {
        private readonly IInteractiveActivityTypeService _interactiveActivityTypeService;

        public InteractiveActivityTypesController(IInteractiveActivityTypeService interactiveActivityTypeService)
        {
            _interactiveActivityTypeService = interactiveActivityTypeService;
        }

        // GET: api/InteractiveActivityTypes
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<InteractiveActivityType> interactiveActivityTypes = new Collection<InteractiveActivityType>();

            await Task.Run(() =>
            {
                interactiveActivityTypes = _interactiveActivityTypeService.Get();
            });

            if (interactiveActivityTypes == null) return new DynamicJsonArray(new object[0]);

            var jArray = interactiveActivityTypes
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.SwfFile
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/InteractiveActivityTypes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var interactiveActivityType = new InteractiveActivityType();

            await Task.Run(() =>
            {
                interactiveActivityType = _interactiveActivityTypeService.Get(id);
            });

            if (interactiveActivityType == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = interactiveActivityType.Id;
            exObj.Description = interactiveActivityType.Description;
            exObj.SwfFile = interactiveActivityType.SwfFile;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/InteractiveActivityTypes
        [HttpPost]
        public int Post([FromBody]InteractiveActivityType interactiveActivityType)
        {
            return _interactiveActivityTypeService.Post(interactiveActivityType);
        }

        // PATCH: api/InteractiveActivityTypes/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]InteractiveActivityType interactiveActivityType)
        {
            _interactiveActivityTypeService.Patch(id, interactiveActivityType);
        }

        // DELETE: api/InteractiveActivityTypes/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _interactiveActivityTypeService.Delete(id);
        }
    }
}
