namespace VietCommerce.Core.Models;

/// <summary>
/// Standardized error response format with error codes for programmatic handling
/// Requirements: 8.1, 8.2, 8.3, 8.4, 8.5
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Machine-readable error code for programmatic handling
    /// Examples: INVALID_DATE_RANGE, NEGATIVE_BUDGET, CAMPAIGN_NOT_DRAFT, etc.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Field-level validation errors
    /// Key: field name, Value: array of error messages for that field
    /// </summary>
    public Dictionary<string, string[]> Errors { get; set; } = new();

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // -----------------------
    // Factory methods
    // -----------------------

    /// <summary>
    /// Create a validation error response (400)
    /// </summary>
    public static ErrorResponse ValidationError(
        string message = "Validation failed",
        Dictionary<string, string[]> errors = null,
        string errorCode = "VALIDATION_ERROR")
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors ?? new Dictionary<string, string[]>(),
            StatusCode = 400
        };
    }

    /// <summary>
    /// Create a not found error response (404)
    /// </summary>
    public static ErrorResponse NotFound(
        string message = "Resource not found",
        string errorCode = "NOT_FOUND")
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = new Dictionary<string, string[]>(),
            StatusCode = 404
        };
    }

    /// <summary>
    /// Create a conflict error response (409)
    /// </summary>
    public static ErrorResponse Conflict(
        string message = "Business rule violation",
        string errorCode = "CONFLICT")
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = new Dictionary<string, string[]>(),
            StatusCode = 409
        };
    }

    /// <summary>
    /// Create an unauthorized error response (401)
    /// </summary>
    public static ErrorResponse Unauthorized(
        string message = "Unauthorized",
        string errorCode = "UNAUTHORIZED")
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = new Dictionary<string, string[]>(),
            StatusCode = 401
        };
    }

    /// <summary>
    /// Create a forbidden error response (403)
    /// </summary>
    public static ErrorResponse Forbidden(
        string message = "Forbidden",
        string errorCode = "FORBIDDEN")
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = new Dictionary<string, string[]>(),
            StatusCode = 403
        };
    }

    /// <summary>
    /// Create a server error response (500)
    /// </summary>
    public static ErrorResponse ServerError(
        string message = "An internal server error occurred",
        string errorCode = "INTERNAL_SERVER_ERROR")
    {
        return new ErrorResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            Errors = new Dictionary<string, string[]>(),
            StatusCode = 500
        };
    }
}
