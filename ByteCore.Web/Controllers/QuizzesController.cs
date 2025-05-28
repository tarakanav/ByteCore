using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ByteCore.BusinessLogic;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.QuizScope;

namespace ByteCore.Web.Controllers
{
    [CustomAuthorize]
    [RoutePrefix("Quizzes")]
    public class QuizzesController : BaseController
    {
        // GET: Quizzes
        public ActionResult Index(int page = 1)
        {
            var quizBl = Bl.GetQuizBl();
            if (page < 1)
            {
                page = 1;
            }
            const int pageSize = 10;
            var quizzes = quizBl.GetQuizzes(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)quizBl.GetQuizCount() / pageSize);
            return View(quizzes);
        }

        // GET: Quizzes/1
        [Route("{id:int}")]
        public ActionResult Quiz(int id)
        {
            var quizBl = Bl.GetQuizBl();
            var quiz = quizBl.GetQuiz(id);

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
            var quizBl = Bl.GetQuizBl();
            try
            {
                var quizResult = await quizBl.SubmitQuizResultAsync(id, userAnswers, User.Identity.Name);
                return RedirectToAction("Result", new { id, resultId = quizResult.Id });
            }
            catch (InvalidOperationException)
            {
                ModelState.AddModelError("", "An error occurred while submitting the quiz. Please try again.");
                var quiz = quizBl.GetQuiz(id);
                return View("Quiz", quiz);
            }
        }

        // GET: Quizzes/1/Result
        [Route("{id:int}/Result/{resultId:int}")]
        public ActionResult Result(int id, int resultId)
        {
            var quizBl = Bl.GetQuizBl();
            var quizResult = quizBl.GetQuizResult(id, resultId);

            if (quizResult == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(quizResult);
        }
        
        // GET: Quizzes/Create
        [CustomAuthorize("Admin")]
        [HttpGet]
        [Route("Create")]
        public ActionResult Create()
        {
            var quiz = new Quiz();
            return View(quiz);
        }

        // POST: Quizzes/Create
        [CustomAuthorize("Admin")]
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Quiz quiz)
        {
            var quizBl = Bl.GetQuizBl();
            ValidateQuiz(quiz);
            if (!ModelState.IsValid)
                return View(quiz);

            await quizBl.AddQuizAsync(quiz);
            return RedirectToAction("Quiz", new { id = quiz.Id });
        }

        // GET: Quizzes/1/Edit
        [CustomAuthorize("Admin")]
        [Route("{id:int}/Edit")]
        public ActionResult Edit(int id)
        {
            var quizBl = Bl.GetQuizBl();
            var quiz = quizBl.GetQuiz(id);

            if (quiz == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            return View(quiz);
        }
        
        // POST: Quizzes/1/Edit
        [CustomAuthorize("Admin")]
        [HttpPost]
        [Route("{id:int}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Quiz quiz)
        {
            var quizBl = Bl.GetQuizBl();
            ValidateQuiz(quiz);
            if (!ModelState.IsValid)
                return View(quiz);

            await quizBl.UpdateQuizAsync(id, quiz);
            return RedirectToAction("Quiz", new { id });
        }
        
        // POST: Quizzes/1/Delete
        [CustomAuthorize("Admin")]
        [HttpPost]
        [Route("{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var quizBl = Bl.GetQuizBl();
            var quiz = quizBl.GetQuiz(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }

            await quizBl.DeleteQuizAsync(id);
            return RedirectToAction("Index");
        }
        
        private void ValidateQuiz(Quiz quiz)
        {
            if (quiz.Questions.Count < 1 || quiz.Questions.Count > 25)
                ModelState.AddModelError("Questions",
                    "A quiz must have between 1 and 25 questions.");

            if (string.IsNullOrWhiteSpace(quiz.Title) || quiz.Title.Length > 100)
                ModelState.AddModelError("Title",
                    "Title is required and must be under 100 characters.");

            if (quiz.RewardPoints < 0)
                ModelState.AddModelError("RewardPoints",
                    "Reward points must be 0 or more.");

            if (quiz.PassingPercentage < 0 || quiz.PassingPercentage > 100)
                ModelState.AddModelError("PassingPercentage",
                    "Passing percentage must be between 0 and 100.");

            for (int qi = 0; qi < quiz.Questions.Count; qi++)
            {
                var q = quiz.Questions[qi];
                var qKey = $"Questions[{qi}]";

                if (string.IsNullOrWhiteSpace(q.QuestionText))
                    ModelState.AddModelError($"{qKey}.QuestionText",
                        $"Question #{qi+1}: text is required.");

                if (q.Options.Count < 1 || q.Options.Count > 16)
                    ModelState.AddModelError($"{qKey}.Options",
                        $"Question #{qi+1}: must have 1–16 options.");

                for (int oi = 0; oi < q.Options.Count; oi++)
                {
                    if (string.IsNullOrWhiteSpace(q.Options[oi].OptionText))
                        ModelState.AddModelError(
                            $"{qKey}.Options[{oi}].OptionText",
                            $"Question #{qi+1}, option #{oi+1}: text is required.");
                }
            }
        }
    }
}