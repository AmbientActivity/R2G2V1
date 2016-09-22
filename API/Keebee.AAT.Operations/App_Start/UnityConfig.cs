using Keebee.AAT.Operations.Service.Services;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
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
            container.RegisterType<IConfigService, ConfigService>();
            container.RegisterType<IConfigDetailService, ConfigDetailService>();
            container.RegisterType<IPhidgetStyleTypeService, PhidgetStyleTypeService>();
            container.RegisterType<IPhidgetTypeService, PhidgetTypeService>();
            container.RegisterType<IResponseTypeService, ResponseTypeService>();
            container.RegisterType<IActivityEventLogService, ActivityEventLogService>();
            container.RegisterType<IGameEventLogService, GameEventLogService>();
            container.RegisterType<IRfidEventLogService, RfidEventLogService>();
            container.RegisterType<IMediaFileService, MediaFileService>();
            container.RegisterType<IMediaFileStreamService, MediaFileStreamService>();
            container.RegisterType<IMediaPathTypeService, MediaPathTypeService>();
            container.RegisterType<IResidentMediaFileService, ResidentMediaFileService>();
            container.RegisterType<IPublicMediaFileService, PublicMediaFileService>();
            container.RegisterType<IStatusService, StatusService>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}