using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ByteCore.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Registration
        public ActionResult Registration()
        {
            return View();
        }
        
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }
        
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, bool rememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }
            
            if (email == "test@example.com" && password == "password")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }
    }
}