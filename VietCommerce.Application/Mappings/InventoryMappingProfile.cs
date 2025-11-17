using AutoMapper;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Application.Mappings
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            // Inventory Entity <-> InventoryDto
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.QuantityActual,
                    opt => opt.MapFrom(src => src.QuantityAvailable - src.QuantityReserved))
                .ForMember(dest => dest.IsLowStock,
                    opt => opt.MapFrom(src => (src.QuantityAvailable - src.QuantityReserved) <= src.ReorderLevel));

            CreateMap<InventoryDto, Inventory>()
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryMovements, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                //.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
                //.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // InventoryMovement Entity <-> InventoryMovementDto
            CreateMap<InventoryMovement, InventoryMovementDto>()
                .ForMember(dest => dest.MovementType,
                    opt => opt.MapFrom(src => src.MovementType.ToString()))
                .ForMember(dest => dest.PerformedByName,
                    opt => opt.MapFrom(src => src.PerformedBy != null
                        ? src.PerformedBy.Name
                        : null));

            CreateMap<InventoryMovementDto, InventoryMovement>()
                .ForMember(dest => dest.MovementType,
                    opt => opt.MapFrom(src => Enum.Parse<Core.Enums.Products.InventoryMovementType>(src.MovementType)))
                .ForMember(dest => dest.Inventory, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Transfer, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedBy, opt => opt.Ignore())
                .ForMember(dest => dest.PerformedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // Inventory Entity -> CheckStockResponse
            CreateMap<Inventory, CheckStockResponse>()
                .ForMember(dest => dest.IsAvailable,
                    opt => opt.MapFrom((src, dest, _, context) =>
                    {
                        var requiredQty = context.Items.ContainsKey("RequiredQuantity")
                            ? (int)context.Items["RequiredQuantity"]
                            : 0;
                        return (src.QuantityAvailable - src.QuantityReserved) >= requiredQty;
                    }))
                .ForMember(dest => dest.QuantityActual,
                    opt => opt.MapFrom(src => src.QuantityAvailable - src.QuantityReserved))
                .ForMember(dest => dest.IsLowStock,
                    opt => opt.MapFrom(src => (src.QuantityAvailable - src.QuantityReserved) <= src.ReorderLevel))
                .ForMember(dest => dest.Message,
                    opt => opt.MapFrom((src, dest, _, context) =>
                    {
                        var requiredQty = context.Items.ContainsKey("RequiredQuantity")
                            ? (int)context.Items["RequiredQuantity"]
                            : 0;
                        var actualQty = src.QuantityAvailable - src.QuantityReserved;

                        if (actualQty >= requiredQty)
                            return "Đủ hàng";
                        else if (actualQty > 0)
                            return $"Chỉ còn {actualQty} sản phẩm";
                        else
                            return "Hết hàng";
                    }));
        }
    }
}