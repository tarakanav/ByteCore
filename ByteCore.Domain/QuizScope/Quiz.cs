using System.Collections.Generic;

namespace ByteCore.Domain.QuizScope
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual List<Question> Questions { get; set; } = new List<Question>();
        public int RewardPoints { get; set; }
        public decimal PassingPercentage { get; set; }
    }
}