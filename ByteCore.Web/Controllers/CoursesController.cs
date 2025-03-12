using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ByteCore.Web.Models;
using System.Web.Mvc;
using ByteCore.Web.Services;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses")]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _coursesService;

        public CoursesController(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        // GET: Courses
        public ActionResult Index()
        {
            var courses = _coursesService.GetCourses();
            return View(courses);
        }

        // GET: Courses/Overview
        public ActionResult Overview()
        {
            return View();
        }

        // GET: Courses/{id}
        [Route("{id:int}")]
        public ActionResult Course(int id)
        {
            var course = _coursesService.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return View("CourseUnenrolled", course);
            }

            var isEnrolled = _coursesService.IsUserEnrolled(course.Id, User.Identity.Name);
            return View(isEnrolled ? "CourseEnrolled" : "CourseUnenrolled", course);
        }

        // GET: /Courses/{id}/Roadmap
        [Route("{id:int}/Roadmap")]
        public ActionResult Roadmap(int id)
        {
            var course = _coursesService.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        // POST: Courses/1/Enroll
        [Authorize]
        [Route("{id:int}/Enroll")]
        [HttpPost]
        public async Task<ActionResult> Enroll(int id)
        {
            var course = _coursesService.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }
            
            if (_coursesService.IsUserEnrolled(id, User.Identity.Name))
            {
                TempData["EnrollMessage"] = "You are already enrolled in the course!";
                return RedirectToAction("Course", new { id });
            }
            
            await _coursesService.EnrollUserAsync(id, User.Identity.Name);
            
            TempData["EnrollMessage"] = "You are now enrolled in the course!";
            return RedirectToAction("Course", new { id });
        }
        
        // GET: Courses/Create
        [Authorize]
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            var course = new CourseModel
            {
                Chapters = new List<ChapterModel>
                {
                    new ChapterModel
                    {
                        Sections = new List<SectionModel>
                        {
                            new SectionModel { Type = SectionType.Read },
                        }
                    }
                }
            };
            return View(course);
        }
        
        // POST: Courses/Create
        [Authorize]
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CourseModel model)
        {
            try
            {
                await _coursesService.CreateCourseAsync(model);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }
    }
}