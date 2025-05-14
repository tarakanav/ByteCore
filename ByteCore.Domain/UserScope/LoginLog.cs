using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.UserScope
{
    public class LoginLog
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        [MaxLength(45)]
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
    }
}