using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.Web.Models;
using ByteCore.Web.Services;

namespace ByteCore.Web.Controllers
{
    [RoutePrefix("Courses/{courseId:int}/Chapters")]
    public class ChapterController : Controller
    {
        private readonly ICoursesService _coursesService;

        public ChapterController(ICoursesService coursesService)
        {
            _coursesService = coursesService;
        }

        [Authorize]
        [Route("{chapterId:int}")]
        public ActionResult Index(int courseId, int chapterId)
        {
            var chapter = _coursesService.GetChapter(courseId, chapterId);
            if (chapter == null)
                return HttpNotFound();

            return View(chapter);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSectionProgress(int sectionId, bool isCompleted)
        {
            return Json(new { success = true });
        }

        [Authorize]
        [HttpPost]
        public ActionResult CompleteChapter(int courseId, int chapterId)
        {
            var course = _coursesService.GetCourse(courseId);
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