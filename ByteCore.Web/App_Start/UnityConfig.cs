using System.Web.Mvc;
using ByteCore.Web.Controllers;
using ByteCore.Web.Data;
using ByteCore.Web.Services;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;

namespace ByteCore.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IPasswordService, PasswordService>();
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<AccountController>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}