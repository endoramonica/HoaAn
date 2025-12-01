using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VietCommerce.Data.Context.Configurations.SocialCommunity
{
    /// <summary>
    /// EF Core configuration cho Bookmark entity
    /// ✅ Composite Primary Key: (PostId, CustomerId)
    /// ✅ KHÔNG có Id field
    /// </summary>
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            // ========== TABLE NAME ==========
            builder.ToTable("Bookmarks");

            // ========== COMPOSITE PRIMARY KEY ==========
            builder.HasKey(b => new { b.PostId, b.CustomerId });

            // ========== PROPERTIES ==========
            builder.Property(b => b.PostId)
                .IsRequired();

            builder.Property(b => b.CustomerId)
                .IsRequired();

            builder.Property(b => b.BookmarkedOn)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // ========== RELATIONSHIPS ==========

            // Bookmark → Post (Many-to-One)
            builder.HasOne(b => b.Post)
                .WithMany(p => p.Bookmarks)
                .HasForeignKey(b => b.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Bookmark → Customer (Many-to-One)
            builder.HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== INDEXES ==========
            builder.HasIndex(b => b.PostId)
                .HasDatabaseName("IX_Bookmarks_PostId");

            builder.HasIndex(b => b.CustomerId)
                .HasDatabaseName("IX_Bookmarks_CustomerId");

            builder.HasIndex(b => b.BookmarkedOn)
                .HasDatabaseName("IX_Bookmarks_BookmarkedOn");
        }
    }
}
