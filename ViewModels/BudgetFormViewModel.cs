using System.ComponentModel.DataAnnotations;

namespace expensemanager.ViewModels
{
    public class BudgetFormViewModel
    {
        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.UtcNow.Year;

        [Range(1, 12)]
        public int Month { get; set; } = DateTime.UtcNow.Month;

        [Range(0.00, 100000000)]
        [DataType(DataType.Currency)]
        public decimal MonthlyAmount { get; set; }
    }
}
