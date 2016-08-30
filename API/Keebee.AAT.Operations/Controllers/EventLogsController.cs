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
    [RoutePrefix("api/eventlogs")]
    public class EventLogsController : ApiController
    {
        private readonly IEventLogService _eventLogService;

        public EventLogsController(IEventLogService eventLogService)
        {
            _eventLogService = eventLogService;
        }

        // GET: api/EventLogs
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<EventLog> eventLogs = new Collection<EventLog>();

            await Task.Run(() =>
            {
                eventLogs = _eventLogService.Get()
                    .OrderByDescending(o => o.DateEntry); ;
            });

            if (eventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.EventLogs = eventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    EntryType = x.EventLogEntryType.Description,
                    x.ActivityType.PhidgetType,
                    ActivityType = x.ActivityType.Description,
                    ResponseTypeCategory = x.ResponseType.ResponseTypeCategory.Description,
                    ResponseType = x.ResponseType.Description,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/EventLogs/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var eventLog = new EventLog();

            await Task.Run(() =>
            {
                eventLog = _eventLogService.Get(id);
            });

            if (eventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{eventLog.DateEntry:D}";
            exObj.Time = $"{eventLog.DateEntry:T}";
            exObj.EventType = eventLog.EventLogEntryType.Description;
            exObj.Resident = (eventLog.Resident != null)
                ? $"{eventLog.Resident.FirstName} {eventLog.Resident.LastName}"
                : "N/A";
            exObj.ActivityType = eventLog.ActivityType.Description;
            exObj.PhidgetType = eventLog.ActivityType.PhidgetType;
            exObj.ResponseTypeCategory = eventLog.ResponseType.ResponseTypeCategory.Description;
            exObj.ResponseType = eventLog.ResponseType.Description;
            exObj.Description = eventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/EventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string date)
        {
            IEnumerable<EventLog> eventLogs = new Collection<EventLog>();

            await Task.Run(() =>
            {
                eventLogs = _eventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (eventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.EventLogs = eventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    EntryType = x.EventLogEntryType.Description,
                    x.ActivityType.PhidgetType,
                    ActivityType = x.ActivityType.Description,
                    ResponseTypeCategory = x.ResponseType.ResponseTypeCategory.Description,
                    ResponseType = x.ResponseType.Description,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/EventLogs
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var eventLog = serializer.Deserialize<EventLog>(value);
            _eventLogService.Post(eventLog);
        }

        // PATCH: api/EventLogs/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var eventLog = serializer.Deserialize<EventLog>(value);
            _eventLogService.Patch(id, eventLog);
        }

        // DELETE: api/EventLogs/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _eventLogService.Delete(id);
        }
    }
}
