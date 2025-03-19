using System.Web.Mvc;
using ByteCore.BusinessLogic.Data;
using ByteCore.BusinessLogic.Implementations;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Web.Controllers;
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
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserBl, UserBl>();
            container.RegisterType<IQuizBl, QuizBl>();
            container.RegisterType<ICourseBl, CourseBl>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}