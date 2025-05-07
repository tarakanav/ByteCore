using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;
using ByteCore.Web.Models;

namespace ByteCore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserBl _userBl;

        public AccountController(IUserBl userBl)
        {
            _userBl = userBl;
        }

        // GET: Account/Registration
        public ActionResult Registration()
        {
            ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
            
            return View();
        }

        // POST: Account/Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registration(string username, string email, string password, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields should be presented.");
                return View();
            }

            try
            {
                await _userBl.RegisterUserAsync(username, email, password, Request.Browser.Browser);
                var cookie = await _userBl.GetUserCookieAsync(email, true);
                Response.Cookies.Add(cookie);
                
                return Redirect(returnUrl ?? "/");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"];
            
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }
            
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string email, string password, bool rememberMe = false, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "You should enter email and password.");
                return View();
            }

            try
            {
                _userBl.AuthenticateUser(email, password, Request.Browser.Browser);
                var cookie = await _userBl.GetUserCookieAsync(email, rememberMe);
                Response.Cookies.Add(cookie);

                return Redirect(returnUrl ?? "/");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Account/Dashboard
        [CustomAuthorize]
        public ActionResult Dashboard()
        {
            var user = _userBl.GetUserByEmail(User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/Manage
        [CustomAuthorize]
        public ActionResult Manage()
        {
            var user = _userBl.GetUserByEmail(User.Identity.Name);
            
            if (user == null)
            {
                return HttpNotFound();
            }
            
            return View(user);
        }

        // POST: Account/Manage
        [HttpPost]
        [CustomAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userBl.UpdateUserAsync(User.Identity.Name, model);
                    var cookie = await _userBl.GetUserCookieAsync(model.Email, false);
                    Response.Cookies.Add(cookie);

                    return RedirectToAction("Dashboard");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }
        
        // GET: Account/Logout
        public ActionResult Logout()
        {
            var cookie = Request.Cookies["X-KEY"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.UtcNow.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
    
            if (Session != null)
            {
                Session["User"] = null;
                Session.Clear();
                Session.Abandon();
            }
            
            return RedirectToAction("Login");
        }

    }
}