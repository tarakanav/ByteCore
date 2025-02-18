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
                        Title = "Модуль 1: Введение в Python",
                        Topics = new List<CourseRoadmapTopic>
                        {
                            new CourseRoadmapTopic
                            {
                                Title = "Знакомство с Python и установка среды разработки",
                                Lessons = new List<string>
                                {
                                    "Установка Python",
                                    "Настройка среды разработки"
                                }
                            },
                            new CourseRoadmapTopic
                            {
                                Title = "Регистрация в ChatGPT и настройка под себя",
                                Lessons = new List<string>
                                {
                                    "Создание аккаунта",
                                    "Настройка ChatGPT"
                                }
                            }
                        }
                    },
                    new CourseRoadmapModule
                    {
                        Title = "Модуль 2: Основы программирования на Python",
                        Topics = new List<CourseRoadmapTopic>
                        {
                            new CourseRoadmapTopic
                            {
                                Title = "Основы синтаксиса, переменные и типы данных",
                                Lessons = new List<string>
                                {
                                    "Переменные и их типы",
                                    "Основные операторы"
                                }
                            },
                            new CourseRoadmapTopic
                            {
                                Title = "Коллекции и циклы",
                                Lessons = new List<string>
                                {
                                    "Списки, кортежи, словари",
                                    "Циклы for и while"
                                }
                            }
                        }
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
                Title = "Python и Искусственный Интеллект",
                ShortDescription = "Изучение Python, основ программирования и ИИ",
                Description = "Курс охватывает основы Python, ООП, работу с базами данных, веб-разработку и машинное обучение.",
                Instructor = "John Doe",
                Duration = "14 недель",
                StartDate = new DateTime(2025, 3, 1),
                ImageUrl = "/images/python-course.jpg"
            };
        }
    }
}
