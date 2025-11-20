// File: NotificationMappingProfile.cs
using AutoMapper;
using VietCommerce.Core.DTOs;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.Entities.Notifications;

namespace VietCommerce.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        // Notification → NotificationDto
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.TemplateName,
                opt => opt.MapFrom(src => src.Template != null ? src.Template.Name : null))
            .ForMember(dest => dest.TemplateType,
                opt => opt.MapFrom(src => src.Template != null ? src.Template.Type : null));

        // NotificationTemplate → NotificationTemplateDto
        CreateMap<NotificationTemplate, NotificationTemplateDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Reverse map (nếu cần create/update từ DTO → Entity)
        CreateMap<NotificationDto, Notification>()
            .ForMember(dest => dest.Template, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<NotificationTemplateDto, NotificationTemplate>()
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());
    }
}