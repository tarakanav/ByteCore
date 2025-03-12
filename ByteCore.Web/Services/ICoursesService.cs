using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.Web.Models;

namespace ByteCore.Web.Services
{
    public interface ICoursesService
    {
        IEnumerable<CourseModel> GetCourses();
        CourseModel GetCourse(int id);
        bool IsUserEnrolled(int courseId, string email);
        Task EnrollUserAsync(int id, string email);
        Task CreateCourseAsync(CourseModel course);
        ChapterModel GetChapter(int courseId, int chapterId);
    }
}