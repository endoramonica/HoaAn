using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for ApplyVoucherDto
/// Validates: Requirements 3.2, 3.4, 3.5
/// </summary>
public class ApplyVoucherDtoValidator : AbstractValidator<ApplyVoucherDto>
{
    public ApplyVoucherDtoValidator()
    {
        // Requirement 3.2: Voucher code is required
        RuleFor(x => x.VoucherCode)
            .NotEmpty().WithMessage("Voucher code is required")
            .MaximumLength(100).WithMessage("Voucher code cannot exceed 100 characters")
            .Matches(@"^[A-Z0-9_-]+$").WithMessage("Voucher code must contain only uppercase letters, numbers, hyphens, and underscores");
    }
}
