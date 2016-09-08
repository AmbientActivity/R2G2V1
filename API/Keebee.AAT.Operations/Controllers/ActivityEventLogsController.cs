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
    [RoutePrefix("api/ActivityEventLogs")]
    public class ActivityEventLogsController : ApiController
    {
        private readonly IActivityEventLogService _activityEventLogService;

        public ActivityEventLogsController(IActivityEventLogService activityEventLogService)
        {
            _activityEventLogService = activityEventLogService;
        }

        // GET: api/ActivityEventLogs
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<ActivityEventLog> activityEventLogs = new Collection<ActivityEventLog>();

            await Task.Run(() =>
            {
                activityEventLogs = _activityEventLogService.Get()
                    .OrderByDescending(o => o.DateEntry);
            });

            if (activityEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ActivityEventLogs = activityEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    x.ActivityType.PhidgetType,
                    ActivityType = x.ActivityType.Description,
                    ResponseTypeCategory = x.ResponseType.ResponseTypeCategory.Description,
                    ResponseType = x.ResponseType.Description,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActivityEventLogs/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var activityEventLog = new ActivityEventLog();

            await Task.Run(() =>
            {
                activityEventLog = _activityEventLogService.Get(id);
            });

            if (activityEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{activityEventLog.DateEntry:D}";
            exObj.Time = $"{activityEventLog.DateEntry:T}";
            exObj.Resident = (activityEventLog.Resident != null)
                ? $"{activityEventLog.Resident.FirstName} {activityEventLog.Resident.LastName}"
                : "N/A";
            exObj.ActivityType = activityEventLog.ActivityType.Description;
            exObj.PhidgetType = activityEventLog.ActivityType.PhidgetType;
            exObj.ResponseTypeCategory = activityEventLog.ResponseType.ResponseTypeCategory.Description;
            exObj.ResponseType = activityEventLog.ResponseType.Description;
            exObj.Description = activityEventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActivityEventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string date)
        {
            IEnumerable<ActivityEventLog> activityEventLogs = new Collection<ActivityEventLog>();

            await Task.Run(() =>
            {
                activityEventLogs = _activityEventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (activityEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ActivityEventLogs = activityEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    x.ActivityType.PhidgetType,
                    ActivityType = x.ActivityType.Description,
                    ResponseTypeCategory = x.ResponseType.ResponseTypeCategory.Description,
                    ResponseType = x.ResponseType.Description,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/ActivityEventLogs
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var activityEventLog = serializer.Deserialize<ActivityEventLog>(value);
            _activityEventLogService.Post(activityEventLog);
        }

        // PATCH: api/ActivityEventLogs/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var activityEventLog = serializer.Deserialize<ActivityEventLog>(value);
            _activityEventLogService.Patch(id, activityEventLog);
        }

        // DELETE: api/ActivityEventLogs/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _activityEventLogService.Delete(id);
        }
    }
}
