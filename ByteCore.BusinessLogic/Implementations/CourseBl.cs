using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class CourseBl : ICourseBl
    {
        private readonly ApplicationDbContext _db;

        public CourseBl(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Course> GetCourses(int page = 1, int pageSize = 20)
        {
            return _db.Courses
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Course GetCourse(int id)
        {
            return _db.Courses
                .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                .Include(x => x.Chapters.Select(chapter => chapter.Sections))
                .Include(x => x.Chapters.Select(chapter => chapter.Sections.Select(section => section.Quiz)))
                .Include(x => x.Chapters.Select(chapter => chapter.Sections.Select(section => section.Quiz.Questions)))
                .Include(x => x.Chapters.Select(chapter =>
                    chapter.Sections.Select(section => section.Quiz.Questions.Select(question => question.Options))))
                .FirstOrDefault(x => x.Id == id);
        }

        public bool IsUserEnrolled(int courseId, string email)
        {
            var course = _db.Courses
                .Include(courseModel => courseModel.EnrolledUsers.Select(userCourse => userCourse.User))
                .FirstOrDefault(x => x.Id == courseId);

            if (course == null)
            {
                return false;
            }

            return course.EnrolledUsers.Any(x => x.User.Email == email);
        }

        public Task EnrollUserAsync(int id, string email)
        {
            var user = _db.Users.FirstOrDefault(x => x.Email == email);
            var course = _db.Courses.FirstOrDefault(x => x.Id == id);

            if (user == null || course == null)
            {
                throw new KeyNotFoundException("User or course not found.");
            }

            var userCourse = new UserCourse
            {
                Course = course,
                User = user
            };

            _db.UserCourses.Add(userCourse);
            return _db.SaveChangesAsync();
        }

        public async Task CreateCourseAsync(Course course)
        {
            for (var i = 1; i <= course.Chapters.Count; i++)
            {
                course.Chapters[i - 1].ChapterNumber = i;
            }

            await ValidateCourseAsync(course);

            if (course.Chapters != null)
            {
                foreach (var section in course.Chapters.Where(chapter => chapter.Sections != null)
                             .SelectMany(chapter => chapter.Sections))
                {
                    switch (section.Type)
                    {
                        case SectionType.Read:
                            section.VideoUrl = null;
                            section.Quiz = null;
                            break;
                        case SectionType.Video:
                            section.TextContent = null;
                            section.Quiz = null;
                            break;
                        case SectionType.Quiz:
                            var quiz = await _db.Quizzes.FirstOrDefaultAsync(x => x.Id == section.QuizId);
                            if (quiz == null)
                            {
                                throw new InvalidOperationException("The specified quiz does not exist");
                            }

                            section.Quiz = quiz;
                            section.TextContent = null;
                            section.VideoUrl = null;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
        }

        public Chapter GetChapter(int courseId, int chapterNumber)
        {
            var course = GetCourse(courseId);
            if (course?.Chapters != null && course.Chapters.Count >= chapterNumber)
            {
                return course.Chapters.FirstOrDefault(x => x.ChapterNumber == chapterNumber);
            }

            return null;
        }

        public void MarkChapterAsCompleted(int courseId, int chapterNumber, string userEmail)
        {
            var course = _db.Courses
                .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                .FirstOrDefault(c => c.Id == courseId);
            var user = _db.Users.FirstOrDefault(x => x.Email == userEmail);
            if (course?.Chapters == null || course.Chapters.Count < chapterNumber) return;
            var chapter = course.Chapters.FirstOrDefault(x => x.ChapterNumber == chapterNumber);
            if (chapter == null) return;
            if (chapter.UsersCompleted.Contains(user)) return;
            course.Chapters.FirstOrDefault(x => x.Id == chapter.Id)?.UsersCompleted.Add(user);
            _db.SaveChanges();
        }

        public void MarkChapterAsIncompleted(int courseId, int chapterNumber, string userEmail)
        {
            var course = _db.Courses
                .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                .FirstOrDefault(c => c.Id == courseId);
            var user = _db.Users.FirstOrDefault(x => x.Email == userEmail);
            if (course?.Chapters == null || course.Chapters.Count < chapterNumber) return;
            var chapter = course.Chapters.FirstOrDefault(x => x.ChapterNumber == chapterNumber);
            if (chapter == null) return;
            if (!chapter.UsersCompleted.Contains(user)) return;
            chapter.UsersCompleted.Remove(user);
            _db.SaveChanges();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            await ValidateCourseAsync(course, isUpdate: true);

            var existing = await _db.Courses
                .Include(c => c.Chapters)
                .FirstOrDefaultAsync(c => c.Id == course.Id);

            if (existing == null)
                throw new InvalidOperationException("Course not found");

            existing.Title = course.Title;
            existing.ShortDescription = course.ShortDescription;
            existing.Description = course.Description;
            existing.Instructor = course.Instructor;
            existing.Duration = course.Duration;
            existing.StartDate = course.StartDate;
            existing.ImageUrl = course.ImageUrl;

            foreach (var updatedChapter in course.Chapters)
            {
                var existingChapter = existing.Chapters
                    .FirstOrDefault(c => c.Id == updatedChapter.Id);

                if (existingChapter != null)
                {
                    existingChapter.ChapterNumber = updatedChapter.ChapterNumber;
                    existingChapter.Title = updatedChapter.Title;
                }
            }

            var duplicateNumbers = existing.Chapters
                .GroupBy(c => c.ChapterNumber)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateNumbers.Any())
                throw new InvalidOperationException("Chapter numbers must be unique.");

            await _db.SaveChangesAsync();
        }

        public async Task UpdateChapterAsync(Chapter chapter)
        {
            if (string.IsNullOrWhiteSpace(chapter.Title))
                throw new InvalidOperationException("Chapter title is required");

            var course = await _db.Courses
                .Include(c => c.Chapters.Select(ch => ch.Sections))
                .FirstOrDefaultAsync(c => c.Id == chapter.CourseId);

            if (course == null)
                throw new InvalidOperationException("Course not found");

            var existingChapter = course.Chapters
                .FirstOrDefault(ch => ch.Id == chapter.Id);

            if (existingChapter == null)
                throw new InvalidOperationException("Chapter not found");

            existingChapter.Title = chapter.Title;
            existingChapter.Description = chapter.Description;

            var toRemove = existingChapter.Sections
                .Where(es => chapter.Sections.All(ns => ns.Id != es.Id))
                .ToList();
            foreach (var rem in toRemove)
            {
                existingChapter.Sections.Remove(rem);
                _db.Set<Section>().Remove(rem);
            }

            foreach (var incoming in chapter.Sections)
            {
                if (incoming.Id != 0)
                {
                    var sec = existingChapter.Sections.First(es => es.Id == incoming.Id);
                    sec.Title = incoming.Title;
                    sec.Description = incoming.Description;
                    sec.Type = incoming.Type;

                    sec.TextContent = null;
                    sec.VideoUrl = null;
                    sec.QuizId = null;
                    sec.Quiz = null;

                    switch (incoming.Type)
                    {
                        case SectionType.Read:
                            if (string.IsNullOrWhiteSpace(incoming.TextContent))
                                throw new InvalidOperationException("Text content is required");
                            sec.TextContent = incoming.TextContent;
                            break;
                        case SectionType.Video:
                            if (string.IsNullOrWhiteSpace(incoming.VideoUrl))
                                throw new InvalidOperationException("Video URL is required");
                            sec.VideoUrl = incoming.VideoUrl;
                            break;
                        case SectionType.Quiz:
                            if (incoming.QuizId == null)
                                throw new InvalidOperationException("Quiz ID is required");
                            sec.QuizId = incoming.QuizId;
                            break;
                    }
                }
                else
                {
                    incoming.Chapter = existingChapter;
                    switch (incoming.Type)
                    {
                        case SectionType.Read:
                            if (string.IsNullOrWhiteSpace(incoming.TextContent))
                                throw new InvalidOperationException("Text content is required");
                            incoming.VideoUrl = null;
                            incoming.Quiz = null;
                            break;
                        case SectionType.Video:
                            if (string.IsNullOrWhiteSpace(incoming.VideoUrl))
                                throw new InvalidOperationException("Video URL is required");
                            incoming.TextContent = null;
                            incoming.Quiz = null;
                            break;
                        case SectionType.Quiz:
                            if (incoming.QuizId == null)
                                throw new InvalidOperationException("Quiz ID is required");
                            incoming.TextContent = null;
                            incoming.VideoUrl = null;
                            break;
                    }

                    existingChapter.Sections.Add(incoming);
                }
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _db.Courses
                .Include(c => c.EnrolledUsers)
                .Include(c => c.Chapters.Select(ch => ch.UsersCompleted))
                .Include(c => c.Chapters.Select(ch => ch.Sections))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                throw new InvalidOperationException("Course not found.");

            if (course.EnrolledUsers.Any())
                _db.UserCourses.RemoveRange(course.EnrolledUsers);

            foreach (var chapter in course.Chapters)
            {
                if (chapter.UsersCompleted != null && chapter.UsersCompleted.Any())
                    chapter.UsersCompleted.Clear();
            }

            var allSections = course.Chapters.SelectMany(ch => ch.Sections).ToList();
            if (allSections.Any())
                _db.Set<Section>().RemoveRange(allSections);

            _db.Set<Chapter>().RemoveRange(course.Chapters);

            _db.Courses.Remove(course);

            await _db.SaveChangesAsync();
        }


        public async Task DeleteChapterAsync(int courseId, int chapterId)
        {
            var course = await _db.Courses
                .Include(c => c.Chapters.Select(ch => ch.Sections))
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new InvalidOperationException("Course not found.");

            var chapter = course.Chapters
                .FirstOrDefault(ch => ch.ChapterNumber == chapterId);

            if (chapter == null)
                throw new InvalidOperationException("Chapter not found.");

            if (chapter.Sections.Any())
                _db.Set<Section>().RemoveRange(chapter.Sections);
            _db.Set<Chapter>().Remove(chapter);
            await _db.SaveChangesAsync();
        }

        public async Task AddChapterAsync(Chapter chapter)
        {
            if (string.IsNullOrWhiteSpace(chapter.Title))
                throw new InvalidOperationException("Chapter title is required.");

            var course = await _db.Courses
                .Include(c => c.Chapters)
                .FirstOrDefaultAsync(c => c.Id == chapter.CourseId);

            if (course == null)
                throw new InvalidOperationException("Course not found.");

            chapter.Id = 0;
            chapter.Sections = new List<Section>();

            course.Chapters.Add(chapter);
            await _db.SaveChangesAsync();
        }

        public int GetCourseCount()
        {
            return _db.Courses.Count();
        }

        public int GetEnrollmentCount()
        {
            return _db.UserCourses.Count();
        }

        public UserCourse GetLatestEnrollment()
        {
            return _db.UserCourses
                .OrderByDescending(uc => uc.EnrolledDate)
                .ThenByDescending(uc => uc.Id)
                .FirstOrDefault();
        }

        public IEnumerable<int> GetEnrollmentCountByDate(DateTime fromDate, DateTime toDate)
        {
            var start = fromDate.Date;
            var endExclusive = toDate.Date.AddDays(1);
            var days = (toDate.Date - start).Days + 1;

            var enrollsByDate = _db.UserCourses
                .Where(uc => uc.EnrolledDate >= start && uc.EnrolledDate < endExclusive)
                .GroupBy(uc => DbFunctions.TruncateTime(uc.EnrolledDate))
                .Select(g => new
                {
                    Date = g.Key.Value,
                    Count = g.Count()
                })
                .ToDictionary(g => g.Date, g => g.Count);

            var result = new List<int>(days);
            for (int i = 0; i < days; i++)
            {
                var day = start.AddDays(i);
                enrollsByDate.TryGetValue(day, out int count);
                result.Add(count);
            }

            return result;
        }


        private async Task ValidateCourseAsync(Course course, bool isUpdate = false)
        {
            if (!isUpdate && await _db.Courses.AnyAsync(x => x.Title == course.Title))
                throw new InvalidOperationException("A course with this title already exists");
            if (string.IsNullOrWhiteSpace(course.Title))
                throw new InvalidOperationException("Course title is required");
            if (string.IsNullOrWhiteSpace(course.Description))
                throw new InvalidOperationException("Course description is required");

            if (!isUpdate && course.Chapters != null)
            {
                foreach (var chapter in course.Chapters)
                {
                    if (chapter.ChapterNumber <= 0)
                        throw new InvalidOperationException("Chapter number must be greater than 0");
                    if (chapter.ChapterNumber > course.Chapters.Count)
                        throw new InvalidOperationException("Chapter number exceeds the total number of chapters");
                    if (string.IsNullOrWhiteSpace(chapter.Title))
                        throw new InvalidOperationException("Chapter title is required");
                    if (chapter.Sections == null) continue;
                    foreach (var section in chapter.Sections)
                    {
                        if (string.IsNullOrWhiteSpace(section.Title))
                            throw new InvalidOperationException("Section title is required");
                        switch (section.Type)
                        {
                            case SectionType.Read when string.IsNullOrWhiteSpace(section.TextContent):
                                throw new InvalidOperationException("Text content is required");
                            case SectionType.Video when string.IsNullOrWhiteSpace(section.VideoUrl):
                                throw new InvalidOperationException("Video URL is required");
                            case SectionType.Quiz when section.QuizId == null:
                                throw new InvalidOperationException("Quiz ID is required");
                        }
                    }
                }
            }
        }
    }
}