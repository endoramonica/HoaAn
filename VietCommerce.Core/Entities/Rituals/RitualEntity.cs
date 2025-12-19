using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Rituals;

/// <summary>
/// Represents a Vietnamese cultural ritual pattern that can be detected through user behavior.
/// Examples: Đầy Tháng, Tết, Lễ Cúng Tổ Tiên, Lễ Cúng Thần Tài
/// </summary>
public class RitualEntity : AuditableEntity
{
    /// <summary>
    /// Unique identifier for the ritual
    /// </summary>
    [MaxLength(100)]
    public string RitualId { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the ritual (e.g., "Đầy Tháng", "Tết")
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// JSON array of action types that form the pattern for this ritual
    /// Example: ["ViewProduct", "AddToCart", "BrowseCategory"]
    /// </summary>
    public string ActionSequencePatternJson { get; set; } = string.Empty;

    /// <summary>
    /// JSON array of required product IDs or category IDs for this ritual
    /// Example: ["prod-1", "prod-2", "cat-ritual-items"]
    /// </summary>
    public string RequiredItemsJson { get; set; } = string.Empty;

    /// <summary>
    /// Confidence threshold (0-1) for pattern matching
    /// Patterns matching above this threshold will be considered valid
    /// </summary>
    [Column(TypeName = "decimal(3,2)")]
    public decimal ConfidenceThreshold { get; set; } = 0.7m;

    /// <summary>
    /// Cultural significance and description of the ritual
    /// </summary>
    [MaxLength(1000)]
    public string CulturalSignificance { get; set; } = string.Empty;

    /// <summary>
    /// JSON array of sources or references for the ritual
    /// Example: ["Vietnamese Tradition", "Cultural Heritage"]
    /// </summary>
    public string SourcesJson { get; set; } = string.Empty;

    /// <summary>
    /// Whether this ritual is active and should be used for pattern matching
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property for related dismissals
    /// </summary>
    public virtual ICollection<RitualDismissalEntity> Dismissals { get; set; } = new List<RitualDismissalEntity>();

    /// <summary>
    /// Navigation property for related recommendation logs
    /// </summary>
    public virtual ICollection<RecommendationLogEntity> RecommendationLogs { get; set; } = new List<RecommendationLogEntity>();
}
