using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IEventLogService
    {
        IEnumerable<EventLog> Get();
        EventLog Get(int id);
        IEnumerable<EventLog> GetForDate(string date);
        void Post(EventLog eventLog);
        void Patch(int id, EventLog eventLog);
        void Delete(int id);
    }

    public class EventLogService : IEventLogService
    {
        public IEnumerable<EventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var eventLogs = container.EventLogs
                .Expand("EventLogEntryType,Resident,ActivityType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return eventLogs;
        }

        public EventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var eventLog = container.EventLogs.ByKey(id)
                .Expand("EventLogEntryType,Resident,ActivityType,ResponseType($expand=ResponseTypeCategory)")
                .GetValue();

            return eventLog;
        }

        public IEnumerable<EventLog> GetForDate(string date)
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

            var eventLogs = container.EventLogs.AddQueryOption("$filter", filter)
                .Expand("EventLogEntryType,Resident,ActivityType,ResponseType($expand=ResponseTypeCategory)")
                .ToList();

            return eventLogs;
        }

        public void Post(EventLog eventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToEventLogs(eventLog);
            container.SaveChanges();
        }

        public void Patch(int id, EventLog eventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.EventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResidentId != null)
                el.ResidentId = eventLog.ResidentId;

            if (el.EventLogEntryTypeId != null)
                el.EventLogEntryTypeId = eventLog.EventLogEntryTypeId;

            if (el.ActivityTypeId != null)
                el.ActivityTypeId = eventLog.ActivityTypeId;

            if (el.ResponseTypeId != null)
                el.ResponseTypeId = eventLog.ResponseTypeId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var eventLog = container.EventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (eventLog == null) return;

            container.DeleteObject(eventLog);
            container.SaveChanges();
        }
    }
}
