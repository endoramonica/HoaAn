// Core/Validators/Auth/ChangePasswordValidator.cs
using FluentValidation;
using VietCommerce.Core.DTOs.Auth;

namespace VietCommerce.Core.Validators.Auth;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequestDTO>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters")
            .MaximumLength(100).WithMessage("New password cannot exceed 100 characters")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm new password is required")
            .Equal(x => x.NewPassword).WithMessage("New passwords do not match");
    }
}