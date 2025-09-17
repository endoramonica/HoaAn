namespace BE.Data.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = "";
        public string TargetType { get; set; } = "";
        public Guid TargetId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Meta { get; set; } = "";

        public User User { get; set; } = null!;
    }
}