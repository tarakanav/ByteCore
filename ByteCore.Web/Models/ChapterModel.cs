using System.Collections.Generic;
using ByteCore.Domain.CourseScope;
using ByteCore.Domain.QuizScope;

namespace ByteCore.Web.Models
{
    public class ChapterModel
    {
        public Chapter Chapter { get; set; }
        
        public Dictionary<int, QuizResult> QuizResults { get; set; }
            = new Dictionary<int, QuizResult>();
    }
}