using System.Collections.Generic;
using ByteCore.Domain.UserScope;

namespace ByteCore.Domain.QuizScope
{
    public class QuizResult
    {
        public int Id { get; set; }
        public Quiz Quiz { get; set; }
        public User User { get; set; }
        public List<QuizResultAnswer> Answers { get; set; } = new List<QuizResultAnswer>();
    }
}