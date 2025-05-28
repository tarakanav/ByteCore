// File: ByteCore.BusinessLogic.APIs.CourseApi.cs
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

        private async Task ValidateCourseInApiContextAsync(Course course, bool isUpdate, ApplicationDbContext db)
        {
            if (!isUpdate && await db.Courses.AnyAsync(x => x.Title == course.Title))
                throw new InvalidOperationException("A course with this title already exists");
            if (string.IsNullOrWhiteSpace(course.Title))
                throw new InvalidOperationException("Course title is required");
            if (string.IsNullOrWhiteSpace(course.Description))
                throw new InvalidOperationException("Course description is required");
        }

        internal async Task CreateCourseAction(Course course)
        {
            using (var db = new ApplicationDbContext())
            {
                await ValidateCourseInApiContextAsync(course, false, db);
                if (course.Chapters != null)
                {
                    foreach (var chapter in course.Chapters)
                    {
                        if (chapter.Sections != null)
                        {
                            foreach (var section in chapter.Sections)
                            {
                                section.TextContent = section.Type == SectionType.Read ? section.TextContent : null;
                                section.VideoUrl = section.Type == SectionType.Video ? section.VideoUrl : null;
                                section.Quiz = section.Type == SectionType.Quiz ? section.Quiz : null;
                                if (section.Type != SectionType.Quiz) section.QuizId = null;
                                else if (section.QuizId != null && section.Quiz == null) section.Quiz = await db.Quizzes.FindAsync(section.QuizId);
                            }
                        }
                    }
                }
                db.Courses.Add(course);
                await db.SaveChangesAsync();
            }
        }

        internal async Task UpdateCourseAction(Course course)
        {
            using (var db = new ApplicationDbContext())
            {
                await ValidateCourseInApiContextAsync(course, true, db);
                var existing = await db.Courses.Include(c => c.Chapters.Select(ch => ch.Sections)).FirstOrDefaultAsync(c => c.Id == course.Id);
                if (existing == null)
                    throw new InvalidOperationException("Course not found.");
                
                existing.Title = course.Title;
                existing.ShortDescription = course.ShortDescription;
                existing.Description = course.Description;
                existing.Instructor = course.Instructor;
                existing.Duration = course.Duration;
                existing.StartDate = course.StartDate;
                existing.ImageUrl = course.ImageUrl;

                var chapterIdsToKeep = new HashSet<int>();
                if (course.Chapters != null)
                {
                    foreach (var updatedChapter in course.Chapters)
                    {
                        var existingChapter = updatedChapter.Id != 0 ? existing.Chapters.FirstOrDefault(c => c.Id == updatedChapter.Id) : null;
                        if (existingChapter != null)
                        {
                            existingChapter.ChapterNumber = updatedChapter.ChapterNumber;
                            existingChapter.Title = updatedChapter.Title;
                            chapterIdsToKeep.Add(existingChapter.Id);
                        }
                        else
                        {
                            updatedChapter.Id = 0;
                            updatedChapter.CourseId = existing.Id;
                            existing.Chapters.Add(updatedChapter);
                        }
                    }
                }
                var chaptersToRemove = existing.Chapters.Where(c => c.Id != 0 && !chapterIdsToKeep.Contains(c.Id)).ToList();
                if (chaptersToRemove.Any())
                {
                    foreach(var chapterToRemove in chaptersToRemove)
                    {
                        var sectionsInChapterToRemove = chapterToRemove.Sections.ToList();
                        if(sectionsInChapterToRemove.Any()) db.Set<Section>().RemoveRange(sectionsInChapterToRemove);
                    }
                    db.Set<Chapter>().RemoveRange(chaptersToRemove);
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
                var course = await db.Courses
                    .Include(c => c.Chapters.Select(ch => ch.Sections))
                    .FirstOrDefaultAsync(c => c.Id == chapter.CourseId);

                if (course == null)
                    throw new InvalidOperationException("Course not found for the chapter.");

                var existingChapter = course.Chapters.FirstOrDefault(ch => ch.Id == chapter.Id);

                if (existingChapter == null)
                    throw new InvalidOperationException("Chapter not found to update.");
                
                existingChapter.Title = chapter.Title;
                existingChapter.Description = chapter.Description;

                var sectionIdsToKeep = new HashSet<int>();
                if (chapter.Sections != null)
                {
                    foreach (var updatedSection in chapter.Sections)
                    {
                        var existingSection = updatedSection.Id != 0 ? existingChapter.Sections.FirstOrDefault(s => s.Id == updatedSection.Id) : null;
                        if (existingSection != null)
                        {
                            existingSection.Title = updatedSection.Title;
                            existingSection.Description = updatedSection.Description;
                            existingSection.Type = updatedSection.Type;
                            existingSection.TextContent = updatedSection.Type == SectionType.Read ? updatedSection.TextContent : null;
                            existingSection.VideoUrl = updatedSection.Type == SectionType.Video ? updatedSection.VideoUrl : null;
                            existingSection.QuizId = updatedSection.Type == SectionType.Quiz ? updatedSection.QuizId : null;
                            if (existingSection.Type == SectionType.Quiz && existingSection.QuizId != null) existingSection.Quiz = await db.Quizzes.FindAsync(existingSection.QuizId); else existingSection.Quiz = null;
                            sectionIdsToKeep.Add(existingSection.Id);
                        }
                        else
                        {
                            updatedSection.Id = 0;
                            updatedSection.ChapterId = existingChapter.Id;
                            if (updatedSection.Type == SectionType.Quiz && updatedSection.QuizId != null) updatedSection.Quiz = await db.Quizzes.FindAsync(updatedSection.QuizId); else updatedSection.Quiz = null;
                            existingChapter.Sections.Add(updatedSection);
                        }
                    }
                }
                var sectionsToRemove = existingChapter.Sections.Where(s => s.Id != 0 && !sectionIdsToKeep.Contains(s.Id)).ToList();
                if(sectionsToRemove.Any()) db.Set<Section>().RemoveRange(sectionsToRemove);
                
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
                    .Include(c => c.Chapters.Select(ch => ch.Sections.Select(s => s.Quiz)))
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                    throw new InvalidOperationException("Course not found.");
                
                if (course.EnrolledUsers.Any()) db.UserCourses.RemoveRange(course.EnrolledUsers);
                
                foreach (var ch in course.Chapters) ch.UsersCompleted.Clear();
                
                var allSections = course.Chapters.SelectMany(ch => ch.Sections).ToList();
                if (allSections.Any()) db.Set<Section>().RemoveRange(allSections);
                
                if (course.Chapters.Any()) db.Set<Chapter>().RemoveRange(course.Chapters);
                
                db.Courses.Remove(course);
                await db.SaveChangesAsync();
            }
        }

        internal async Task DeleteChapterAction(int courseId, int chapterNumber)
        {
            using (var db = new ApplicationDbContext())
            {
                var course = await db.Courses
                    .Include(c => c.Chapters.Select(ch => ch.Sections.Select(s => s.Quiz)))
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null) throw new InvalidOperationException("Course not found.");
                
                var chapter = course.Chapters.FirstOrDefault(ch => ch.ChapterNumber == chapterNumber);

                if (chapter == null)
                    throw new InvalidOperationException("Chapter not found.");
                
                var sectionsToRemove = chapter.Sections.ToList();
                if(sectionsToRemove.Any()) db.Set<Section>().RemoveRange(sectionsToRemove);
                
                db.Set<Chapter>().Remove(chapter);
                await db.SaveChangesAsync();
            }
        }

        internal async Task AddChapterAction(Chapter chapter)
        {
            using (var db = new ApplicationDbContext())
            {
                var course = await db.Courses.Include(c => c.Chapters).FirstOrDefaultAsync(c => c.Id == chapter.CourseId);
                if (course == null)
                    throw new InvalidOperationException("Course not found.");
                
                chapter.Id = 0;
                if (chapter.Sections != null)
                {
                    foreach (var section in chapter.Sections)
                    {
                        section.Id = 0;
                        section.ChapterId = chapter.Id; 
                        if (section.Type == SectionType.Quiz && section.QuizId != null) section.Quiz = await db.Quizzes.FindAsync(section.QuizId); else section.Quiz = null;
                    }
                }
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
    }
}