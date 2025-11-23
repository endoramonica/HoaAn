using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VietCommerce.Data.Context.Configurations.SocialCommunity
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.ToTable("Likes");

            // Composite key: 1 customer chỉ like 1 post 1 lần
            builder.HasKey(l => new { l.PostId, l.CustomerId });

            // Properties
            builder.Property(l => l.LikedOn)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(l => l.Customer)
                .WithMany()
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(l => l.PostId)
                .HasDatabaseName("IX_Likes_PostId");

            builder.HasIndex(l => new { l.CustomerId, l.LikedOn })
                .HasDatabaseName("IX_Likes_Customer_LikedOn");
        }
    }
}
