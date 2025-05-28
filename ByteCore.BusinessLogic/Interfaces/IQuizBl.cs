using System.Collections.Generic;
using System.Threading.Tasks;
using ByteCore.Domain.QuizScope;

namespace ByteCore.BusinessLogic.Interfaces
{
    public interface IQuizBl
    {
        List<Quiz> GetQuizzes(int page = 1, int pageSize = 20);
        Quiz GetQuiz(int id);
        QuizResult GetQuizResult(int id, int resultId);
        Task<QuizResult> SubmitQuizResultAsync(int id, List<int> userAnswers, string email);
        Task AddQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(int id, Quiz quiz);
        Task DeleteQuizAsync(int id);
        int GetQuizCount();
        int GetQuizResultCount();
    }
}