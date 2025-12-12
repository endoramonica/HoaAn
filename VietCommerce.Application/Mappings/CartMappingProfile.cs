// VietCommerce.Application/Mappings/CartMappingProfile.cs
using AutoMapper;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Helpers;

namespace VietCommerce.Application.Mappings
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            // Cart -> CartDto
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
                .ReverseMap();

            // CartItem -> CartItemDto
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product.Code))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.GetActivePrice()))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.GetMainImageUrl()))
                .ForMember(dest => dest.StockAvailable, opt => opt.MapFrom(src => src.Product.Stock))
                .ReverseMap();
            // CartItem to CartItemDetailDto
            CreateMap<CartItem, CartItemDetailDto>()
                .ForMember(dest => dest.CartItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product.Slug))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.Product.SKU))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.GetMainImageUrl()))
                .ForMember(dest => dest.UnitPrice,
                    opt =>
                    {
                        opt.PreCondition(src => src.Product?.Prices.Any() == true);
                        opt.MapFrom(src => src.Product!.Prices.First().Price);
                    })
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Product.Prices.FirstOrDefault().Price))
                .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src => src.Product.Stock))
                .ForMember(dest => dest.IsProductActive, opt => opt.MapFrom(src => src.Product.IsActive))
                .ForMember(dest => dest.PurchaseCount, opt => opt.MapFrom(src => src.Product.PurchaseCount))
                .ForMember(dest => dest.AvgRating, opt => opt.MapFrom(src => src.Product.AvgRating))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Product.ReviewCount))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.CustomizationPrice, opt => opt.MapFrom(src => src.CustomizationPrice))
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.FinalPrice))
                .ForMember(dest => dest.Customizations, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.CustomizationsJson)
                        ? null
                        : JsonSerializationHelper.DeserializeCustomizations(src.CustomizationsJson)))
                .ReverseMap();

        }
    }
}