using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for CreatePromotionDto
/// Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5
/// </summary>
public class CreatePromotionDtoValidator : AbstractValidator<CreatePromotionDto>
{
    public CreatePromotionDtoValidator()
    {
        // Requirement 2.3: Promotion name is required
        RuleFor(x => x.PromotionName)
            .NotEmpty().WithMessage("Promotion name is required")
            .MaximumLength(255).WithMessage("Promotion name cannot exceed 255 characters");

        // Requirement 2.3: PromotionType is required
        RuleFor(x => x.PromotionType)
            .IsInEnum().WithMessage("Promotion type must be a valid value");

        // Requirement 2.3: DiscountValue is required and must be positive
        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than 0");

        // Requirement 2.4: EndDate must be greater than StartDate
        RuleFor(x => x)
            .Must(x => x.EndDate > x.StartDate)
            .WithMessage("End date must be greater than start date")
            .WithName("EndDate");

        // Optional: MinOrderAmount validation
        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount must be greater than or equal to 0")
            .When(x => x.MinOrderAmount.HasValue);

        // Optional: MaxDiscount validation
        RuleFor(x => x.MaxDiscount)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum discount must be greater than or equal to 0")
            .When(x => x.MaxDiscount.HasValue);

        // Optional: UsageLimit validation
        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).WithMessage("Usage limit must be greater than 0")
            .When(x => x.UsageLimit.HasValue);

        // Optional: Conditions validation
        RuleFor(x => x.Conditions)
            .MaximumLength(1000).WithMessage("Conditions cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Conditions));
    }
}
