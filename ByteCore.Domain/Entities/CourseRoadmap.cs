using System.Collections.Generic;

namespace ByteCore.Domain.Entities
{
    public class CourseRoadmap
    {
        public Course Course { get; set; }
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
