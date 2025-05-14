using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ByteCore.Domain.UserScope;

namespace ByteCore.Domain.QuizScope
{
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }
        public List<QuizResultAnswer> Answers { get; set; } = new List<QuizResultAnswer>();
    }
}