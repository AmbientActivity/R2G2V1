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
    [RoutePrefix("api/ActiveResidentEventLogs")]
    public class ActiveResidentEventLogsController : ApiController
    {
        private readonly IActiveResidentEventLogService _activeResidentEventLogService;

        public ActiveResidentEventLogsController(IActiveResidentEventLogService activeResidentEventLogService)
        {
            _activeResidentEventLogService = activeResidentEventLogService;
        }

        // GET: api/ActiveResidentEventLogs
        [HttpGet]
        public async Task<DynamicJsonArray> Get()
        {
            IEnumerable<ActiveResidentEventLog> activeResidentEventLogs = new Collection<ActiveResidentEventLog>();

            await Task.Run(() =>
            {
                activeResidentEventLogs = _activeResidentEventLogService.Get()
                    .OrderByDescending(o => o.DateEntry);
            });

            if (activeResidentEventLogs == null) return new DynamicJsonArray(new DynamicJsonArray(new object[0]));

            var jArray = activeResidentEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/ActiveResidentEventLogs/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var activeResidentEventLog = new ActiveResidentEventLog();

            await Task.Run(() =>
            {
                activeResidentEventLog = _activeResidentEventLogService.Get(id);
            });

            if (activeResidentEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{activeResidentEventLog.DateEntry:D}";
            exObj.Time = $"{activeResidentEventLog.DateEntry:T}";
            exObj.Resident = (activeResidentEventLog.Resident != null)
                ? $"{activeResidentEventLog.Resident.FirstName} {activeResidentEventLog.Resident.LastName}"
                : "N/A";
            exObj.Description = activeResidentEventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/ActiveResidentEventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonArray> Get(string date)
        {
            IEnumerable<ActiveResidentEventLog> activeResidentEventLogs = new Collection<ActiveResidentEventLog>();

            await Task.Run(() =>
            {
                activeResidentEventLogs = _activeResidentEventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (activeResidentEventLogs == null) return new DynamicJsonArray(new DynamicJsonArray(new object[0]));

            var jArray = activeResidentEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // GET: api/ActiveResidentEventLogs?residentId=6
        [HttpGet]
        public async Task<DynamicJsonArray> GetForResident(int residentId)
        {
            IEnumerable<ActiveResidentEventLog> activeResidentEventLogs = new Collection<ActiveResidentEventLog>();

            await Task.Run(() =>
            {
                activeResidentEventLogs = _activeResidentEventLogService.GetForResident(residentId);
            });

            if (activeResidentEventLogs == null) return new DynamicJsonArray(new DynamicJsonArray(new object[0]));

            var jArray = activeResidentEventLogs
                .Select(x => new
                {
                    x.Id,
                    x.Description
                }).ToArray();

            return new DynamicJsonArray(jArray);
        }

        // POST: api/ActiveResidentEventLogs
        [HttpPost]
        public int Post([FromBody]ActiveResidentEventLog activeResidentEventLog)
        {
            return _activeResidentEventLogService.Post(activeResidentEventLog);
        }

        // PATCH: api/ActiveResidentEventLogs/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]ActiveResidentEventLog activeResidentEventLog)
        {
            _activeResidentEventLogService.Patch(id, activeResidentEventLog);
        }

        // DELETE: api/ActiveResidentEventLogs/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _activeResidentEventLogService.Delete(id);
        }
    }
}
