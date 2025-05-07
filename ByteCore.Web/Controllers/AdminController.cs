using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserBl _userBl;
        private readonly ICourseBl _courseBl;
        private readonly IQuizBl _quizBl;
        private readonly IAdminBl _adminBl;

        public AdminController(IUserBl userBl, ICourseBl courseBl, IQuizBl quizBl, IAdminBl adminBl)
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
            var model = new AdminDashboardModel();
            model.TotalUsers = _userBl.GetUserCount();
            model.TotalCourses = _courseBl.GetCourseCount();
            model.TotalQuizzes = _quizBl.GetQuizCount();
            model.TotalEnrollments = _courseBl.GetEnrollmentCount();
            model.TotalQuizResults = _quizBl.GetQuizResultCount();
            model.ProjectStartDate = _userBl.GetFirstUser().RegistrationTime;
            var latestEnrollment = _courseBl.GetLatestEnrollment();
            model.LastEnrollmentDate = latestEnrollment?.EnrolledDate ?? DateTime.MinValue;
            model.ActiveUsers = _userBl.GetActiveUserCount(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date);
            model.NewUsers = _userBl.GetUserCountByRegistrationDate(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date);
            model.NewEnrollments = _courseBl.GetEnrollmentCountByDate(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date);
            model.CelsiusTemperature = _adminBl.GetCurrentTemperature();
            model.LastUser = _userBl.GetLastUser();
            model.BrowserUsages = _userBl.GetBrowserUsages();
            return View(model);
        }
    }
}