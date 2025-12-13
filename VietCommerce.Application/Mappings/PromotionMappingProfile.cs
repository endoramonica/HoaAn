using AutoMapper;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;

namespace VietCommerce.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Promotion entity and DTOs
    /// </summary>
    public class PromotionMappingProfile : Profile
    {
        public PromotionMappingProfile()
        {
            // Promotion -> PromotionDto
            CreateMap<Promotion, PromotionDto>();

            // CreatePromotionDto -> Promotion
            CreateMap<CreatePromotionDto, Promotion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UsedCount, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Campaign, opt => opt.Ignore())
                .ForMember(dest => dest.PromotionProducts, opt => opt.Ignore())
                .ForMember(dest => dest.Vouchers, opt => opt.Ignore());

            // UpdatePromotionDto -> Promotion
            CreateMap<UpdatePromotionDto, Promotion>()
                .ForMember(dest => dest.PromotionName, opt => opt.Condition(src => src.PromotionName != null))
                .ForMember(dest => dest.PromotionType, opt => opt.Condition(src => src.PromotionType.HasValue))
                .ForMember(dest => dest.DiscountValue, opt => opt.Condition(src => src.DiscountValue.HasValue))
                .ForMember(dest => dest.MinOrderAmount, opt => opt.Condition(src => src.MinOrderAmount.HasValue))
                .ForMember(dest => dest.MaxDiscount, opt => opt.Condition(src => src.MaxDiscount.HasValue))
                .ForMember(dest => dest.UsageLimit, opt => opt.Condition(src => src.UsageLimit.HasValue))
                .ForMember(dest => dest.Conditions, opt => opt.Condition(src => src.Conditions != null))
                .ForMember(dest => dest.StartDate, opt => opt.Condition(src => src.StartDate.HasValue))
                .ForMember(dest => dest.EndDate, opt => opt.Condition(src => src.EndDate.HasValue))
                .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UsedCount, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Campaign, opt => opt.Ignore())
                .ForMember(dest => dest.PromotionProducts, opt => opt.Ignore())
                .ForMember(dest => dest.Vouchers, opt => opt.Ignore());
        }
    }
}
