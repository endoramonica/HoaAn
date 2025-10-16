using AutoMapper;
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Application.Mappings {

    public class AuthMappingProfile : Profile
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
        public class UserMappingProfile : Profile
        {
            public UserMappingProfile()
            {
                // User -> UserDetailDTO
                CreateMap<User, UserDetailDTO>()
                    .ForMember(dest => dest.StoreName,
                        opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                    .ForMember(dest => dest.Roles,
                        opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));

                // User -> UserListDTO
                CreateMap<User, UserListDTO>()
                    .ForMember(dest => dest.FullName,
                        opt => opt.MapFrom(src => src.Name ?? string.Empty))
                    .ForMember(dest => dest.PhoneNumber,
                        opt => opt.MapFrom(src => src.Phone))
                    .ForMember(dest => dest.StoreName,
                        opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                    .ForMember(dest => dest.Roles,
                        opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()))
                    .ForMember(dest => dest.CreatedDate,
                        opt => opt.MapFrom(src => src.CreatedAt))
                    .ForMember(dest => dest.CustomerId,
                        opt => opt.Ignore()) // Set from other source if needed
                    .ForMember(dest => dest.TenantId,
                        opt => opt.Ignore()); // Set from other source if needed

                // UserCreateDTO -> User
                CreateMap<UserCreateDTO, User>()
                    .ForMember(dest => dest.Name,
                        opt => opt.MapFrom(src => src.FullName))
                    .ForMember(dest => dest.Email,
                        opt => opt.MapFrom(src => src.Email.ToLower()))
                    .ForMember(dest => dest.Phone,
                        opt => opt.MapFrom(src => src.PhoneNumber))
                    .ForMember(dest => dest.IsActive,
                        opt => opt.MapFrom(src => true))
                    .ForMember(dest => dest.PasswordHash,
                        opt => opt.Ignore()) // Will be set by password hasher
                    .ForMember(dest => dest.Id,
                        opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt,
                        opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedAt,
                        opt => opt.Ignore())
                    .ForMember(dest => dest.StoreId,
                        opt => opt.Ignore()) // Must be set separately
                    .ForMember(dest => dest.Status,
                        opt => opt.MapFrom(src => VietCommerce.Core.Enums.Users.UserStatus.ACTIVE));

                // UserUpdateDTO -> User (for updating existing user)
                CreateMap<UserUpdateDTO, User>()
                    .ForMember(dest => dest.Name,
                        opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.FullName) ? src.FullName : dest.Name))
                    .ForMember(dest => dest.Email,
                        opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.Email) ? src.Email.ToLower() : dest.Email))
                    .ForMember(dest => dest.Phone,
                        opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.PhoneNumber) ? src.PhoneNumber : dest.Phone))
                    .ForMember(dest => dest.IsActive,
                        opt => opt.MapFrom(src => src.IsActive))
                   ; // Don't touch other properties

                // Role -> String (for role names)
                CreateMap<Role, string>()
                    .ConvertUsing(r => r.Name);

                // UserRole -> String (for getting role name from UserRole)
                CreateMap<UserRole, string>()
                    .ConvertUsing(ur => ur.Role.Name);
            }
        };
    }
}
