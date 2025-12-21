using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Application.Validators.Products;

/// <summary>
/// Validator for ProductPriceCreateDto
/// Validates: Requirements 1.4, 1.5
/// Property 1: Price Validation
/// </summary>
public class ProductPriceCreateDtoValidator : AbstractValidator<ProductPriceCreateDto>
{
    public ProductPriceCreateDtoValidator()
    {
        // Requirement 1.5: Price must be greater than 0
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        // Requirement 1.4: EffectiveFrom must be before EffectiveTo (if provided)
        RuleFor(x => x)
            .Must(BeValidEffectiveDateRange)
            .WithMessage("EffectiveFrom must be before EffectiveTo");

        // ProductId must not be empty
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        // PriceType must be valid
        RuleFor(x => x.PriceType)
            .IsInEnum()
            .WithMessage("Price type must be a valid value");

        // EffectiveFrom must not be in the future (optional - can be adjusted based on business rules)
        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Effective from date is required");
    }

    /// <summary>
    /// Validates that EffectiveFrom is before EffectiveTo (if EffectiveTo is provided)
    /// </summary>
    private static bool BeValidEffectiveDateRange(ProductPriceCreateDto dto)
    {
        // If EffectiveTo is not provided, it's valid
        if (!dto.EffectiveTo.HasValue)
            return true;

        // EffectiveFrom must be before EffectiveTo
        return dto.EffectiveFrom < dto.EffectiveTo.Value;
    }
}
