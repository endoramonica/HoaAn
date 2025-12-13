using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Application.Helpers
{
    /// <summary>
    /// Helper class to build TaggedProductDto from Product entity.
    /// Handles live product data retrieval including pricing, discounts, and images.
    /// </summary>
    public static class TaggedProductHelper
    {
        /// <summary>
        /// Builds a TaggedProductDto from a Product entity.
        /// Extracts current pricing, discount information, and thumbnail image.
        /// </summary>
        /// <param name="product">The Product entity to convert</param>
        /// <returns>TaggedProductDto with live product data, or null if product is null</returns>
        public static TaggedProductDto? BuildTaggedProductDto(Product? product)
        {
            // Handle null product case
            if (product == null)
            {
                return null;
            }

            // Get the active regular price
            var regularPrice = product.Prices?
                .Where(p => p.PriceType == PriceType.REGULAR && p.IsActive)
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefault();

            // Get the active sale price if available
            var salePrice = product.Prices?
                .Where(p => p.PriceType == PriceType.SALE && p.IsActive)
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefault();

            // Determine current price and discount
            decimal currentPrice = regularPrice?.Price ?? 0;
            decimal originalPrice = regularPrice?.Price ?? 0;
            bool hasDiscount = false;
            int discountPercentage = 0;

            if (salePrice != null && salePrice.Price < currentPrice)
            {
                hasDiscount = true;
                currentPrice = salePrice.Price;
                discountPercentage = CalculateDiscountPercentage(originalPrice, currentPrice);
            }

            // Get the main product image (thumbnail)
            var mainImage = product.Images?
                .Where(i => i.IsMain)
                .OrderBy(i => i.DisplayOrder)
                .FirstOrDefault();

            var thumbnailUrl = mainImage?.ThumbnailUrl ?? mainImage?.Url ?? string.Empty;

            // Format the price
            var formattedPrice = FormatPrice(currentPrice);

            return new TaggedProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = currentPrice,
                Currency = "VND",
                FormattedPrice = formattedPrice,
                ThumbnailUrl = thumbnailUrl,
                HasDiscount = hasDiscount,
                DiscountPercentage = discountPercentage
            };
        }

        /// <summary>
        /// Calculates discount percentage between original and current price.
        /// </summary>
        /// <param name="originalPrice">The original price</param>
        /// <param name="currentPrice">The current (discounted) price</param>
        /// <returns>Discount percentage (0-100)</returns>
        private static int CalculateDiscountPercentage(decimal originalPrice, decimal currentPrice)
        {
            if (originalPrice <= 0)
            {
                return 0;
            }

            var percentage = ((originalPrice - currentPrice) / originalPrice) * 100;
            return (int)Math.Round(percentage);
        }

        /// <summary>
        /// Formats a price value as a string with currency symbol.
        /// </summary>
        /// <param name="price">The price to format</param>
        /// <returns>Formatted price string (e.g., "45,000 VND")</returns>
        private static string FormatPrice(decimal price)
        {
            return $"{price:N0} VND";
        }
    }
}
