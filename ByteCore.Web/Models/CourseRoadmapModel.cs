using System;
using System.Collections.Generic;

namespace ByteCore.Web.Models
{
    public class CourseRoadmapModel
    {
        public CourseModel Course { get; set; }
        public List<RoadmapStep> Steps { get; set; }
    }

    public class RoadmapStep
    {
        public string StepTitle { get; set; }
        public string Description { get; set; }
        public string Week { get; set; }
    }
}
