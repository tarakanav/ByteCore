using System.Web.Mvc;
using ByteCore.Data;
using ByteCore.Domain.Services.Implementations;
using ByteCore.Domain.Services.Interfaces;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;

namespace ByteCore.Web
{
    public static class DependencyInjectionConfig
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
            container.RegisterType<IQuizzesService, QuizzesService>();
            container.RegisterType<ICoursesService, CoursesService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}