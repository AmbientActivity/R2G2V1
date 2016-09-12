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
            builder.EntitySet<Resident>("Residents");
            builder.EntitySet<Profile>("Profiles");
            builder.EntitySet<PhidgetType>("PhidgetTypes");
            builder.EntitySet<ResponseType>("ResponseTypes");
            builder.EntitySet<ResponseTypeCategory>("ResponseTypeCategories");
            builder.EntitySet<GameType>("GameTypes");
            builder.EntitySet<Response>("Responses");
            builder.EntitySet<AmbientResponse>("AmbientResponses");
            builder.EntitySet<Caregiver>("Caregivers");
            builder.EntitySet<User>("Users");
            builder.EntitySet<ActivityEventLog>("ActivityEventLogs");
            builder.EntitySet<GameEventLog>("GameEventLogs");
            builder.EntitySet<RfidEventLog>("RfidEventLogs");
            builder.EntitySet<PersonalPicture>("PersonalPictures");
            builder.EntitySet<MediaFile>("MediaFiles");
            builder.EntitySet<MediaFileStream>("MediaFileStreams");

            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}

