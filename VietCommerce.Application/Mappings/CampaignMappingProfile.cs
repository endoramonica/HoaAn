using AutoMapper;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;

namespace VietCommerce.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Campaign entity and DTOs
    /// </summary>
    public class CampaignMappingProfile : Profile
    {
        public CampaignMappingProfile()
        {
            // Campaign -> CampaignDto
            CreateMap<Campaign, CampaignDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IsValidDateRange, opt => opt.MapFrom(src => src.IsValidDateRange))
                .ForMember(dest => dest.RemainingBudget, opt => opt.MapFrom(src => src.RemainingBudget))
                .ForMember(dest => dest.BudgetUtilizationPercentage, opt => opt.MapFrom(src => src.BudgetUtilizationPercentage));

            // CreateCampaignDto -> Campaign
            CreateMap<CreateCampaignDto, Campaign>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ActualCost, opt => opt.Ignore())
                .ForMember(dest => dest.Promotions, opt => opt.Ignore())
                .ForMember(dest => dest.Impressions, opt => opt.Ignore())
                .ForMember(dest => dest.Clicks, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore());

            // UpdateCampaignDto -> Campaign
            CreateMap<UpdateCampaignDto, Campaign>()
                .ForMember(dest => dest.CampaignName, opt => opt.Condition(src => src.CampaignName != null))
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.StartDate, opt => opt.Condition(src => src.StartDate.HasValue))
                .ForMember(dest => dest.EndDate, opt => opt.Condition(src => src.EndDate.HasValue))
                .ForMember(dest => dest.Budget, opt => opt.Condition(src => src.Budget.HasValue))
                .ForMember(dest => dest.TargetingRules, opt => opt.Condition(src => src.TargetingRules != null))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StoreId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ActualCost, opt => opt.Ignore())
                .ForMember(dest => dest.CampaignType, opt => opt.Ignore())
                .ForMember(dest => dest.Promotions, opt => opt.Ignore())
                .ForMember(dest => dest.Impressions, opt => opt.Ignore())
                .ForMember(dest => dest.Clicks, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore());
        }
    }
}
