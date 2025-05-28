using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.APIs
{
    public class QuizApi
    {
        internal List<Quiz> GetQuizzesAction(int page, int pageSize)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Quizzes
                    .Include(q => q.Questions.Select(x => x.Options))
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        internal Quiz GetQuizAction(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Quizzes
                    .Include(x => x.Questions.Select(q => q.Options))
                    .FirstOrDefault(q => q.Id == id);
            }
        }

        internal User GetUserAction(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        internal QuizResult GetQuizResultAction(int quizId, int resultId)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.QuizResults
                    .Include(qr => qr.Quiz)
                    .Include(qr => qr.Quiz.Questions)
                    .Include(qr => qr.Answers.Select(a => a.Question))
                    .Include(qr => qr.User)
                    .FirstOrDefault(qr => qr.Id == resultId && qr.Quiz.Id == quizId);
            }
        }

        internal async Task<QuizResult> SaveQuizResultAction(QuizResult result)
        {
            using (var db = new ApplicationDbContext())
            {
                if (result.Quiz != null && result.Quiz.Id != 0)
                {
                    db.Quizzes.Attach(result.Quiz);
                }
                if (result.User != null && result.User.Id != 0)
                {
                    db.Users.Attach(result.User);
                }

                foreach (var answer in result.Answers)
                {
                    if (answer.Question != null && answer.Question.Id != 0)
                    {
                        db.Questions.Attach(answer.Question);
                    }
                }

                db.QuizResults.Add(result);
                await db.SaveChangesAsync();
                return result;
            }
        }

        internal async Task SaveQuizAction(Quiz quiz)
        {
            using (var db = new ApplicationDbContext())
            {
                if (quiz.Id == 0)
                {
                    db.Quizzes.Add(quiz);
                }
                else
                {
                    var existingQuiz = await db.Quizzes
                        .Include(q => q.Questions.Select(x => x.Options))
                        .FirstOrDefaultAsync(q => q.Id == quiz.Id);

                    if (existingQuiz == null)
                        throw new InvalidOperationException("Quiz not found.");

                    existingQuiz.Title = quiz.Title;
                    existingQuiz.PassingPercentage = quiz.PassingPercentage;
                    existingQuiz.RewardPoints = quiz.RewardPoints;

                    var incomingQuestionIds = quiz.Questions.Where(q => q.Id != 0).Select(q => q.Id).ToList();
                    var questionsToDelete = existingQuiz.Questions
                        .Where(q => !incomingQuestionIds.Contains(q.Id))
                        .ToList();

                    foreach (var questionToDelete in questionsToDelete)
                    {
                        db.Questions.Remove(questionToDelete);
                    }

                    foreach (var incomingQuestion in quiz.Questions)
                    {
                        var existingQuestion = existingQuiz.Questions.FirstOrDefault(q => q.Id == incomingQuestion.Id);
                        if (existingQuestion == null)
                        {
                            incomingQuestion.QuizId = existingQuiz.Id;
                            existingQuiz.Questions.Add(incomingQuestion);
                        }
                        else
                        {
                            existingQuestion.QuestionText = incomingQuestion.QuestionText;
                            existingQuestion.CorrectOption = incomingQuestion.CorrectOption;
                            
                            var incomingOptionIds = incomingQuestion.Options.Where(o => o.Id != 0).Select(o => o.Id).ToList();
                            var optionsToDelete = existingQuestion.Options
                                .Where(o => !incomingOptionIds.Contains(o.Id))
                                .ToList();

                            foreach (var optionToDelete in optionsToDelete)
                            {
                                db.QuestionOptions.Remove(optionToDelete);
                            }

                            foreach (var incomingOption in incomingQuestion.Options)
                            {
                                var existingOption = existingQuestion.Options.FirstOrDefault(o => o.Id == incomingOption.Id);
                                if (existingOption == null)
                                {
                                    incomingOption.QuestionId = existingQuestion.Id;
                                    existingQuestion.Options.Add(incomingOption);
                                }
                                else
                                {
                                    existingOption.OptionText = incomingOption.OptionText;
                                }
                            }
                        }
                    }
                }
                await db.SaveChangesAsync();
            }
        }

        internal async Task DeleteQuizAction(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var quiz = await db.Quizzes
                    .Include(q => q.Questions.Select(x => x.Options))
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    throw new InvalidOperationException("Quiz not found.");

                var sections = await db.Set<Section>()
                    .Where(s => s.QuizId == id)
                    .ToListAsync();
                if (sections.Any())
                {
                    db.Set<Section>().RemoveRange(sections);
                }

                var relatedResults = await db.QuizResults
                    .Where(r => r.Quiz.Id == id)
                    .Include(r => r.Answers)
                    .ToListAsync();

                foreach (var result in relatedResults)
                {
                    db.QuizResultAnswers.RemoveRange(result.Answers);
                }
                db.QuizResults.RemoveRange(relatedResults);

                if (quiz.Questions.Any())
                {
                    db.Questions.RemoveRange(quiz.Questions);
                }

                db.Quizzes.Remove(quiz);
                await db.SaveChangesAsync();
            }
        }

        internal int GetQuizCountAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Quizzes.Count();
            }
        }

        internal int GetQuizResultCountAction()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.QuizResults.Count();
            }
        }
    }
}