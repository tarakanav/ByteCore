using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.Data;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class QuizBl : IQuizBl
    {
        public IEnumerable<Quiz> GetQuizzes(int page = 1, int pageSize = 20)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Quizzes
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        public Quiz GetQuiz(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Quizzes
                    .Include(x => x.Questions)
                    .Include(x => x.Questions.Select(q => q.Options))
                    .FirstOrDefault(q => q.Id == id);
            }
        }

        public QuizResult GetQuizResult(int id, int resultId)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.QuizResults
                    .Include(q => q.Quiz)
                    .Include(q => q.Answers)
                    .Include(q => q.User)
                    .FirstOrDefault(q => q.Id == resultId && q.Quiz.Id == id);
            }
        }

        public async Task<QuizResult> SubmitQuizResultAsync(int id, List<int> userAnswers, string email)
        {
            using (var db = new ApplicationDbContext())
            {
                var quiz = db.Quizzes.Include(quizModel => quizModel.Questions).FirstOrDefault(q => q.Id == id);
                var user = db.Users.FirstOrDefault(u => u.Email == email);

                if (quiz == null || user == null)
                {
                    return null;
                }

                if (quiz.Questions.Count != userAnswers?.Count)
                {
                    throw new InvalidOperationException("Invalid number of answers.");
                }

                var answers = new List<QuizResultAnswer>();

                for (var i = 0; i < quiz.Questions.Count; i++)
                {
                    var question = quiz.Questions[i];
                    var answer = userAnswers[i];
                    var correct = question.CorrectOption == answer;

                    answers.Add(new QuizResultAnswer
                    {
                        Question = question,
                        SelectedOption = answer,
                        IsCorrect = correct,
                    });
                }

                var quizResult = new QuizResult
                {
                    Quiz = quiz,
                    Answers = answers,
                    User = user
                };

                db.QuizResults.Add(quizResult);
                await db.SaveChangesAsync();

                return quizResult;
            }
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Quizzes.Add(quiz);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateQuizAsync(int id, Quiz quiz)
        {
            using (var db = new ApplicationDbContext())
            {
                var existingQuiz = await db.Quizzes
                    .Include(quizModel => quizModel.Questions)
                    .Include(quizModel => quizModel.Questions.Select(q => q.Options))
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (existingQuiz == null)
                {
                    throw new InvalidOperationException("Quiz not found.");
                }

                existingQuiz.Title = quiz.Title;
                existingQuiz.PassingPercentage = quiz.PassingPercentage;
                existingQuiz.RewardPoints = quiz.RewardPoints;

                var incomingQuestionIds = quiz.Questions.Where(q => q.Id != 0).Select(q => q.Id).ToList();
                var questionsToDelete = existingQuiz.Questions
                    .Where(q => !incomingQuestionIds.Contains(q.Id))
                    .ToList();

                foreach (var question in questionsToDelete)
                {
                    db.Questions.Remove(question);
                }

                foreach (var incomingQuestion in quiz.Questions)
                {
                    if (incomingQuestion.Id == 0)
                    {
                        incomingQuestion.QuizId = existingQuiz.Id;
                        existingQuiz.Questions.Add(incomingQuestion);
                        foreach (var newOption in incomingQuestion.Options)
                        {
                            newOption.QuestionId =
                                incomingQuestion.Id;
                        }
                    }
                    else
                    {
                        var existingQuestion = existingQuiz.Questions
                            .FirstOrDefault(q => q.Id == incomingQuestion.Id);

                        if (existingQuestion != null)
                        {
                            existingQuestion.QuestionText = incomingQuestion.QuestionText;
                            existingQuestion.CorrectOption = incomingQuestion.CorrectOption;

                            var incomingOptionIds =
                                incomingQuestion.Options.Where(o => o.Id != 0).Select(o => o.Id).ToList();

                            var optionsToDelete = existingQuestion.Options
                                .Where(o => !incomingOptionIds.Contains(o.Id))
                                .ToList();

                            foreach (var option in optionsToDelete)
                            {
                                db.QuestionOptions.Remove(option);
                            }

                            foreach (var incomingOption in incomingQuestion.Options)
                            {
                                if (incomingOption.Id == 0)
                                {
                                    incomingOption.QuestionId = existingQuestion.Id;
                                    existingQuestion.Options.Add(incomingOption);
                                }
                                else
                                {
                                    var existingOption = existingQuestion.Options
                                        .FirstOrDefault(o => o.Id == incomingOption.Id);

                                    if (existingOption != null)
                                    {
                                        existingOption.OptionText = incomingOption.OptionText;
                                    }
                                }
                            }
                        }
                    }
                }

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteQuizAsync(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var quiz = db.Quizzes
                    .Include(q => q.Questions)
                    .FirstOrDefault(q => q.Id == id);

                if (quiz == null)
                    throw new InvalidOperationException("Quiz not found.");

                var sections = db.Set<Section>()
                    .Where(s => s.QuizId == id)
                    .ToList();
                if (sections.Any())
                {
                    db.Set<Section>().RemoveRange(sections);
                }

                var relatedResults = db.QuizResults
                    .Where(r => r.Quiz.Id == id)
                    .Include(r => r.Answers)
                    .ToList();

                foreach (var result in relatedResults)
                {
                    db.QuizResultAnswers.RemoveRange(result.Answers);
                }

                db.QuizResults.RemoveRange(relatedResults);
                db.Questions.RemoveRange(quiz.Questions);
                db.Quizzes.Remove(quiz);
                await db.SaveChangesAsync();
            }
        }

        public int GetQuizCount()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Quizzes.Count();
            }
        }

        public int GetQuizResultCount()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.QuizResults.Count();
            }
        }
    }
}