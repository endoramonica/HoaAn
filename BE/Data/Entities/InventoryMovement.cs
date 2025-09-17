namespace BE.Data.Entities
{
    public class InventoryMovement
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public int ChangeAmount { get; set; }
        public MovementType MovementType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }

        public Inventory Inventory { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}