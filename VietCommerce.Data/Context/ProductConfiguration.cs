using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Context;

/// <summary>
/// Configuration cho Product entity
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        // Primary Key
        builder.HasKey(p => p.Id);
        
        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);
            
        // Stats columns - với decimal precision
        builder.Property(p => p.AvgRating)
            .HasColumnType("decimal(3,2)");
            
        builder.Property(p => p.TrendingScore)
            .HasColumnType("decimal(18,2)");
        
        // Indexes - Performance Critical
        builder.HasIndex(p => p.StoreId)
            .HasDatabaseName("IX_Products_StoreId");
            
        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");
            
        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasDatabaseName("IX_Products_SKU_Unique");
            
        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Products_IsActive");
            
        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Products_IsDeleted");
            
        // Composite index cho search & filter
        builder.HasIndex(p => new { p.StoreId, p.IsActive, p.IsDeleted })
            .HasDatabaseName("IX_Products_Store_Active_Deleted");
            
        // Index cho trending/sorting
        builder.HasIndex(p => p.TrendingScore)
            .HasDatabaseName("IX_Products_TrendingScore");
            
        builder.HasIndex(p => p.ViewCount)
            .HasDatabaseName("IX_Products_ViewCount");
            
        builder.HasIndex(p => p.PurchaseCount)
            .HasDatabaseName("IX_Products_PurchaseCount");
            
        builder.HasIndex(p => p.AvgRating)
            .HasDatabaseName("IX_Products_AvgRating");
        
        // Relationships
        builder.HasOne(p => p.Store)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Query Filter - Global Soft Delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
