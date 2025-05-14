using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.QuizScope
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string QuestionText { get; set; }
        [InverseProperty(nameof(QuestionOption.Question))]
        public List<QuestionOption> Options { get; set; } = new List<QuestionOption>();
        public int CorrectOption { get; set; }
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}