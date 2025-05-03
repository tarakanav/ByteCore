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
        private readonly ApplicationDbContext _db;

        public QuizBl(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Quiz> GetQuizzes()
        {
            return _db.Quizzes.ToList();
        }

        public Quiz GetQuiz(int id)
        {
            return _db.Quizzes
                .Include(x => x.Questions)
                .Include(x => x.Questions.Select(q => q.Options))
                .FirstOrDefault(q => q.Id == id);
        }

        public QuizResult GetQuizResult(int id, int resultId)
        {
            return _db.QuizResults
                .Include(q => q.Quiz)
                .Include(q => q.Answers)
                .Include(q => q.User)
                .FirstOrDefault(q => q.Id == resultId && q.Quiz.Id == id);
        }

        public async Task<QuizResult> SubmitQuizResultAsync(int id, List<int> userAnswers, string email)
        {
            var quiz = _db.Quizzes.Include(quizModel => quizModel.Questions).FirstOrDefault(q => q.Id == id);
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            
            if (quiz == null || user == null)
            {
                return null;
            }

            if (quiz.Questions.Count != userAnswers.Count)
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
            
            _db.QuizResults.Add(quizResult);
            await _db.SaveChangesAsync();
            
            return quizResult;
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();
        }

        public Task UpdateQuizAsync(int id, Quiz quiz)
        {
            var existingQuiz = _db.Quizzes.Include(quizModel => quizModel.Questions).FirstOrDefault(q => q.Id == id);
            
            if (existingQuiz == null)
            {
                throw new InvalidOperationException("Quiz not found.");
            }
            
            existingQuiz.Title = quiz.Title;
            existingQuiz.PassingPercentage = quiz.PassingPercentage;
            existingQuiz.RewardPoints = quiz.RewardPoints;
            existingQuiz.Questions = quiz.Questions;
            
            return _db.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(int id)
        {
            var quiz = _db.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefault(q => q.Id == id);

            if (quiz == null)
                throw new InvalidOperationException("Quiz not found.");

            var sections = _db.Set<Section>()
                .Where(s => s.QuizId == id)
                .ToList();
            if (sections.Any())
            {
                _db.Set<Section>().RemoveRange(sections);
            }

            var relatedResults = _db.QuizResults
                .Where(r => r.Quiz.Id == id)
                .Include(r => r.Answers)
                .ToList();

            foreach (var result in relatedResults)
            {
                _db.QuizResultAnswers.RemoveRange(result.Answers);
            }
            _db.QuizResults.RemoveRange(relatedResults);
            _db.Questions.RemoveRange(quiz.Questions);
            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
        }
    }
}