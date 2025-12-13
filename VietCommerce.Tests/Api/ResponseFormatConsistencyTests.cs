using Xunit;
using VietCommerce.Core.Models;

namespace VietCommerce.Tests.Api;

/// <summary>
/// Tests for consistent API response format across all endpoints
/// Requirements: 8.1, 8.2, 8.3, 8.4, 8.5
/// </summary>
public class ResponseFormatConsistencyTests
{
    /// <summary>
    /// Property 28: Success Response Format Consistency
    /// **Feature: campaign-promotion-api, Property 28: Success Response Format Consistency**
    /// **Validates: Requirements 8.1**
    /// 
    /// For any successful API request, the response should contain success=true, data object, and message field
    /// </summary>
    [Fact]
    public void SuccessResponseFormat_ContainsRequiredFields()
    {
        // Arrange
        var testData = new { Id = Guid.NewGuid(), Name = "Test Campaign" };
        var message = "Success";

        // Act
        var response = ApiResponse<object>.SuccessResponse(testData, message);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(testData, response.Data);
        Assert.Equal(message, response.Message);
        Assert.Null(response.Errors);
    }

    /// <summary>
    /// Property 29: Validation Error Response Format
    /// **Feature: campaign-promotion-api, Property 29: Validation Error Response Format**
    /// **Validates: Requirements 8.2**
    /// 
    /// For any validation failure, the response should return HTTP 400 with field-level error details in errors object
    /// </summary>
    [Fact]
    public void ValidationErrorResponse_ContainsFieldLevelErrors()
    {
        // Arrange
        var message = "Validation failed";
        var errors = new[] { "Field1 is required", "Field2 must be positive" };

        // Act
        var response = ErrorResponse.ValidationError(message, new Dictionary<string, string[]>(), "VALIDATION_ERROR");

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal("VALIDATION_ERROR", response.ErrorCode);
        Assert.Equal(400, response.StatusCode);
        Assert.NotNull(response.Errors);
    }

    /// <summary>
    /// Property 30: Not Found Error Response
    /// **Feature: campaign-promotion-api, Property 30: Not Found Error Response**
    /// **Validates: Requirements 8.3**
    /// 
    /// For any request for non-existent resource, the response should return HTTP 404 with descriptive message
    /// </summary>
    [Fact]
    public void NotFoundErrorResponse_HasCorrectStatusCode()
    {
        // Arrange
        var message = "Campaign not found";
        var errorCode = "CAMPAIGN_NOT_FOUND";

        // Act
        var response = ErrorResponse.NotFound(message, errorCode);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(errorCode, response.ErrorCode);
        Assert.Equal(404, response.StatusCode);
    }

    /// <summary>
    /// Property 31: Business Rule Violation Response
    /// **Feature: campaign-promotion-api, Property 31: Business Rule Violation Response**
    /// **Validates: Requirements 8.4**
    /// 
    /// For any business rule violation, the response should return HTTP 409 with specific conflict reason and errorCode
    /// </summary>
    [Fact]
    public void ConflictErrorResponse_HasCorrectStatusCode()
    {
        // Arrange
        var message = "Cannot add promotion to non-DRAFT campaign";
        var errorCode = "CAMPAIGN_NOT_DRAFT";

        // Act
        var response = ErrorResponse.Conflict(message, errorCode);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(errorCode, response.ErrorCode);
        Assert.Equal(409, response.StatusCode);
    }

    /// <summary>
    /// Property 32: Authorization Error Response
    /// **Feature: campaign-promotion-api, Property 32: Authorization Error Response**
    /// **Validates: Requirements 8.5**
    /// 
    /// For any unauthorized request, the response should return HTTP 401 or 403 with appropriate message
    /// </summary>
    [Fact]
    public void UnauthorizedErrorResponse_HasCorrectStatusCode()
    {
        // Arrange
        var message = "User not authenticated";
        var errorCode = "UNAUTHORIZED";

        // Act
        var response = ErrorResponse.Unauthorized(message, errorCode);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(errorCode, response.ErrorCode);
        Assert.Equal(401, response.StatusCode);
    }

    /// <summary>
    /// Forbidden error response test
    /// </summary>
    [Fact]
    public void ForbiddenErrorResponse_HasCorrectStatusCode()
    {
        // Arrange
        var message = "User lacks permission";
        var errorCode = "FORBIDDEN";

        // Act
        var response = ErrorResponse.Forbidden(message, errorCode);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(errorCode, response.ErrorCode);
        Assert.Equal(403, response.StatusCode);
    }

    /// <summary>
    /// Server error response test
    /// </summary>
    [Fact]
    public void ServerErrorResponse_HasCorrectStatusCode()
    {
        // Arrange
        var message = "An internal server error occurred";
        var errorCode = "INTERNAL_SERVER_ERROR";

        // Act
        var response = ErrorResponse.ServerError(message, errorCode);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(errorCode, response.ErrorCode);
        Assert.Equal(500, response.StatusCode);
    }

    /// <summary>
    /// Test that all error responses have timestamp
    /// </summary>
    [Fact]
    public void ErrorResponse_AlwaysHasTimestamp()
    {
        // Arrange
        var beforeTime = DateTime.UtcNow;

        // Act
        var response = ErrorResponse.ValidationError("Test error");

        var afterTime = DateTime.UtcNow;

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Timestamp >= beforeTime);
        Assert.True(response.Timestamp <= afterTime);
    }

    /// <summary>
    /// Test that success response has default message
    /// </summary>
    [Fact]
    public void SuccessResponse_HasDefaultMessage()
    {
        // Arrange
        var testData = new { Id = 1 };

        // Act
        var response = ApiResponse<object>.SuccessResponse(testData);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("Success", response.Message);
    }

    /// <summary>
    /// Test that failure response has default message
    /// </summary>
    [Fact]
    public void FailureResponse_HasDefaultMessage()
    {
        // Arrange & Act
        var response = ApiResponse<object>.FailureResponse();

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Failed", response.Message);
    }

    /// <summary>
    /// Test that failure response includes errors array
    /// </summary>
    [Fact]
    public void FailureResponse_IncludesErrorsArray()
    {
        // Arrange
        var message = "Validation failed";
        var errors = new[] { "Error 1", "Error 2" };

        // Act
        var response = ApiResponse<object>.FailureResponse(message, errors);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotNull(response.Errors);
        Assert.Equal(2, response.Errors.Length);
        Assert.Contains("Error 1", response.Errors);
        Assert.Contains("Error 2", response.Errors);
    }

    /// <summary>
    /// Test that validation error response can have field-level errors
    /// </summary>
    [Fact]
    public void ValidationErrorResponse_CanHaveFieldLevelErrors()
    {
        // Arrange
        var fieldErrors = new Dictionary<string, string[]>
        {
            { "CampaignName", new[] { "Campaign name is required" } },
            { "Budget", new[] { "Budget must be non-negative" } }
        };

        // Act
        var response = ErrorResponse.ValidationError(
            "Validation failed",
            fieldErrors,
            "VALIDATION_ERROR");

        // Assert
        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.NotNull(response.Errors);
        Assert.Equal(2, response.Errors.Count);
        Assert.True(response.Errors.ContainsKey("CampaignName"));
        Assert.True(response.Errors.ContainsKey("Budget"));
    }
}
