using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IUserBl _userBl;
        private readonly ICourseBl _courseBl;
        private readonly IQuizBl _quizBl;
        private readonly IAuditLogBl _auditLogBl;
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
            _auditLogBl = auditLogBl;
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
                NewUsers = _userBl.GetUserCountByRegistrationDate(DateTime.UtcNow.AddDays(-6).Date,
                    DateTime.UtcNow.Date),
                NewEnrollments = _courseBl
                    .GetEnrollmentCountByDate(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date).ToList(),
                CelsiusTemperature = _adminBl.GetCurrentTemperature(),
                LastUser = _userBl.GetLastUser(),
                BrowserUsages = _userBl.GetBrowserUsages(),
                LastEnrollmentDate = _courseBl.GetLatestEnrollment()?.EnrolledDate ?? DateTime.MinValue
            };
            return View(model);
        }

        // GET: Admin/LogsAudit
        [CustomAuthorize("Admin")]
        [Route("LogsAudit")]
        public ActionResult Audit(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            const int pageSize = 20;
            var users = _auditLogBl.GetAll(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_auditLogBl.GetLogCount() / pageSize);
            return View("AuditLog", users);
        }

        // GET: Admin/LogsLogin
        [CustomAuthorize("Admin")]
        [Route("LogsLogin")]
        public ActionResult Login(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            const int pageSize = 20;
            var loginLogs = _userBl.GetLoginLogs(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_userBl.GetLoginLogCount() / pageSize);
            return View("LoginLog", loginLogs);
        }

        // GET: Admin/ManageUsers
        [CustomAuthorize("Admin")]
        [Route("ManageUsers")]
        public ActionResult ManageUsers(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            const int pageSize = 20;
            var users = _userBl.GetAll(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_userBl.GetUserCount() / pageSize);
            return View(users.ToList());
        }

        // POST: Admin/ManageUsers
        [CustomAuthorize("Admin")]
        [Route("ManageUsers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUsers(List<User> users)
        {
            if (users == null || users.Count == 0 || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid data provided.");
                users = _userBl.GetAll(1, 20).ToList();
                return View(users);
            }

            var currentAdmin = users.FirstOrDefault(x => x.Email == User.Identity.Name);
            if (currentAdmin != null &&
                !string.Equals(currentAdmin.Role, "Admin", StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("", "You cannot modify your own role.");
                users = _userBl.GetAll(1, 20).ToList();
                return View(users);
            }

            try
            {
                _userBl.UpdateUserRange(users);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating users: {ex.Message}");
                users = _userBl.GetAll(1, 20).ToList();
                return View(users);
            }
            return RedirectToAction("ManageUsers");
        }
    }
}