namespace VietCommerce.Core.Models;


/// Message payload for Redis Pub/Sub cache invalidation
/// Channel: permissions:invalidate

public class CacheInvalidationMessage
{
    
    /// User ID whose cache should be invalidated
    
    public Guid UserId { get; set; }

    
    /// Invalidation type: "user" (direct assignment) or "role" (role permission changed)
    
    public string Type { get; set; } = "user";

    
    /// Role ID (if Type = "role")
    
    public Guid? RoleId { get; set; }

    
    /// Timestamp of invalidation event
    
    public DateTime Timestamp { get; set; }

    
    /// Optional: Source API instance (for debugging)
    
    public string? Source { get; set; }
}