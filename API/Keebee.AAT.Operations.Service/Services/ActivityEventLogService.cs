using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IActivityEventLogService
    {
        IEnumerable<ActivityEventLog> Get();
        ActivityEventLog Get(int id);
        IEnumerable<ActivityEventLog> GetForDate(string date);
        IEnumerable<ActivityEventLog> GetForConfig(int configId);
        IEnumerable<ActivityEventLog> GetForConfigDetail(int configDetailId);
        IEnumerable<ActivityEventLog> GetForResident(int residentId);
        IEnumerable<ActivityEventLog> GetWithConfigDetail();
        IEnumerable<ActivityEventLog> GetConfigDetails();
        int Post(ActivityEventLog activityEventLog);
        void Patch(int id, ActivityEventLog activityEventLog);
        void Delete(int id);
        void DeleteForResident(int residentId);
    }

    public class ActivityEventLogService : IActivityEventLogService
    {
        public IEnumerable<ActivityEventLog> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs
                .Expand("Resident,ConfigDetail($expand=PhidgetType,ResponseType($expand=ResponseTypeCategory))")
                .AsEnumerable();

            return activityEventLogs;
        }

        public ActivityEventLog Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLog = container.ActivityEventLogs.ByKey(id)
                .Expand("Resident,ConfigDetail($expand=ResponseType($expand=ResponseTypeCategory))")
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

            var activityEventLogs = container.ActivityEventLogs
                .AddQueryOption("$filter", filter)
                .Expand("Resident,ConfigDetail($expand=PhidgetType,ResponseType($expand=ResponseTypeCategory))")
                .ToList();

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForConfig(int configId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs
                .AddQueryOption("$filter", $"ConfigDetail/ConfigId eq {configId}")
                .Expand("Resident,ConfigDetail");

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForConfigDetail(int configDetailId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs
                .AddQueryOption("$filter", $"ConfigDetailId eq {configDetailId}")
                .Expand("Resident,ConfigDetail");

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs
                .AddQueryOption("$filter", $"ResidentId eq {residentId}");

            return activityEventLogs;
        }

        public IEnumerable<ActivityEventLog> GetWithConfigDetail()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs
                .Expand("ConfigDetail")
                .AsEnumerable();

            return activityEventLogs;
        }

        internal class ActivityEventLogs
        {
            public IEnumerable<ActivityEventLog> value { get; set; }
        }
        public IEnumerable<ActivityEventLog> GetConfigDetails()
        {
            var url = $"{ODataHost.Url}/ActivityEventLogs?$apply=groupby((ConfigDetail/ConfigId, ConfigDetailId), aggregate(Id with sum as Total))";
            var request = (HttpWebRequest)WebRequest.Create(url);

            IEnumerable<ActivityEventLog> detailsInUse;

            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var data = new StreamReader(stream);
                    var result = data.ReadToEnd();

                    var serializer = new JavaScriptSerializer();
                    detailsInUse = serializer.Deserialize<ActivityEventLogs>(result).value;
                }
            }

            //var container = new Container(new Uri(ODataHost.Url));

            //var detailsInUse = container.ActivityEventLogs
            //    .Expand("ConfigDetail")
            //    .AsEnumerable()
            //    .GroupBy(x => new { x.ConfigDetail.ConfigId, x.ConfigDetailId }, (key, group) => new
            //            ConfigDetail
            //    {
            //        Id = (int)key.ConfigDetailId,
            //        ConfigId = key.ConfigId
            //    }).AsEnumerable();

            return detailsInUse;
        }

        public int Post(ActivityEventLog activityEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToActivityEventLogs(activityEventLog);
            container.SaveChanges();

            return activityEventLog.Id;
        }

        public void Patch(int id, ActivityEventLog activityEventLog)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ActivityEventLogs.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (activityEventLog.ResidentId != null)
                el.ResidentId = activityEventLog.ResidentId;

            if (activityEventLog.ConfigDetailId > 0)
                el.ConfigDetailId = activityEventLog.ConfigDetailId;

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

        public void DeleteForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityEventLogs = container.ActivityEventLogs.Where(e => e.ResidentId == residentId).SingleOrDefault();
            if (activityEventLogs == null) return;

            container.DeleteObject(activityEventLogs);
            container.SaveChanges();
        }
    }
}
