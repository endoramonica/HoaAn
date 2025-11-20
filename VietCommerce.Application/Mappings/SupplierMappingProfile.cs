using AutoMapper;
using VietCommerce.Core.DTOs.Suppliers;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Application.Mappings;

/// <summary>
/// AutoMapper profile for Supplier entity mappings
/// </summary>
public class SupplierMappingProfile : Profile
{
    public SupplierMappingProfile()
    {
        // ========================================
        // Supplier Entity -> SupplierDto
        // ========================================
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.StatusDisplay,
                opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ProductCount,
                opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
            .ForMember(dest => dest.StockTransferCount,
                opt => opt.MapFrom(src => src.StockTransfers != null ? src.StockTransfers.Count : 0));

        // ========================================
        // CreateSupplierRequest -> Supplier Entity
        // ========================================
        CreateMap<CreateSupplierRequest, Supplier>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Auto-generated
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Auto-generated
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Auto-generated
            .ForMember(dest => dest.Products, opt => opt.Ignore()) // Navigation
            .ForMember(dest => dest.StockTransfers, opt => opt.Ignore()); // Navigation

        // ========================================
        // UpdateSupplierRequest -> Supplier Entity
        // ========================================
        CreateMap<UpdateSupplierRequest, Supplier>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        // Only map non-null properties (partial update support)
    }
}