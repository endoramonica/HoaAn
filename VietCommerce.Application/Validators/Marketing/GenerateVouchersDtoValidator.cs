using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for GenerateVouchersDto
/// Validates: Requirements 3.1
/// </summary>
public class GenerateVouchersDtoValidator : AbstractValidator<GenerateVouchersDto>
{
    public GenerateVouchersDtoValidator()
    {
        // Requirement 3.1: Quantity is required and must be positive
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10000");

        // Requirement 3.1: ExpiryDate is required and must be in the future
        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("Expiry date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future");

        // Optional: Prefix validation
        RuleFor(x => x.Prefix)
            .MaximumLength(50).WithMessage("Prefix cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9_]*$").WithMessage("Prefix must contain only uppercase letters, numbers, and underscores")
            .When(x => !string.IsNullOrEmpty(x.Prefix));
    }
}
