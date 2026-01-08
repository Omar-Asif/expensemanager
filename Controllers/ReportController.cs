using expensemanager.Constants;
using expensemanager.Services;
using Microsoft.AspNetCore.Mvc;

namespace expensemanager.Controllers
{
    /// <summary>
    /// Generates user-specific PDF reports backed by QuestPDF.
    /// </summary>
    public class ReportController : Controller
    {
        private readonly IReportService _reports;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/>.
        /// </summary>
        public ReportController(IReportService reports) => _reports = reports;

        /// <summary>
        /// Returns a PDF summary of the current user's expenses.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> MySummary()
        {
            var userId = HttpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId is null) return RedirectToAction("Login", "Auth");

            var pdf = await _reports.GenerateUserExpenseSummaryAsync(userId.Value);
            return File(pdf, "application/pdf", $"ExpenseSummary_{DateTime.UtcNow:yyyyMMddHHmm}.pdf");
        }
    }
}
