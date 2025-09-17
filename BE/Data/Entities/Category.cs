namespace BE.Data.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; } = "";
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }

        public Store Store { get; set; } = null!;

        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}