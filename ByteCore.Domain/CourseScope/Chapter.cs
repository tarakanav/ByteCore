﻿using System.Collections.Generic;
using System.Linq;

namespace ByteCore.Domain.CourseScope
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual List<Section> Sections { get; set; } = new List<Section>();
        
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