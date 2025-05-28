using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web.Controllers
{
    public class TermsController : BaseController
    {
        // GET: /Terms/
        public ActionResult Index()
        {
            return View();
        }
    }
}