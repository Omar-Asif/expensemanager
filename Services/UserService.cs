using expensemanager.Data;
using expensemanager.Models;
using expensemanager.Security;
using Microsoft.EntityFrameworkCore;

namespace expensemanager.Services
{
    /// <summary>
    /// Implements user operations using EF Core and secure password hashing.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/>.
        /// </summary>
        public UserService(ApplicationDbContext db) => _db = db;

        /// <inheritdoc />
        public async Task<User?> AuthenticateAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null) return null;
            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }

        /// <inheritdoc />
        public async Task<User> RegisterAsync(string name, string email, string password, string role = "User", CancellationToken ct = default)
        {
            if (await EmailExistsAsync(email, ct))
                throw new InvalidOperationException("Email already registered.");

            var entity = new User
            {
                Name = name,
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role
            };

            _db.Users.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        /// <inheritdoc />
        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => _db.Users.AnyAsync(u => u.Email == email, ct);

        /// <inheritdoc />
        public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}
