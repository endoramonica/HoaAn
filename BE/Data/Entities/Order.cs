namespace BE.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CreatedBy { get; set; }
        public string Status { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public Store Store { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}