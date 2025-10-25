// VietCommerce.Application/Mappings/CartMappingProfile.cs
using AutoMapper;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.Entities.Orders;

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
        }
    }
}