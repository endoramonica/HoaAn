namespace VietCommerce.Core.DTOs.Inventory
{
    public class CheckStockResponse
    {
        public Guid ProductId { get; set; }
        public bool IsAvailable { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }
        public int QuantityActual { get; set; }
        public bool IsLowStock { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
