using FluentValidation;
using VietCommerce.Core.DTOs.Admin;

namespace VietCommerce.Api.Validators.Admin;

public class AdminUserUpdateValidator : AbstractValidator<AdminUserUpdateDTO>
{
    public AdminUserUpdateValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.RoleIds)
            .Must(x => x == null || x.All(id => id != Guid.Empty))
            .WithMessage("Invalid role IDs provided")
            .When(x => x.RoleIds != null);
    }
}
