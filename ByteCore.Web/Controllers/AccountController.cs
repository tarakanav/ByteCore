using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class AccountController : Controller
    {
        private static List<UserModel> Users = new List<UserModel>
        {
            new UserModel
            {
                Id = 1,
                Name = "Sample User",
                Email = "sample@example.com",
                EnrolledCourses = new List<CourseModel>
                {
                    new CourseModel { Id = 1, Title = "Course 1", ShortDescription = "Short description 1" },
                    new CourseModel { Id = 2, Title = "Course 2", ShortDescription = "Short description 2" }
                }
            }
        };

        // GET: Account/Registration
        public ActionResult Registration()
        {
            return View();
        }

        // POST: Account/Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields should be presented.");
                return View();
            }
            
            if (Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "User with this email already exists.");
                return View();
            }
            
            var newUser = new UserModel
            {
                Id = Users.Max(u => u.Id) + 1,
                Name = username,
                Email = email,
                EnrolledCourses = new List<CourseModel>()
            };
            Users.Add(newUser);

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "You should enter email and password.");
                return View();
            }
            
            var user = Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid credentials.");
            return View();
        }

        // GET: Account/Dashboard
        //[Authorize]
        public ActionResult Dashboard()
        {
            //var userEmail = User.Identity.Name;
            var userEmail = "sample@example.com";
            var user = Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }
        
        // GET: Account/Manage
        //[Authorize]
        public ActionResult Manage()
        {
            //var userEmail = User.Identity.Name;
            var userEmail = "sample@example.com";
            var user = Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Account/Manage
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = Users.FirstOrDefault(u => u.Id == model.Id);
                if (user != null)
                {
                    user.Name = model.Name;
                    user.Email = model.Email;
                }

                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
        
        // POST: /Account/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string name, string email)
        {
            var user = Users.FirstOrDefault();
            if (user != null)
            {
                user.Name = name;
                user.Email = email;
            }
            return RedirectToAction("Dashboard");
        }

    }
}