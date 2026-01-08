namespace expensemanager.ViewModels
{
    public class IncomeListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class IncomeListViewModel
    {
        public IReadOnlyList<IncomeListItemViewModel> Items { get; set; } = Array.Empty<IncomeListItemViewModel>();
        public decimal TotalAmount { get; set; }
    }
}
