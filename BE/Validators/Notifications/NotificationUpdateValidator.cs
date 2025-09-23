using FluentValidation;
using VietCommerce.Core.DTOs.Notifications;

namespace VietCommerce.Core.Validators.Notifications;

public class NotificationUpdateValidator : AbstractValidator<NotificationUpdateDTO>
{
    public NotificationUpdateValidator()
    {
        // Add validation rules if needed
    }
}