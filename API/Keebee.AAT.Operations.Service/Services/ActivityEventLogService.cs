﻿using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IActivityEventLogService
    {
        IEnumerable<ActivityEventLog> Get();
        ActivityEventLog Get(int id);
        IEnumerable<ActivityEventLog> GetForDate(string date);
        void Post(ActivityEventLog activityEventLog);
        void Patch(int id, ActivityEventLog activityEventLog);
        void Delete(int id);
    }

    public class ActivityEventLogService : IActivityEventLogService
    {
        public IEnumerable<ActivityEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs
                .Expand("Resident,ActivityType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return activityEventLogs;
        }

        public ActivityEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLog = container.ActivityEventLogs.ByKey(id)
                .Expand("Resident,ActivityType,ResponseType($expand=ResponseTypeCategory)")
                .GetValue();

            return activityEventLog;
        }

        public IEnumerable<ActivityEventLog> GetForDate(string date)
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

            var activityEventLogs = container.ActivityEventLogs.AddQueryOption("$filter", filter)
                .Expand("Resident,ActivityType,ResponseType($expand=ResponseTypeCategory)")
                .ToList();

            return activityEventLogs;
        }

        public void Post(ActivityEventLog activityEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToActivityEventLogs(activityEventLog);
            container.SaveChanges();
        }

        public void Patch(int id, ActivityEventLog activityEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ActivityEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResidentId != null)
                el.ResidentId = activityEventLog.ResidentId;

            if (el.ActivityTypeId != null)
                el.ActivityTypeId = activityEventLog.ActivityTypeId;

            if (el.ResponseTypeId != null)
                el.ResponseTypeId = activityEventLog.ResponseTypeId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLog = container.ActivityEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (activityEventLog == null) return;

            container.DeleteObject(activityEventLog);
            container.SaveChanges();
        }
    }
}