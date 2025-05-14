using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.QuizScope
{
    public class QuestionOption
    {
        [Key]
        public int Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}