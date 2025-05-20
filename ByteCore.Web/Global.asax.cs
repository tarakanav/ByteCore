using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
           AreaRegistration.RegisterAllAreas();
           RouteConfig.RegisterRoutes(RouteTable.Routes);
           UnityConfig.RegisterComponents();
           BundleConfig.RegisterBundles(BundleTable.Bundles);
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

            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
            var userClaims = new ClaimsPrincipal(identity);
            HttpContext.Current.User = userClaims;
        }
    }
}