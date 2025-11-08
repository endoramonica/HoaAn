namespace VietCommerce.Core.DTOs.Auth;

public class PasswordResetToken
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}