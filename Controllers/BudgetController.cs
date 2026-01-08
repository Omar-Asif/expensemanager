using expensemanager.Constants;
using expensemanager.Data;
using expensemanager.Filters;
using expensemanager.Models;
using expensemanager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Controllers
{
    [AuthorizeUser]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BudgetController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var now = DateTime.UtcNow;
            var existing = await _db.Budgets.AsNoTracking().FirstOrDefaultAsync(b => b.UserId == userId && b.Year == now.Year && b.Month == now.Month);
            var vm = new BudgetFormViewModel
            {
                Year = existing?.Year ?? now.Year,
                Month = existing?.Month ?? now.Month,
                MonthlyAmount = existing?.MonthlyAmount ?? 0m
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BudgetFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var existing = await _db.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Year == model.Year && b.Month == model.Month);
            if (existing is null)
            {
                _db.Budgets.Add(new Budget { UserId = userId, Year = model.Year, Month = model.Month, MonthlyAmount = model.MonthlyAmount });
            }
            else
            {
                existing.MonthlyAmount = model.MonthlyAmount;
            }
            await _db.SaveChangesAsync();
            TempData["Message"] = "Budget saved.";
            return RedirectToAction(nameof(Index));
        }
    }
}
