using ByteCore.Web.Models;
using System.Web.Mvc;

namespace ByteCore.Web.Controllers
{
    public class AboutUsController : Controller
    {
       
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
