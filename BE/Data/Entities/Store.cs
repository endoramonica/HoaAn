namespace BE.Data.Entities
{
    public class Store
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public bool IsActive { get; set; }

        public Tenant Tenant { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}