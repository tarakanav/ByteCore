using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ByteCore.Domain.QuizScope
{
    [Table("Quizzes")]
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        [InverseProperty(nameof(Question.Quiz))]
        public virtual List<Question> Questions { get; set; } = new List<Question>();
        public int RewardPoints { get; set; }
        public decimal PassingPercentage { get; set; }
    }
}