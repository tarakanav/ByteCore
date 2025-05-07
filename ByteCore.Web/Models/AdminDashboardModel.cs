using System;
using System.Collections.Generic;
using ByteCore.Domain.UserScope;

namespace ByteCore.Web.Models
{
    public class AdminDashboardModel
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalQuizResults { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime LastEnrollmentDate { get; set; }
        public List<int> ActiveUsers { get; set; }
        public List<int> NewUsers { get; set; }
        public List<int> NewEnrollments { get; set; }
        public int CelsiusTemperature { get; set; }
        public User LastUser { get; set; }
        public Dictionary<string, int> BrowserUsages { get; set; }
    }
}