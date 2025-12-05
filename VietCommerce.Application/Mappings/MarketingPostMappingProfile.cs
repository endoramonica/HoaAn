using AutoMapper;
using System.Text.Json;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Application.Mappings;

/// <summary>
/// AutoMapper profile for MarketingPost entity
/// Handles bidirectional mapping between entities and DTOs
/// Includes JSON serialization for list properties
/// </summary>
public class MarketingPostMappingProfile : Profile
{
    public MarketingPostMappingProfile()
    {
        // ========== ENTITY TO DTO MAPPINGS ==========

        // MarketingPost → MarketingPostResponseDto
        CreateMap<MarketingPost, MarketingPostResponseDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.ImageUrls)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(src.ImageUrls, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Hashtags)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(src.Hashtags, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.DisplayLocation, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.DisplayLocation)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(src.DisplayLocation, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.MetaKeywords)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(src.MetaKeywords, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.SocialPosts, opt => opt.MapFrom(src => new SocialMediaPostsDto
            {
                Facebook = src.FacebookPost,
                Instagram = src.InstagramPost,
                Twitter = src.TwitterPost,
                LinkedIn = src.LinkedInPost
            }))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // MarketingPost → MarketingPostListDto
        CreateMap<MarketingPost, MarketingPostListDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Hashtags)
                    ? null
                    : JsonSerializer.Deserialize<List<string>>(src.Hashtags, (JsonSerializerOptions?)null)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // MarketingPost → MarketingPostDetailDto
        CreateMap<MarketingPost, MarketingPostDetailDto>()
            .IncludeBase<MarketingPost, MarketingPostResponseDto>();

        // ========== DTO TO ENTITY MAPPINGS ==========

        // CreateMarketingPostDto → MarketingPost
        CreateMap<CreateMarketingPostDto, MarketingPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
                src.ImageUrls != null && src.ImageUrls.Any()
                    ? JsonSerializer.Serialize(src.ImageUrls, (JsonSerializerOptions?)null)
                    : null))
            .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src =>
                src.Hashtags != null && src.Hashtags.Any()
                    ? JsonSerializer.Serialize(src.Hashtags, (JsonSerializerOptions?)null)
                    : null))
            .ForMember(dest => dest.DisplayLocation, opt => opt.MapFrom(src =>
                src.DisplayLocation != null && src.DisplayLocation.Any()
                    ? JsonSerializer.Serialize(src.DisplayLocation, (JsonSerializerOptions?)null)
                    : null))
            .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom(src =>
                src.MetaKeywords != null && src.MetaKeywords.Any()
                    ? JsonSerializer.Serialize(src.MetaKeywords, (JsonSerializerOptions?)null)
                    : null))
            .ForMember(dest => dest.FacebookPost, opt => opt.MapFrom(src => src.SocialPosts != null ? src.SocialPosts.Facebook : null))
            .ForMember(dest => dest.InstagramPost, opt => opt.MapFrom(src => src.SocialPosts != null ? src.SocialPosts.Instagram : null))
            .ForMember(dest => dest.TwitterPost, opt => opt.MapFrom(src => src.SocialPosts != null ? src.SocialPosts.Twitter : null))
            .ForMember(dest => dest.LinkedInPost, opt => opt.MapFrom(src => src.SocialPosts != null ? src.SocialPosts.LinkedIn : null))
            .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.PriorityScore > 80))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        // UpdateMarketingPostDto → MarketingPost (for partial updates)
        CreateMap<UpdateMarketingPostDto, MarketingPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
            .ForMember(dest => dest.Content, opt => opt.Condition(src => src.Content != null))
            .ForMember(dest => dest.ShortDescription, opt => opt.Condition(src => src.ShortDescription != null))
            .ForMember(dest => dest.ImageUrl, opt => opt.Condition(src => src.ImageUrl != null))
            .ForMember(dest => dest.ImageData, opt => opt.Condition(src => src.ImageData != null))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom((src, dest) =>
                src.ImageUrls != null
                    ? (src.ImageUrls.Any() ? JsonSerializer.Serialize(src.ImageUrls, (JsonSerializerOptions?)null) : null)
                    : dest.ImageUrls))
            .ForMember(dest => dest.ProductId, opt => opt.Condition(src => src.ProductId.HasValue))
            .ForMember(dest => dest.Topic, opt => opt.Condition(src => src.Topic != null))
            .ForMember(dest => dest.Platform, opt => opt.Condition(src => src.Platform != null))
            .ForMember(dest => dest.Tone, opt => opt.Condition(src => src.Tone != null))
            .ForMember(dest => dest.Hashtags, opt => opt.MapFrom((src, dest) =>
                src.Hashtags != null
                    ? (src.Hashtags.Any() ? JsonSerializer.Serialize(src.Hashtags, (JsonSerializerOptions?)null) : null)
                    : dest.Hashtags))
            .ForMember(dest => dest.PriorityScore, opt => opt.Condition(src => src.PriorityScore.HasValue))
            .ForMember(dest => dest.DisplayLocation, opt => opt.MapFrom((src, dest) =>
                src.DisplayLocation != null
                    ? (src.DisplayLocation.Any() ? JsonSerializer.Serialize(src.DisplayLocation, (JsonSerializerOptions?)null) : null)
                    : dest.DisplayLocation))
            .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom((src, dest) =>
                src.PriorityScore.HasValue ? src.PriorityScore.Value > 80 : dest.IsFeatured))
            .ForMember(dest => dest.MetaTitle, opt => opt.Condition(src => src.MetaTitle != null))
            .ForMember(dest => dest.MetaDescription, opt => opt.Condition(src => src.MetaDescription != null))
            .ForMember(dest => dest.MetaKeywords, opt => opt.MapFrom((src, dest) =>
                src.MetaKeywords != null
                    ? (src.MetaKeywords.Any() ? JsonSerializer.Serialize(src.MetaKeywords, (JsonSerializerOptions?)null) : null)
                    : dest.MetaKeywords))
            .ForMember(dest => dest.FacebookPost, opt => opt.MapFrom((src, dest) =>
                src.SocialPosts != null ? src.SocialPosts.Facebook : dest.FacebookPost))
            .ForMember(dest => dest.InstagramPost, opt => opt.MapFrom((src, dest) =>
                src.SocialPosts != null ? src.SocialPosts.Instagram : dest.InstagramPost))
            .ForMember(dest => dest.TwitterPost, opt => opt.MapFrom((src, dest) =>
                src.SocialPosts != null ? src.SocialPosts.Twitter : dest.TwitterPost))
            .ForMember(dest => dest.LinkedInPost, opt => opt.MapFrom((src, dest) =>
                src.SocialPosts != null ? src.SocialPosts.LinkedIn : dest.LinkedInPost))
            .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue))
            .ForMember(dest => dest.ScheduledDate, opt => opt.Condition(src => src.ScheduledDate.HasValue))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductName, opt => opt.Ignore())
            .ForMember(dest => dest.Views, opt => opt.Ignore())
            .ForMember(dest => dest.Clicks, opt => opt.Ignore())
            .ForMember(dest => dest.Shares, opt => opt.Ignore())
            .ForMember(dest => dest.PublishedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());
    }
}
