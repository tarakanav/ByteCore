using System.Collections.Generic;
using ByteCore.Model.Models;

namespace ByteCore.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<QuestionOption> Options { get; set; }
        public int CorrectOption { get; set; }
    }
}