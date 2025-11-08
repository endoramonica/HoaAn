using AutoMapper;
using System.Linq;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Helpers;

namespace VietCommerce.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // ===================================
            // CREATE MAPPING
            // ===================================
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.Sku ?? string.Empty))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.StoreId, opt => opt.Ignore())
                .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteCount, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseCount, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore())
                .ForMember(dest => dest.AvgRating, opt => opt.Ignore())
                .ForMember(dest => dest.TrendingScore, opt => opt.Ignore())
                .ForMember(dest => dest.StatsUpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.PromotionProducts, opt => opt.Ignore())
                .ForMember(dest => dest.Prices, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Inventories, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.CartItems, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryMovements, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.Views, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.TransferItems, opt => opt.Ignore())
                .ForMember(dest => dest.Suppliers, opt => opt.Ignore());

            // ===================================
            // UPDATE MAPPING
            // ===================================
            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ===================================
            // ENTITY → DETAIL DTO
            // ===================================
            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.ShortDescription, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.SKU))
                .ForMember(dest => dest.Barcode, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.BrandId, opt => opt.Ignore())
                .ForMember(dest => dest.BrandName, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images != null && src.Images.Any() ? src.Images.Where(i => !string.IsNullOrEmpty(i.Url)).Select(i => i.Url).ToList() : new List<string>()))
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.MetaTitle, opt => opt.Ignore())
                .ForMember(dest => dest.MetaDescription, opt => opt.Ignore())
                .ForMember(dest => dest.MetaKeywords, opt => opt.Ignore())
                .ForMember(dest => dest.IsFeatured, opt => opt.Ignore())
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => (int)src.ViewCount))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.FavoriteCount))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AvgRating))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.ReviewCount))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : null))

                // Mapping DisplayPrice từ PriceCalculationHelper
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).DiscountedPrice))
                .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).OriginalPrice))
                .ForMember(dest => dest.DisplayPrice, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).DiscountedPrice))
                .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).OriginalPrice));


            // ===================================
            // ENTITY → LIST DTO
            // ===================================
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.ShortDescription, opt => opt.Ignore())
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src => src.Images != null && src.Images.Any() ? src.Images.FirstOrDefault()!.Url : null))
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => (int)src.ViewCount))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.FavoriteCount))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AvgRating))
                .ForMember(dest => dest.IsFeatured, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).DiscountedPrice))
                .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).OriginalPrice))
            // AutoMapper mapping
            .ForMember(dest => dest.DisplayPrice, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src)))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).DiscountedPrice))
            .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => PriceCalculationHelper.GetDisplayPrice(src).OriginalPrice));


            // ===================================
            // PRICE, IMAGE, REVIEW MAPPINGS (unchanged)
            // ===================================
            CreateMap<ProductPriceCreateDto, ProductPrice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Creator, opt => opt.Ignore());

            CreateMap<ProductPrice, ProductPriceDto>()
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Name : null));

            CreateMap<ProductImageCreateDto, ProductImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<ProductImage, ProductImageDto>()
                .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsPrimary, opt => opt.Ignore());

            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : "Unknown"))
                .ForMember(dest => dest.UserAvatar, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerifiedPurchase, opt => opt.MapFrom(src => src.OrderItem != null))
                .ForMember(dest => dest.MediaUrls, opt => opt.MapFrom(src => src.MediaUrls));
        }
    }
}
