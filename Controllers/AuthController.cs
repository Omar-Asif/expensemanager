using expensemanager.Constants;
using expensemanager.Services;
using expensemanager.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace expensemanager.Controllers
{
    /// <summary>
    /// Handles custom session-based authentication: login, registration, and logout.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IUserService _users;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/>.
        /// </summary>
        public AuthController(IUserService users) => _users = users;

        /// <summary>
        /// Returns the login view.
        /// </summary>
        [HttpGet]
        public IActionResult Login() => View();

        /// <summary>
        /// Authenticates user, creates session, and redirects to role-specific dashboard or returnUrl.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _users.AuthenticateAsync(model.Email, model.Password);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View(model);
            }

            HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
            HttpContext.Session.SetString(SessionKeys.UserRole, user.Role);
            HttpContext.Session.SetString(SessionKeys.UserName, user.Name);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase)
                ? RedirectToAction("Admin", "Dashboard")
                : RedirectToAction("User", "Dashboard");
        }

        /// <summary>
        /// Returns the registration view.
        /// </summary>
        [HttpGet]
        public IActionResult Register() => View();

        /// <summary>
        /// Registers a new user, stores hashed password, creates session, and redirects to dashboard.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var user = await _users.RegisterAsync(model.Name, model.Email, model.Password);
                HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
                HttpContext.Session.SetString(SessionKeys.UserRole, user.Role);
                HttpContext.Session.SetString(SessionKeys.UserName, user.Name);

                return string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction("Admin", "Dashboard")
                    : RedirectToAction("User", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        /// <summary>
        /// Clears session and returns to login.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
