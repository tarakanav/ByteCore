using System.Collections.Generic;

namespace ByteCore.Web.Models
{
    public class QuizModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<QuestionModel> Questions { get; set; }
        public int RewardPoints { get; set; }
    }

    public class QuestionModel
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public int CorrectOptionIndex { get; set; }
    }
}