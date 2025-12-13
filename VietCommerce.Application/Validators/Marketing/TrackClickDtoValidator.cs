using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for TrackClickDto
/// Validates: Requirements 7.2
/// </summary>
public class TrackClickDtoValidator : AbstractValidator<TrackClickDto>
{
    public TrackClickDtoValidator()
    {
        // Requirement 7.2: SessionId is required
        RuleFor(x => x.SessionId)
            .NotEmpty().WithMessage("Session ID is required")
            .MaximumLength(100).WithMessage("Session ID cannot exceed 100 characters");

        // Requirement 7.2: Page is required
        RuleFor(x => x.Page)
            .NotEmpty().WithMessage("Page is required")
            .MaximumLength(100).WithMessage("Page cannot exceed 100 characters");

        // Optional: RecordedAt validation (if provided, should not be too far in the future)
        RuleFor(x => x.RecordedAt)
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(5)).WithMessage("Recorded at cannot be in the future")
            .When(x => x.RecordedAt.HasValue);
    }
}
