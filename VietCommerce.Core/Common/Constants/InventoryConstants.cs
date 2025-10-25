namespace VietCommerce.Core.Common.Constants
{
    public static class InventoryConstants
    {
        public const int DEFAULT_REORDER_LEVEL = 10;
        public const int MAX_QUANTITY_PER_TRANSACTION = 9999;
        public const int LOW_STOCK_THRESHOLD = 5;
        public static class MovementTypes
        {
            public const string PURCHASE = "PURCHASE";
            public const string SALE = "SALE";
            public const string ADJUSTMENT = "ADJUSTMENT";
            public const string RETURN = "RETURN";
            public const string TRANSFER = "TRANSFER";
        }
    }
}
