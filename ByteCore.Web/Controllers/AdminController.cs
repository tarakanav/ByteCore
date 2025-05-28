using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ByteCore.BusinessLogic;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class AdminController : BaseController
    {
        // GET: Admin
        [CustomAuthorize("Admin")]
        public ActionResult Index()
        {
            var userBl = Bl.GetUserBl();
            var courseBl = Bl.GetCourseBl();
            var quizBl = Bl.GetQuizBl();
            var adminBl = Bl.GetAdminBl();
            
            var model = new AdminDashboardModel
            {
                TotalUsers = userBl.GetUserCount(),
                TotalCourses = courseBl.GetCourseCount(),
                TotalQuizzes = quizBl.GetQuizCount(),
                TotalEnrollments = courseBl.GetEnrollmentCount(),
                TotalQuizResults = quizBl.GetQuizResultCount(),
                ProjectStartDate = userBl.GetFirstUser().RegistrationTime,
                ActiveUsers = userBl.GetActiveUserCount(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date),
                NewUsers = userBl.GetUserCountByRegistrationDate(DateTime.UtcNow.AddDays(-6).Date,
                    DateTime.UtcNow.Date),
                NewEnrollments = courseBl
                    .GetEnrollmentCountByDate(DateTime.UtcNow.AddDays(-6).Date, DateTime.UtcNow.Date).ToList(),
                CelsiusTemperature = adminBl.GetCurrentTemperature(),
                LastUser = userBl.GetLastUser(),
                BrowserUsages = userBl.GetBrowserUsages(),
                LastEnrollmentDate = courseBl.GetLatestEnrollment()?.EnrolledDate ?? DateTime.MinValue
            };
            return View(model);
        }

        // GET: Admin/LogsAudit
        [CustomAuthorize("Admin")]
        [Route("LogsAudit")]
        public ActionResult Audit(int page = 1)
        {
            var auditLogBl = Bl.GetAuditLogBl();
            if (page < 1)
            {
                page = 1;
            }

            const int pageSize = 20;
            var users = auditLogBl.GetAll(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)auditLogBl.GetLogCount() / pageSize);
            return View("AuditLog", users);
        }

        // GET: Admin/LogsLogin
        [CustomAuthorize("Admin")]
        [Route("LogsLogin")]
        public ActionResult Login(int page = 1)
        {
            var userBl = Bl.GetUserBl();
            if (page < 1)
            {
                page = 1;
            }

            const int pageSize = 20;
            var loginLogs = userBl.GetLoginLogs(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)userBl.GetLoginLogCount() / pageSize);
            return View("LoginLog", loginLogs);
        }

        // GET: Admin/ManageUsers
        [CustomAuthorize("Admin")]
        [Route("ManageUsers")]
        public ActionResult ManageUsers(int page = 1)
        {
            var userBl = Bl.GetUserBl();
            if (page < 1)
            {
                page = 1;
            }

            const int pageSize = 20;
            var users = userBl.GetAll(page, pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)userBl.GetUserCount() / pageSize);
            return View(users.ToList());
        }

        // POST: Admin/ManageUsers
        [CustomAuthorize("Admin")]
        [Route("ManageUsers")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUsers(List<User> users)
        {
            var userBl = Bl.GetUserBl();
            if (users == null || users.Count == 0 || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid data provided.");
                users = userBl.GetAll(1, 20).ToList();
                return View(users);
            }

            var currentAdmin = users.FirstOrDefault(x => x.Email == User.Identity.Name);
            if (currentAdmin != null &&
                !string.Equals(currentAdmin.Role, "Admin", StringComparison.CurrentCultureIgnoreCase))
            {
                ModelState.AddModelError("", "You cannot modify your own role.");
                users = userBl.GetAll(1, 20).ToList();
                return View(users);
            }

            try
            {
                userBl.UpdateUserRange(users);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating users: {ex.Message}");
                users = userBl.GetAll(1, 20).ToList();
                return View(users);
            }
            return RedirectToAction("ManageUsers");
        }
    }
}