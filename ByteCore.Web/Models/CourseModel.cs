using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByteCore.Web.Models
{
	public class CourseModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string ShortDescription { get; set; }
		public string Description { get; set; }
		public string Instructor { get; set; }
		public TimeSpan Duration { get; set; }
		public DateTime StartDate { get; set; }
		public string ImageUrl { get; set; }
		public List<ChapterModel> Chapters { get; set; }
		public List<UserModel> EnrolledUsers { get; set; }
	}
}