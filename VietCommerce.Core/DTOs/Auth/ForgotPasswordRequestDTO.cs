using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Auth;
public class ForgotPasswordRequestDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
