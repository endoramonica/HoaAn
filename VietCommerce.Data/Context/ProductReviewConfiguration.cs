using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Context;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("ProductReviews");
        
        // Primary Key
        builder.HasKey(pr => pr.Id);
        
        // Properties
        builder.Property(pr => pr.Rating)
            .IsRequired();
            
        builder.Property(pr => pr.Comment)
            .HasMaxLength(2000);
            
        builder.Property(pr => pr.AdminReply)
            .HasMaxLength(1000);
        
        // Unique Constraint - 1 review per user per product per order
        builder.HasIndex(pr => new { pr.UserId, pr.ProductId, pr.OrderItemId })
            .IsUnique()
            .HasDatabaseName("IX_ProductReviews_User_Product_OrderItem_Unique");
        
        // Indexes cho queries
        builder.HasIndex(pr => pr.ProductId)
            .HasDatabaseName("IX_ProductReviews_ProductId");
            
        builder.HasIndex(pr => pr.UserId)
            .HasDatabaseName("IX_ProductReviews_UserId");
            
        builder.HasIndex(pr => pr.Rating)
            .HasDatabaseName("IX_ProductReviews_Rating");
            
        builder.HasIndex(pr => pr.IsVisible)
            .HasDatabaseName("IX_ProductReviews_IsVisible");
            
        builder.HasIndex(pr => pr.IsDeleted)
            .HasDatabaseName("IX_ProductReviews_IsDeleted");
            
        // Composite index cho filtering visible & not deleted reviews
        builder.HasIndex(pr => new { pr.ProductId, pr.IsVisible, pr.IsDeleted })
            .HasDatabaseName("IX_ProductReviews_Product_Visible_Deleted");
        
        // 🆕 COMPOSITE INDEX: Product + Rating + IsVisible (Issue 1 Fix)
        builder.HasIndex(pr => new { pr.ProductId, pr.Rating, pr.IsVisible })
            .HasDatabaseName("IX_ProductReviews_Product_Rating_Visible");
        
        // Relationships
        builder.HasOne(pr => pr.User)
            .WithMany()
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(pr => pr.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pr => pr.OrderItem)
            .WithMany(oi => oi.Reviews)
            .HasForeignKey(pr => pr.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Query Filter - Global Soft Delete
        builder.HasQueryFilter(pr => !pr.IsDeleted);
    }
}
