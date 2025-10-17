using System.Security.Cryptography;
using System.Text;

namespace VietCommerce.Api.Helpers;

public static class PasswordHelper
{
    public static void CreatePasswordHash(string password, out string hash)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        hash = $"{salt}.{computedHash}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = parts[1];

        using var hmac = new HMACSHA512(salt);
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

        return computedHash == expectedHash;
    }
}
