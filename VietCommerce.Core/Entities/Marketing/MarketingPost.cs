using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.Entities.Marketing;

/// <summary>
/// Marketing Post entity for admin-managed promotional content
/// Separate from Social Community Posts (Customer posts)
/// </summary>
public class MarketingPost : AuditableEntity, ISoftDelete
{
    // Primary Key inherited from BaseEntity (via AuditableEntity)
    // public Guid Id { get; set; }

    // Basic Content
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(10000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    // Media
    public string? ImageUrl { get; set; }
    public string? ImageData { get; set; } // Base64 for backward compatibility
    public string? ImageUrls { get; set; } // JSON array

    // Product Relation
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; } // Denormalized for performance
    public virtual Product? Product { get; set; }

    // Content Metadata
    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public string? Hashtags { get; set; } // JSON array

    // Priority and Display Control
    public int PriorityScore { get; set; } = 50; // 1-100, default 50
    public string? DisplayLocation { get; set; } // JSON array: homepage_banner, product_section, featured_section, sidebar
    public bool IsFeatured { get; set; } = false; // Auto-set when PriorityScore > 80

    // SEO
    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    public string? MetaKeywords { get; set; } // JSON array

    // Social Media Variants
    public string? FacebookPost { get; set; }
    public string? InstagramPost { get; set; }
    public string? TwitterPost { get; set; }
    public string? LinkedInPost { get; set; }

    // Publishing
    public MarketingPostStatus Status { get; set; } = MarketingPostStatus.Draft;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    // Analytics
    public int Views { get; set; } = 0;
    public int Clicks { get; set; } = 0;
    public int Shares { get; set; } = 0;

    // Audit Fields inherited from AuditableEntity:
    // public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedAt { get; set; }
    // public Guid CreatedBy { get; set; }
    // public Guid? UpdatedBy { get; set; }

    // Soft Delete (from ISoftDelete)
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Concurrency
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
