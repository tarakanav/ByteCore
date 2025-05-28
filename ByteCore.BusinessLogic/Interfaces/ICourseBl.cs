using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Interfaces
{
    public interface ICourseBl
    {
        List<Course> GetCourses(int page = 1, int pageSize = 20);
        Course GetCourse(int id);
        bool IsUserEnrolled(int courseId, string email);
        Task EnrollUserAsync(int id, string email);
        Task CreateCourseAsync(Course course);
        Chapter GetChapter(int courseId, int chapterNumber);
        void MarkChapterAsCompleted(int courseId, int chapterNumber, string userEmail);
        void MarkChapterAsIncompleted(int courseId, int chapterNumber, string userEmail);
        Task UpdateCourseAsync(Course course);
        Task UpdateChapterAsync(Chapter chapter);
        Task DeleteCourseAsync(int id);
        Task DeleteChapterAsync(int courseId, int chapterId);
        Task AddChapterAsync(Chapter chapter);
        int GetCourseCount();
        int GetEnrollmentCount();
        UserCourse GetLatestEnrollment();
        List<int> GetEnrollmentCountByDate(DateTime fromDate, DateTime toDate);
    }
}