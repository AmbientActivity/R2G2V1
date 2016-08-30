﻿using Keebee.AAT.DataAccess.Models;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace Keebee.AAT.DataAccess
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ODataConventionModelBuilder { Namespace = "KeebeeAAT" };

            builder.EntitySet<Resident>("Residents");
            builder.EntitySet<Profile>("Profiles");
            builder.EntitySet<ProfileDetail>("ProfileDetails");
            builder.EntitySet<ActivityType>("ActionTypes");
            builder.EntitySet<Response>("Responses");
            builder.EntitySet<ResponseType>("ResponseTypes");
            builder.EntitySet<ResponseTypeCategory>("ResponseTypeCategories");
            builder.EntitySet<AmbientResponse>("AmbientResponses");
            builder.EntitySet<Caregiver>("Caregivers");
            builder.EntitySet<User>("Users");
            builder.EntitySet<EventLog>("EventLogs");
            builder.EntitySet<GamingEventLog>("GamingEventLogs");
            builder.EntitySet<EventLogEntryType>("EventLogEntryTypes");
            builder.EntitySet<PersonalPicture>("PersonalPictures");
            builder.EntitySet<MediaFile>("MediaFiles");

            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}

