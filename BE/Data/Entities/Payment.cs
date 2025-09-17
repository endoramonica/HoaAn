namespace BE.Data.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string Method { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Status { get; set; } = "";

        public Order Order { get; set; } = null!;
    }
}