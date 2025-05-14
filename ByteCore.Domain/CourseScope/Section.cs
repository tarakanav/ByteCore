using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ByteCore.Domain.QuizScope;

namespace ByteCore.Domain.CourseScope
{
    public class Section
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string TextContent { get; set; }
        public string VideoUrl { get; set; }
        [ForeignKey(nameof(Quiz))]
        public int? QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public SectionType Type { get; set; } = SectionType.Read;
        [ForeignKey(nameof(Chapter))]
        public int ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }
        
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