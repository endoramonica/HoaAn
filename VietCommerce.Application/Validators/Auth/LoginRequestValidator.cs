// Core/Validators/Auth/LoginRequestValidator.cs
using FluentValidation;
using VietCommerce.Core.DTOs.Auth;
namespace VietCommerce.Core.Validators.Auth;
public class LoginRequestValidator : AbstractValidator<LoginDTO>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}
