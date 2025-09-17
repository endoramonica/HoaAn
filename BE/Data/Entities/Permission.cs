namespace BE.Data.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}