using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VietCommerce.Data.Context.Configurations.SocialCommunity
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.ToTable("Bookmarks");

            // Composite key
            builder.HasKey(b => new { b.PostId, b.CustomerId });

            // Properties
            builder.Property(b => b.BookmarkedOn)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(b => b.Customer)
                .WithMany()
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Post)
                .WithMany(p => p.Bookmarks)
                .HasForeignKey(b => b.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => new { b.CustomerId, b.BookmarkedOn })
                .HasDatabaseName("IX_Bookmarks_Customer_BookmarkedOn");
        }
    }
}
