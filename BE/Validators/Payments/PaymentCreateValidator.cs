using FluentValidation;
using VietCommerce.Core.DTOs.Payments;

namespace VietCommerce.Core.Validators.Payments;

public class PaymentCreateValidator : AbstractValidator<PaymentCreateDTO>
{
    public PaymentCreateValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.PaymentMethodId)
            .NotEmpty().WithMessage("Payment method ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");
    }
}