// Core/DTOs/Auth/ChangePasswordRequestDTO.cs
using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Auth;

public class ChangePasswordRequestDTO
{
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm new password is required")]
    [Compare("NewPassword", ErrorMessage = "New passwords do not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}