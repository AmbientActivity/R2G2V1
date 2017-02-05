using Keebee.AAT.DataAccess.Models;
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

            builder.EntitySet<Config>("Configs");
            builder.EntitySet<ConfigDetail>("ConfigDetails");
            builder.EntitySet<PhidgetStyleType>("PhidgetStyleTypes");
            builder.EntitySet<PhidgetType>("PhidgetTypes");
            builder.EntitySet<ResponseType>("ResponseTypes");
            builder.EntitySet<ResponseTypeCategory>("ResponseTypeCategories");
            builder.EntitySet<InteractiveActivityType>("InteractiveActivityTypes");
            builder.EntitySet<ActivityEventLog>("ActivityEventLogs");
            builder.EntitySet<InteractiveActivityEventLog>("InteractiveActivityEventLogs");
            builder.EntitySet<ActiveResidentEventLog>("ActiveResidentEventLogs");
            builder.EntitySet<MediaFile>("MediaFiles");
            builder.EntitySet<MediaFileStream>("MediaFileStreams");
            builder.EntitySet<MediaPathType>("MediaPathTypes");
            builder.EntitySet<ResidentMediaFile>("ResidentMediaFiles");
            builder.EntitySet<PublicMediaFile>("PublicMediaFiles");
            builder.EntitySet<Resident>("Residents");
            builder.EntitySet<ActiveResident>("ActiveResidents");
            builder.EntitySet<User>("Users");
            builder.EntitySet<Role>("Roles");
            builder.EntitySet<UserRole>("UserRoles");

            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}

