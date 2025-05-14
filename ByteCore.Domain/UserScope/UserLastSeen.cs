using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.UserScope
{
    public class UserLastSeen
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime SeenOn { get; set; } = DateTime.Today;
        
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}