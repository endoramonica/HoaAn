namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// Result of bulk operations on marketing posts
/// </summary>
public class BulkOperationResult
{
    public int TotalRequested { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<BulkOperationError> Errors { get; set; } = new();
}

/// <summary>
/// Error details for failed bulk operations
/// </summary>
public class BulkOperationError
{
    public Guid PostId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
