using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Core.Validators.Products;

public class ProductCreateValidator : AbstractValidator<ProductCreateDTO>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(1, 200).WithMessage("Name must be between 1 and 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");
    }
}