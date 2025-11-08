using VietCommerce.Core.DTOs.Users;
namespace VietCommerce.Core.DTOs.Auth;
public class AuthResponseDTO
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public UserInfoDTO User { get; set; }
}
