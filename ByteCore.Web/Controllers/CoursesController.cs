using ByteCore.Web.Models;
using System;
using System.Collections.Generic;
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

        // GET: Courses/{id}
        [Route("{id:int}")]
        public ActionResult Course(int id)
        {
            var course = GetCourseById(id);
            if (course == null)
            {
                return HttpNotFound();
            }
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
                Modules = new List<CourseRoadmapModule>
                {
                    new CourseRoadmapModule
                    {
                        Title = "Module 1: Introduction to Python",
                        Topics = new List<CourseRoadmapTopic>
                        {
                            new CourseRoadmapTopic
                            {
                                Title = "Getting Started with Python and Setting Up Environment",
                                Lessons = new List<string>
                                {
                                    "Installing Python",
                                    "Setting Up Development Environment"
                                }
                            },
                            new CourseRoadmapTopic
                            {
                                Title = "Understanding AI Basics",
                                Lessons = new List<string>
                                {
                                    "Introduction to AI",
                                    "History and Evolution of AI"
                                }
                            }
                        }
                    },
                    new CourseRoadmapModule
                    {
                        Title = "Module 2: Python Programming Basics",
                        Topics = new List<CourseRoadmapTopic>
                        {
                            new CourseRoadmapTopic
                            {
                                Title = "Syntax, Variables, and Data Types",
                                Lessons = new List<string>
                                {
                                    "Understanding Variables",
                                    "Operators and Expressions"
                                }
                            },
                            new CourseRoadmapTopic
                            {
                                Title = "Collections and Loops",
                                Lessons = new List<string>
                                {
                                    "Lists, Tuples, and Dictionaries",
                                    "For and While Loops"
                                }
                            }
                        }
                    }
                }
            };

            ViewBag.CourseTitle = course.Title;
            return View(roadmap);
        }

        // Example method to retrieve a course
        private CourseModel GetCourseById(int id)
        {
            return new CourseModel
            {
                Id = id,
                Title = "Python and Artificial Intelligence",
                ShortDescription = "Learn Python, programming fundamentals, and AI.",
                Description = "This course covers Python basics, OOP, databases, web development, and machine learning.",
                Instructor = "John Doe",
                Duration = "14 weeks",
                StartDate = new DateTime(2025, 3, 1),
                ImageUrl = "https://placehold.co/800x600"
            };
        }
    }
}
