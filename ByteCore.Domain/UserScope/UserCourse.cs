using ByteCore.Domain.CourseScope;

namespace ByteCore.Domain.UserScope
{
    public class UserCourse
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public User User { get; set; }
        public Course Course { get; set; }
    }
}