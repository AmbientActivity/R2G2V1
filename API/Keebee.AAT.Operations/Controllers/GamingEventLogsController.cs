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
    [RoutePrefix("api/gamingeventlogs")]
    public class GamingEventLogsController : ApiController
    {
        private readonly IGamingEventLogService _gamingEventLogService;

        public GamingEventLogsController(IGamingEventLogService gamingEventLogService)
        {
            _gamingEventLogService = gamingEventLogService;
        }

        // GET: api/GamingEventLog
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<GamingEventLog> gamingEventLog = new Collection<GamingEventLog>();

            await Task.Run(() =>
            {
                gamingEventLog = _gamingEventLogService.Get()
                    .OrderByDescending(o => o.DateEntry);
            });

            if (gamingEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.GamingEventLog = gamingEventLog
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = x.DateEntry.ToString("hh:mm:ss:ff tt"),
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    EntryType = x.EventLogEntryType.Description,
                    x.Difficultylevel,
                    Success = x.IsSuccess,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/GamingEventLog/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var gamingEventLog = new GamingEventLog();

            await Task.Run(() =>
            {
                gamingEventLog = _gamingEventLogService.Get(id);
            });

            if (gamingEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{gamingEventLog.DateEntry:D}";
            exObj.Time = $"{gamingEventLog.DateEntry:T}";
            exObj.Resident = (gamingEventLog.Resident != null)
                ? $"{gamingEventLog.Resident.FirstName} {gamingEventLog.Resident.LastName}"
                : "N/A";
            exObj.EventType = gamingEventLog.EventLogEntryType.Description;
            exObj.DifficultyLevel = gamingEventLog.Difficultylevel;
            exObj.IsSuccess = gamingEventLog.IsSuccess;
            exObj.Description = gamingEventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/GamingEventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string date)
        {
            IEnumerable<GamingEventLog> gamingEventLogs = new Collection<GamingEventLog>();

            await Task.Run(() =>
            {
                gamingEventLogs = _gamingEventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (gamingEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.GamingEventLogs = gamingEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = x.DateEntry.ToString("hh:mm:ss:ff tt"),
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    EntryType = x.EventLogEntryType.Description,
                    x.Difficultylevel,
                    x.IsSuccess,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/GamingEventLog
        [HttpPost]
        public void Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var gamingEventLog = serializer.Deserialize<GamingEventLog>(value);
            _gamingEventLogService.Post(gamingEventLog);
        }

        // PATCH: api/GamingEventLog/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var gamingEventLog = serializer.Deserialize<GamingEventLog>(value);
            _gamingEventLogService.Patch(id, gamingEventLog);
        }

        // DELETE: api/GamingEventLog/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _gamingEventLogService.Delete(id);
        }
    }
}
