using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.Web.Data;
using ByteCore.Web.Models;

namespace ByteCore.Web.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly ApplicationDbContext _db;

        public CoursesService(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<CourseModel> GetCourses()
        {
            return _db.Courses.ToList();
        }

        public CourseModel GetCourse(int id)
        {
            return _db.Courses
                .Include(x => x.Chapters)
                .Include(x => x.Chapters.Select(chapter => chapter.Sections))
                .Include(x => x.Chapters.Select(chapter => chapter.Sections.Select(section => section.Quiz)))
                .FirstOrDefault(x => x.Id == id);
        }

        public bool IsUserEnrolled(int courseId, string email)
        {
            var course = _db.Courses
                .Include(courseModel => courseModel.EnrolledUsers)
                .FirstOrDefault(x => x.Id == courseId);

            if (course == null)
            {
                return false;
            }

            return course.EnrolledUsers.Any(x => x.Email == email);
        }

        public Task EnrollUserAsync(int id, string email)
        {
            var user = _db.Users.FirstOrDefault(x => x.Email == email);
            var course = _db.Courses.FirstOrDefault(x => x.Id == id);

            if (user == null || course == null)
            {
                throw new KeyNotFoundException("User or course not found.");
            }

            course.EnrolledUsers.Add(user);
            return _db.SaveChangesAsync();
        }

        public async Task CreateCourseAsync(CourseModel course)
        {
            await ValidateCourseAsync(course);

            if (course.Chapters != null)
            {
                foreach (var section in course.Chapters.Where(chapter => chapter.Sections != null).SelectMany(chapter => chapter.Sections))
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

        public ChapterModel GetChapter(int courseId, int chapterId)
        {
            var course = GetCourse(courseId);
            if (course?.Chapters != null && course.Chapters.Count >= chapterId)
            {
                return course.Chapters.OrderBy(x => x.Id).ElementAtOrDefault(chapterId - 1);
            }
            return null;
        }


        private async Task ValidateCourseAsync(CourseModel course)
        {
            if (await _db.Courses.AnyAsync(x => x.Title == course.Title))
            {
                throw new InvalidOperationException("A course with this title already exists");
            }

            if (string.IsNullOrEmpty(course.Title))
            {
                throw new InvalidOperationException("Course title is required");
            }

            if (string.IsNullOrEmpty(course.Description))
            {
                throw new InvalidOperationException("Course description is required");
            }

            if (course.Chapters == null || !course.Chapters.Any()) return;
            {
                foreach (var chapter in course.Chapters)
                {
                    if (string.IsNullOrEmpty(chapter.Title))
                    {
                        throw new InvalidOperationException("Chapter title is required");
                    }

                    if (chapter.Sections == null || !chapter.Sections.Any()) continue;
                    foreach (var section in chapter.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Title))
                        {
                            throw new InvalidOperationException("Section title is required");
                        }

                        switch (section.Type)
                        {
                            case SectionType.Read:
                                if (string.IsNullOrEmpty(section.TextContent))
                                {
                                    throw new InvalidOperationException(
                                        "Text content is required for reading sections");
                                }

                                break;
                            case SectionType.Video:
                                if (string.IsNullOrEmpty(section.VideoUrl))
                                {
                                    throw new InvalidOperationException("Video URL is required for video sections");
                                }

                                break;
                            case SectionType.Quiz:
                                if (section.Quiz == null)
                                {
                                    throw new InvalidOperationException("Quiz ID is required for quiz sections");
                                }
                                
                                var quiz = await _db.Quizzes.FirstOrDefaultAsync(x => x.Id == section.Quiz.Id);
                                if (quiz == null)
                                {
                                    throw new InvalidOperationException("The specified quiz does not exist");
                                }

                                break;
                            default:
                                throw new InvalidOperationException("Invalid section type");
                        }
                    }
                }
            }
        }
    }
}