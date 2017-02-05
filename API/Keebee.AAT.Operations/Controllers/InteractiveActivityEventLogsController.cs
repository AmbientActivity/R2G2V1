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
    [RoutePrefix("api/InteractiveActivityEventLogs")]
    public class InteractiveActivityEventLogsController : ApiController
    {
        private readonly IInteractiveActivityEventLogService _interactiveActivityEventLogService;

        public InteractiveActivityEventLogsController(IInteractiveActivityEventLogService interactiveActivityEventLogService)
        {
            _interactiveActivityEventLogService = interactiveActivityEventLogService;
        }

        // GET: api/InteractiveActivityEventLog
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<InteractiveActivityEventLog> interactiveActivityEventLog = new Collection<InteractiveActivityEventLog>();

            await Task.Run(() =>
            {
                interactiveActivityEventLog = _interactiveActivityEventLogService.Get()
                    .OrderByDescending(o => o.DateEntry);
            });

            if (interactiveActivityEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.InteractiveActivityEventLog = interactiveActivityEventLog
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = x.DateEntry.ToString("hh:mm:ss:ff tt"),
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    InteractiveActivityType = x.InteractiveActivityType.Description,
                    x.Difficultylevel,
                    Success = x.IsSuccess,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/InteractiveActivityEventLog/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var interactiveActivityEventLog = new InteractiveActivityEventLog();

            await Task.Run(() =>
            {
                interactiveActivityEventLog = _interactiveActivityEventLogService.Get(id);
            });

            if (interactiveActivityEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{interactiveActivityEventLog.DateEntry:D}";
            exObj.Time = $"{interactiveActivityEventLog.DateEntry:T}";
            exObj.Resident = (interactiveActivityEventLog.Resident != null)
                ? $"{interactiveActivityEventLog.Resident.FirstName} {interactiveActivityEventLog.Resident.LastName}"
                : "N/A";
            exObj.EventType = interactiveActivityEventLog.InteractiveActivityType.Description;
            exObj.DifficultyLevel = interactiveActivityEventLog.Difficultylevel;
            exObj.IsSuccess = interactiveActivityEventLog.IsSuccess;
            exObj.Description = interactiveActivityEventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/InteractiveActivityEventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string date)
        {
            IEnumerable<InteractiveActivityEventLog> interactiveActivityEventLogs = new Collection<InteractiveActivityEventLog>();

            await Task.Run(() =>
            {
                interactiveActivityEventLogs = _interactiveActivityEventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (interactiveActivityEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.InteractiveActivityEventLogs = interactiveActivityEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = x.DateEntry.ToString("hh:mm:ss:ff tt"),
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    InteractiveActivityType = x.InteractiveActivityType.Description,
                    x.Difficultylevel,
                    x.IsSuccess,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/InteractiveActivityEventLogs?residentId=6
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResident(int residentId)
        {
            IEnumerable<InteractiveActivityEventLog> interactiveActivityEventLogs = new Collection<InteractiveActivityEventLog>();

            await Task.Run(() =>
            {
                interactiveActivityEventLogs = _interactiveActivityEventLogService.GetForResident(residentId);
            });

            if (interactiveActivityEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.InteractiveActivityEventLogs = interactiveActivityEventLogs
                .Select(x => new
                {
                    x.Id,
                    x.Description
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/InteractiveActivityEventLog
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var interactiveActivityEventLog = serializer.Deserialize<InteractiveActivityEventLog>(value);
            return _interactiveActivityEventLogService.Post(interactiveActivityEventLog);
        }

        // PATCH: api/InteractiveActivityEventLog/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var interactiveActivityEventLog = serializer.Deserialize<InteractiveActivityEventLog>(value);
            _interactiveActivityEventLogService.Patch(id, interactiveActivityEventLog);
        }

        // DELETE: api/InteractiveActivityEventLog/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _interactiveActivityEventLogService.Delete(id);
        }
    }
}
