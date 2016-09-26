using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IRfidEventLogService
    {
        IEnumerable<RfidEventLog> Get();
        RfidEventLog Get(int id);
        IEnumerable<RfidEventLog> GetForDate(string date);
        IEnumerable<RfidEventLog> GetForResident(int residentId);
        void Post(RfidEventLog rfidEventLog);
        void Patch(int id, RfidEventLog rfidEventLog);
        void Delete(int id);
    }

    public class RfidEventLogService : IRfidEventLogService
    {
        public IEnumerable<RfidEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var rfidEventLogs = container.RfidEventLogs
                .Expand("Resident")
                .AsEnumerable();

            return rfidEventLogs;
        }

        public RfidEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var rfidEventLog = container.RfidEventLogs.ByKey(id)
                .Expand("Resident")
                .GetValue();

            return rfidEventLog;
        }

        public IEnumerable<RfidEventLog> GetForDate(string date)
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

            var rfidEventLogs = container.RfidEventLogs.AddQueryOption("$filter", filter)
                .Expand("Resident")
                .ToList();

            return rfidEventLogs;
        }

        public IEnumerable<RfidEventLog> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var rfidEventLogs = container.RfidEventLogs
                .AddQueryOption("$filter", $"ResidentId eq {residentId}");

            return rfidEventLogs;
        }

        public void Post(RfidEventLog rfidEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToRfidEventLogs(rfidEventLog);
            container.SaveChanges();
        }

        public void Patch(int id, RfidEventLog rfidEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.RfidEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResidentId != null)
                el.ResidentId = rfidEventLog.ResidentId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var rfidEventLog = container.RfidEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (rfidEventLog == null) return;

            container.DeleteObject(rfidEventLog);
            container.SaveChanges();
        }
    }
}
