using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using ByteCore.Web.Models;
using ByteCore.Web.Services;

namespace ByteCore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
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
                var newUser = await _userService.RegisterUserAsync(username, email, password);
                
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, newUser.Name),
                    new Claim(ClaimTypes.Email, newUser.Email),
                }, "Password");

                var principal = new ClaimsPrincipal(identity);
                HttpContext.User = principal;
                FormsAuthentication.SetAuthCookie(email, true);
                
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
        public ActionResult Login(string email, string password, bool rememberMe = false, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "You should enter email and password.");
                return View();
            }

            try
            {
                var user = _userService.AuthenticateUser(email, password);
        
                var identity = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                }, "Password");
                
                if (user.Name == "admin") 
                    Roles.AddUserToRole(user.Name, "Admin");

                var principal = new ClaimsPrincipal(identity);
                HttpContext.User = principal;
                FormsAuthentication.SetAuthCookie(user.Email, rememberMe);

                return Redirect(returnUrl ?? "/");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Account/Dashboard
        [Authorize]
        public ActionResult Dashboard()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/Manage
        [Authorize]
        public ActionResult Manage()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            
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
        public async Task<ActionResult> Manage(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.UpdateUserAsync(User.Identity.Name, model);
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, model.Name),
                        new Claim(ClaimTypes.Email, model.Email),
                    }, "Password");

                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.User = principal;
                    FormsAuthentication.SetAuthCookie(model.Email, true);

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
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}