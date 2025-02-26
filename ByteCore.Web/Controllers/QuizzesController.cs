using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Quizzes")]
    public class QuizzesController : Controller
    {
        private readonly List<QuizModel> _quizzes = new List<QuizModel>
        {
            new QuizModel
            {
                Id = 1,
                Title = "C# Basics Quiz",
                RewardPoints = 100,
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        QuestionText = "What is the default access modifier for a class in C#?",
                        Options = new List<string> { "private", "public", "internal", "protected" },
                        CorrectOptionIndex = 2
                    },
                    new QuestionModel
                    {
                        QuestionText = "Which keyword is used to define a method in C#?",
                        Options = new List<string> { "func", "def", "method", "void" }, CorrectOptionIndex = 3
                    }
                }
            },
            new QuizModel
            {
                Id = 2,
                Title = "ASP.NET MVC Quiz",
                RewardPoints = 150,
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        QuestionText = "What does MVC stand for?",
                        Options = new List<string>
                        {
                            "Model View Container", "Model View Controller", "Module View Controller",
                            "Model Variable Controller"
                        },
                        CorrectOptionIndex = 1
                    },
                    new QuestionModel
                    {
                        QuestionText = "Which method is used to render a view in a controller?",
                        Options = new List<string> { "RenderView()", "DisplayView()", "ReturnView()", "View()" },
                        CorrectOptionIndex = 3
                    }
                }
            },
            new QuizModel
            {
                Id = 3,
                Title = "JavaScript Fundamentals Quiz",
                RewardPoints = 120,
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        QuestionText = "Which keyword declares a variable?",
                        Options = new List<string> { "var", "int", "string", "declare" }, CorrectOptionIndex = 0
                    },
                    new QuestionModel
                    {
                        QuestionText = "What does '===' check?",
                        Options = new List<string> { "Only value", "Only type", "Value and type", "None of these" },
                        CorrectOptionIndex = 2
                    }
                }
            },
            new QuizModel
            {
                Id = 4,
                Title = "SQL & Databases Quiz",
                RewardPoints = 180,
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        QuestionText = "What is SQL?",
                        Options = new List<string>
                        {
                            "Structured Question Language", "Simple Query Language", "Structured Query Language",
                            "Standard Query Language"
                        },
                        CorrectOptionIndex = 2
                    },
                    new QuestionModel
                    {
                        QuestionText = "Which SQL statement is used to retrieve data?",
                        Options = new List<string> { "GET", "SELECT", "FETCH", "PULL" }, CorrectOptionIndex = 1
                    }
                }
            },
            new QuizModel
            {
                Id = 5,
                Title = "Data Structures & Algorithms Quiz",
                RewardPoints = 200,
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        QuestionText = "Which data structure follows LIFO?",
                        Options = new List<string> { "Queue", "Stack", "Array", "Linked List" }, CorrectOptionIndex = 1
                    },
                    new QuestionModel
                    {
                        QuestionText = "What is the worst-case complexity of QuickSort?",
                        Options = new List<string> { "O(n)", "O(log n)", "O(n^2)", "O(n log n)" },
                        CorrectOptionIndex = 2
                    }
                }
            }
        };

        // GET: Quizzes
        public ActionResult Index()
        {
            return View(_quizzes);
        }

        // GET: Quizzes/1
        [Route("{id:int}")]
        public ActionResult Quiz(int id)
        {
            var quiz = _quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(quiz);
        }

        // POST: Quizzes/1
        [HttpPost]
        [Route("{id:int}")]
        public ActionResult Submit(int id, List<int> userAnswers)
        {
            var quiz = _quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var correctAnswers = quiz.Questions
                .Where((t, i) => i < userAnswers.Count && userAnswers[i] == t.CorrectOptionIndex)
                .Count();

            ViewBag.Score = correctAnswers;
            ViewBag.Total = quiz.Questions.Count;
            ViewBag.RewardPoints = quiz.RewardPoints;
            return View("Result");
        }
    }
}