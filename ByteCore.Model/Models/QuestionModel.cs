using System.Collections.Generic;

namespace ByteCore.Model.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<QuestionOptionModel> Options { get; set; }
        public int CorrectOption { get; set; }
    }
}