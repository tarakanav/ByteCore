using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IAuditLogBl AuditLogBl;

        public BaseController(IAuditLogBl auditLogBl)
        {
            AuditLogBl = auditLogBl;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var log = new AuditLog
            {
                UserEmail         = User?.Identity?.IsAuthenticated == true 
                    ? User.Identity.Name 
                    : null,
                ActionTime     = DateTime.UtcNow,
                ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                ActionName     = filterContext.ActionDescriptor.ActionName,
                HttpMethod     = request.HttpMethod,
                UrlAccessed    = request.Url?.AbsoluteUri,
                IpAddress      = request.UserHostAddress,
                UserAgent      = request.UserAgent,
                QueryString    = request.QueryString.HasKeys() 
                    ? request.QueryString.ToString() 
                    : null,
                FormData       = ReadFormData(request)
            };

            AuditLogBl.SaveLog(log);

            base.OnActionExecuting(filterContext);
        }
        
        private string ReadFormData(HttpRequestBase request)
        {
            if (request.Form == null || request.Form.Count == 0)
                return null;

            return string.Join("&",
                request.Form.AllKeys
                    .Select(k => $"{HttpUtility.UrlEncode(k)}={HttpUtility.UrlEncode(request.Form[k])}")
            );
        }
    }
}