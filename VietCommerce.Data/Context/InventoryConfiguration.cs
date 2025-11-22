using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(i => new { i.StoreId, i.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_Inventories_Store_Product_Unique");

        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.IsDeleted);
        builder.HasIndex(i => i.QuantityAvailable);
        builder.HasIndex(i => i.ReorderLevel);

        builder.HasIndex(i => i.CreatedAt);

        // Store
        builder.HasOne(i => i.Store)
            .WithMany(s => s.Inventories)   // nếu Store có Inventories
            .HasForeignKey(i => i.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Inventories)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // tránh cascade nguy hiểm

        // Tenant
        // Tenant
        builder.HasOne(i => i.Tenant)
               .WithMany(t => t.Inventories) // <- sử dụng navigation collection
               .HasForeignKey(i => i.TenantId)
               .OnDelete(DeleteBehavior.Restrict);


        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
