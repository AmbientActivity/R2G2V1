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
    [RoutePrefix("api/GameEventLogs")]
    public class GameEventLogsController : ApiController
    {
        private readonly IGameEventLogService _gameEventLogService;

        public GameEventLogsController(IGameEventLogService gameEventLogService)
        {
            _gameEventLogService = gameEventLogService;
        }

        // GET: api/GameEventLog
        [HttpGet]
        public async Task<DynamicJsonObject> Get()
        {
            IEnumerable<GameEventLog> gameEventLog = new Collection<GameEventLog>();

            await Task.Run(() =>
            {
                gameEventLog = _gameEventLogService.Get()
                    .OrderByDescending(o => o.DateEntry);
            });

            if (gameEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.GameEventLog = gameEventLog
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = x.DateEntry.ToString("hh:mm:ss:ff tt"),
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    GameType = x.GameType.Description,
                    x.Difficultylevel,
                    Success = x.IsSuccess,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/GameEventLog/5
        [HttpGet]
        [Route("{id}")]
        public async Task<DynamicJsonObject> Get(int id)
        {
            var gameEventLog = new GameEventLog();

            await Task.Run(() =>
            {
                gameEventLog = _gameEventLogService.Get(id);
            });

            if (gameEventLog == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.Date = $"{gameEventLog.DateEntry:D}";
            exObj.Time = $"{gameEventLog.DateEntry:T}";
            exObj.Resident = (gameEventLog.Resident != null)
                ? $"{gameEventLog.Resident.FirstName} {gameEventLog.Resident.LastName}"
                : "N/A";
            exObj.EventType = gameEventLog.GameType.Description;
            exObj.DifficultyLevel = gameEventLog.Difficultylevel;
            exObj.IsSuccess = gameEventLog.IsSuccess;
            exObj.Description = gameEventLog.Description;

            return new DynamicJsonObject(exObj);
        }

        // GET: api/GameEventLogs?date=08/26/2016
        [HttpGet]
        public async Task<DynamicJsonObject> Get(string date)
        {
            IEnumerable<GameEventLog> gameEventLogs = new Collection<GameEventLog>();

            await Task.Run(() =>
            {
                gameEventLogs = _gameEventLogService.GetForDate(date)
                    .OrderBy(o => o.DateEntry);
            });

            if (gameEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.GameEventLogs = gameEventLogs
                .Select(x => new
                {
                    Date = $"{x.DateEntry:D}",
                    Time = x.DateEntry.ToString("hh:mm:ss:ff tt"),
                    Resident = (x.Resident != null) ? $"{x.Resident.FirstName} {x.Resident.LastName}" : "N/A",
                    GameType = x.GameType.Description,
                    x.Difficultylevel,
                    x.IsSuccess,
                    x.Description,
                    ResidentId = x.Resident?.Id ?? -1
                });

            return new DynamicJsonObject(exObj);
        }

        // GET: api/GameEventLogs?residentId=6
        [HttpGet]
        public async Task<DynamicJsonObject> GetForResident(int residentId)
        {
            IEnumerable<GameEventLog> gameEventLogs = new Collection<GameEventLog>();

            await Task.Run(() =>
            {
                gameEventLogs = _gameEventLogService.GetForResident(residentId);
            });

            if (gameEventLogs == null) return new DynamicJsonObject(new ExpandoObject());

            dynamic exObj = new ExpandoObject();
            exObj.GameEventLogs = gameEventLogs
                .Select(x => new
                {
                    x.Id,
                    x.Description
                });

            return new DynamicJsonObject(exObj);
        }

        // POST: api/GameEventLog
        [HttpPost]
        public int Post([FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var gameEventLog = serializer.Deserialize<GameEventLog>(value);
            return _gameEventLogService.Post(gameEventLog);
        }

        // PATCH: api/GameEventLog/5
        [HttpPatch]
        [Route("{id}")]
        public void Patch(int id, [FromBody]string value)
        {
            var serializer = new JavaScriptSerializer();
            var gameEventLog = serializer.Deserialize<GameEventLog>(value);
            _gameEventLogService.Patch(id, gameEventLog);
        }

        // DELETE: api/GameEventLog/5
        [HttpDelete]
        [Route("{id}")]
        public void Delete(int id)
        {
            _gameEventLogService.Delete(id);
        }
    }
}
