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
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<QuizResultAnswer> QuizResultAnswers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }

        public ApplicationDbContext() : base("name=DefaultConnection") { }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditLog>()
                .HasOptional(a => a.User)         // ← “optional” nav property
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)     // ← points at your nullable int?
                .WillCascadeOnDelete(false);      // ← prevents cascade delete
        }

    }
}