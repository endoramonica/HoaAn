using System;
using VietCommerce.Core.Enums.Products;
using static VietCommerce.Core.Common.Constants.PricingConstants;
namespace VietCommerce.Core.Mappers
{
    public static class PriceTypeMapper
    {
        /// <summary>
        /// Chuy?n t? enum PriceType sang string (dùng cho DB / API / UI)
        /// </summary>
        public static string ToStringValue(this PriceType priceType)
        {
            return priceType switch
            {
                PriceType.REGULAR => PriceTypes.REGULAR,
                PriceType.SALE => PriceTypes.SALE,
                PriceType.WHOLESALE => PriceTypes.WHOLESALE,
                PriceType.MEMBER => PriceTypes.MEMBER,
                PriceType.FLASH_SALE => PriceTypes.FLASH_SALE,
                _ => throw new ArgumentOutOfRangeException(nameof(priceType), priceType, null)
            };
        }
        /// <summary>
        /// Chuy?n t? string (DB / API) sang enum PriceType
        /// </summary>
        public static PriceType ToEnum(string priceType)
        {
            return priceType?.ToUpperInvariant() switch
            {
                var s when s == PriceTypes.REGULAR => PriceType.REGULAR,
                var s when s == PriceTypes.SALE => PriceType.SALE,
                var s when s == PriceTypes.WHOLESALE => PriceType.WHOLESALE,
                var s when s == PriceTypes.MEMBER => PriceType.MEMBER,
                var s when s == PriceTypes.FLASH_SALE => PriceType.FLASH_SALE,
                _ => throw new ArgumentException($"Invalid price type: {priceType}")
            };
        }
    }
}
