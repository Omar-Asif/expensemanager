using System.Diagnostics;
using expensemanager.Constants;
using expensemanager.Models;
using Microsoft.AspNetCore.Mvc;

namespace expensemanager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // If authenticated, redirect to appropriate dashboard so Home is not empty.
            var uid = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (uid is not null)
            {
                var role = HttpContext.Session.GetString(SessionKeys.UserRole) ?? "User";
                return string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                    ? RedirectToAction("Admin", "Dashboard")
                    : RedirectToAction("User", "Dashboard");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
