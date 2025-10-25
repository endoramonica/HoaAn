using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Context;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        // 🔹 Primary Key
        builder.HasKey(i => i.Id);

        // 🔹 RowVersion for Optimistic Concurrency
        builder.Property(i => i.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // 🔹 Unique Constraint - 1 inventory per product per store
        builder.HasIndex(i => new { i.StoreId, i.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_Inventories_Store_Product_Unique");

        // 🔹 Indexes
        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("IX_Inventories_ProductId");

        builder.HasIndex(i => i.IsDeleted)
            .HasDatabaseName("IX_Inventories_IsDeleted");

        // 🔹 Index for low stock alerts
        builder.HasIndex(i => new { i.QuantityAvailable, i.ReorderLevel })
            .HasDatabaseName("IX_Inventories_LowStock");

        // 🔹 Index on CreatedAt (performance optimization)
        builder.HasIndex(i => i.CreatedAt)
            .HasDatabaseName("IX_Inventories_CreatedAt");

        // ===========================================================
        // 🔗 RELATIONSHIPS
        // ===========================================================

        // Store
        builder.HasOne(i => i.Store)
            .WithMany()
            .HasForeignKey(i => i.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Inventories)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // 🆕 Tenant (Optional relationship)
        builder.HasOne(i => i.Tenant)
            .WithMany()
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===========================================================
        // 🔎 GLOBAL QUERY FILTER (Soft Delete)
        // ===========================================================
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
