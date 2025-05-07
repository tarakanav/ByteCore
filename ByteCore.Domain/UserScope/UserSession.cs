using System;

namespace ByteCore.Domain.UserScope
{
    public class UserSession
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string CookieString { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}