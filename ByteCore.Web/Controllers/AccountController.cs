using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ByteCore.BusinessLogic;
using ByteCore.BusinessLogic.Attributes;
using ByteCore.BusinessLogic.Interfaces;
using ByteCore.Domain.UserScope;

namespace ByteCore.Web.Controllers
{
    public class AccountController : BaseController
    {
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
            var userBl = Bl.GetUserBl();
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields should be presented.");
                return View();
            }

            try
            {
                await userBl.RegisterUserAsync(
                    username, 
                    email, 
                    password, 
                    Request.Browser.Browser,
                    Request.UserHostAddress,
                    Request.UserAgent);
                var cookie = await userBl.GetUserCookieAsync(email, true);
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
            var userBl = Bl.GetUserBl();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "You should enter email and password.");
                return View();
            }

            try
            {
                userBl.AuthenticateUser(email, password, Request.Browser.Browser, Request.UserHostAddress, Request.UserAgent);
                var cookie = await userBl.GetUserCookieAsync(email, rememberMe);
                Response.Cookies.Add(cookie);

                return Redirect(returnUrl ?? "/");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Account/Dashboard
        [CustomAuthorize]
        public ActionResult Dashboard()
        {
            var userBl = Bl.GetUserBl();
            var user = userBl.GetUserByEmail(User.Identity.Name);

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
            var userBl = Bl.GetUserBl();
            var user = userBl.GetUserByEmail(User.Identity.Name);
            
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
            var userBl = Bl.GetUserBl();
            if (ModelState.IsValid)
            {
                try
                {
                    await userBl.UpdateUserAsync(User.Identity.Name, model);
                    var cookie = await userBl.GetUserCookieAsync(model.Email, false);
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