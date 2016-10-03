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
    [RoutePrefix("api/RfidEventLogs")]
    public class RfidEventLogsController : ApiController
    {
        private readonly IRfidEventLogService _rfidEventLogService;

        public RfidEventLogsController(IRfidEventLogService rfidEventLogService)
        {
            _rfidEventLogService = rfidEventLogService;
        }

        // GET: api/RfidEventLogs
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<RfidEventLog> rfidEventLogs = new Collection<RfidEventLog>();

            await Task.Run(() =>
            {
                rfidEventLogs = _rfidEventLogService.Get()
                    .OrderByDescending(o => o.DateEntry);
            });

            if (rfidEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.RfidEventLogs = rfidEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/RfidEventLogs/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var rfidEventLog = new RfidEventLog();

            await Task.Run(() =>
            {
                rfidEventLog = _rfidEventLogService.Get(id);
            });

            if (rfidEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{rfidEventLog.DateEntry:D}";
            exObj.Time = $"{rfidEventLog.DateEntry:T}";
            exObj.Resident = (rfidEventLog.Resident != null)
                ? $"{rfidEventLog.Resident.FirstName} {rfidEventLog.Resident.LastName}"
                : "N/A";
            exObj.Description = rfidEventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/RfidEventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string date)
        {
            IEnumerable<RfidEventLog> rfidEventLogs = new Collection<RfidEventLog>();

            await Task.Run(() =>
            {
                rfidEventLogs = _rfidEventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (rfidEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.RfidEventLogs = rfidEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = $"{x.DateEntry:T}",
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/RfidEventLogs?residentId=6
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResident(int residentId)
        {
            IEnumerable<RfidEventLog> rfidEventLogs = new Collection<RfidEventLog>();

            await Task.Run(() =>
            {
                rfidEventLogs = _rfidEventLogService.GetForResident(residentId);
            });

            if (rfidEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.RfidEventLogs = rfidEventLogs
                .Select(x => new
                {
                    x.Id,
                    x.Description
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/RfidEventLogs
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var rfidEventLog = serializer.Deserialize<RfidEventLog>(value);
            return _rfidEventLogService.Post(rfidEventLog);
        }

        // PATCH: api/RfidEventLogs/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var rfidEventLog = serializer.Deserialize<RfidEventLog>(value);
            _rfidEventLogService.Patch(id, rfidEventLog);
        }

        // DELETE: api/RfidEventLogs/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _rfidEventLogService.Delete(id);
        }
    }
}
