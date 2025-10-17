using System.Security.Cryptography;
using System.Text;

namespace VietCommerce.Core.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }

    public static string GenerateResetToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').Replace("=", "");
    }

    public static bool ValidatePasswordStrength(string password, out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return false;
        }

        if (password.Length < 6)
            errors.Add("Password must be at least 6 characters long");

        if (password.Length > 100)
            errors.Add("Password cannot exceed 100 characters");

        if (!password.Any(char.IsLetter))
            errors.Add("Password must contain at least one letter");

        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one number");

        return !errors.Any();
    }
}