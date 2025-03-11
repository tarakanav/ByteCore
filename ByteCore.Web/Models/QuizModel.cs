using System.Collections.Generic;

namespace ByteCore.Web.Models
{
    public class QuizModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual List<QuestionModel> Questions { get; set; }
        public int RewardPoints { get; set; }
    }
}