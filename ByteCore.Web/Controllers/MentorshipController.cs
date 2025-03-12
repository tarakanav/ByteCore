using System.Collections.Generic;
using System.Web.Mvc;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class MentorshipController : Controller
    {
        // GET: Mentorship/Index
        public ActionResult Index()
        {
            var mentors = new List<MentorModel>
            {
                new MentorModel { Id = 1, Name = "Alex Johnson", Expertise = "Web Development", Rate = 50, Rating = 4.8, ImageUrl = "/content/images/mentor1.jpg" },
                new MentorModel { Id = 2, Name = "Sarah Smith", Expertise = "Machine Learning", Rate = 70, Rating = 4.9, ImageUrl = "/content/images/mentor2.jpg" },
                new MentorModel { Id = 3, Name = "David Brown", Expertise = "Cybersecurity", Rate = 60, Rating = 4.7, ImageUrl = "/content/images/mentor3.jpg" }
            };

            return View(mentors);
        }

        // GET: Mentorship/Book/1
        public ActionResult Book(int id)
        {
            var mentor = new MentorModel { Id = id, Name = "Sample Mentor", Expertise = "Sample Expertise", Rate = 50, Rating = 4.8, ImageUrl = "/content/images/default.jpg" };
            return View(mentor);
        }

        // POST: Mentorship/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Book(MentorModel mentor)
        {
            if (ModelState.IsValid)
            {
                TempData["Success"] = "Your session has been booked successfully!";
                return RedirectToAction("Index");
            }
            return View(mentor);
        }
    }
}