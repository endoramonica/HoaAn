namespace BE.Data.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public Store Store { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
    }
}