using System.Collections.Generic;

namespace ByteCore.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual List<Course> EnrolledCourses { get; set; } = new List<Course>();
    }
}