using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.UserScope
{
    public class UserSession
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public User User { get; set; }
        public string CookieString { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}