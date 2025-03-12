using System.Data.Entity;
using System.Threading.Tasks;
using ByteCore.Domain.Entities;

namespace ByteCore.Domain.Services.Interfaces
{
    public interface IDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Course> Courses { get; }
        DbSet<Quiz> Quizzes { get; }
        DbSet<Question> Questions { get; }
        DbSet<QuizResult> QuizResults { get; }
        DbSet<QuizResultAnswer> QuizResultAnswers { get; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}