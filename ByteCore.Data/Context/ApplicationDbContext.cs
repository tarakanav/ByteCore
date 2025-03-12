using System.Data.Entity;
using ByteCore.Domain.Entities;
using ByteCore.Domain.Services.Interfaces;

namespace ByteCore.Data.Context
{
    public class ApplicationDbContext : DbContext, IDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<QuizResultAnswer> QuizResultAnswers { get; set; }

        public ApplicationDbContext() : base("name=DefaultConnection") { }
    }
}