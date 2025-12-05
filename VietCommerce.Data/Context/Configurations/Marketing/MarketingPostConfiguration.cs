using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Data.Context.Configurations.Marketing;

/// <summary>
/// EF Core configuration for MarketingPost entity
/// </summary>
public class MarketingPostConfiguration : IEntityTypeConfiguration<MarketingPost>
{
    public void Configure(EntityTypeBuilder<MarketingPost> builder)
    {
        builder.ToTable("MarketingPosts");
        builder.HasKey(p => p.Id);

        // ========== String Properties ==========
        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .IsRequired()
            .HasComment("Marketing post title");

        builder.Property(p => p.Content)
            .HasMaxLength(10000)
            .IsRequired()
            .HasComment("Main content of the marketing post");

        builder.Property(p => p.ShortDescription)
            .HasMaxLength(500)
            .HasComment("Short description for previews");

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(1000)
            .HasComment("Primary image URL");

        builder.Property(p => p.ImageData)
            .HasComment("Base64 image data for backward compatibility");

        builder.Property(p => p.ImageUrls)
            .HasComment("JSON array of multiple image URLs");

        builder.Property(p => p.ProductName)
            .HasMaxLength(200)
            .HasComment("Denormalized product name for performance");

        builder.Property(p => p.Topic)
            .HasMaxLength(100);

        builder.Property(p => p.Platform)
            .HasMaxLength(50)
            .HasComment("Target platform (e.g., Facebook, Instagram)");

        builder.Property(p => p.Tone)
            .HasMaxLength(50)
            .HasComment("Content tone (e.g., Professional, Casual)");

        builder.Property(p => p.Hashtags)
            .HasComment("JSON array of hashtags");

        // ========== Priority and Display Properties ==========
        builder.Property(p => p.PriorityScore)
            .IsRequired()
            .HasDefaultValue(50)
            .HasComment("Priority score (1-100) for display ranking");

        builder.Property(p => p.DisplayLocation)
            .HasComment("JSON array of display locations (homepage_banner, product_section, featured_section, sidebar)");

        builder.Property(p => p.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Auto-set to true when PriorityScore > 80");

        // ========== SEO Properties ==========
        builder.Property(p => p.MetaTitle)
            .HasMaxLength(200)
            .HasComment("SEO meta title");

        builder.Property(p => p.MetaDescription)
            .HasMaxLength(500)
            .HasComment("SEO meta description");

        builder.Property(p => p.MetaKeywords)
            .HasComment("JSON array of SEO keywords");

        // ========== Social Media Variants ==========
        builder.Property(p => p.FacebookPost)
            .HasComment("Facebook-optimized content");

        builder.Property(p => p.InstagramPost)
            .HasComment("Instagram-optimized content");

        builder.Property(p => p.TwitterPost)
            .HasComment("Twitter-optimized content");

        builder.Property(p => p.LinkedInPost)
            .HasComment("LinkedIn-optimized content");

        // ========== Publishing Properties ==========
        builder.Property(p => p.Status)
            .IsRequired()
            .HasDefaultValue(MarketingPostStatus.Draft)
            .HasComment("Publishing status: Draft, Published, Scheduled");

        builder.Property(p => p.ScheduledDate)
            .HasComment("Scheduled publication date");

        builder.Property(p => p.PublishedDate)
            .HasComment("Actual publication date");

        // ========== Analytics Properties ==========
        builder.Property(p => p.Views)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Total view count");

        builder.Property(p => p.Clicks)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Total click count");

        builder.Property(p => p.Shares)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Total share count");

        // ========== Soft Delete Properties ==========
        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.DeletedAt)
            .HasComment("Soft delete timestamp");

        builder.Property(p => p.DeletedBy)
            .HasComment("User who deleted the post");

        // ========== Concurrency ==========
        builder.Property(p => p.RowVersion)
            .IsRowVersion()
            .HasComment("Concurrency token");

        // ========== Relationships ==========
        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_MarketingPosts_Products");

        // ========== Indexes ==========
        // Single column indexes
        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_MarketingPosts_Status");

        builder.HasIndex(p => p.ProductId)
            .HasDatabaseName("IX_MarketingPosts_ProductId")
            .HasFilter("[ProductId] IS NOT NULL");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_MarketingPosts_CreatedAt");

        builder.HasIndex(p => p.PublishedDate)
            .HasDatabaseName("IX_MarketingPosts_PublishedDate")
            .HasFilter("[PublishedDate] IS NOT NULL");

        builder.HasIndex(p => p.ScheduledDate)
            .HasDatabaseName("IX_MarketingPosts_ScheduledDate")
            .HasFilter("[ScheduledDate] IS NOT NULL");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_MarketingPosts_IsDeleted");

        builder.HasIndex(p => p.PriorityScore)
            .HasDatabaseName("IX_MarketingPosts_PriorityScore");

        builder.HasIndex(p => p.IsFeatured)
            .HasDatabaseName("IX_MarketingPosts_IsFeatured")
            .HasFilter("[IsFeatured] = 1");

        // Composite indexes for common queries
        builder.HasIndex(p => new { p.Status, p.IsDeleted })
            .HasDatabaseName("IX_MarketingPosts_Status_IsDeleted");

        builder.HasIndex(p => new { p.ProductId, p.IsDeleted })
            .HasDatabaseName("IX_MarketingPosts_ProductId_IsDeleted")
            .HasFilter("[ProductId] IS NOT NULL");

        builder.HasIndex(p => new { p.Status, p.ScheduledDate })
            .HasDatabaseName("IX_MarketingPosts_Status_ScheduledDate")
            .HasFilter("[ScheduledDate] IS NOT NULL");

        builder.HasIndex(p => new { p.PriorityScore, p.PublishedDate })
            .HasDatabaseName("IX_MarketingPosts_PriorityScore_PublishedDate")
            .HasFilter("[PublishedDate] IS NOT NULL");

        builder.HasIndex(p => new { p.IsFeatured, p.PriorityScore, p.IsDeleted })
            .HasDatabaseName("IX_MarketingPosts_Featured_Priority_IsDeleted");

        // ========== Query Filters ==========
        // Global filter for soft delete
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
