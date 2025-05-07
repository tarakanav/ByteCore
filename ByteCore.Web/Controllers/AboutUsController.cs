using ByteCore.Web.Models;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;

namespace ByteCore.Web.Controllers
{
    public class AboutUsController : BaseController
    {
        public AboutUsController(IAuditLogBl auditLogBl) : base(auditLogBl)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

      
        [HttpPost]
        public ActionResult SubmitForm(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Message"] = "Thank you for reaching out! We'll get back to you soon.";
                return RedirectToAction("Index");
            }
            
            return View("Index");
        }
    }
}
