using AutoMapper;
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Helpers;
public static class MappingExtensions
{
    public static User ToEntity(this RegisterRequestDTO dto, IMapper mapper)
        => mapper.Map<User>(dto);
    public static User ToEntity(this SocialLoginRequestDTO dto, IMapper mapper)
        => mapper.Map<User>(dto);
    public static AuthResponseDTO ToAuthResponse(this User user, IMapper mapper)
        => mapper.Map<AuthResponseDTO>(user);
    public static void UpdateFromSocialLogin(this User user, SocialLoginRequestDTO dto)
    {
        user.Provider ??= dto.Provider;
        user.AvatarUrl ??= dto.AvatarUrl;
    }
}
