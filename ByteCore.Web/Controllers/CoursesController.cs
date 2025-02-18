using ByteCore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses")]
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Index()
        {
            return View();
        }

        // GET: Courses/Overview
        public ActionResult Overview()
        {
            return View();
        }

        // GET: Courses/1
        [Route("{id:int}")]
        public ActionResult Course(int id)
        {
            var course = new Course
            {
                Id = id,
                Title = "Introduction to Web Development",
                ShortDescription = "Learn the fundamentals of HTML, CSS, and JavaScript.",
                Description = "This course covers the basics of web development including responsive design, front-end frameworks, and modern JavaScript techniques. You'll work on hands-on projects and gain real-world skills.",
                Instructor = "Jane Doe",
                Duration = "6 weeks",
                StartDate = DateTime.Now.AddDays(14),
                ImageUrl = "https://dummyimage.com/600x400/343a40/6c757d"
            };

            return View(course);
        }
    }
}