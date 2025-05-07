using System;

namespace ByteCore.Domain.UserScope
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}