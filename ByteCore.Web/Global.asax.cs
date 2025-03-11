using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using ByteCore.Web.Controllers;
using ByteCore.Web.Data;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;

namespace ByteCore.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
           AreaRegistration.RegisterAllAreas();
           RouteConfig.RegisterRoutes(RouteTable.Routes);
           UnityConfig.RegisterComponents();
        }
    }
}