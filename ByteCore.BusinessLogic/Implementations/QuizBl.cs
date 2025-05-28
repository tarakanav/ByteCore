using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ByteCore.BusinessLogic.APIs;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.QuizScope;

namespace ByteCore.BusinessLogic.Implementations
{
    public class QuizBl : QuizApi, IQuizBl
    {
        public List<Quiz> GetQuizzes(int page = 1, int pageSize = 20)
        {
            var quizzes = GetQuizzesAction(page, pageSize);
            return quizzes.Select(q => { q.Title = q.Title.Trim(); return q; }).ToList();
        }

        public Quiz GetQuiz(int id)
        {
            var quiz = GetQuizAction(id);
            quiz?.Questions.ForEach(q => q.Options = q.Options.OrderBy(o => o.Id).ToList());
            return quiz;
        }

        public QuizResult GetQuizResult(int quizId, int resultId)
        {
            return GetQuizResultAction(quizId, resultId);
        }

        public async Task<QuizResult> SubmitQuizResultAsync(int quizId, List<int> userAnswers, string email)
        {
            var quiz = GetQuizAction(quizId);
            var user = GetUserAction(email);

            if (quiz == null || user == null)
                throw new KeyNotFoundException("Quiz or user not found.");

            if (userAnswers == null || quiz.Questions.Count != userAnswers.Count)
                throw new InvalidOperationException("Invalid number of answers.");

            var answers = quiz.Questions.Select((question, idx) => new QuizResultAnswer
            {
                Question = question,
                SelectedOption = userAnswers[idx],
                IsCorrect = question.CorrectOption == userAnswers[idx]
            }).ToList();

            var result = new QuizResult
            {
                Quiz = quiz,
                User = user,
                Answers = answers
            };

            return await SaveQuizResultAction(result);
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            if (quiz.Questions == null || !quiz.Questions.Any())
                throw new InvalidOperationException("Quiz must have at least one question.");

            await SaveQuizAction(quiz);
        }

        public async Task UpdateQuizAsync(int id, Quiz quiz)
        {
            if (quiz.Id == 0)
                throw new InvalidOperationException("Quiz ID must be provided for update.");
            
            var existing = GetQuizAction(quiz.Id);
            if (existing == null)
                throw new InvalidOperationException("Quiz not found.");
            
            await SaveQuizAction(quiz);
        }

        public async Task DeleteQuizAsync(int id)
        {
            await DeleteQuizAction(id);
        }

        public int GetQuizCount()
        {
            return GetQuizCountAction();
        }

        public int GetQuizResultCount()
        {
            return GetQuizResultCountAction();
        }
    }
}