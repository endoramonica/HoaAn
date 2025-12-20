using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Entities.Rituals;

/// <summary>
/// Represents a user action tracked for ritual pattern detection.
/// Actions include: viewing products, adding to cart, browsing categories, etc.
/// </summary>
public class ActionEntity : AuditableEntity
{
    /// <summary>
    /// User ID who performed the action
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Session ID to group actions within a browsing session
    /// </summary>
    [MaxLength(450)]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Type of action performed
    /// Examples: "ViewProduct", "AddToCart", "BrowseCategory", "RemoveFromCart"
    /// </summary>
    [MaxLength(100)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the action was performed
    /// </summary>
    public DateTime ActionTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Product ID associated with the action (if applicable)
    /// </summary>
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Category ID associated with the action (if applicable)
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// JSON metadata containing additional context about the action
    /// Example: {"quantity": 2, "price": 50000, "source": "search"}
    /// </summary>
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
}
