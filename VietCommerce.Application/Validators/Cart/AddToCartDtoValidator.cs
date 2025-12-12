using FluentValidation;
using VietCommerce.Core.DTOs.Cart;

namespace VietCommerce.Application.Validators.Cart;

/// <summary>
/// Validator for AddToCartDto with customizations support
/// Validates: Requirements 3.1
/// </summary>
public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartDtoValidator()
    {
        // Requirement 3.1: ProductId validation - Required and not empty
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        // Requirement 3.1: Quantity validation - Must be greater than 0
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        // Requirement 3.1: Validate Customizations structure (if present)
        RuleFor(x => x.Customizations)
            .Must(BeValidCustomizationsStructure)
            .WithMessage("Customizations must have valid structure")
            .When(x => x.Customizations != null && x.Customizations.Count > 0);

        // Requirement 3.1: Validate each customization has valid OptionId and Quantity
        RuleForEach(x => x.Customizations)
            .SetValidator(new CartItemCustomizationDtoValidator())
            .When(x => x.Customizations != null && x.Customizations.Count > 0);
    }

    /// <summary>
    /// Validates that Customizations structure is valid
    /// </summary>
    private static bool BeValidCustomizationsStructure(List<CartItemCustomizationDto>? customizations)
    {
        if (customizations == null)
            return true;

        if (customizations.Count == 0)
            return true;

        // Check for duplicate option IDs
        var optionIds = customizations.Select(c => c.OptionId).ToList();
        if (optionIds.Count != optionIds.Distinct().Count())
            return false;

        return true;
    }
}

/// <summary>
/// Validator for CartItemCustomizationDto
/// Validates individual customization entries
/// </summary>
public class CartItemCustomizationDtoValidator : AbstractValidator<CartItemCustomizationDto>
{
    public CartItemCustomizationDtoValidator()
    {
        // Requirement 3.1: OptionId validation - Required and not empty
        RuleFor(x => x.OptionId)
            .NotEmpty().WithMessage("Option ID is required")
            .NotNull().WithMessage("Option ID cannot be null");

        // Requirement 3.1: Quantity validation - Must be greater than 0
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Customization quantity must be greater than 0");

        // UnitPrice validation - Must be greater than or equal to 0
        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0");

        // TotalPrice validation - Must be greater than or equal to 0
        RuleFor(x => x.TotalPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Total price must be greater than or equal to 0");
    }
}
