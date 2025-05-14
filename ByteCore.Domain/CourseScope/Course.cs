using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ByteCore.Domain.UserScope;

namespace ByteCore.Domain.CourseScope
{
	public class Course
	{
		[Key]
		public int Id { get; set; }
		[Required, MaxLength(200)]
		public string Title { get; set; }
		[Required, MaxLength(500)]
		public string ShortDescription { get; set; }
		public string Description { get; set; }
		[Required, MaxLength(100)]
		public string Instructor { get; set; }
		[Required]
		public TimeSpan Duration { get; set; }
		[Required]
		public DateTime StartDate { get; set; }
		[MaxLength(200)]
		public string ImageUrl { get; set; }
		[InverseProperty(nameof(Chapter.Course))]
		public List<Chapter> Chapters { get; set; } = new List<Chapter>();
		[InverseProperty(nameof(UserCourse.Course))]
		public List<UserCourse> EnrolledUsers { get; set; } = new List<UserCourse>();
	}
}