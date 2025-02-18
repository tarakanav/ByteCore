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
            var course = new CourseModel
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
        // GET: /Courses/{id}/Roadmap
        [Route("{id:int}/Roadmap")]
        public ActionResult Roadmap(int id)
        {
            var course = GetCourseById(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            var roadmap = new CourseRoadmapModel
            {
                Course = course,
                Steps = new List<RoadmapStep>
                {
                    new RoadmapStep
                    {
                        StepTitle = "Introduction",
                        Description = "Overview of the course",
                        Week = "1"
                    },
                    new RoadmapStep
                    {
                        StepTitle = "Fundamentals",
                        Description = "Basic concepts and principles",
                        Week = "2"
                    },
                    new RoadmapStep
                    {
                        StepTitle = "Advanced Topics",
                        Description = "Deep dive into complex subjects",
                        Week = "3"
                    }
                }
            };

            ViewBag.CourseTitle = course.Title;
            return View(roadmap);
        }

        // Пример метода для получения курса
        private CourseModel GetCourseById(int id)
        {
            return new CourseModel
            {
                Id = id,
                Title = "Sample Course",
                ShortDescription = "This is a sample course description",
                Description = "Detailed course description here...",
                Instructor = "John Doe",
                Duration = "4 weeks",
                StartDate = new DateTime(2025, 3, 1),
                ImageUrl = "/images/sample-course.jpg"
            };
        }

        // Пример метода для получения шагов roadmap
        private List<string> GetRoadmapSteps(int courseId)
        {
            return new List<string>
            {
                "Introduction to the Course",
                "Module 1: Basics",
                "Module 2: Intermediate Concepts",
                "Module 3: Advanced Topics",
                "Final Project"
            };
        }
    }
}