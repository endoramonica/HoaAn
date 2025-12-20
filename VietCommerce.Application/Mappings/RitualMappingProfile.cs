using AutoMapper;
using System.Text.Json;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Core.Helpers;

namespace VietCommerce.Application.Mappings
{
    /// <summary>
    /// Converter for deserializing metadata JSON to dictionary
    /// </summary>
    public class MetadataConverter : IValueConverter<string, Dictionary<string, object>?>
    {
        public Dictionary<string, object>? Convert(string sourceMember, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(sourceMember))
                return null;

            return JsonSerializer.Deserialize<Dictionary<string, object>>(sourceMember);
        }
    }

    /// <summary>
    /// Converter for serializing metadata dictionary to JSON
    /// </summary>
    public class ReverseMetadataConverter : IValueConverter<Dictionary<string, object>?, string>
    {
        public string Convert(Dictionary<string, object>? sourceMember, ResolutionContext context)
        {
            if (sourceMember == null)
                return null!;

            return JsonSerializer.Serialize(sourceMember);
        }
    }

    /// <summary>
    /// AutoMapper profile for ritual-related entities and DTOs
    /// </summary>
    public class RitualMappingProfile : Profile
    {
        public RitualMappingProfile()
        {
            // ActionEntity -> ActionDto
            CreateMap<ActionEntity, ActionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ActionType))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.ActionTimestamp))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Metadata, opt => opt.ConvertUsing(new MetadataConverter(), src => src.MetadataJson));

            // ActionDto -> ActionEntity
            CreateMap<ActionDto, ActionEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.SessionId))
                .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.ActionTimestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.MetadataJson, opt => opt.ConvertUsing(new ReverseMetadataConverter(), src => src.Metadata))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
