namespace VietCommerce.Core.DTOs.Inventory
{
    public class CheckStockRequest
    {
        public Guid ProductId { get; set; }
        public int RequiredQuantity { get; set; }
    }
}
