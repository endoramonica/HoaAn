namespace VietCommerce.Core.DTOs.Inventory
{
    public class InventoryMovementDto
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? TransferId { get; set; }
        public int ChangeAmount { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public int? QuantityBefore { get; set; }
        public int? QuantityAfter { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PerformedByName { get; set; }
    }
}
