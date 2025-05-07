using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IAuditLogBl auditLogBl) : base(auditLogBl)
        {
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}