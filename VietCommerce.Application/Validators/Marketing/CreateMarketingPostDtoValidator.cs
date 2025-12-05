using FluentValidation;
using System.Text.RegularExpressions;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Application.Validators.Marketing;

/// <summary>
/// Validator for CreateMarketingPostDto
/// Validates: Requirements 12.1, 12.2, 12.3, 12.4, 12.6
/// </summary>
public class CreateMarketingPostDtoValidator : AbstractValidator<CreateMarketingPostDto>
{
    public CreateMarketingPostDtoValidator()
    {
        // Requirement 12.1: Title validation - Required, 3-200 characters
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

        // Requirement 12.2: Content validation - Required, min 10 characters
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(10).WithMessage("Content must be at least 10 characters")
            .MaximumLength(10000).WithMessage("Content cannot exceed 10000 characters");

        // Requirement 12.3: Hashtags validation - Alphanumeric and underscores only
        RuleFor(x => x.Hashtags)
            .Must(BeValidHashtags).WithMessage("Hashtags must contain only alphanumeric characters and underscores")
            .When(x => x.Hashtags != null && x.Hashtags.Count > 0);

        // Requirement 12.4: ScheduledDate validation - Must be future date if provided
        RuleFor(x => x.ScheduledDate)
            .Must(BeFutureDate).WithMessage("Scheduled date must be in the future")
            .When(x => x.ScheduledDate.HasValue);

        // Requirement 12.6: Status validation - Must be valid enum value
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid value (Draft, Published, or Scheduled)");

        // Additional validations for optional fields
        RuleFor(x => x.ShortDescription)
            .MaximumLength(500).WithMessage("Short description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ShortDescription));

        RuleFor(x => x.PriorityScore)
            .InclusiveBetween(1, 100).WithMessage("Priority score must be between 1 and 100");

        RuleFor(x => x.MetaTitle)
            .MaximumLength(200).WithMessage("Meta title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(500).WithMessage("Meta description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));
    }

    /// <summary>
    /// Validates that all hashtags contain only alphanumeric characters and underscores
    /// </summary>
    private static bool BeValidHashtags(List<string>? hashtags)
    {
        if (hashtags == null || hashtags.Count == 0)
            return true;

        var hashtagPattern = new Regex(@"^[a-zA-Z0-9_]+$");
        foreach (var tag in hashtags)
        {
            if (string.IsNullOrWhiteSpace(tag) || !hashtagPattern.IsMatch(tag))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Validates that the scheduled date is in the future
    /// </summary>
    private static bool BeFutureDate(DateTime? scheduledDate)
    {
        if (!scheduledDate.HasValue)
            return true;

        return scheduledDate.Value > DateTime.UtcNow;
    }
}
