using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses/{courseId:int}/Chapters")]
    public class ChapterController : Controller
    {
        private readonly ICourseBl _courseBl;
        private readonly IQuizBl _quizBl;

        public ChapterController(ICourseBl courseBl, IQuizBl quizBl)
        {
            _courseBl = courseBl;
            _quizBl = quizBl;
        }

        [CustomAuthorize]
        [Route("{chapterId:int}")]
        public ActionResult Index(int courseId, int chapterId)
        {
            var chapter = _courseBl.GetChapter(courseId, chapterId);
            if (chapter == null)
                return HttpNotFound();

            var vm = new ChapterModel
            {
                Chapter = chapter,
                QuizResults = new Dictionary<int, QuizResult>()
            };

            return View(vm);
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{chapterId:int}")]
        public async Task<ActionResult> Index(
            int courseId,
            int chapterId,
            int sectionId,
            List<int> userAnswers)
        {
            try
            {
                var chapter = _courseBl.GetChapter(courseId, chapterId);
                if (chapter == null) return HttpNotFound();

                var section = chapter.Sections
                    .FirstOrDefault(s => s.Id == sectionId && s.Type == SectionType.Quiz);
                if (section?.Quiz == null)
                    return HttpNotFound();

                var results = new Dictionary<int, QuizResult>();

                if (ModelState.IsValid)
                {
                    var result = await _quizBl
                        .SubmitQuizResultAsync(section.Quiz.Id, userAnswers, User.Identity.Name);
                    results[sectionId] = result;
                }

                var vm = new ChapterModel
                {
                    Chapter = chapter,
                    QuizResults = results
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", new { courseId, chapterId });
            }
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult UpdateSectionProgress(int sectionId, bool isCompleted)
        {
            return Json(new { success = true });
        }

        [CustomAuthorize]
        [HttpPost]
        public ActionResult CompleteChapter(int courseId, int chapterId)
        {
            var course = _courseBl.GetCourse(courseId);
            if (course == null)
            {
                return HttpNotFound();
            }

            var chapter = course.Chapters.OrderBy(x => x.GetChapterNumber()).ElementAtOrDefault(chapterId - 1);
            if (chapter == null)
            {
                return HttpNotFound();
            }

            var nextChapter = course.Chapters.OrderBy(c => c.GetChapterNumber()).FirstOrDefault(c =>
                c.GetChapterNumber() > chapter.GetChapterNumber());
            return nextChapter != null
                ? RedirectToAction("Index", new { courseId = course.Id, chapterId = nextChapter.GetChapterNumber() })
                : RedirectToAction("Course", "Courses", new { id = course.Id });
        }
    }
}