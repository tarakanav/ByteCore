using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly IAuditLogBl _auditLogBl;
        private DateTime? _actionStart;

        public BaseController(IAuditLogBl auditLogBl)
        {
            _auditLogBl = auditLogBl;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod == "GET")
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            
            _actionStart = DateTime.UtcNow;
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User?.Identity?.IsAuthenticated == true && _actionStart.HasValue)
            {
                var request = filterContext.HttpContext.Request;
                var response = filterContext.HttpContext.Response;
                var identity = new ClaimsIdentity(User.Identity);

                var elapsedMs = (int)(DateTime.UtcNow - _actionStart.Value).TotalMilliseconds;

                long? contentLength = null;
                var clHeader = response.Headers["Content-Length"];
                if (long.TryParse(clHeader, out var parsed))
                    contentLength = parsed;
                var parseResult = int.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

                var log = new AuditLog
                {
                    UserId = parseResult ? userId : 0,
                    ActionTime = _actionStart.Value,

                    ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    ActionName = filterContext.ActionDescriptor.ActionName,

                    HttpMethod = request.HttpMethod,
                    UrlAccessed = request.Url?.AbsoluteUri,
                    IpAddress = request.UserHostAddress,
                    UserAgent = request.UserAgent,

                    QueryString = request.QueryString.HasKeys()
                        ? request.QueryString.ToString()
                        : null,
                    FormData = ReadFormData(request),

                    ResponseStatusCode = response.StatusCode,
                    ResponseContentLength = contentLength,
                    ExecutionTimeMs = elapsedMs
                };

                _auditLogBl.SaveLog(log);
            }

            base.OnActionExecuted(filterContext);
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