namespace VietCommerce.Core.Models;
/// Session metadata stored in Redis
/// Key pattern: session:{userId}
public class SessionData
{
    /// JWT ID (jti claim) - unique per token
    public string Jti { get; set; } = string.Empty;
    /// User ID
    public Guid UserId { get; set; }
    /// Token issued at (UTC)
    public DateTime IssuedAt { get; set; }
    /// Token expires at (UTC)
    public DateTime ExpiresAt { get; set; }
    /// Device/browser info (optional)
    public string? UserAgent { get; set; }
    /// IP address (optional, for audit)
    public string? IpAddress { get; set; }
    public DateTime LastActivityAt { get; set; }
}
