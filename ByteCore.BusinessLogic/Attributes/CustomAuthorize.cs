using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Attributes
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public CustomAuthorize() { }

        public CustomAuthorize(string roles)
        {
            Roles = roles;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var userBl = DependencyResolver.Current.GetService<IUserBl>();
            User user = null;

            var cookie = request.Cookies["X-KEY"];
            if (cookie != null)
            {
                if (cookie.Expires != DateTime.MinValue && cookie.Expires < DateTime.UtcNow)
                {
                    httpContext.Response.Cookies.Remove("X-KEY");
                }
                else
                {
                    if (httpContext.Session != null && httpContext.Session["User"] != null)
                    {
                        user = httpContext.Session["User"] as User;
                    }
                    if (user == null)
                    {
                        user = userBl.GetUserByCookie(cookie.Value);
                        if (user != null && httpContext.Session != null)
                        {
                            httpContext.Session["User"] = user;
                        }
                    }
                }
            }

            if (user == null)
            {
                var currentUrl = HttpUtility.UrlEncode(request.RawUrl);
                var loginUrl = $"/Account/Login?returnUrl={currentUrl}";
                filterContext.Result = new RedirectResult(loginUrl);
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(Roles))
            {
                var allowedRoles = Roles.Split(',')
                                        .Select(r => r.Trim())
                                        .ToList();
                if (!allowedRoles.Any(r => string.Equals(user.Role, r, StringComparison.OrdinalIgnoreCase)))
                {
                    filterContext.Result = new RedirectResult("/Error/403");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
