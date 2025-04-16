using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Web.Controllers;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;

namespace ByteCore.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
           AreaRegistration.RegisterAllAreas();
           RouteConfig.RegisterRoutes(RouteTable.Routes);
           UnityConfig.RegisterComponents();
        }
        
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var cookie = HttpContext.Current.Request.Cookies["X-KEY"];
            if (cookie == null)
                return;
            
            var userBl = DependencyResolver.Current.GetService<IUserBl>();
            
            var user = userBl.GetUserByCookie(cookie.Value);
            if (user == null)
                return;
            
            var identity = new ClaimsIdentity("CustomCookie");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "CustomIdentityProvider"));
            
            HttpContext.Current.User = new ClaimsPrincipal(identity);
        }
    }
}