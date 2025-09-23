namespace VietCommerce.Core.Common.Constants
{
    public static class CampaignConstants
    {
        public const int MAX_CAMPAIGN_NAME_LENGTH = 255;
        public const int MAX_DESCRIPTION_LENGTH = 1000;
        public const int MAX_PROMOTIONS_PER_CAMPAIGN = 50;
        public const decimal MAX_DISCOUNT_PERCENTAGE = 100;
        public const decimal MAX_BUDGET = 999999999.99m;
        
        public static class DefaultDurations
        {
            public const int FLASH_SALE_HOURS = 24;
            public const int WEEKLY_PROMOTION_DAYS = 7;
            public const int MONTHLY_CAMPAIGN_DAYS = 30;
            public const int SEASONAL_CAMPAIGN_DAYS = 90;
        }
    }
}
