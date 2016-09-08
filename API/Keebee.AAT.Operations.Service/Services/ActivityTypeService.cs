using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IActivityTypeService
    {
        IEnumerable<ActivityType> Get();
        ActivityType Get(int id);
        void Post(ActivityType activityType);
        void Patch(int id, ActivityType activityType);
        void Delete(int id);
    }

    public class ActivityTypeService : IActivityTypeService
    {
        public IEnumerable<ActivityType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityTypes = container.ActivityTypes
                .AsEnumerable();

            return activityTypes;
        }

        public ActivityType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityType = container.ActivityTypes.ByKey(id)
                .GetValue();

            return activityType;
        }

        public void Post(ActivityType activityType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToActivityTypes(activityType);
            container.SaveChanges();
        }

        public void Patch(int id, ActivityType activityType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ActivityTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.Description != null)
                el.Description = activityType.Description;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var activityType = container.ActivityTypes.Where(e => e.Id == id).SingleOrDefault();
            if (activityType == null) return;

            container.DeleteObject(activityType);
            container.SaveChanges();
        }
    }
}
