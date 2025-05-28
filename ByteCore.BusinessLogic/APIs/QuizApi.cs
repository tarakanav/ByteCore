using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.Domain.QuizScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.APIs
{
    public class QuizApi
    {
        internal List<Quiz> GetQuizzesAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
                return db.Quizzes
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
        }

        internal Quiz GetQuizAction(int id)
        {
            using (var db = new ApplicationDbContext())
                return db.Quizzes
                    .Include(x => x.Questions.Select(q => q.Options))
                    .FirstOrDefault(q => q.Id == id);
        }

        internal User GetUserAction(string email)
        {
            using (var db = new ApplicationDbContext())
                return db.Users.FirstOrDefault(u => u.Email == email);
        }

        internal QuizResult GetQuizResultAction(int quizId, int resultId)
        {
            using (var db = new ApplicationDbContext())
                return db.QuizResults
                    .Include(qr => qr.Quiz)
                    .Include(qr => qr.Answers)
                    .Include(qr => qr.User)
                    .FirstOrDefault(qr => qr.Id == resultId && qr.Quiz.Id == quizId);
        }

        internal async Task<QuizResult> SaveQuizResultAction(QuizResult result)
        {
            using (var db = new ApplicationDbContext())
            {
                db.QuizResults.Add(result);
                await db.SaveChangesAsync();
                return result;
            }
        }

        internal async Task SaveQuizAction(Quiz quiz)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Entry(quiz).State = quiz.Id == 0 ? EntityState.Added : EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        internal async Task DeleteQuizAction(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var quiz = new Quiz { Id = id };
                db.Entry(quiz).State = EntityState.Deleted;
                await db.SaveChangesAsync();
            }
        }

        internal int GetQuizCountAction()
        {
            using (var db = new ApplicationDbContext())
                return db.Quizzes.Count();
        }

        internal int GetQuizResultCountAction()
        {
            using (var db = new ApplicationDbContext())
                return db.QuizResults.Count();
        }
    }
}