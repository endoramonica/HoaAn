using FluentValidation;
using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Core.Validators.Orders;

public class OrderUpdateValidator : AbstractValidator<OrderUpdateDTO>
{
    public OrderUpdateValidator()
    {
        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).When(x => x.TotalAmount.HasValue).WithMessage("Total amount must be greater than 0");

        RuleFor(x => x.Notes)
            .Length(0, 500).When(x => x.Notes != null).WithMessage("Notes must not exceed 500 characters");
    }
}