using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Attributes
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var request = httpContext.Request;
            var userBl = DependencyResolver.Current.GetService<IUserBl>();

            var cookie = request.Cookies["X-KEY"];
            User user = null;

            if (cookie != null)
            {
                if (cookie.Expires != DateTime.MinValue && cookie.Expires < DateTime.Now)
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

            if (user != null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var currentUrl = HttpUtility.UrlEncode(request.RawUrl);
            var loginUrl = $"/Account/Login?returnUrl={currentUrl}";
            filterContext.Result = new RedirectResult(loginUrl);
        }
    }
}
