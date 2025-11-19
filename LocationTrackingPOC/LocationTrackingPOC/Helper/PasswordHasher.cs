using System.Security.Cryptography;
using System.Text;

namespace LocationTrackingPOC.Helper
{
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
