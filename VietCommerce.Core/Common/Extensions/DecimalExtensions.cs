using System;
namespace VietCommerce.Core.Common.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal ToSafeDecimal(this decimal? value, decimal defaultValue = 0)
        {
            return value ?? defaultValue;
        }
        public static decimal RoundTo(this decimal value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
        }
        public static bool IsBetween(this decimal value, decimal min, decimal max, bool inclusive = true)
        {
            return inclusive 
                ? value >= min && value <= max 
                : value > min && value < max;
        }
        public static string ToCurrencyString(this decimal value, string currencySymbol = "?")
        {
            return $"{value:N2} {currencySymbol}";
        }
    }
}
