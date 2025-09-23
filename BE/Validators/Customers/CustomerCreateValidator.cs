using FluentValidation;
using VietCommerce.Core.DTOs.Customers;

namespace VietCommerce.Core.Validators.Customers;

public class CustomerCreateValidator : AbstractValidator<CustomerCreateDTO>
{
    public CustomerCreateValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .Length(1, 100).WithMessage("Full name must be between 1 and 100 characters");
    }
}