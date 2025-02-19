using System.Collections.Generic;

namespace ByteCore.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<CourseModel> EnrolledCourses { get; set; }
    }
}