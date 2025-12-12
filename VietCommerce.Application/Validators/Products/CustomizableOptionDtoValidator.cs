using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Application.Validators.Products;

/// <summary>
/// Validator for CustomizableOptionDto
/// Validates: Requirements 1.2, 1.3
/// </summary>
public class CustomizableOptionDtoValidator : AbstractValidator<CustomizableOptionDto>
{
    public CustomizableOptionDtoValidator()
    {
        // Requirement 1.2: Id validation - Required and not empty
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Option ID is required")
            .NotNull().WithMessage("Option ID cannot be null");

        // Requirement 1.2: Name validation - Required and not empty
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Option name is required")
            .NotNull().WithMessage("Option name cannot be null");

        // Requirement 1.2: UnitPrice validation - Must be greater than 0
        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        // Requirement 1.3: MinQuantity validation - Must be greater than 0
        RuleFor(x => x.MinQuantity)
            .GreaterThan(0).WithMessage("Minimum quantity must be greater than 0");

        // Requirement 1.3: MinQuantity ≤ MaxQuantity validation
        RuleFor(x => x)
            .Must(BeValidQuantityRange).WithMessage("Minimum quantity must be less than or equal to maximum quantity")
            .When(x => x.MaxQuantity.HasValue);
    }

    /// <summary>
    /// Validates that MinQuantity is less than or equal to MaxQuantity (if MaxQuantity exists)
    /// </summary>
    private static bool BeValidQuantityRange(CustomizableOptionDto option)
    {
        if (!option.MaxQuantity.HasValue)
            return true;

        return option.MinQuantity <= option.MaxQuantity.Value;
    }
}
