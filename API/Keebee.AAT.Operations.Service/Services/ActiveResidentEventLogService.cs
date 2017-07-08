using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IActiveResidentEventLogService
    {
        IEnumerable<ActiveResidentEventLog> Get();
        ActiveResidentEventLog Get(int id);
        IEnumerable<ActiveResidentEventLog> GetForDate(string date);
        IEnumerable<ActiveResidentEventLog> GetForResident(int residentId);
        int Post(ActiveResidentEventLog activeResidentEventLog);
        void Patch(int id, ActiveResidentEventLog activeResidentEventLog);
        void Delete(int id);
    }

    public class ActiveResidentEventLogService : IActiveResidentEventLogService
    {
        public IEnumerable<ActiveResidentEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLogs = container.ActiveResidentEventLogs
                .Expand("Resident")
                .AsEnumerable();

            return activeResidentEventLogs;
        }

        public ActiveResidentEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLog = container.ActiveResidentEventLogs.ByKey(id)
                .Expand("Resident");

            ActiveResidentEventLog result;
            try { result = activeResidentEventLog.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<ActiveResidentEventLog> GetForDate(string date)
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

            var activeResidentEventLogs = container.ActiveResidentEventLogs.AddQueryOption("$filter", filter)
                .Expand("Resident")
                .ToList();

            return activeResidentEventLogs;
        }

        public IEnumerable<ActiveResidentEventLog> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLogs = container.ActiveResidentEventLogs
                .AddQueryOption("$filter", $"ResidentId eq {residentId}");

            return activeResidentEventLogs;
        }

        public int Post(ActiveResidentEventLog activeResidentEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToActiveResidentEventLogs(activeResidentEventLog);
            container.SaveChanges();

            return activeResidentEventLog.Id;
        }

        public void Patch(int id, ActiveResidentEventLog activeResidentEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ActiveResidentEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (activeResidentEventLog.ResidentId != null)
                el.ResidentId = activeResidentEventLog.ResidentId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLog = container.ActiveResidentEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (activeResidentEventLog == null) return;

            container.DeleteObject(activeResidentEventLog);
            container.SaveChanges();
        }
    }
}
