using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IInteractiveActivityEventLogService
    {
        IEnumerable<InteractiveActivityEventLog> Get();
        InteractiveActivityEventLog Get(int id);
        IEnumerable<InteractiveActivityEventLog> GetForDate(string date);
        IEnumerable<InteractiveActivityEventLog> GetForResident(int residentid);
        int Post(InteractiveActivityEventLog activeResidentEventLog);
        void Patch(int id, InteractiveActivityEventLog activeResidentEventLog);
        void Delete(int id);
    }

    public class InteractiveActivityEventLogService : IInteractiveActivityEventLogService
    {
        public IEnumerable<InteractiveActivityEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLogs = container.InteractiveActivityEventLogs
                .Expand("InteractiveActivityType,Resident")
                .AsEnumerable();

            return activeResidentEventLogs;
        }

        public InteractiveActivityEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLog = container.InteractiveActivityEventLogs.ByKey(id)
                .Expand("InteractiveActivityType,Resident");

            InteractiveActivityEventLog result;
            try { result = activeResidentEventLog.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<InteractiveActivityEventLog> GetForDate(string date)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var dateFrom = DateTime.ParseExact(date, "MM/dd/yyyy", null);
            var dateTo = dateFrom.AddDays(1);

            var monthFrom = (dateFrom.Month < 10 ? "0" : "") + dateFrom.Month;
            var dayFrom = (dateFrom.Day < 10 ? "0" : "") + dateFrom.Day;

            var monthTo = (dateTo.Month < 10 ? "0" : "") + dateTo.Month;
            var dayTo = (dateTo.Day < 10 ? "0" : "") + dateTo.Day;

            string from = $"{dateFrom.Year}-{monthFrom}-{dayFrom}T00:00:00.000-00:00";
            string to = $"{dateTo.Year}-{monthTo}-{dayTo}T00:00:00.000-00:00";

            string filter = $"DateEntry gt {from} and DateEntry lt {to}";

            var activeResidentEventLogs = container.InteractiveActivityEventLogs.AddQueryOption("$filter", filter)
                .Expand("InteractiveActivityType,Resident");

            var list = new List<InteractiveActivityEventLog>();
            try { list = activeResidentEventLogs.ToList(); }
            catch { }

            return list;
        }

        public IEnumerable<InteractiveActivityEventLog> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLogs = container.InteractiveActivityEventLogs
                .AddQueryOption("$filter", $"ResidentId eq {residentId}");

            return activeResidentEventLogs;
        }

        public int Post(InteractiveActivityEventLog activeResidentEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToInteractiveActivityEventLogs(activeResidentEventLog);
            container.SaveChanges();

            return activeResidentEventLog.Id;
        }

        public void Patch(int id, InteractiveActivityEventLog interactiveActivityEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.InteractiveActivityEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (interactiveActivityEventLog.ResidentId != null)
                el.ResidentId = interactiveActivityEventLog.ResidentId;

            if (interactiveActivityEventLog.InteractiveActivityTypeId != null)
                el.InteractiveActivityTypeId = interactiveActivityEventLog.InteractiveActivityTypeId;

            if (interactiveActivityEventLog.Description != null)
                el.Description = interactiveActivityEventLog.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activeResidentEventLog = container.InteractiveActivityEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (activeResidentEventLog == null) return;

            container.DeleteObject(activeResidentEventLog);
            container.SaveChanges();
        }
    }
}
