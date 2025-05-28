using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.APIs
{
    public class CourseApi
    {
        internal List<Course> GetCoursesAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Courses
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        internal Course GetCourseAction(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var course = db.Courses
                    .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                    .Include(c => c.Chapters.Select(ch => ch.Sections.Select(s => s.Quiz.Questions.Select(q => q.Options))))
                    .FirstOrDefault(x => x.Id == id);
                if (course != null)
                    course.Chapters = course.Chapters.OrderBy(x => x.ChapterNumber).ToList();
                return course;
            }
        }

        internal Chapter GetChapterAction(int courseId, int chapterNumber)
        {
            var course = GetCourseAction(courseId);
            return course?.Chapters.FirstOrDefault(c => c.ChapterNumber == chapterNumber);
        }

        internal bool IsUserEnrolledAction(int courseId, string email)
        {
            using (var db = new ApplicationDbContext())
            {
                var course = db.Courses
                    .Include(c => c.EnrolledUsers.Select(eu => eu.User))
                    .FirstOrDefault(x => x.Id == courseId);
                return course != null && course.EnrolledUsers.Any(x => x.User.Email == email);
            }
        }

        internal Task EnrollUserAction(int id, string email)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(x => x.Email == email);
                var course = db.Courses.Find(id);
                if (user == null || course == null)
                    throw new KeyNotFoundException("User or course not found.");
                db.UserCourses.Add(new UserCourse { Course = course, User = user });
                db.SaveChanges();
                return Task.CompletedTask;
            }
        }

        internal async Task CreateCourseAction(Course course)
        {
            using (var db = new ApplicationDbContext())
            {
                for (var i = 1; i <= course.Chapters.Count; i++)
                    course.Chapters[i - 1].ChapterNumber = i;
                await ValidateCourseAsync(course, false);
                foreach (var section in course.Chapters.SelectMany(ch => ch.Sections ?? new List<Section>()))
                {
                    section.TextContent = section.Type == SectionType.Read ? section.TextContent : null;
                    section.VideoUrl = section.Type == SectionType.Video ? section.VideoUrl : null;
                    section.Quiz = section.Type == SectionType.Quiz ? section.Quiz : null;
                }
                db.Courses.Add(course);
                await db.SaveChangesAsync();
            }
        }

        internal async Task UpdateCourseAction(Course course)
        {
            using (var db = new ApplicationDbContext())
            {
                await ValidateCourseAsync(course, true);
                var existing = await db.Courses.Include(c => c.Chapters).FirstOrDefaultAsync(c => c.Id == course.Id);
                if (existing == null)
                    throw new InvalidOperationException("Course not found.");
                existing.Title = course.Title;
                existing.ShortDescription = course.ShortDescription;
                existing.Description = course.Description;
                existing.Instructor = course.Instructor;
                existing.Duration = course.Duration;
                existing.StartDate = course.StartDate;
                existing.ImageUrl = course.ImageUrl;
                foreach (var upd in course.Chapters)
                {
                    var chap = existing.Chapters.FirstOrDefault(c => c.Id == upd.Id);
                    if (chap != null)
                        chap.ChapterNumber = upd.ChapterNumber;
                }
                await db.SaveChangesAsync();
            }
        }

        internal void MarkChapterAsCompletedAction(int courseId, int chapterNumber, string userEmail)
        {
            using (var db = new ApplicationDbContext())
            {
                var chapter = db.Courses
                    .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                    .FirstOrDefault(c => c.Id == courseId)?
                    .Chapters.FirstOrDefault(ch => ch.ChapterNumber == chapterNumber);
                var user = db.Users.FirstOrDefault(u => u.Email == userEmail);
                if (chapter != null && user != null && !chapter.UsersCompleted.Contains(user))
                {
                    chapter.UsersCompleted.Add(user);
                    db.SaveChanges();
                }
            }
        }

        internal void MarkChapterAsIncompletedAction(int courseId, int chapterNumber, string userEmail)
        {
            using (var db = new ApplicationDbContext())
            {
                var chapter = db.Courses
                    .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                    .FirstOrDefault(c => c.Id == courseId)?
                    .Chapters.FirstOrDefault(ch => ch.ChapterNumber == chapterNumber);
                var user = db.Users.FirstOrDefault(u => u.Email == userEmail);
                if (chapter != null && user != null && chapter.UsersCompleted.Contains(user))
                {
                    chapter.UsersCompleted.Remove(user);
                    db.SaveChanges();
                }
            }
        }

        internal async Task UpdateChapterAction(Chapter chapter)
        {
            using (var db = new ApplicationDbContext())
            {
                if (string.IsNullOrWhiteSpace(chapter.Title))
                    throw new InvalidOperationException("Chapter title is required");
                var course = await db.Courses.Include(c => c.Chapters.Select(ch => ch.Sections)).FirstOrDefaultAsync(c => c.Id == chapter.CourseId);
                var existing = course?.Chapters.FirstOrDefault(ch => ch.Id == chapter.Id);
                if (existing == null)
                    throw new InvalidOperationException("Chapter not found.");
                existing.Title = chapter.Title;
                existing.Description = chapter.Description;
                await db.SaveChangesAsync();
            }
        }

        internal async Task DeleteCourseAction(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var course = await db.Courses
                    .Include(c => c.EnrolledUsers)
                    .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                    .Include(c => c.Chapters.Select(ch => ch.Sections))
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                    throw new InvalidOperationException("Course not found.");
                db.UserCourses.RemoveRange(course.EnrolledUsers);
                foreach (var ch in course.Chapters)
                    ch.UsersCompleted.Clear();
                db.Set<Section>().RemoveRange(course.Chapters.SelectMany(ch => ch.Sections));
                db.Set<Chapter>().RemoveRange(course.Chapters);
                db.Courses.Remove(course);
                await db.SaveChangesAsync();
            }
        }

        internal async Task DeleteChapterAction(int courseId, int chapterId)
        {
            using (var db = new ApplicationDbContext())
            {
                var chapter = await db.Courses
                    .Include(c => c.Chapters.Select(ch => ch.Sections))
                    .Where(c => c.Id == courseId)
                    .SelectMany(c => c.Chapters)
                    .FirstOrDefaultAsync(ch => ch.ChapterNumber == chapterId);
                if (chapter == null)
                    throw new InvalidOperationException("Chapter not found.");
                db.Set<Section>().RemoveRange(chapter.Sections);
                db.Set<Chapter>().Remove(chapter);
                await db.SaveChangesAsync();
            }
        }

        internal async Task AddChapterAction(Chapter chapter)
        {
            using (var db = new ApplicationDbContext())
            {
                if (string.IsNullOrWhiteSpace(chapter.Title))
                    throw new InvalidOperationException("Chapter title is required.");
                var course = await db.Courses.Include(c => c.Chapters).FirstOrDefaultAsync(c => c.Id == chapter.CourseId);
                if (course == null)
                    throw new InvalidOperationException("Course not found.");
                chapter.Id = 0;
                chapter.Sections = new List<Section>();
                course.Chapters.Add(chapter);
                await db.SaveChangesAsync();
            }
        }

        internal int GetCourseCountAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Courses.Count();
            }
        }

        internal int GetEnrollmentCountAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.UserCourses.Count();
            }
        }

        internal UserCourse GetLatestEnrollmentAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.UserCourses
                    .OrderByDescending(uc => uc.EnrolledDate)
                    .ThenByDescending(uc => uc.Id)
                    .FirstOrDefault();
            }
        }

        internal List<int> GetEnrollmentCountByDateAction(DateTime fromDate, DateTime toDate)
        {
            using (var db = new ApplicationDbContext())
            {
                var start = fromDate.Date;
                var end = toDate.Date.AddDays(1);
                var days = (toDate.Date - start).Days + 1;
                var counts = db.UserCourses
                    .Where(uc => uc.EnrolledDate >= start && uc.EnrolledDate < end)
                    .GroupBy(uc => DbFunctions.TruncateTime(uc.EnrolledDate))
                    .Select(g => new { Date = g.Key.Value, Count = g.Count() })
                    .ToDictionary(g => g.Date, g => g.Count);
                var result = new List<int>(days);
                for (var i = 0; i < days; i++)
                {
                    var day = start.AddDays(i);
                    counts.TryGetValue(day, out var count);
                    result.Add(count);
                }
                return result;
            }
        }

        private async Task ValidateCourseAsync(Course course, bool isUpdate)
        {
            using (var db = new ApplicationDbContext())
            {
                if (!isUpdate && await db.Courses.AnyAsync(x => x.Title == course.Title))
                    throw new InvalidOperationException("A course with this title already exists");
                if (string.IsNullOrWhiteSpace(course.Title))
                    throw new InvalidOperationException("Course title is required");
                if (string.IsNullOrWhiteSpace(course.Description))
                    throw new InvalidOperationException("Course description is required");
            }
        }
    }
}
