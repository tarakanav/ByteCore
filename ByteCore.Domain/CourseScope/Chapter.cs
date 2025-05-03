using System.Collections.Generic;
using System.Linq;
using ByteCore.Domain.UserScope;

namespace ByteCore.Domain.CourseScope
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ChapterNumber { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual List<User> UsersCompleted { get; set; } = new List<User>();
        public virtual List<Section> Sections { get; set; } = new List<Section>();
    }
}