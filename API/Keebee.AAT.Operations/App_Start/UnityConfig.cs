using Keebee.AAT.Operations.Service.Services;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace Keebee.AAT.Operations
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IResidentService, ResidentService>();
            container.RegisterType<IProfileService, ProfileService>();
            container.RegisterType<IConfigService, ConfigService>();
            container.RegisterType<IConfigDetailService, ConfigDetailService>();
            container.RegisterType<IPhidgetTypeService, PhidgetTypeService>();
            container.RegisterType<IResponseTypeService, ResponseTypeService>();
            container.RegisterType<IAmbientResponseService, AmbientResponseService>();
            container.RegisterType<IResponseService, ResponseService>();
            container.RegisterType<IPersonalPictureService, PersonalPictureService>();
            container.RegisterType<IActivityEventLogService, ActivityEventLogService>();
            container.RegisterType<IGameEventLogService, GameEventLogService>();
            container.RegisterType<IRfidEventLogService, RfidEventLogService>();
            container.RegisterType<IMediaService, MediaService>();
            container.RegisterType<IStatusService, StatusService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}