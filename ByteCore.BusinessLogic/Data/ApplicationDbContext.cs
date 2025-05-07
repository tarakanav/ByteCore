using System.Data.Entity;
using ByteCore.BusinessLogic.Implementations;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;
using ByteCore.Domain.UserScope;

namespace ByteCore.BusinessLogic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<QuizResultAnswer> QuizResultAnswers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }

        public ApplicationDbContext() : base("name=DefaultConnection") { }
    }
}