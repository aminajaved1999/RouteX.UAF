using RouteX.UAF.LogicLayer.Interfaces;
using RouteX.UAF.LogicLayer.Managers;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace RouteX.UAF.WebApi
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<IRoleManager, RoleManager>();
            container.RegisterType<IUserManager, UserManager>();
            container.RegisterType<IAuthManager, AuthManager>();

            container.RegisterType<IBusManager, BusManager>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}