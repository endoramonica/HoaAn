namespace BE.Data.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int LoyaltyPoints { get; set; }

        public Store Store { get; set; } = null!;
        public User? User { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    }
}