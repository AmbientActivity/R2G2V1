using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IGamingEventLogService
    {
        IEnumerable<GamingEventLog> Get();
        GamingEventLog Get(int id);
        IEnumerable<GamingEventLog> GetForDate(string date);
        void Post(GamingEventLog gamingEventLog);
        void Patch(int id, GamingEventLog gamingEventLog);
        void Delete(int id);
    }

    public class GamingEventLogService : IGamingEventLogService
    {
        public IEnumerable<GamingEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gamingEventLogs = container.GamingEventLogs
                .Expand("EventLogEntryType,Resident")
                .AsEnumerable();

            return gamingEventLogs;
        }

        public GamingEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gamingEventLog = container.GamingEventLogs.ByKey(id)
                .Expand("EventLogEntryType,Resident")
                .GetValue();

            return gamingEventLog;
        }

        public IEnumerable<GamingEventLog> GetForDate(string date)
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

            var eventLogs = container.GamingEventLogs.AddQueryOption("$filter", filter)
                .Expand("EventLogEntryType,Resident")
                .ToList();

            return eventLogs;
        }

        public void Post(GamingEventLog gamingEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToGamingEventLogs(gamingEventLog);
            container.SaveChanges();
        }

        public void Patch(int id, GamingEventLog gamingEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.GamingEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResidentId != null)
                el.ResidentId = gamingEventLog.ResidentId;

            if (el.EventLogEntryTypeId != null)
                el.EventLogEntryTypeId = gamingEventLog.EventLogEntryTypeId;

            if (el.Description != null)
                el.Description = gamingEventLog.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var gamingEventLog = container.GamingEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (gamingEventLog == null) return;

            container.DeleteObject(gamingEventLog);
            container.SaveChanges();
        }
    }
}
