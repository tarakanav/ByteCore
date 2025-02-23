using System.Web.Mvc;

namespace ByteCore.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error/NotFound
        public ActionResult NotFound()
        {
            return View();
        }
    }
}