using expensemanager.Constants;
using expensemanager.Data;
using expensemanager.Filters;
using expensemanager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Controllers
{
    /// <summary>
    /// Provides admin and user dashboards with aggregated statistics.
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/>.
        /// </summary>
        public DashboardController(ApplicationDbContext db) => _db = db;

        /// <summary>
        /// Admin-only dashboard with global stats.
        /// </summary>
        [AuthorizeUser("Admin")]
        public async Task<IActionResult> Admin()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalUsers = await _db.Users.CountAsync(),
                TotalExpenses = await _db.Expenses.CountAsync(),
                TotalExpenseAmount = await _db.Expenses.SumAsync(e => (decimal?)e.Amount) ?? 0m,
                CategoriesCount = await _db.Categories.CountAsync()
            };
            return View(vm);
        }

        /// <summary>
        /// User dashboard with personal stats only.
        /// </summary>
        [AuthorizeUser]
        public async Task<IActionResult> User()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var userName = HttpContext.Session.GetString(SessionKeys.UserName) ?? string.Empty;

            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var expensesQuery = _db.Expenses.Where(e => e.UserId == userId);
            var incomeQuery = _db.Incomes.Where(i => i.UserId == userId);

            var vm = new UserDashboardViewModel
            {
                UserId = userId,
                UserName = userName,
                ExpensesCount = await expensesQuery.CountAsync(),
                TotalExpenseAmount = await expensesQuery.SumAsync(e => (decimal?)e.Amount) ?? 0m,
                MonthlyExpenseAmount = await expensesQuery.Where(e => e.Date >= monthStart).SumAsync(e => (decimal?)e.Amount) ?? 0m,
                TotalIncomeAmount = await incomeQuery.SumAsync(i => (decimal?)i.Amount) ?? 0m
            };
            return View(vm);
        }
    }
}
