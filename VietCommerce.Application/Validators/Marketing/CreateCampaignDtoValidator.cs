using FluentValidation;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for CreateCampaignDto
/// Validates: Requirements 1.1, 1.2, 1.3
/// </summary>
public class CreateCampaignDtoValidator : AbstractValidator<CreateCampaignDto>
{
    public CreateCampaignDtoValidator()
    {
        // Requirement 1.1: Campaign name is required
        RuleFor(x => x.CampaignName)
            .NotEmpty().WithMessage("Campaign name is required")
            .MaximumLength(255).WithMessage("Campaign name cannot exceed 255 characters");

        // Requirement 1.1: StoreId is required
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required");

        // Requirement 1.1: CampaignType is required
        RuleFor(x => x.CampaignType)
            .IsInEnum().WithMessage("Campaign type must be a valid value");

        // Requirement 1.2: EndDate must be greater than StartDate
        RuleFor(x => x)
            .Must(x => x.EndDate > x.StartDate)
            .WithMessage("End date must be greater than start date")
            .WithName("EndDate");

        // Requirement 1.3: Budget must be non-negative
        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0");

        // Optional: Description validation
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Optional: TargetingRules validation (if provided, should be valid JSON)
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
