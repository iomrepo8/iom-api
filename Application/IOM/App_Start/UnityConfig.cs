using System.Web.Http;
using IOM.Hubs;
using IOM.Services;
using IOM.Services.Interface;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Unity;
using Unity.WebApi;

namespace IOM
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IRepositoryService, RepositoryService>();
            container.RegisterType<INotificationServices, NotificationServices>();
            container.RegisterType<IRoleServices, RoleServices>();
            container.RegisterType<IPermissionServices, PermissionServices>();
            container.RegisterType<ITagServices, TagServices>();
            
            container.RegisterType<IAzureStorageServices, AzureStorageServices>();
            container.RegisterType<ITaskGroupServices, TaskGroupServices>();

            container.RegisterType<DashboardDataHub, DashboardDataHub>();
            container.RegisterType<NotificationHub, NotificationHub>();
            container.RegisterType<AutoOutHub, AutoOutHub>();
            container.RegisterType<TkManagementHub, TkManagementHub>();
            container.RegisterType<IHubActivator, UnityHubActivator>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new UnityHubActivator(container));
        }
    }
}