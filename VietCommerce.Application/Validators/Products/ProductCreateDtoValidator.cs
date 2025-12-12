using FluentValidation;
using VietCommerce.Core.DTOs.Products;

namespace VietCommerce.Application.Validators.Products;

/// <summary>
/// Validator for ProductCreateDto with package product fields
/// Validates: Requirements 1.1, 1.2
/// </summary>
public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    private readonly CustomizableOptionDtoValidator _customizableOptionValidator;

    public ProductCreateDtoValidator()
    {
        _customizableOptionValidator = new CustomizableOptionDtoValidator();

        // Requirement 1.1: Validate Details list is not empty (if package product)
        RuleFor(x => x.Details)
            .Must(BeValidDetailsForPackage)
            .WithMessage("Details list must not be empty for package products")
            .When(x => x.Details != null);

        // Requirement 1.2: Validate CustomizableOptions structure (if present)
        RuleFor(x => x.CustomizableOptions)
            .Must(BeValidCustomizableOptionsStructure)
            .WithMessage("CustomizableOptions must have valid structure")
            .When(x => x.CustomizableOptions != null && x.CustomizableOptions.Count > 0);

        // Requirement 1.2: Validate each option using CustomizableOptionDto validator
        RuleForEach(x => x.CustomizableOptions)
            .SetValidator(_customizableOptionValidator)
            .When(x => x.CustomizableOptions != null && x.CustomizableOptions.Count > 0);
    }

    /// <summary>
    /// Validates that Details list is not empty if provided
    /// </summary>
    private static bool BeValidDetailsForPackage(List<string>? details)
    {
        if (details == null)
            return true;

        return details.Count > 0 && details.All(d => !string.IsNullOrWhiteSpace(d));
    }

    /// <summary>
    /// Validates that CustomizableOptions structure is valid
    /// </summary>
    private static bool BeValidCustomizableOptionsStructure(List<CustomizableOptionDto>? options)
    {
        if (options == null)
            return true;

        if (options.Count == 0)
            return true;

        // Check for duplicate option IDs
        var optionIds = options.Select(o => o.Id).ToList();
        if (optionIds.Count != optionIds.Distinct().Count())
            return false;

        return true;
    }
}
