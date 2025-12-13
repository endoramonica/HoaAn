using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for UpdateCampaignDto
/// Validates: Requirements 1.2, 1.3, 1.4
/// </summary>
public class UpdateCampaignDtoValidator : AbstractValidator<UpdateCampaignDto>
{
    public UpdateCampaignDtoValidator()
    {
        // Campaign name validation (optional field)
        RuleFor(x => x.CampaignName)
            .MaximumLength(255).WithMessage("Campaign name cannot exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.CampaignName));

        // Description validation (optional field)
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Requirement 1.2: If both dates are provided, EndDate must be greater than StartDate
        RuleFor(x => x)
            .Must(x => !x.StartDate.HasValue || !x.EndDate.HasValue || x.EndDate > x.StartDate)
            .WithMessage("End date must be greater than start date")
            .WithName("EndDate");

        // Requirement 1.3: Budget must be non-negative if provided
        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0")
            .When(x => x.Budget.HasValue);

        // TargetingRules validation (if provided, should be valid JSON)
        RuleFor(x => x.TargetingRules)
            .Must(BeValidJson).WithMessage("Targeting rules must be valid JSON")
            .When(x => !string.IsNullOrEmpty(x.TargetingRules));
    }

    /// <summary>
    /// Validates that a string is valid JSON
    /// </summary>
    private static bool BeValidJson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return true;

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
