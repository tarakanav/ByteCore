using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ByteCore.Domain.UserScope;

namespace ByteCore.Domain.CourseScope
{
    public class Chapter
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        public int ChapterNumber { get; set; }
        [ForeignKey(nameof(ByteCore.Domain.CourseScope.Course))]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        [InverseProperty(nameof(User.CompletedChapters))]
        public virtual List<User> UsersCompleted { get; set; } = new List<User>();
        [InverseProperty(nameof(Section.Chapter))]
        public virtual List<Section> Sections { get; set; } = new List<Section>();
    }
}