namespace VietCommerce.Core.DTOs.Inventory
{
    public class AdjustInventoryRequest
    {
        public Guid ProductId { get; set; }
        public int AdjustmentAmount { get; set; } // Dương (tăng) hoặc âm (giảm)
        public string Reason { get; set; } = string.Empty;
        public Guid? OrderId { get; set; }
        public Guid? TransferId { get; set; }
    }
}
