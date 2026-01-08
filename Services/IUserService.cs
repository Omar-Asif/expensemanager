using expensemanager.Models;

namespace expensemanager.Services
{
    /// <summary>
    /// User-related operations including authentication and registration.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Validates credentials and returns the matching user or null.
        /// </summary>
        Task<User?> AuthenticateAsync(string email, string password, CancellationToken ct = default);
        /// <summary>
        /// Registers a new user with hashed password and returns the created entity.
        /// </summary>
        Task<User> RegisterAsync(string name, string email, string password, string role = "User", CancellationToken ct = default);
        /// <summary>
        /// Checks whether an email is already registered.
        /// </summary>
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        /// <summary>
        /// Retrieves a user by id.
        /// </summary>
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
