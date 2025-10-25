// Core/DTOs/Auth/ResetPasswordRequestDTO.cs
using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Auth;
public class ResetPasswordRequestDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Reset token is required")]
    public string Token { get; set; } = string.Empty;
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string NewPassword { get; set; } = string.Empty;
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
