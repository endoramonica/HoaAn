using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Entities.Rituals;

/// <summary>
/// Tracks when a user dismisses or disables a ritual recommendation.
/// Used to respect user preferences and reduce future recommendations of that type.
/// </summary>
public class RitualDismissalEntity : AuditableEntity
{
    /// <summary>
    /// User ID who dismissed the ritual
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Ritual ID that was dismissed
    /// </summary>
    public Guid RitualId { get; set; }

    /// <summary>
    /// Timestamp when the dismissal occurred
    /// </summary>
    public DateTime DismissedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional reason for dismissal provided by the user
    /// </summary>
    [MaxLength(500)]
    public string? DismissalReason { get; set; }

    /// <summary>
    /// Whether this is a permanent disable (user explicitly said not interested)
    /// vs a temporary dismissal (just hide for now)
    /// </summary>
    public bool IsDisabled { get; set; } = false;

    /// <summary>
    /// Navigation property to User
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    /// <summary>
    /// Navigation property to Ritual
    /// </summary>
    [ForeignKey(nameof(RitualId))]
    public virtual RitualEntity? Ritual { get; set; }
}
