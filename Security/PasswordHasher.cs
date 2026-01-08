using System.Security.Cryptography;
using System.Text;

namespace expensemanager.Security
{
    public static class PasswordHasher
    {
        // Computes SHA256 hash and returns hex string
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static bool Verify(string input, string expectedHash)
        {
            return Hash(input) == (expectedHash ?? string.Empty).ToLowerInvariant();
        }
    }
}
