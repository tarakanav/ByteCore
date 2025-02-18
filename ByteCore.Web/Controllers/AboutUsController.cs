using ByteCore.Models;
using System.Web.Mvc;

namespace ByteCore.Controllers
{
    public class AboutUsController : Controller
    {
        // GET: /AboutUs
        public ActionResult Index()
        {
            return View();
        }

        // POST: /AboutUs/SubmitForm
        [HttpPost]
        public ActionResult SubmitForm(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Здесь можно обрабатывать форму, например, отправить email или сохранить данные
                TempData["Message"] = "Thank you for reaching out! We'll get back to you soon.";
                return RedirectToAction("Index");
            }

            // Если что-то пошло не так, возвращаем обратно на страницу
            return View("Index");
        }
    }
}
