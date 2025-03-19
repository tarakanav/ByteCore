using System.Collections.Generic;

namespace ByteCore.Domain.QuizScope
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        public int CorrectOption { get; set; }
    }
}