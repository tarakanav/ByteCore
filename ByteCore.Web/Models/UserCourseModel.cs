namespace ByteCore.Web.Models
{
    public class UserCourseModel
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public UserModel User { get; set; }
        public CourseModel Course { get; set; }
    }
}