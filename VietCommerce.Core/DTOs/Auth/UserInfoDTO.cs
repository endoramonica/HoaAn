namespace VietCommerce.Core.DTOs.Auth;

public class UserInfoDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty; // ví d? "Google"
    public List<string> Roles { get; set; } = new();

}
