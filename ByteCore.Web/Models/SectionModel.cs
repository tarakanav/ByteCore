using System.Linq;

namespace ByteCore.Web.Models
{
    public class SectionModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TextContent { get; set; }
        public string VideoUrl { get; set; }
        public QuizModel Quiz { get; set; }
        public SectionType Type { get; set; }
        public virtual ChapterModel Chapter { get; set; }
        
        public int GetSectionNumber()
        {
            if (Chapter?.Sections == null)
                return 1;

            var sections = Chapter.Sections.OrderBy(s => s.Id).ToList();
            var index = sections.IndexOf(this) + 1;
            return index;
        }
    }
    
    public enum SectionType
    {
        Read,
        Video,
        Quiz
    }
}