using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ByteCore.BusinessLogic;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses/{courseId:int}/Chapters")]
    public class ChapterController : BaseController
    {
        [CustomAuthorize]
        [Route("{chapterId:int}")]
        public ActionResult Index(int courseId, int chapterId)
        {
            var courseBl = Bl.GetCourseBl();
            if (!courseBl.IsUserEnrolled(courseId, User.Identity.Name))
            {
                return RedirectToAction("Course", "Courses", new { id = courseId });
            }
            
            var chapter = courseBl.GetChapter(courseId, chapterId);
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
            var courseBl = Bl.GetCourseBl();
            var quizBl = Bl.GetQuizBl();
            try
            {
                if (!courseBl.IsUserEnrolled(courseId, User.Identity.Name))
                {
                    return RedirectToAction("Course", "Courses", new { id = courseId });
                }
                
                var chapter = courseBl.GetChapter(courseId, chapterId);
                if (chapter == null) return HttpNotFound();
                
                var section = chapter.Sections
                    .FirstOrDefault(s => s.GetSectionNumber() == sectionId && s.Type == SectionType.Quiz);
                if (section?.Quiz == null)
                    return HttpNotFound();

                var results = new Dictionary<int, QuizResult>();

                if (ModelState.IsValid)
                {
                    var result = await quizBl
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
        [ValidateAntiForgeryToken]
        [Route("{chapterNumber:int}/Complete")]
        public ActionResult CompleteChapter(int courseId, int chapterNumber)
        {
            var courseBl = Bl.GetCourseBl();
            if (!courseBl.IsUserEnrolled(courseId, User.Identity.Name))
            {
                return RedirectToAction("Course", "Courses", new { id = courseId });
            }
            
            var course = courseBl.GetCourse(courseId);
            if (course == null)
            {
                return HttpNotFound();
            }

            var chapter = course.Chapters.FirstOrDefault(x => x.ChapterNumber == chapterNumber);
            if (chapter == null)
            {
                return HttpNotFound();
            }

            courseBl.MarkChapterAsCompleted(courseId, chapterNumber, User.Identity.Name);
            var nextChapter = course.Chapters.OrderBy(c => c.ChapterNumber).FirstOrDefault(c =>
                c.ChapterNumber > chapter.ChapterNumber);
            return nextChapter != null
                ? RedirectToAction("Index", new { courseId = course.Id, chapterId = nextChapter.ChapterNumber })
                : RedirectToAction("Course", "Courses", new { id = course.Id });
        }

        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{chapterNumber:int}/Uncomplete")]
        public ActionResult UncompleteChapter(int courseId, int chapterNumber)
        {
            var courseBl = Bl.GetCourseBl();
            var course = courseBl.GetCourse(courseId);
            if (course == null)
            {
                return HttpNotFound();
            }

            var chapter = course.Chapters.FirstOrDefault(x => x.ChapterNumber == chapterNumber);
            if (chapter == null)
            {
                return HttpNotFound();
            }

            courseBl.MarkChapterAsIncompleted(courseId, chapterNumber, User.Identity.Name);

            return RedirectToAction("Index", new { courseId, chapterId = chapterNumber });
        }


        [CustomAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{chapterNumber:int}/MarkComplete")]
        public JsonResult MarkChapterComplete(int courseId, int chapterNumber)
        {
            var courseBl = Bl.GetCourseBl();
            if (!courseBl.IsUserEnrolled(courseId, User.Identity.Name))
            {
                return Json(new { success = false, error = "User not enrolled" });
            }
            
            var course = courseBl.GetCourse(courseId);
            if (course == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { success = false, error = "Course not found" });
            }

            var chapter = course.Chapters.FirstOrDefault(x => x.ChapterNumber == chapterNumber);
            if (chapter == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { success = false, error = "Chapter not found" });
            }

            courseBl.MarkChapterAsCompleted(courseId, chapterNumber, User.Identity.Name);

            return Json(new { success = true });
        }

        // GET: /Courses/{courseId}/Chapters/{chapterId}/Edit
        [CustomAuthorize("Moderator,Admin")]
        [HttpGet]
        [Route("{chapterId:int}/Edit")]
        public ActionResult Edit(int courseId, int chapterId)
        {
            var courseBl = Bl.GetCourseBl();
            var chapter = courseBl.GetChapter(courseId, chapterId);
            if (chapter == null)
                return HttpNotFound();

            return View(chapter);
        }

        // POST: /Courses/{courseId}/Chapters/{chapterId}/Edit
        [CustomAuthorize("Moderator,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{chapterId:int}/Edit")]
        public async Task<ActionResult> Edit(Chapter model)
        {
            var courseBl = Bl.GetCourseBl();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await courseBl.UpdateChapterAsync(model);
                return RedirectToAction("Edit", "Courses", new { id = model.CourseId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        
        // POST: /Courses/{courseId}/Chapters/{chapterId}/Delete
        [CustomAuthorize("Moderator,Admin")]
        [HttpPost]
        [Route("{chapterId:int}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int courseId, int chapterId)
        {
            var courseBl = Bl.GetCourseBl();
            var quiz = courseBl.GetChapter(courseId, chapterId);
            if (quiz == null)
            {
                return HttpNotFound();
            }

            await courseBl.DeleteChapterAsync(courseId, chapterId);
            return RedirectToAction("Index", "Courses");
        }
        
        // GET: /Courses/5/Chapters/Create
        [CustomAuthorize("Moderator,Admin")]
        [HttpGet]
        [Route("Create")]
        public ActionResult Create(int courseId)
        {
            var courseBl = Bl.GetCourseBl();
            var course = courseBl.GetCourse(courseId);
            if (course == null) return HttpNotFound();

            var chapter = new Chapter { CourseId = courseId };
            return View(chapter);
        }

        // POST: /Courses/5/Chapters/Create
        [CustomAuthorize("Moderator,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<ActionResult> Create(Chapter model)
        {
            var courseBl = Bl.GetCourseBl();
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await courseBl.AddChapterAsync(model);
                return RedirectToAction("Edit", "Courses", new { id = model.CourseId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}