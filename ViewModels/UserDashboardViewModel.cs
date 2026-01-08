namespace expensemanager.ViewModels
{
    /// <summary>
    /// Personal statistics for the signed-in user's dashboard.
    /// </summary>
    public class UserDashboardViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ExpensesCount { get; set; }
        public decimal TotalExpenseAmount { get; set; }
        public decimal MonthlyExpenseAmount { get; set; }
        public decimal TotalIncomeAmount { get; set; }
        public decimal RemainingBalance => TotalIncomeAmount - TotalExpenseAmount;
    }
}
