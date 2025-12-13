using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for GetCampaignStatsQueryDto
/// Validates: Requirements 7.4, 7.5
/// </summary>
public class GetCampaignStatsQueryDtoValidator : AbstractValidator<GetCampaignStatsQueryDto>
{
    public GetCampaignStatsQueryDtoValidator()
    {
        // Requirement 7.5: If both dates are provided, ToDate must be greater than FromDate
        RuleFor(x => x)
            .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.ToDate > x.FromDate)
            .WithMessage("End date must be greater than start date")
            .WithName("ToDate");

        // Optional: FromDate should not be too far in the future
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(5)).WithMessage("From date cannot be in the future")
            .When(x => x.FromDate.HasValue);

        // Optional: ToDate should not be too far in the future
        RuleFor(x => x.ToDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(5)).WithMessage("To date cannot be in the future")
            .When(x => x.ToDate.HasValue);
    }
}
