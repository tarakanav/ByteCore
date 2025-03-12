using System;
using System.Collections.Generic;

namespace ByteCore.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Instructor { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime StartDate { get; set; }
        public string ImageUrl { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public List<User> EnrolledUsers { get; set; } = new List<User>();
    }
}