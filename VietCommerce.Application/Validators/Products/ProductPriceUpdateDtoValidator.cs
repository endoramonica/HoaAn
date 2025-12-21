using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Application.Validators.Products;

/// <summary>
/// Validator for ProductPriceUpdateDto
/// Validates: Requirements 1.4, 1.5
/// Property 1: Price Validation
/// </summary>
public class ProductPriceUpdateDtoValidator : AbstractValidator<ProductPriceUpdateDto>
{
    public ProductPriceUpdateDtoValidator()
    {
        // Requirement 1.5: Price must be greater than 0 (if provided)
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .When(x => x.Price.HasValue);

        // Requirement 1.4: EffectiveTo must be a valid date (if provided)
        RuleFor(x => x.EffectiveTo)
            .NotEmpty()
            .WithMessage("Effective to date must be provided")
            .When(x => x.EffectiveTo.HasValue);
    }
}
