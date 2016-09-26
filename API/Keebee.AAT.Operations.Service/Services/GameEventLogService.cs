using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IGameEventLogService
    {
        IEnumerable<GameEventLog> Get();
        GameEventLog Get(int id);
        IEnumerable<GameEventLog> GetForDate(string date);
        IEnumerable<GameEventLog> GetForResident(int residentid);
        void Post(GameEventLog gameEventLog);
        void Patch(int id, GameEventLog gameEventLog);
        void Delete(int id);
    }

    public class GameEventLogService : IGameEventLogService
    {
        public IEnumerable<GameEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gameEventLogs = container.GameEventLogs
                .Expand("GameType,Resident")
                .AsEnumerable();

            return gameEventLogs;
        }

        public GameEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gameEventLog = container.GameEventLogs.ByKey(id)
                .Expand("GameType,Resident")
                .GetValue();

            return gameEventLog;
        }

        public IEnumerable<GameEventLog> GetForDate(string date)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var dateFrom = DateTime.ParseExact(date, "MM/dd/yyyy", null);
            var dateTo = dateFrom.AddDays(1);

            var monthFrom = (dateFrom.Month < 10 ? "0" : "") + dateFrom.Month;
            var dayFrom = (dateFrom.Day < 10 ? "0" : "") + dateFrom.Day;

            var monthTo = (dateTo.Month < 10 ? "0" : "") + dateTo.Month;
            var dayTo = (dateTo.Day < 10 ? "0" : "") + dateTo.Day;

            string from = $"{dateFrom.Year}-{monthFrom}-{dayFrom}";
            string to = $"{dateTo.Year}-{monthTo}-{dayTo}";

            string filter = $"DateEntry gt {from} and DateEntry lt {to}";

            var gameEventLogs = container.GameEventLogs.AddQueryOption("$filter", filter)
                .Expand("GameType,Resident")
                .ToList();

            return gameEventLogs;
        }

        public IEnumerable<GameEventLog> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gameEventLogs = container.GameEventLogs
                .AddQueryOption("$filter", $"ResidentId eq {residentId}");

            return gameEventLogs;
        }

        public void Post(GameEventLog gameEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToGameEventLogs(gameEventLog);
            container.SaveChanges();
        }

        public void Patch(int id, GameEventLog gameEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.GameEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResidentId != null)
                el.ResidentId = gameEventLog.ResidentId;

            if (el.GameTypeId != null)
                el.GameTypeId = gameEventLog.GameTypeId;

            if (el.Description != null)
                el.Description = gameEventLog.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gameEventLog = container.GameEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (gameEventLog == null) return;

            container.DeleteObject(gameEventLog);
            container.SaveChanges();
        }
    }
}
