using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses/{courseId:int}/Chapters")]
    public class ChapterController : Controller
    {
        public List<CourseModel> GetCourses()
        {
            var courses = new List<CourseModel>
            {
                new CourseModel
                {
                    Id = 1,
                    Title = "Python and Artificial Intelligence",
                    ShortDescription = "Learn Python, programming fundamentals, and AI.",
                    Description =
                        "This course covers Python basics, OOP, databases, web development, and machine learning.",
                    Instructor = "John Doe",
                    Duration = TimeSpan.FromDays(70),
                    StartDate = new DateTime(2025, 3, 1),
                    ImageUrl = "https://placehold.co/800x600",
                    Chapters = new List<ChapterModel>
                    {
                        new ChapterModel
                        {
                            Id = 1,
                            Title = "Introduction",
                            CourseId = 1,
                            Description = "Introduction to the course and Python programming.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 1,
                                    Title = "Welcome",
                                    TextContent =
                                        "Welcome to the course! In this section, we'll introduce the course objectives and structure.",
                                    Type = SectionType.Read,
                                    ChapterId = 1,
                                    Description = "Welcome to the course!"
                                },
                                new SectionModel
                                {
                                    Id = 2,
                                    Title = "Overview",
                                    TextContent =
                                        "This video provides an overview of what you'll learn throughout the course.",
                                    Type = SectionType.Video,
                                    ChapterId = 1,
                                    Description = "Course overview video"
                                }
                            }
                        },
                        new ChapterModel
                        {
                            Id = 2,
                            Title = "Getting Started",
                            CourseId = 1,
                            Description =
                                "Setting up the development environment and writing your first Python program.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 3,
                                    Title = "Setting Up Environment",
                                    TextContent =
                                        "Learn how to set up your development environment for Python programming.",
                                    Type = SectionType.Read,
                                    ChapterId = 2,
                                    Description = "Setting up the environment"
                                },
                                new SectionModel
                                {
                                    Id = 4,
                                    Title = "Hello World",
                                    TextContent = "Write your first Python program: the classic 'Hello, World!'.",
                                    Type = SectionType.Read,
                                    ChapterId = 2,
                                    Description = "Your first Python program"
                                }
                            }
                        },
                        new ChapterModel
                        {
                            Id = 3,
                            Title = "Python Basics",
                            CourseId = 1,
                            Description = "Learn the basics of Python programming language.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 5,
                                    Title = "Variables and Data Types",
                                    TextContent = "Understand variables and data types in Python.",
                                    Type = SectionType.Read,
                                    ChapterId = 3,
                                    Description = "Variables and data types"
                                },
                                new SectionModel
                                {
                                    Id = 6,
                                    Title = "Operators and Expressions",
                                    TextContent = "Learn about operators and expressions to perform calculations.",
                                    Type = SectionType.Read,
                                    ChapterId = 3,
                                    Description = "Operators and expressions"
                                }
                            }
                        },
                        new ChapterModel
                        {
                            Id = 4,
                            Title = "Object-Oriented Programming",
                            CourseId = 1,
                            Description = "Dive into object-oriented programming with Python.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 7,
                                    Title = "Classes and Objects",
                                    TextContent = "Explore inheritance and polymorphism concepts in Python OOP." +
                                                  "Learn how to create subclasses and override methods." +
                                                  "Understand polymorphism and how it simplifies code." +
                                                  "Discover the benefits of inheritance and polymorphism.\n" +
                                                  "Learn how to create subclasses and override methods." +
                                                  "Understand polymorphism and how it simplifies code." +
                                                  "Discover the benefits of inheritance and polymorphism." +
                                                  "Learn how to create subclasses and override methods." +
                                                  "Understand polymorphism and how it simplifies code.\n\n" +
                                                  "Discover the benefits of inheritance and polymorphism." +
                                                  "Learn how to create subclasses and override methods." +
                                                  "Understand polymorphism and how it simplifies code.",
                                    Type = SectionType.Read,
                                    ChapterId = 4,
                                    Description = "Classes and objects"
                                },
                                new SectionModel
                                {
                                    Id = 8,
                                    Title = "Inheritance and Polymorphism",
                                    TextContent = "Explore inheritance and polymorphism concepts in Python OOP." +
                                                  "Learn how to create subclasses and override methods." +
                                                  "Understand polymorphism and how it simplifies code." +
                                                    "Discover the benefits of inheritance and polymorphism.\n" +
                                                  "Learn how to create subclasses and override methods." +
                                                    "Understand polymorphism and how it simplifies code." +
                                                  "Discover the benefits of inheritance and polymorphism." +
                                                    "Learn how to create subclasses and override methods." +
                                                    "Understand polymorphism and how it simplifies code.\n\n" +
                                                  "Discover the benefits of inheritance and polymorphism." +
                                                    "Learn how to create subclasses and override methods." +
                                                    "Understand polymorphism and how it simplifies code.",
                                    Type = SectionType.Read,
                                    ChapterId = 4,
                                    Description = "Inheritance and polymorphism"
                                }
                            }
                        }
                    }
                },
                new CourseModel
                {
                    Id = 2,
                    Title = "Web Development using ASP.NET",
                    ShortDescription = "Learn ASP.NET, MVC, Web API & Entity Framework",
                    Description = "Course contains main information about web development using ASP.NET.",
                    Instructor = "Jane Smith",
                    Duration = TimeSpan.FromDays(70),
                    StartDate = new DateTime(2025, 4, 1),
                    ImageUrl = "https://placehold.co/800x600",
                    Chapters = new List<ChapterModel>
                    {
                        new ChapterModel
                        {
                            Id = 5,
                            Title = "Introduction",
                            CourseId = 2,
                            Description = "Introduction to the course and ASP.NET framework.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 9,
                                    Title = "Welcome",
                                    TextContent =
                                        "Welcome to the ASP.NET course! Let's start with an introduction to the framework.",
                                    Type = SectionType.Read,
                                    ChapterId = 5,
                                    Description = "Welcome to the course!"
                                },
                                new SectionModel
                                {
                                    Id = 10,
                                    Title = "Overview",
                                    TextContent =
                                        "This video provides an overview of ASP.NET and what you'll learn.",
                                    Type = SectionType.Video,
                                    ChapterId = 5,
                                    Description = "Course overview video"
                                }
                            }
                        },
                        new ChapterModel
                        {
                            Id = 6,
                            Title = "Getting Started",
                            CourseId = 2,
                            Description =
                                "Setting up the development environment and creating your first ASP.NET project.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 11,
                                    Title = "Setting Up Environment",
                                    TextContent = "Set up your development environment for ASP.NET applications.",
                                    Type = SectionType.Read,
                                    ChapterId = 6,
                                    Description = "Setting up the environment"
                                },
                                new SectionModel
                                {
                                    Id = 12,
                                    Title = "Hello World",
                                    TextContent =
                                        "Create your first ASP.NET application: the 'Hello, World!' project.",
                                    Type = SectionType.Read,
                                    ChapterId = 6,
                                    Description = "Your first ASP.NET project"
                                }
                            }
                        },
                        new ChapterModel
                        {
                            Id = 7,
                            Title = "ASP.NET Basics",
                            CourseId = 2,
                            Description = "Learn the basics of ASP.NET MVC and data access.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 13,
                                    Title = "Controllers and Views",
                                    TextContent = "Learn about controllers and views in ASP.NET MVC.",
                                    Type = SectionType.Read,
                                    ChapterId = 7,
                                    Description = "Controllers and views"
                                },
                                new SectionModel
                                {
                                    Id = 14,
                                    Title = "Models and Data Access",
                                    TextContent =
                                        "Understand models and how to access data using Entity Framework.",
                                    Type = SectionType.Read,
                                    ChapterId = 7,
                                    Description = "Models and data access"
                                }
                            }
                        },
                        new ChapterModel
                        {
                            Id = 8,
                            Title = "Web API",
                            CourseId = 2,
                            Description = "Learn how to build RESTful services with ASP.NET Web API.",
                            Sections = new List<SectionModel>
                            {
                                new SectionModel
                                {
                                    Id = 15,
                                    Title = "Introduction to Web API",
                                    TextContent =
                                        "Get introduced to building RESTful services with ASP.NET Web API.",
                                    Type = SectionType.Read,
                                    ChapterId = 8,
                                    Description = "Introduction to Web API"
                                },
                                new SectionModel
                                {
                                    Id = 16,
                                    Title = "Consuming Web API",
                                    TextContent = "Learn how to consume Web API services in your applications.",
                                    Type = SectionType.Read,
                                    ChapterId = 8,
                                    Description = "Consuming Web API"
                                }
                            }
                        }
                    }
                }
            };

            foreach (var course in courses)
            {
                foreach (var chapter in course.Chapters)
                {
                    foreach (var section in chapter.Sections)
                    {
                        section.Chapter = chapter;
                    }

                    chapter.Course = course;
                }
            }

            return courses;
        }

        [Route("{chapterId:int}")]
        public ActionResult Index(int courseId, int chapterId)
        {
            var chapter = GetChapterById(courseId, chapterId);
            if (chapter == null)
                return HttpNotFound();

            return View(chapter);
        }

        [HttpPost]
        public ActionResult UpdateSectionProgress(int sectionId, bool isCompleted)
        {
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult CompleteChapter(int courseId, int chapterId)
        {
            var course = GetCourses().FirstOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return HttpNotFound();
            }

            var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
            if (chapter == null)
            {
                return HttpNotFound();
            }

            // Mark the chapter as completed
            // Redirect to the next chapter
            var nextChapter = course.Chapters.OrderBy(c => c.GetChapterNumber()).FirstOrDefault(c =>
                c.GetChapterNumber() > chapter.GetChapterNumber());
            return nextChapter != null
                ? RedirectToAction("Index", new { courseId = course.Id, chapterId = nextChapter.Id })
                : RedirectToAction("Course", "Courses", new { id = course.Id });
        }


        private ChapterModel GetChapterById(int courseId, int chapterId)
        {
            var course = GetCourses().FirstOrDefault(c => c.Id == courseId);
            return course?.Chapters.FirstOrDefault(c => c.GetChapterNumber() == chapterId);
        }
    }
}