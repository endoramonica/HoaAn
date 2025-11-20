using AutoMapper;
using VietCommerce.Core.DTOs.Logistics;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Application.Mappings
{
    public class StockTransferMappingProfile : Profile
    {
        public StockTransferMappingProfile()
        {
            // ===================================
            // StockTransfer Mappings
            // ===================================
            CreateMap<StockTransfer, StockTransferDto>()
                .ForMember(dest => dest.RequestedByName,
                    opt => opt.MapFrom(src => src.RequestedByUser != null ? src.RequestedByUser.Name : null))
                .ForMember(dest => dest.ApprovedByName,
                    opt => opt.MapFrom(src => src.ApprovedByUser != null ? src.ApprovedByUser.Name : null))
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
                .ForMember(dest => dest.TransferItems,
                    opt => opt.MapFrom(src => src.TransferItems));

            CreateMap<CreateStockTransferRequest, StockTransfer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StockTransferStatus.Pending))
                .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.TransferItems, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryMovements, opt => opt.Ignore());

            // ===================================
            // TransferItem Mappings
            // ===================================
            CreateMap<TransferItem, TransferItemDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ProductCode,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Code : null))
                .ForMember(dest => dest.ProductSKU,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.SKU : null));

            CreateMap<CreateTransferItemRequest, TransferItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.StockTransferId, opt => opt.Ignore())
                .ForMember(dest => dest.Received, opt => opt.Ignore())
                .ForMember(dest => dest.StockTransfer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            // ===================================
            // Reverse Mappings (if needed)
            // ===================================
            CreateMap<StockTransferDto, StockTransfer>()
                .ForMember(dest => dest.RequestedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryMovements, opt => opt.Ignore());

            CreateMap<TransferItemDto, TransferItem>()
                .ForMember(dest => dest.StockTransfer, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());
        }
    }
}