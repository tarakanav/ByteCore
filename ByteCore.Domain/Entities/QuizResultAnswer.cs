using ByteCore.Model.Models;

namespace ByteCore.Domain.Entities
{
    public class QuizResultAnswer
    {
        public int Id { get; set; }
        public Question Question { get; set; }
        public int SelectedOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}