namespace BE.Data.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Domain { get; set; } = "";
        public string Status { get; set; } = "";
        public string Plan { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public ICollection<Store> Stores { get; set; } = new List<Store>();
    }
}