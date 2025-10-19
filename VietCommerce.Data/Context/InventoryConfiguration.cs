using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Context;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");
        
        // Primary Key
        builder.HasKey(i => i.Id);
        
        // RowVersion for Optimistic Concurrency
        builder.Property(i => i.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
        
        // Unique Constraint - 1 inventory per product per store
        builder.HasIndex(i => new { i.StoreId, i.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_Inventories_Store_Product_Unique");
        
        // Indexes
        builder.HasIndex(i => i.ProductId)
            .HasDatabaseName("IX_Inventories_ProductId");
            
        builder.HasIndex(i => i.IsDeleted)
            .HasDatabaseName("IX_Inventories_IsDeleted");
            
        // Index for low stock alerts
        builder.HasIndex(i => new { i.QuantityAvailable, i.ReorderLevel })
            .HasDatabaseName("IX_Inventories_LowStock");
        
        // 🆕 INDEX TRÊN CreatedAt (Issue 3 Fix)
        builder.HasIndex(i => i.CreatedAt)
            .HasDatabaseName("IX_Inventories_CreatedAt");
        
        // Relationships
        builder.HasOne(i => i.Store)
            .WithMany()
            .HasForeignKey(i => i.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Inventories)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // 🆕 GLOBAL QUERY FILTER - SOFT DELETE (Issue 2 Fix)
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

