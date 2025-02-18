using System;
using System.Collections.Generic;

namespace ByteCore.Web.Models
{
    public class CourseRoadmapModel
    {
        public CourseModel Course { get; set; }
        public List<CourseRoadmapModule> Modules { get; set; }
    }

    public class CourseRoadmapModule
    {
        public string Title { get; set; }
        public List<CourseRoadmapTopic> Topics { get; set; }
    }

    public class CourseRoadmapTopic
    {
        public string Title { get; set; }
        public List<string> Lessons { get; set; }
    }
}
