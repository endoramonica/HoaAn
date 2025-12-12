using System;
using System.Collections.Generic;
using System.Linq;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.DTOs.Cart;

namespace VietCommerce.Core.Helpers
{
    public static class PriceCalculationHelper
    {
        /// <summary>
        /// Lấy giá hiện tại theo loại giá
        /// </summary>
        public static decimal GetCurrentPrice(Product product, PriceType priceType = PriceType.REGULAR)
        {
            var currentTime = DateTime.UtcNow;
            var currentPrice = product.Prices?
                .Where(p => p.PriceType == priceType &&
                            p.IsActive &&
                            p.EffectiveFrom <= currentTime &&
                            (p.EffectiveTo == null || p.EffectiveTo > currentTime))
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefault();

            // ✅ FIX: Return 0 only if no prices exist, otherwise return the price
            // This ensures consistency between list and detail endpoints
            return currentPrice?.Price ?? 0m;
        }

        /// <summary>
        /// Tính số tiền giảm dựa trên Promotion
        /// </summary>
        public static decimal CalculateDiscountAmount(decimal originalPrice, Promotion promotion)
        {
            return promotion.PromotionType switch
            {
                PromotionType.PERCENTAGE => originalPrice * (promotion.DiscountValue / 100),
                PromotionType.FIXED_AMOUNT => Math.Min(promotion.DiscountValue, originalPrice),
                _ => 0
            };
        }

        /// <summary>
        /// Áp dụng MaxDiscount nếu có
        /// </summary>
        public static decimal ApplyMaxDiscount(decimal discountAmount, Promotion promotion)
        {
            if (promotion.MaxDiscount.HasValue)
            {
                return Math.Min(discountAmount, promotion.MaxDiscount.Value);
            }
            return discountAmount;
        }

        /// <summary>
        /// Lấy giá hiển thị (DisplayPrice) đã áp dụng khuyến mãi
        /// </summary>
        public static DisplayPriceResult GetDisplayPrice(Product product)
        {
            var originalPrice = GetCurrentPrice(product, PriceType.REGULAR);
            var discountedPrice = originalPrice;
            decimal discountAmount = 0;
            string? promotionName = null;

            var now = DateTime.UtcNow;

            if (product.PromotionProducts != null && product.PromotionProducts.Any())
            {
                var activePromotions = product.PromotionProducts
                    .Where(pp => pp.Promotion.IsActive &&
                                 pp.Promotion.StartDate <= now &&
                                 pp.Promotion.EndDate > now)
                    .Select(pp => pp.Promotion)
                    .ToList();

                if (activePromotions.Any())
                {
                    // Giả sử chỉ áp dụng 1 promotion ưu tiên (promotion có DiscountValue cao nhất)
                    var promotion = activePromotions.OrderByDescending(p => p.DiscountValue).First();

                    discountAmount = CalculateDiscountAmount(originalPrice, promotion);
                    discountAmount = ApplyMaxDiscount(discountAmount, promotion);
                    discountedPrice = originalPrice - discountAmount;
                    promotionName = promotion.PromotionName;
                }
            }

            return new DisplayPriceResult
            {
                OriginalPrice = originalPrice,
                DiscountedPrice = discountedPrice,
                DiscountAmount = discountAmount,
                PromotionName = promotionName
            };
        }

        /// <summary>
        /// Calculates the final price for a package product with customizations.
        /// Formula: basePrice + sum(customization quantities × unit prices)
        /// </summary>
        /// <param name="basePrice">The base price of the package product</param>
        /// <param name="customizations">List of customizations selected by the customer, or null if no customizations</param>
        /// <returns>The final price including customization surcharges</returns>
        /// <remarks>
        /// If customizations is null or empty, returns the base price.
        /// Each customization's contribution is calculated as: Quantity × UnitPrice
        /// </remarks>
        public static decimal CalculateFinalPrice(decimal basePrice, List<CartItemCustomizationDto>? customizations)
        {
            if (customizations == null || customizations.Count == 0)
            {
                return basePrice;
            }

            decimal customizationPrice = customizations.Sum(c => c.Quantity * c.UnitPrice);
            return basePrice + customizationPrice;
        }
    }


}
