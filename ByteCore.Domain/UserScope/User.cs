using System;
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
        public string LatestBrowserUsed { get; set; }
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public virtual List<UserLastSeen> LastSeenHistory { get; set; }
        public virtual List<UserCourse> EnrolledCourses { get; set; }
        public virtual List<Chapter> CompletedChapters { get; set; }
    }
}