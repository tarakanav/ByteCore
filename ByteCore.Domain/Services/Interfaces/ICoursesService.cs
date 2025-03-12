using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.Domain.Entities;
using ByteCore.Model.Models;

namespace ByteCore.Domain.Services.Interfaces
{
    public interface ICoursesService
    {
        IEnumerable<Course> GetCourses();
        Course GetCourse(int id);
        bool IsUserEnrolled(int courseId, string email);
        Task EnrollUserAsync(int id, string email);
        Task CreateCourseAsync(Course course);
        Chapter GetChapter(int courseId, int chapterId);
    }
}