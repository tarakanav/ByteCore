using System.Collections.Generic;
using ByteCore.Domain.CourseScope;

namespace ByteCore.Domain.UserScope
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
        public virtual List<Course> EnrolledCourses { get; set; }
    }
}