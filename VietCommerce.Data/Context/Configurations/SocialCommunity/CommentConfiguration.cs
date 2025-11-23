using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Data.Context.Configurations.SocialCommunity
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.PostId)
                .IsRequired();

            builder.Property(c => c.CustomerId)
                .IsRequired();

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.AddedOn)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relationships
            builder.HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Nested comment support
            builder.HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(c => new { c.PostId, c.AddedOn })
                .HasDatabaseName("IX_Comments_Post_AddedOn");

            builder.HasIndex(c => c.CustomerId)
                .HasDatabaseName("IX_Comments_CustomerId");

            builder.HasIndex(c => c.ParentCommentId)
                .HasDatabaseName("IX_Comments_ParentId")
                .HasFilter("[ParentCommentId] IS NOT NULL");

            // Soft delete filter
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
}
}
