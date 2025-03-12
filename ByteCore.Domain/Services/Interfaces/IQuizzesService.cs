using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.Domain.Entities;
using ByteCore.Model.Models;

namespace ByteCore.Domain.Services.Interfaces
{
    public interface IQuizzesService
    {
        IEnumerable<Quiz> GetQuizzes();
        Quiz GetQuiz(int id);
        QuizResult GetQuizResult(int id, int resultId);
        Task<QuizResult> SubmitQuizResultAsync(int id, List<int> userAnswers, string email);
        Task AddQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(int id, Quiz quiz);
        Task DeleteQuizAsync(int id);
    }
}