using FluentValidation;
using VietCommerce.Core.DTOs.Notifications;

namespace VietCommerce.Core.Validators.Notifications;

public class NotificationCreateValidator : AbstractValidator<NotificationCreateDTO>
{
    public NotificationCreateValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(1, 1000).WithMessage("Title must be between 1 and 1000 characters");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required")
            .Length(1, 5000).WithMessage("Message must be between 1 and 5000 characters");
    }
}