using FluentValidation;
using VietCommerce.Core.DTOs.Users;

namespace VietCommerce.Core.Validators.Users;

public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format")
            .Length(1, 255).When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Email must be between 1 and 255 characters");

        RuleFor(x => x.PhoneNumber)
            .Length(0, 20).WithMessage("Phone number must not exceed 20 characters");
    }
}