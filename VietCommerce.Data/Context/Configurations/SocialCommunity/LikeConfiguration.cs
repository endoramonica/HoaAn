using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VietCommerce.Data.Context.Configurations.SocialCommunity
{

    /// <summary>
    /// EF Core configuration cho Like entity
    /// ✅ Composite Primary Key: (PostId, CustomerId)
    /// ✅ KHÔNG có Id field
    /// </summary>
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            // ========== TABLE NAME ==========
            builder.ToTable("Likes");

            // ========== COMPOSITE PRIMARY KEY ==========
            builder.HasKey(l => new { l.PostId, l.CustomerId });

            // ========== PROPERTIES ==========
            builder.Property(l => l.PostId)
                .IsRequired();

            builder.Property(l => l.CustomerId)
                .IsRequired();

            builder.Property(l => l.LikedOn)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // ========== RELATIONSHIPS ==========

            // Like → Post (Many-to-One)
            builder.HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Like → Customer (Many-to-One)
            builder.HasOne(l => l.Customer)
                .WithMany()
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== INDEXES ==========
            builder.HasIndex(l => l.PostId)
                .HasDatabaseName("IX_Likes_PostId");

            builder.HasIndex(l => l.CustomerId)
                .HasDatabaseName("IX_Likes_CustomerId");

            builder.HasIndex(l => l.LikedOn)
                .HasDatabaseName("IX_Likes_LikedOn");
        }
    }
}
