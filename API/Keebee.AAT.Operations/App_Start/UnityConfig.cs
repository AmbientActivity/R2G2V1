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
            container.RegisterType<IProfileDetailService, ProfileDetailService>();
            container.RegisterType<IAmbientResponseService, AmbientResponseService>();
            container.RegisterType<IResponseService, ResponseService>();
            container.RegisterType<IPersonalPictureService, PersonalPictureService>();
            container.RegisterType<IEventLogService, EventLogService>();
            container.RegisterType<IGamingEventLogService, GamingEventLogService>();
            container.RegisterType<IMediaService, MediaService>();
            container.RegisterType<IStatusService, StatusService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}