namespace BE.Data.Entities
{
    public class UserAddress
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid StoreId { get; set; }
        public string Type { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string District { get; set; } = "";
        public string Ward { get; set; } = "";
        public string Phone { get; set; } = "";
        public bool IsDefault { get; set; }

        public User User { get; set; } = null!;
        public Customer? Customer { get; set; } 
        public Store Store { get; set; } = null!;
    }
}