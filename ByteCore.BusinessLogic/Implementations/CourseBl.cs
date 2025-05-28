using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.APIs;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class CourseBl : CourseApi, ICourseBl
    {
        public IEnumerable<Course> GetCourses(int page = 1, int pageSize = 20) => GetCoursesAction(page, pageSize);
        public Course GetCourse(int id) => GetCourseAction(id);
        public Chapter GetChapter(int courseId, int chapterNumber) => GetChapterAction(courseId, chapterNumber);
        public bool IsUserEnrolled(int courseId, string email) => IsUserEnrolledAction(courseId, email);
        public Task EnrollUserAsync(int id, string email) => EnrollUserAction(id, email);
        public Task CreateCourseAsync(Course course) => CreateCourseAction(course);
        public Task UpdateCourseAsync(Course course) => UpdateCourseAction(course);
        public Task UpdateChapterAsync(Chapter chapter) => UpdateChapterAction(chapter);
        public Task DeleteCourseAsync(int id) => DeleteCourseAction(id);
        public Task DeleteChapterAsync(int courseId, int chapterId) => DeleteChapterAction(courseId, chapterId);
        public Task AddChapterAsync(Chapter chapter) => AddChapterAction(chapter);
        public int GetCourseCount() => GetCourseCountAction();
        public int GetEnrollmentCount() => GetEnrollmentCountAction();
        public UserCourse GetLatestEnrollment() => GetLatestEnrollmentAction();
        public IEnumerable<int> GetEnrollmentCountByDate(DateTime fromDate, DateTime toDate) => GetEnrollmentCountByDateAction(fromDate, toDate);
        public void MarkChapterAsCompleted(int courseId, int chapterNumber, string userEmail) => MarkChapterAsCompletedAction(courseId, chapterNumber, userEmail);
        public void MarkChapterAsIncompleted(int courseId, int chapterNumber, string userEmail) => MarkChapterAsIncompletedAction(courseId, chapterNumber, userEmail);
    }
}