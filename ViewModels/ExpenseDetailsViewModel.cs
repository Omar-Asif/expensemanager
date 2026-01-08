namespace expensemanager.ViewModels
{
    /// <summary>
    /// Detailed view model used for expense details and delete confirmation.
    /// </summary>
    public class ExpenseDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the expense identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the expense.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the amount of the expense.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date of the expense.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the description of the expense.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category title of the expense.
        /// </summary>
        public string CategoryTitle { get; set; } = string.Empty;
    }
}
