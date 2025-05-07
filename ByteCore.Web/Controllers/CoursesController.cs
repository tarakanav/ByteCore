using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses")]
    public class CoursesController : BaseController
    {
        private readonly ICourseBl _courseBl;

        public CoursesController(ICourseBl courseBl, IAuditLogBl auditLogBl) : base(auditLogBl)
        {
            _courseBl = courseBl;
        }

        // GET: Courses
        public ActionResult Index()
        {
            var courses = _courseBl.GetCourses();
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
            var course = _courseBl.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return View("CourseUnenrolled", course);
            }

            var isEnrolled = _courseBl.IsUserEnrolled(course.Id, User.Identity.Name);
            return View(isEnrolled ? "CourseEnrolled" : "CourseUnenrolled", course);
        }

        // GET: /Courses/{id}/Roadmap
        [Route("{id:int}/Roadmap")]
        public ActionResult Roadmap(int id)
        {
            var course = _courseBl.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        // POST: Courses/1/Enroll
        [CustomAuthorize]
        [Route("{id:int}/Enroll")]
        [HttpPost]
        public async Task<ActionResult> Enroll(int id)
        {
            var course = _courseBl.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }
            
            if (_courseBl.IsUserEnrolled(id, User.Identity.Name))
            {
                TempData["EnrollMessage"] = "You are already enrolled in the course!";
                return RedirectToAction("Course", new { id });
            }
            
            await _courseBl.EnrollUserAsync(id, User.Identity.Name);
            
            TempData["EnrollMessage"] = "You are now enrolled in the course!";
            return RedirectToAction("Course", new { id });
        }
        
        // GET: Courses/Create
        [CustomAuthorize("Admin")]
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            var course = new Course();
            return View(course);
        }
        
        // GET: Courses/1/Edit
        [CustomAuthorize("Admin")]
        [HttpGet]
        [Route("{id:int}/Edit")]
        public ActionResult Edit(int id)
        {
            var course = _courseBl.GetCourse(id);
            
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }
        
        // POST: Courses/Create
        [CustomAuthorize("Admin")]
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Course model)
        {
            try
            {
                await _courseBl.CreateCourseAsync(model);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View(model);
        }
        
        [CustomAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{id:int}/Edit")]
        public async Task<ActionResult> Edit(Course model)
        {
            if (!ModelState.IsValid)
            {
                model = _courseBl.GetCourse(model.Id);
                return View(model);
            }

            try
            {
                await _courseBl.UpdateCourseAsync(model);
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model = _courseBl.GetCourse(model.Id);
                return View(model);
            }
        }
        
        // POST: /Courses/{id}/Delete
        [CustomAuthorize("Admin")]
        [HttpPost]
        [Route("{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var quiz = _courseBl.GetCourse(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }

            await _courseBl.DeleteCourseAsync(id);
            return RedirectToAction("Index");
        }
    }
}