namespace VietCommerce.Core.Common.Constants
{
    public static class SystemConstants
    {
        public const int DEFAULT_PAGE_SIZE = 20;
        public const int MAX_PAGE_SIZE = 100;
        public const int MAX_SEARCH_RESULTS = 1000;
        public static class Cache
        {
            public const int DEFAULT_CACHE_DURATION_MINUTES = 30;
            public const int PRODUCT_CACHE_DURATION_MINUTES = 60;
            public const int INVENTORY_CACHE_DURATION_MINUTES = 5;
        }
        public static class Formats
        {
            public const string DATE_FORMAT = "yyyy-MM-dd";
            public const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
            public const string CURRENCY_FORMAT = "N2";
        }
    }
}
