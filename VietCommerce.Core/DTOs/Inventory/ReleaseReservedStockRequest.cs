namespace VietCommerce.Core.DTOs.Inventory
{
    public class ReleaseReservedStockRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
    }
}
