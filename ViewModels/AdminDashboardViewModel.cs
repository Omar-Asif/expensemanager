namespace expensemanager.ViewModels
{
    /// <summary>
    /// Aggregated statistics for the admin dashboard.
    /// </summary>
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalExpenses { get; set; }
        public decimal TotalExpenseAmount { get; set; }
        public int CategoriesCount { get; set; }
    }
}
