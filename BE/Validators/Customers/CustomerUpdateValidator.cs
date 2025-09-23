using FluentValidation;
using VietCommerce.Core.DTOs.Customers;

namespace VietCommerce.Core.Validators.Customers;

public class CustomerUpdateValidator : AbstractValidator<CustomerUpdateDTO>
{
    public CustomerUpdateValidator()
    {
        RuleFor(x => x.FullName)
            .Length(1, 100).When(x => x.FullName != null).WithMessage("Full name must be between 1 and 100 characters");

        RuleFor(x => x.Email)
            .Length(0, 255).When(x => x.Email != null).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .Length(0, 20).When(x => x.PhoneNumber != null).WithMessage("Phone number must not exceed 20 characters");
    }
}