using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Controllers
{
    [RoutePrefix("api/ActivityTypes")]
    public class ActivityTypesController : ApiController
    {
        private readonly IActivityTypeService _activityTypeService;

        public ActivityTypesController(IActivityTypeService activityTypeService)
        {
            _activityTypeService = activityTypeService;
        }

        // GET: api/ActivityTypes
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ActivityType> activityTypes = new Collection<ActivityType>();

            await Task.Run(() =>
            {
                activityTypes = _activityTypeService.Get();
            });

            if (activityTypes == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ActivityTypes = activityTypes
                .Select(x => new
                {
                    x.Id,
                    x.PhidgetType
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActivityTypes/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var activityType = new ActivityType();

            await Task.Run(() =>
            {
                activityType = _activityTypeService.Get(id);
            });

            if (activityType == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();

            exObj.Id = activityType.Id;
            exObj.PhidgetType = activityType.PhidgetType;

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ActivityTypes
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var activityType = serializer.Deserialize<ActivityType>(value);
            _activityTypeService.Post(activityType);
        }

        // PATCH: api/ActivityTypes/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var activityType = serializer.Deserialize<ActivityType>(value);
            _activityTypeService.Patch(id, activityType);
        }

        // DELETE: api/ActivityTypes/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _activityTypeService.Delete(id);
        }
    }
}
