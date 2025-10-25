// Core/Validators/Auth/RegisterRequestValidator.cs
using FluentValidation;
using VietCommerce.Core.DTOs.Auth;
namespace VietCommerce.Core.Validators.Auth;
public class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
{
    private readonly IUserRepository _userRepository;
    public RegisterRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters");
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.StoreId)
            .NotEmpty().WithMessage("Store ID is required");
    }
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userRepository.EmailExistsAsync(email);
        return !user;
    }
}
