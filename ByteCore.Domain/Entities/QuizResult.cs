using System.Collections.Generic;

namespace ByteCore.Domain.Entities
{
    public class QuizResult
    {
        public int Id { get; set; }
        public Quiz Quiz { get; set; }
        public User User { get; set; }
        public List<QuizResultAnswer> Answers { get; set; }
    }
}