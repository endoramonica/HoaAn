namespace VietCommerce.Core.Common.Constants
{
    public static class PricingConstants
    {
        public const decimal MIN_PRICE = 0.01m;
        public const decimal MAX_PRICE = 999999999.99m;
        public const int PRICE_DECIMAL_PLACES = 2;
        public const decimal MAX_DISCOUNT_PERCENTAGE = 99.99m;
        public static class PriceTypes
        {
            public const string REGULAR = "REGULAR";
            public const string SALE = "SALE";
            public const string WHOLESALE = "WHOLESALE";
            public const string MEMBER = "MEMBER";
            public const string FLASH_SALE = "FLASH_SALE";
        }
    }
}
