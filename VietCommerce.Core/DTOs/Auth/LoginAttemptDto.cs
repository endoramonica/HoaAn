namespace VietCommerce.Core.DTOs.Auth;

public class LoginAttemptDto
{
    public string Email { get; set; } = string.Empty;
public int FailedCount { get; set; }
public int LockLevel { get; set; } = 0; // mức khóa tăng dần
}