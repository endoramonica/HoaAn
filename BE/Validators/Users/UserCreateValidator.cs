using FluentValidation;
using VietCommerce.Core.DTOs.Users;

namespace VietCommerce.Core.Validators.Users
{
    public class UserCreateValidator : AbstractValidator<UserCreateDTO>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            // thêm rule khác nếu cần
        }
    }
}