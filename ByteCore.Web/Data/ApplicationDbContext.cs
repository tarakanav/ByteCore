using System.Data.Entity;
using ByteCore.Web.Models;

namespace ByteCore.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<QuizModel> Quizzes { get; set; }

        public ApplicationDbContext() : base("name=DefaultConnection") { }
    }
}