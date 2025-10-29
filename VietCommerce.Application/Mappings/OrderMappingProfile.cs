// ================================================================
// FILE: VietCommerce.Application/Mappings/OrderMappingProfile.cs
// DESCRIPTION: AutoMapper profile for Order-related entities
// ================================================================

using AutoMapper;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Application.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // ============================================
            // ORDER ENTITY → DTOs
            // ============================================

            // Order → CheckoutResponseDto
            CreateMap<Order, CheckoutResponseDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src =>
                    src.Store != null ? src.Store.Name : null))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null ? src.Customer.Name : null))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.OrderShipping));

            // Order → OrderDetailDto
            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src =>
                    src.Store != null ? src.Store.Name : null))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null ? src.Customer.Name : null))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src =>
                    src.Customer != null ? src.Customer.Email : null))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src =>
                    src.Customer != null ? src.Customer.Phone : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.ShippingFee, opt => opt.MapFrom(src => src.ShippingFee))
                .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src =>
                    src.CreatedByUser != null ? src.CreatedByUser.Name : null))
                .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.OrderShipping))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.StatusHistories, opt => opt.MapFrom(src =>
                    src.OrderStatusHistories.OrderByDescending(h => h.ChangedAt)))
                // Payment fields - set manually if needed
                .ForMember(dest => dest.IsPaid, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
                .ForMember(dest => dest.PaidAt, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore());

            // Order → OrderListDTO
            CreateMap<Order, OrderListDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src =>
                    src.OrderItems != null ? src.OrderItems.Count : 0));

            // ============================================
            // ORDER ITEM ENTITY → DTOs
            // ============================================

            // OrderItem → OrderItemDTO
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
                    src.Product != null && src.Product.Images != null && src.Product.Images.Any()
                        ? src.Product.Images.FirstOrDefault()!.Url
                        : null))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

            // ============================================
            // ORDER SHIPPING ENTITY → DTOs
            // ============================================

            // OrderShipping → OrderShippingDto
            CreateMap<OrderShipping, OrderShippingDto>()
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.RecipientName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(src => src.ShippingMethod))
                .ForMember(dest => dest.ShippingFee, opt => opt.MapFrom(src => src.ShippingCost))
                .ForMember(dest => dest.ShippingStatus, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ShippedAt, opt => opt.MapFrom(src => src.ShippedAt))
                .ForMember(dest => dest.DeliveredAt, opt => opt.MapFrom(src => src.DeliveredAt))
                // Fields not in entity - set manually if needed
                .ForMember(dest => dest.TrackingNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Carrier, opt => opt.Ignore())
                .ForMember(dest => dest.EstimatedDelivery, opt => opt.Ignore());

            // OrderShippingInputDto → OrderShipping
            CreateMap<OrderShippingInputDto, OrderShipping>()
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.RecipientName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Ward))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode ?? string.Empty))
                .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(src => src.ShippingMethod.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ShippingStatus.PREPARING))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                // Ignore auto-generated fields
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingCost, opt => opt.Ignore())
                .ForMember(dest => dest.ShippedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveredAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore());

            // ============================================
            // ORDER STATUS HISTORY ENTITY → DTOs
            // ============================================

            // OrderStatusHistory → OrderStatusHistoryDTO
            CreateMap<OrderStatusHistory, OrderStatusHistoryDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NewStatus))
                .ForMember(dest => dest.ChangedAt, opt => opt.MapFrom(src => src.ChangedAt))
                .ForMember(dest => dest.ChangedByName, opt => opt.MapFrom(src =>
                    src.ChangedByUser != null ? src.ChangedByUser.Name : null))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            // ============================================
            // DTOs → ENTITIES (for creation/updates)
            // ============================================

            // OrderCreateDTO → Order
            CreateMap<OrderCreateDTO, Order>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                // Ignore auto-generated/calculated fields
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingFee, opt => opt.Ignore())
                .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Notes, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                // Ignore navigation properties
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.Payments, opt => opt.Ignore())
                .ForMember(dest => dest.OrderStatusHistories, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryMovements, opt => opt.Ignore())
                .ForMember(dest => dest.OrderShipping, opt => opt.Ignore());

            // OrderItemCreateDTO → OrderItem
            CreateMap<OrderItemCreateDTO, OrderItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                // Ignore all other fields - set in service
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.ProductName, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCode, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());
        }
    }
}