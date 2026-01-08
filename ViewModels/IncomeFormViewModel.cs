using System.ComponentModel.DataAnnotations;

namespace expensemanager.ViewModels
{
    public class IncomeFormViewModel
    {
        public int? Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
