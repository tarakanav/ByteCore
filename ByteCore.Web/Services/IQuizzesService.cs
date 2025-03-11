using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.Web.Models;

namespace ByteCore.Web.Services
{
    public interface IQuizzesService
    {
        List<QuizModel> GetQuizzes();
        QuizModel GetQuiz(int id);
        QuizResultModel GetQuizResult(int id, int resultId);
        Task<QuizResultModel> SubmitQuizResultAsync(int id, List<int> userAnswers, string email);
        Task AddQuizAsync(QuizModel quiz);
        Task UpdateQuizAsync(int id, QuizModel quiz);
    }
}