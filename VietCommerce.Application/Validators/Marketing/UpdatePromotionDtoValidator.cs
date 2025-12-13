using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for UpdatePromotionDto
/// Validates: Requirements 2.3, 2.4, 2.5
/// </summary>
public class UpdatePromotionDtoValidator : AbstractValidator<UpdatePromotionDto>
{
    public UpdatePromotionDtoValidator()
    {
        // Promotion name validation (optional field)
        RuleFor(x => x.PromotionName)
            .MaximumLength(255).WithMessage("Promotion name cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.PromotionName));

        // PromotionType validation (optional field)
        RuleFor(x => x.PromotionType)
            .IsInEnum().WithMessage("Promotion type must be a valid value")
            .When(x => x.PromotionType.HasValue);

        // DiscountValue validation (optional field, must be positive if provided)
        RuleFor(x => x.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than 0")
            .When(x => x.DiscountValue.HasValue);

        // Requirement 2.4: If both dates are provided, EndDate must be greater than StartDate
        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.EndDate > x.StartDate)
            .WithMessage("End date must be greater than start date")
            .WithName("EndDate");

        // MinOrderAmount validation (optional field)
        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum order amount must be greater than or equal to 0")
            .When(x => x.MinOrderAmount.HasValue);

        // MaxDiscount validation (optional field)
        RuleFor(x => x.MaxDiscount)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum discount must be greater than or equal to 0")
            .When(x => x.MaxDiscount.HasValue);

        // UsageLimit validation (optional field)
        RuleFor(x => x.UsageLimit)
            .GreaterThan(0).WithMessage("Usage limit must be greater than 0")
            .When(x => x.UsageLimit.HasValue);

        // Conditions validation (optional field)
        RuleFor(x => x.Conditions)
            .MaximumLength(1000).WithMessage("Conditions cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Conditions));
    }
}
