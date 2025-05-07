using System;

namespace ByteCore.Domain.UserScope
{
    public class UserLastSeen
    {
        public int Id { get; set; }
        
        public DateTime SeenOn { get; set; } = DateTime.Today;
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}