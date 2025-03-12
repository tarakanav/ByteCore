using System.Collections.Generic;

namespace ByteCore.Model.Models
{
    public class QuizResultModel
    {
        public int Id { get; set; }
        public QuizModel Quiz { get; set; }
        public UserModel User { get; set; }
        public List<QuizResultAnswerModel> Answers { get; set; }
    }
}