using System.Web.Mvc;

namespace ByteCore.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/404
        [Route("Error/404")]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}