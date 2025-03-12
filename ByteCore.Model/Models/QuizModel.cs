using System.Collections.Generic;

namespace ByteCore.Model.Models
{
    public class QuizModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual List<QuestionModel> Questions { get; set; }
        public int RewardPoints { get; set; }
        public decimal PassingPercentage { get; set; }
    }
}