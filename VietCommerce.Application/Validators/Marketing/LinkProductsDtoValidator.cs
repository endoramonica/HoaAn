using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for LinkProductsDto
/// Validates: Requirements 2.1, 2.3
/// </summary>
public class LinkProductsDtoValidator : AbstractValidator<LinkProductsDto>
{
    public LinkProductsDtoValidator()
    {
        // ProductIds is required and must have at least one item
        RuleFor(x => x.ProductIds)
            .NotEmpty().WithMessage("At least one product ID is required")
            .Must(x => x.All(id => id != Guid.Empty)).WithMessage("Product IDs cannot be empty GUIDs");

        // Optional: DiscountOverride validation
        RuleFor(x => x.DiscountOverride)
            .GreaterThanOrEqualTo(0).WithMessage("Discount override must be greater than or equal to 0")
            .When(x => x.DiscountOverride.HasValue);
    }
}
