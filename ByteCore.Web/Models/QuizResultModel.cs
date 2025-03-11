using System;

namespace ByteCore.Web.Models
{
    public class QuizResultModel
    {
        public int Id { get; set; }
        public int CorrectAnswersCount { get; set; } 
        public int TotalQuestions { get; set; } 
        
        public double ScorePercentage => TotalQuestions > 0 ? (double)CorrectAnswersCount / TotalQuestions * 100 : 0;
    }
}