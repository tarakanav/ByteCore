using System.Collections.Generic;
using System.Linq;

namespace ByteCore.Web.Models
{
    public class ChapterModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public virtual CourseModel Course { get; set; }
        public virtual ICollection<SectionModel> Sections { get; set; }
        
        public int GetChapterNumber()
        {
            if (Course?.Chapters == null)
                return 1;

            var chapters = Course.Chapters.OrderBy(c => c.Id).ToList();
            var index = chapters.IndexOf(this) + 1;
            return index;
        }
    }
}