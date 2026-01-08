using expensemanager.Constants;
using expensemanager.Data;
using expensemanager.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Controllers
{
    [AuthorizeUser]
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AnalyticsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> ExpenseBreakdown()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var data = await _db.Expenses.AsNoTracking()
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category!.Title)
                .Select(g => new { label = g.Key, value = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.value)
                .ToListAsync();
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> MonthlySpending()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
            var baseDate = new DateTime(2000, 1, 1);

            var grouped = await _db.Expenses.AsNoTracking()
                .Where(e => e.UserId == userId)
                .GroupBy(e => EF.Functions.DateDiffMonth(baseDate, e.Date))
                .Select(g => new { Index = g.Key, Value = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Index)
                .ToListAsync();

            var result = grouped.Select(x =>
            {
                var idx = x.Index;
                var dt = baseDate.AddMonths(idx);
                return new { label = dt.ToString("yyyy-MM"), value = x.Value };
            }).ToList();

            return Json(result);
        }
    }
}
 