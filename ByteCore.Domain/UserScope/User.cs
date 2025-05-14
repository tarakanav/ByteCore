using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;

namespace ByteCore.Domain.UserScope
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(200)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [MaxLength(50)]
        public string Role { get; set; } = "User";
        [MaxLength(200)]
        public string LatestBrowserUsed { get; set; }
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        [InverseProperty(nameof(UserLastSeen.User))]
        public virtual List<UserLastSeen> LastSeenHistory { get; set; } = new List<UserLastSeen>();
        [InverseProperty(nameof(UserCourse.User))]
        public virtual List<UserCourse> EnrolledCourses { get; set; } = new List<UserCourse>();
        [InverseProperty(nameof(Chapter.UsersCompleted))]
        public virtual List<Chapter> CompletedChapters { get; set; } = new List<Chapter>();
        public virtual List<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        [InverseProperty(nameof(LoginLog.User))]
        public virtual List<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();
    }
}