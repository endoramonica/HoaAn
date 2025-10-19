using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Context;

/// <summary>
/// Configuration cho ProductView entity
/// </summary>
public class ProductViewConfiguration : IEntityTypeConfiguration<ProductView>
{
    public void Configure(EntityTypeBuilder<ProductView> builder)
    {
        builder.ToTable("ProductViews");
        
        // Primary Key
        builder.HasKey(pv => pv.Id);
        
        // Properties
        builder.Property(pv => pv.SessionId)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(pv => pv.IpAddress)
            .HasMaxLength(45); // IPv6 max length
            
        builder.Property(pv => pv.UserAgent)
            .HasMaxLength(500);
        
        // Indexes - cho analytics queries
        builder.HasIndex(pv => pv.ProductId)
            .HasDatabaseName("IX_ProductViews_ProductId");
            
        builder.HasIndex(pv => pv.UserId)
            .HasDatabaseName("IX_ProductViews_UserId");
            
        builder.HasIndex(pv => pv.SessionId)
            .HasDatabaseName("IX_ProductViews_SessionId");
            
        builder.HasIndex(pv => pv.ViewedAt)
            .HasDatabaseName("IX_ProductViews_ViewedAt");
            
        // Composite index cho time-based analytics
        builder.HasIndex(pv => new { pv.ProductId, pv.ViewedAt })
            .HasDatabaseName("IX_ProductViews_Product_ViewedAt");
        
        // Relationships
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Views)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pv => pv.User)
            .WithMany()
            .HasForeignKey(pv => pv.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
