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
        private readonly IConfigService _configService;

        public ActivityEventLogsController(IActivityEventLogService activityEventLogService, IConfigService configService)
        {
            _activityEventLogService = activityEventLogService;
            _configService = configService;
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

            var config = _configService.Get();

            dynamic exObj = new ExpandoObject();
            exObj.ActivityEventLogs = activityEventLogs
                .Join(config.SelectMany(x => x.ConfigDetails), 
                al => new { al.ConfigDetail.ConfigId, al.ConfigDetail.PhidgetTypeId },
                cd => new { cd.ConfigId, cd.PhidgetTypeId },
                (al, cd) => new { al, cd })
                .Select(x => new
                {
                    x.al.ConfigDetail.ConfigId,
                    Date = $"{x.al.DateEntry:D}",
                    Time = $"{x.al.DateEntry:T}",
                    Resident = (x.al.Resident != null) ? $"{x.al.Resident.FirstName} {x.al.Resident.LastName}" : "N/A",
                    PhidgetType = x.al.ConfigDetail.PhidgetType.Description,
                    ActivityType = x.cd.Description,
                    ResponseTypeCategory = x.al.ConfigDetail.ResponseType.ResponseTypeCategory.Description,
                    ResponseType = x.al.ConfigDetail.ResponseType.Description,
                    x.al.Description,
                    ResidentId = x.al.Resident?.Id ?? -1
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

            var description = _configService.GetDescription((int)activityEventLog.ConfigDetail.PhidgetTypeId);

            dynamic exObj = new ExpandoObject();
            exObj.ConfigId = activityEventLog.ConfigDetail.ConfigId;
            exObj.Date = $"{activityEventLog.DateEntry:D}";
            exObj.Time = $"{activityEventLog.DateEntry:T}";
            exObj.Resident = (activityEventLog.Resident != null)
                ? $"{activityEventLog.Resident.FirstName} {activityEventLog.Resident.LastName}"
                : "N/A";
            exObj.PhidgetType = activityEventLog.ConfigDetail.PhidgetType.Description;
            exObj.ActivityType = description;
            exObj.ResponseTypeCategory = activityEventLog.ConfigDetail.ResponseType.ResponseTypeCategory.Description;
            exObj.ResponseType = activityEventLog.ConfigDetail.ResponseType.Description;
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

            var config = _configService.GetActiveDetails();

            dynamic exObj = new ExpandoObject();
            exObj.ActivityEventLogs = activityEventLogs
                .Join(config.ConfigDetails, al => al.ConfigDetail.PhidgetTypeId, cd => cd.PhidgetTypeId,
                (al, cd) => new { al, cd })
                .Select(x => new
                {
                    x.al.ConfigDetail.ConfigId,
                    Date = $"{x.al.DateEntry:D}",
                    Time = $"{x.al.DateEntry:T}",
                    Resident = (x.al.Resident != null) ? $"{x.al.Resident.FirstName} {x.al.Resident.LastName}" : "N/A",
                    PhidgetType = x.al.ConfigDetail.PhidgetType.Description,
                    ActivityType = x.cd.Description,
                    ResponseTypeCategory = x.al.ConfigDetail.ResponseType.ResponseTypeCategory.Description,
                    ResponseType = x.al.ConfigDetail.ResponseType.Description,
                    x.al.Description,
                    ResidentId = x.al.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActivityEventLogs?configId=1
        [HttpGet]
        public async Task<DynamicJsonObject> GetForConfig(int configId)
        {
            IEnumerable<ActivityEventLog> activityEventLogs = new Collection<ActivityEventLog>();

            await Task.Run(() =>
            {
                activityEventLogs = _activityEventLogService.GetForConfig(configId);
            });

            if (activityEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ActivityEventLogs = activityEventLogs
                .Select(x => new
                {
                    x.ConfigDetail.ConfigId,
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    x.ConfigDetail.PhidgetTypeId,
                    x.ConfigDetail.ResponseTypeId,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActivityEventLogs?configDetailId
        [HttpGet]
        public async Task<DynamicJsonObject> GetForConfigPhidgetResponseType(int configDetailId)
        {
            IEnumerable<ActivityEventLog> activityEventLogs = new Collection<ActivityEventLog>();

            await Task.Run(() =>
            {
                activityEventLogs = _activityEventLogService.GetForConfigDetail(configDetailId);
            });

            if (activityEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.ActivityEventLogs = activityEventLogs
                .Select(x => new
                {
                    x.ConfigDetail.ConfigId,
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    x.ConfigDetail.PhidgetTypeId,
                    x.ConfigDetail.ResponseTypeId,
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

        // DELETE: api/ActivityEventLogs?residentId=5
        [HttpDelete]
        [Route("{id}")]
        public void DeleteForResident(int residentId)
        {
            _activityEventLogService.Delete(residentId);
        }
    }
}
