using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.QuizScope
{
    public class QuizResultAnswer
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(QuizResult))]
        public int QuizResultId { get; set; }
        public QuizResult QuizResult { get; set; }
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int SelectedOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}