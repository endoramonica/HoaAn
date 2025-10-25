using AutoMapper;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;

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
                // Ignore các field tự động
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())

                // Map các field cơ bản
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.Sku ?? string.Empty))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.StockQuantity))

                // Ignore các field không có trong CreateDto
                .ForMember(dest => dest.StoreId, opt => opt.Ignore()) // Set riêng trong service
                .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
                .ForMember(dest => dest.FavoriteCount, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseCount, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewCount, opt => opt.Ignore())
                .ForMember(dest => dest.AvgRating, opt => opt.Ignore())
                .ForMember(dest => dest.TrendingScore, opt => opt.Ignore())
                .ForMember(dest => dest.StatsUpdatedAt, opt => opt.Ignore())

                // Ignore navigation properties
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
                // Basic info
                .ForMember(dest => dest.ShortDescription, opt => opt.Ignore()) // Chưa có trong Entity
                .ForMember(dest => dest.Description, opt => opt.Ignore()) // Chưa có trong Entity

                // Pricing - lấy từ ProductPrices
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    GetCurrentPrice(src, PriceType.REGULAR)))
                .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src =>
                    GetCurrentPrice(src, PriceType.SALE)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src =>
                    GetCurrentPrice(src, PriceType.COST)))

                // Inventory
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.SKU))
                .ForMember(dest => dest.Barcode, opt => opt.Ignore()) // Chưa có trong Entity

                // Category & Brand
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                    src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.BrandId, opt => opt.Ignore())
                .ForMember(dest => dest.BrandName, opt => opt.Ignore())

                // Images
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any()
                        ? src.Images.Where(i => !string.IsNullOrEmpty(i.Url))
                            .Select(i => i.Url)
                            .ToList()
                        : new List<string>()))

                // Tags & SEO - chưa có trong Entity
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.MetaTitle, opt => opt.Ignore())
                .ForMember(dest => dest.MetaDescription, opt => opt.Ignore())
                .ForMember(dest => dest.MetaKeywords, opt => opt.Ignore())
                .ForMember(dest => dest.IsFeatured, opt => opt.Ignore())

                // Statistics
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => (int)src.ViewCount))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.FavoriteCount))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AvgRating))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.ReviewCount))

                // Store
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src =>
                    src.Store != null ? src.Store.Name : null))

                // Audit
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src =>
                    src.CreatedByUser != null ? src.CreatedByUser.Name : null));

            // ===================================
            // ENTITY → LIST DTO
            // ===================================
            CreateMap<Product, ProductListDto>()
                // Basic info
                .ForMember(dest => dest.ShortDescription, opt => opt.Ignore())

                // Pricing
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    GetCurrentPrice(src, PriceType.REGULAR)))
                .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src =>
                    GetCurrentPrice(src, PriceType.SALE)))

                // Inventory
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Stock))

                // Category
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                    src.Category != null ? src.Category.Name : null))

                // Images
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src =>
                    src.Images != null && src.Images.Any()
                        ? src.Images.FirstOrDefault()!.Url
                        : null))

                // Status - chưa có IsFeatured trong Entity
                .ForMember(dest => dest.IsFeatured, opt => opt.Ignore())

                // Statistics
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => (int)src.ViewCount))
                .ForMember(dest => dest.FavoriteCount, opt => opt.MapFrom(src => src.FavoriteCount))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.AvgRating));

            // ===================================
            // PRICE MAPPINGS
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
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src =>
                    src.Creator != null ? src.Creator.Name : null));

            // ===================================
            // IMAGE MAPPINGS
            // ===================================
            CreateMap<ProductImageCreateDto, ProductImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<ProductImage, ProductImageDto>()
                .ForMember(dest => dest.DisplayOrder, opt => opt.Ignore())
                .ForMember(dest => dest.IsPrimary, opt => opt.Ignore());

            // ===================================
            // REVIEW MAPPINGS
            // ===================================
            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.Name : "Unknown"))
                .ForMember(dest => dest.UserAvatar, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerifiedPurchase, opt => opt.MapFrom(src =>
                    src.OrderItem != null))
                .ForMember(dest => dest.MediaUrls, opt => opt.MapFrom(src => src.MediaUrls));
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Lấy giá hiện tại theo loại giá
        /// </summary>
        private static decimal GetCurrentPrice(Product product, PriceType priceType)
        {
            if (product.Prices == null || !product.Prices.Any())
                return 0;

            var now = DateTime.UtcNow;
            var currentPrice = product.Prices
                .Where(p => p.PriceType == priceType
                    && p.IsActive
                    && p.EffectiveFrom <= now
                    && (p.EffectiveTo == null || p.EffectiveTo > now))
                .OrderByDescending(p => p.EffectiveFrom)
                .Select(p => p.Price)
                .FirstOrDefault();

            return currentPrice;
        }
    }
}