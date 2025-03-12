using System.Collections.Generic;

namespace ByteCore.Model.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public virtual List<CourseModel> EnrolledCourses { get; set; }
    }
}