using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IUserBl _userBl;
        private readonly ICourseBl _courseBl;
        private readonly IQuizBl _quizBl;
        private readonly IAdminBl _adminBl;

        public AdminController(
            IUserBl userBl,
            ICourseBl courseBl,
            IQuizBl quizBl,
            IAdminBl adminBl,
            IAuditLogBl auditLogBl) 
            : base(auditLogBl)
        {
            _userBl = userBl;
            _courseBl = courseBl;
            _quizBl = quizBl;
            _adminBl = adminBl;
        }

        // GET: Admin
        [CustomAuthorize("Admin")]
        public ActionResult Index()
        {
            var model = new AdminDashboardModel
            {
                TotalUsers = _userBl.GetUserCount(),
                TotalCourses = _courseBl.GetCourseCount(),
                TotalQuizzes = _quizBl.GetQuizCount(),
                TotalEnrollments = _courseBl.GetEnrollmentCount(),
                TotalQuizResults = _quizBl.GetQuizResultCount(),
                ProjectStartDate = _userBl.GetFirstUser().RegistrationTime,
                ActiveUsers = _userBl.GetActiveUserCount(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date),
                NewUsers = _userBl.GetUserCountByRegistrationDate(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date),
                NewEnrollments = _courseBl.GetEnrollmentCountByDate(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date),
                CelsiusTemperature = _adminBl.GetCurrentTemperature(),
                LastUser = _userBl.GetLastUser(),
                BrowserUsages = _userBl.GetBrowserUsages(),
                LastEnrollmentDate = _courseBl.GetLatestEnrollment()?.EnrolledDate ?? DateTime.MinValue
            };
            return View(model);
        }
    }
}