using AutoMapper;
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Application.Mappings {

    public partial class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            // 🔹 Auth: RegisterRequestDTO → User
            CreateMap<RegisterRequestDTO, User>()
                   .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
                   .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // hash ở service
                   .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                   .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                   .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                   .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))

                   .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

               // 🔹 SocialLoginRequestDTO → User
CreateMap<SocialLoginRequestDTO, User>()
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
    .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider))
    
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(_ => UserStatus.ACTIVE))
            //.ForAllOtherMembers(opt => opt.Ignore());

            // ✅ User -> UserDetailDTO
            CreateMap<User, UserDetailDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles != null
                        ? src.UserRoles.Select(ur => ur.Role != null ? ur.Role.Name : string.Empty).ToList()
                        : new List<string>()
                ))
                ;
        }
    }
}
