using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ByteCore.Web.Models;
using ByteCore.Web.Services;

namespace ByteCore.Web.Controllers
{
    [Authorize]
    [RoutePrefix("Quizzes")]
    public class QuizzesController : Controller
    {
        private readonly IQuizzesService _quizzesService;

        public QuizzesController(IQuizzesService quizzesService)
        {
            _quizzesService = quizzesService;
        }

        // GET: Quizzes
        public ActionResult Index()
        {
            var quizzes = _quizzesService.GetQuizzes();
            return View(quizzes);
        }

        // GET: Quizzes/1
        [Route("{id:int}")]
        public ActionResult Quiz(int id)
        {
            var quiz = _quizzesService.GetQuiz(id);

            if (quiz == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(quiz);
        }

        // POST: Quizzes/1
        [HttpPost]
        [Route("{id:int}")]
        public async Task<ActionResult> Submit(int id, List<int> userAnswers)
        {
            try
            {
                var quizResult = await _quizzesService.SubmitQuizResultAsync(id, userAnswers, User.Identity.Name);
                return RedirectToAction("Result", new { id, resultId = quizResult.Id });
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("NotFound", "Error");
            }
        }

        // GET: Quizzes/1/Result
        [Route("{id:int}/Result/{resultId:int}")]
        public ActionResult Result(int id, int resultId)
        {
            var quizResult = _quizzesService.GetQuizResult(id, resultId);

            if (quizResult == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(quizResult);
        }
        
        // GET: Quizzes/Create
        [Authorize]
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            var quiz = new QuizModel
            {
                Questions = new List<QuestionModel>
                {
                    new QuestionModel
                    {
                        Options = new List<QuestionOptionModel>
                        {
                            new QuestionOptionModel(),
                            new QuestionOptionModel(),
                            new QuestionOptionModel(),
                            new QuestionOptionModel()
                        }
                    }
                }
            };
            return View(quiz);
        }

        // POST: Quizzes/Create
        [Authorize]
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(QuizModel quiz)
        {
            if (quiz.Questions.Count > 25 || quiz.Questions.Count == 0)
            {
                ModelState.AddModelError("Questions", "A quiz can have a maximum of 25 questions and a minimum of 1 question.");
            }
            
            if (quiz.Questions.Any(q => q.Options.Count > 16 || q.Options.Count == 0))
            {
                ModelState.AddModelError("Questions", "A question can have a maximum of 16 options and a minimum of 1 option.");
            }
            
            if (string.IsNullOrWhiteSpace(quiz.Title) || quiz.Title.Length > 100)
            {
                ModelState.AddModelError("Title", "Title is required and must be less than 100 characters.");
            }
            
            if (quiz.Questions.Any(q => string.IsNullOrWhiteSpace(q.QuestionText)))
            {
                ModelState.AddModelError("Questions", "Question text is required.");
            }
            
            if (quiz.Questions.Any(q => q.Options.Any(o => string.IsNullOrWhiteSpace(o.OptionText))))
            {
                ModelState.AddModelError("Questions", "Option text is required.");
            }

            if (quiz.RewardPoints < 0)
            {
                ModelState.AddModelError("RewardPoints", "Reward points must be greater than or equal to 0.");
            }
            
            if (quiz.PassingPercentage < 0 || quiz.PassingPercentage > 100)
            {
                ModelState.AddModelError("PassingPercentage", "Passing percentage must be between 0 and 100.");
            }
            
            if (ModelState.IsValid)
            {
                await _quizzesService.AddQuizAsync(quiz);
                return RedirectToAction("Quiz", new { id = quiz.Id });
            }
            return View(quiz);
        }

        // GET: Quizzes/1/Edit
        [Authorize]
        [Route("{id:int}/Edit")]
        public ActionResult Edit(int id)
        {
            var quiz = _quizzesService.GetQuiz(id);

            if (quiz == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(quiz);
        }
        
        // POST: Quizzes/1/Edit
        [Authorize]
        [HttpPost]
        [Route("{id:int}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, QuizModel quiz)
        {
            if (quiz.Questions.Count > 25 || quiz.Questions.Count == 0)
            {
                ModelState.AddModelError("Questions", "A quiz can have a maximum of 25 questions and a minimum of 1 question.");
            }
            
            if (quiz.Questions.Any(q => q.Options.Count > 16 || q.Options.Count == 0))
            {
                ModelState.AddModelError("Questions", "A question can have a maximum of 16 options and a minimum of 1 option.");
            }
            
            if (string.IsNullOrWhiteSpace(quiz.Title) || quiz.Title.Length > 100)
            {
                ModelState.AddModelError("Title", "Title is required and must be less than 100 characters.");
            }
            
            if (quiz.Questions.Any(q => string.IsNullOrWhiteSpace(q.QuestionText)))
            {
                ModelState.AddModelError("Questions", "Question text is required.");
            }
            
            if (quiz.Questions.Any(q => q.Options.Any(o => string.IsNullOrWhiteSpace(o.OptionText))))
            {
                ModelState.AddModelError("Questions", "Option text is required.");
            }

            if (quiz.RewardPoints < 0)
            {
                ModelState.AddModelError("RewardPoints", "Reward points must be greater than or equal to 0.");
            }
            
            if (quiz.PassingPercentage < 0 || quiz.PassingPercentage > 100)
            {
                ModelState.AddModelError("PassingPercentage", "Passing percentage must be between 0 and 100.");
            }
            
            if (ModelState.IsValid)
            {
                await _quizzesService.UpdateQuizAsync(id, quiz);
                return RedirectToAction("Quiz", new { id });
            }
            return View(quiz);
        }
        
        // POST: Quizzes/1/Delete
        [Authorize]
        [HttpPost]
        [Route("{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var quiz = _quizzesService.GetQuiz(id);
            if (quiz == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            await _quizzesService.DeleteQuizAsync(id);
            return RedirectToAction("Index");
        }

    }
}