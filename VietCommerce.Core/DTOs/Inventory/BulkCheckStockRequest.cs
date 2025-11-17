namespace VietCommerce.Core.DTOs.Inventory
{
    public class BulkCheckStockRequest
    {
        public List<Guid> ProductIds { get; set; } = new();
    }
}
