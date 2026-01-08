using System.ComponentModel.DataAnnotations;

namespace expensemanager.ViewModels
{
    /// <summary>
    /// Represents user input required to authenticate.
    /// </summary>
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
