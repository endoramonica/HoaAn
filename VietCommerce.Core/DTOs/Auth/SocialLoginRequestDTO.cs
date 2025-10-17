// Core/DTOs/Auth/SocialLoginRequestDTO.cs
using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Auth;

public class SocialLoginRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Provider { get; set; } // "Google", "Facebook", "Apple"...
    public string? ProviderId { get; set; } // ID của tài khoản bên thứ ba
    public string? AvatarUrl { get; set; }
}
