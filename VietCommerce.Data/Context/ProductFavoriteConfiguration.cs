using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Context;

/// <summary>
/// Configuration cho ProductFavorite entity
/// </summary>
public class ProductFavoriteConfiguration : IEntityTypeConfiguration<ProductFavorite>
{
    public void Configure(EntityTypeBuilder<ProductFavorite> builder)
    {
        builder.ToTable("ProductFavorites");
        
        // Primary Key
        builder.HasKey(pf => pf.Id);
        
        // Composite Unique Index - 1 user chỉ favorite 1 product 1 lần
        builder.HasIndex(pf => new { pf.UserId, pf.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_ProductFavorites_User_Product_Unique");
        
        // Index riêng cho queries
        builder.HasIndex(pf => pf.UserId)
            .HasDatabaseName("IX_ProductFavorites_UserId");
            
        builder.HasIndex(pf => pf.ProductId)
            .HasDatabaseName("IX_ProductFavorites_ProductId");
            
        builder.HasIndex(pf => pf.CreatedAt)
            .HasDatabaseName("IX_ProductFavorites_CreatedAt");
        
        // Relationships
        builder.HasOne(pf => pf.User)
            .WithMany()
            .HasForeignKey(pf => pf.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pf => pf.Product)
            .WithMany(p => p.Favorites)
            .HasForeignKey(pf => pf.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
