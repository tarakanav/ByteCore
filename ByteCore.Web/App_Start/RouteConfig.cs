using System.Web.Mvc;
using System.Web.Routing;

namespace ByteCore.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            //    name: "QuizStart",
            //    url: "Quizzes/Quiz/{id}",
            //    defaults: new { controller = "Quizzes", action = "Quiz" },
            //    constraints: new { id = @"\d+" }
            //);

            routes.MapRoute(
                name: "Quizzes",
                url: "Quizzes/{action}/{id}",
                defaults: new { controller = "Quizzes", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
             name: "StartQuiz",
             url: "Quizzes/Start/{id}",
             defaults: new { controller = "Quizzes", action = "Start" },
             constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}