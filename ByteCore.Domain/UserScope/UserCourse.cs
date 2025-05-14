using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ByteCore.Domain.CourseScope;

namespace ByteCore.Domain.UserScope
{
    public class UserCourse
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }
        public User User { get; set; }
        public Course Course { get; set; }
        public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;
    }
}