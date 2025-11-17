namespace VietCommerce.Core.DTOs.Inventory
{
    public class BulkCheckStockResponse
    {
        public List<CheckStockResponse> Results { get; set; } = new();
        public int TotalLowStockItems { get; set; }
    }
}
