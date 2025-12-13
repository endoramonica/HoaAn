using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for campaign targeting rules
/// Defines which pages, frequency, and timing rules apply to a campaign
/// </summary>
public class TargetingRulesDto
{
    /// <summary>
    /// List of pages where the campaign should be displayed
    /// Example: ["home", "product", "checkout"]
    /// </summary>
    [Required(ErrorMessage = "Pages list is required")]
    public List<string> Pages { get; set; } = new List<string>();

    /// <summary>
    /// Frequency of campaign display
    /// Valid values: "once-per-session", "once-per-page", "always"
    /// </summary>
    [Required(ErrorMessage = "Frequency is required")]
    [RegularExpression(@"^(once-per-session|once-per-page|always)$", 
        ErrorMessage = "Frequency must be 'once-per-session', 'once-per-page', or 'always'")]
    public string Frequency { get; set; } = "once-per-session";

    /// <summary>
    /// Delay in milliseconds before showing the campaign
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "DelayMs must be non-negative")]
    public int DelayMs { get; set; } = 0;

    /// <summary>
    /// Auto-dismiss delay in milliseconds (0 means no auto-dismiss)
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "AutoDismissMs must be non-negative")]
    public int AutoDismissMs { get; set; } = 0;
}
