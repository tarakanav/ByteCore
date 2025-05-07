using System.Web.Mvc;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web.Controllers
{
    public class PrivacyController : BaseController
    {
        public PrivacyController(IAuditLogBl auditLogBl) : base(auditLogBl)
        {
        }

        // GET: /Privacy/
        public ActionResult Index()
        {
            return View();
        }
    }
}