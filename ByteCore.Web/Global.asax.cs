using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ByteCore.BusinessLogic;
using ByteCore.Helpers;

namespace ByteCore.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
           AreaRegistration.RegisterAllAreas();
           RouteConfig.RegisterRoutes(RouteTable.Routes);
           BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            AuthenticateRequestHelper.AuthenticateRequest(HttpContext.Current, Bl.GetUserBl());
        }
    }
}