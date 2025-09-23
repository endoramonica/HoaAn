using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Core.Validators.Products;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateDTO>
{
    public ProductUpdateValidator()
    {
        RuleFor(x => x.Name)
            .Length(1, 200).When(x => x.Name != null).WithMessage("Name must be between 1 and 200 characters");

        RuleFor(x => x.Description)
            .Length(0, 1000).When(x => x.Description != null).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).When(x => x.Price.HasValue).WithMessage("Price must be greater than 0");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).When(x => x.StockQuantity.HasValue).WithMessage("Stock quantity cannot be negative");
    }
}