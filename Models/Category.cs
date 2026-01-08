using System.ComponentModel.DataAnnotations;

namespace expensemanager.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Icon { get; set; }

        [Required]
        [RegularExpression("^(Income|Expense)$", ErrorMessage = "Type must be 'Income' or 'Expense'.")]
        public string Type { get; set; } = "Expense";
    }
}
