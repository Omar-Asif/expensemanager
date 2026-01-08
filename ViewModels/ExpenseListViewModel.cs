namespace expensemanager.ViewModels
{
    /// <summary>
    /// Represents a single expense in a list view.
    /// </summary>
    public class ExpenseListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string CategoryTitle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Encapsulates a collection of expenses and aggregate total for list pages.
    /// </summary>
    public class ExpenseListViewModel
    {
        public IReadOnlyList<ExpenseListItemViewModel> Items { get; set; } = Array.Empty<ExpenseListItemViewModel>();
        public decimal TotalAmount { get; set; }
    }
}
