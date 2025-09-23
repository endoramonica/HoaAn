using System;
using System.Linq;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.Helpers
{
    public static class PriceCalculationHelper
    {
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
                
            return currentPrice?.Price ?? 0;
        }
        
        public static decimal CalculateDiscountAmount(decimal originalPrice, Promotion promotion)
        {
            return promotion.PromotionType switch
            {
                PromotionType.PERCENTAGE => originalPrice * (promotion.DiscountValue / 100),
                PromotionType.FIXED_AMOUNT => Math.Min(promotion.DiscountValue, originalPrice),
                _ => 0
            };
        }
        
        public static decimal ApplyMaxDiscount(decimal discountAmount, Promotion promotion)
        {
            if (promotion.MaxDiscount.HasValue)
            {
                return Math.Min(discountAmount, promotion.MaxDiscount.Value);
            }
            return discountAmount;
        }
    }
}
