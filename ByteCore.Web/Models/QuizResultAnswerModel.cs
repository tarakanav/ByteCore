namespace ByteCore.Web.Models
{
    public class QuizResultAnswerModel
    {
        public int Id { get; set; }
        public QuestionModel Question { get; set; }
        public int SelectedOption { get; set; }
        public bool IsCorrect { get; set; }
    }
}