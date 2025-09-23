using FluentValidation;
using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Core.Validators.Orders;

public class OrderCreateValidator : AbstractValidator<OrderCreateDTO>
{
    public OrderCreateValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required")
            .Must(items => items.All(i => i.Quantity > 0 && i.UnitPrice > 0)).WithMessage("All items must have valid quantity and price");

        RuleFor(x => x.Shipping)
            .NotNull().When(x => x.ShippingAmount > 0).WithMessage("Shipping details required when shipping amount > 0");
    }
}