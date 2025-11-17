// ============================================
// FILE: ProductFavoriteMappingProfile.cs
// Path: VietCommerce.Application.Mappings/ProductFavoriteMappingProfile.cs
// Description: AutoMapper profile for ProductFavorite (Wishlist) entities and DTOs
// ============================================

using AutoMapper;
using System.Linq;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.DTOs.Wishlist;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Helpers;

namespace VietCommerce.Application.Mappings
{
    public class ProductFavoriteMappingProfile : Profile
    {
        public ProductFavoriteMappingProfile()
        {
            // ===================================
            // WISHLIST ITEM MAPPINGS
            // ===================================

            // ProductFavorite (Entity) → WishlistItemDto
            CreateMap<ProductFavorite, WishlistItemDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

            // Product (Entity) → ProductInWishlistDto
            CreateMap<Product, ProductInWishlistDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    PriceCalculationHelper.GetDisplayPrice(src).DiscountedPrice))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any()
                        ? src.Images
                            .Where(i => !string.IsNullOrEmpty(i.Url))
                            .OrderBy(i => i.DisplayOrder)
                            .FirstOrDefault()!.Url
                        : null))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.FavoriteCount));

            // ===================================
            // REVERSE MAPPINGS (if needed)
            // ===================================

            // WishlistItemDto → ProductFavorite (for updates)
            CreateMap<WishlistItemDto, ProductFavorite>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
                //.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
        }
    }
}