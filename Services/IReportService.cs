using System.Threading;

namespace expensemanager.Services
{
    /// <summary>
    /// Generates PDF reports for user expenses.
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Produces a PDF summary of expenses for the specified user.
        /// </summary>
        Task<byte[]> GenerateUserExpenseSummaryAsync(int userId, CancellationToken ct = default);
    }
}
