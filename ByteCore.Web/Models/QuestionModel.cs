using System.Collections.Generic;
using ByteCore.Web.Controllers;

namespace ByteCore.Web.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public List<QuestionOptionModel> Options { get; set; }
        public int CorrectOption { get; set; }
    }
}