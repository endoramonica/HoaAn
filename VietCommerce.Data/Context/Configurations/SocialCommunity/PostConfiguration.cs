using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Data.Context.Configurations.SocialCommunity
{

    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.CustomerId)
                .IsRequired();

            builder.Property(p => p.Content)
                .HasMaxLength(5000);

            builder.Property(p => p.PhotoPath)
                .HasMaxLength(500)
                .HasComment("Physical path of image");

            builder.Property(p => p.PhotoUrl)
                .HasMaxLength(1000);

            builder.Property(p => p.PhotoHash)
                .HasMaxLength(64);

            builder.Property(p => p.PostedOn)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.RowVersion)
                .IsRowVersion();

            // Relationships
            builder.HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Bookmarks)
                .WithOne(b => b.Post)
                .HasForeignKey(b => b.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => new { p.CustomerId, p.PostedOn })
                .HasDatabaseName("IX_Posts_Customer_PostedOn");

            builder.HasIndex(p => p.PostedOn)
                .HasDatabaseName("IX_Posts_PostedOn");

            builder.HasIndex(p => p.PhotoHash)
                .HasDatabaseName("IX_Posts_PhotoHash")
                .HasFilter("[PhotoHash] IS NOT NULL");

            // Soft delete filter
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }

    }
