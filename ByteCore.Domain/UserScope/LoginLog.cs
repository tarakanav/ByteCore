using System;

namespace ByteCore.Domain.UserScope
{
    public class LoginLog
    {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}