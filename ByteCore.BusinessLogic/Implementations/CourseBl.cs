using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.APIs;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class CourseBl : CourseApi, ICourseBl
    {
        public List<Course> GetCourses(int page = 1, int pageSize = 20)
        {
            return GetCoursesAction(page, pageSize);
        }

        public Course GetCourse(int id)
        {
            return GetCourseAction(id);
        }

        public Chapter GetChapter(int courseId, int chapterNumber)
        {
            return GetChapterAction(courseId, chapterNumber);
        }

        public bool IsUserEnrolled(int courseId, string email)
        {
            return IsUserEnrolledAction(courseId, email);
        }

        public Task EnrollUserAsync(int id, string email)
        {
            return EnrollUserAction(id, email);
        }
        
        private void PerformBlCourseValidation(Course course, bool isUpdate)
        {
            if (string.IsNullOrWhiteSpace(course.Title))
                throw new InvalidOperationException("Course title is required.");
            if (string.IsNullOrWhiteSpace(course.Description))
                throw new InvalidOperationException("Course description is required.");

            if (course.Chapters != null)
            {
                var chapterNumbers = new HashSet<int>();
                foreach (var chapter in course.Chapters.OrderBy(c => c.ChapterNumber))
                {
                    if (chapter.ChapterNumber <= 0)
                        throw new InvalidOperationException("Chapter number must be greater than 0.");
                    if (!chapterNumbers.Add(chapter.ChapterNumber))
                        throw new InvalidOperationException($"Duplicate chapter number detected: {chapter.ChapterNumber}.");
                    if (string.IsNullOrWhiteSpace(chapter.Title))
                        throw new InvalidOperationException("Chapter title is required.");
                    
                    if (chapter.Sections == null) continue;
                    foreach (var section in chapter.Sections)
                    {
                        if (string.IsNullOrWhiteSpace(section.Title))
                            throw new InvalidOperationException("Section title is required.");
                        switch (section.Type)
                        {
                            case SectionType.Read when string.IsNullOrWhiteSpace(section.TextContent):
                                throw new InvalidOperationException("Text content is required for read section.");
                            case SectionType.Video when string.IsNullOrWhiteSpace(section.VideoUrl):
                                throw new InvalidOperationException("Video URL is required for video section.");
                            case SectionType.Quiz when section.QuizId == null:
                                throw new InvalidOperationException("Quiz ID is required for quiz section.");
                        }
                    }
                }
            }
        }

        public async Task CreateCourseAsync(Course course)
        {
            if (course.Chapters != null)
            {
                var sortedChapters = course.Chapters.OrderBy(c => c.ChapterNumber).ToList();
                for (var i = 0; i < sortedChapters.Count; i++)
                {
                    sortedChapters[i].ChapterNumber = i + 1;
                }
                course.Chapters = sortedChapters;
            }
            PerformBlCourseValidation(course, false);
            await CreateCourseAction(course);
        }

        public async Task UpdateCourseAsync(Course course)
        {
            PerformBlCourseValidation(course, true);
            await UpdateCourseAction(course);
        }

        public void MarkChapterAsCompleted(int courseId, int chapterNumber, string userEmail)
        {
            MarkChapterAsCompletedAction(courseId, chapterNumber, userEmail);
        }

        public void MarkChapterAsIncompleted(int courseId, int chapterNumber, string userEmail)
        {
            MarkChapterAsIncompletedAction(courseId, chapterNumber, userEmail);
        }
        
        private void PerformBlChapterValidation(Chapter chapter)
        {
            if (string.IsNullOrWhiteSpace(chapter.Title))
                 throw new InvalidOperationException("Chapter title is required.");
            if (chapter.Sections != null) 
            {
                 foreach (var section in chapter.Sections)
                 {
                     if (string.IsNullOrWhiteSpace(section.Title))
                         throw new InvalidOperationException("Section title is required.");
                     switch (section.Type)
                     {
                         case SectionType.Read when string.IsNullOrWhiteSpace(section.TextContent):
                             throw new InvalidOperationException("Text content is required for read section.");
                         case SectionType.Video when string.IsNullOrWhiteSpace(section.VideoUrl):
                             throw new InvalidOperationException("Video URL is required for video section.");
                         case SectionType.Quiz when section.QuizId == null:
                             throw new InvalidOperationException("Quiz ID is required for quiz section.");
                     }
                 }
            }
        }
        public async Task UpdateChapterAsync(Chapter chapter)
        {
            PerformBlChapterValidation(chapter);
            await UpdateChapterAction(chapter);
        }

        public async Task DeleteCourseAsync(int id)
        {
            await DeleteCourseAction(id);
        }

        public async Task DeleteChapterAsync(int courseId, int chapterIdOrNumber)
        {
            await DeleteChapterAction(courseId, chapterIdOrNumber);
        }

        public async Task AddChapterAsync(Chapter chapter)
        {
            PerformBlChapterValidation(chapter);
            await AddChapterAction(chapter);
        }
        
        public int GetCourseCount()
        {
            return GetCourseCountAction();
        }

        public int GetEnrollmentCount()
        {
            return GetEnrollmentCountAction();
        }

        public UserCourse GetLatestEnrollment()
        {
            return GetLatestEnrollmentAction();
        }

        public List<int> GetEnrollmentCountByDate(DateTime fromDate, DateTime toDate)
        {
            return GetEnrollmentCountByDateAction(fromDate, toDate);
        }
    }
}