using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Domain.Helpers
{
    public static class SessionKeyHelper
    {
        public static string GenerateKeyBase64(int bytes = 64)
        {
            var b = RandomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(b);
        }

        public static string ComputeSha256Hex(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
