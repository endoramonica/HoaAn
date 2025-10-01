using FluentValidation;
using VietCommerce.Core.DTOs.Admin;

namespace VietCommerce.Api.Validators.Admin;

public class AdminUserCreateValidator : AbstractValidator<AdminUserCreateDTO>
{
    public AdminUserCreateValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required");

        RuleFor(x => x.RoleIds)
            .Must(x => x == null || x.All(id => id != Guid.Empty))
            .WithMessage("Invalid role IDs provided")
            .When(x => x.RoleIds != null);
    }
}
