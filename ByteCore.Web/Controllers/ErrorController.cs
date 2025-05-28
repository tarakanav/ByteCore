using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web.Controllers
{
    public class ErrorController : BaseController
    {
        // GET: Error/403
        [Route("Error/403")]
        public ActionResult AccessDenied()
        {
            Response.StatusCode = 403;
            return View();
        }
        
        // GET: Error/404
        [Route("Error/404")]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}