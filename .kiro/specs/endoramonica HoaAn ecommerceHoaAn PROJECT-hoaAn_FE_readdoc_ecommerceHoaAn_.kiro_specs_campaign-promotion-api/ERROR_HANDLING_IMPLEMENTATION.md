# Error Handling Middleware Implementation

## Overview

This document describes the implementation of the global exception handling middleware for the Campaign & Promotion Management System API.

## Requirements Addressed

- **Requirement 8.1**: Success responses return HTTP 200 with consistent format
- **Requirement 8.2**: Validation failures return HTTP 400 with field-level error details
- **Requirement 8.3**: Not found resources return HTTP 404 with descriptive message
- **Requirement 8.4**: Business rule violations return HTTP 409 with specific conflict reason
- **Requirement 8.5**: Unauthorized requests return HTTP 401 or 403 as appropriate

## Implementation Components

### 1. ExceptionHandlingMiddleware (`VietCommerce.Api/Middleware/ExceptionHandlingMiddleware.cs`)

**Purpose**: Global exception handler that catches all unhandled exceptions and returns consistent error responses.

**Key Features**:
- Catches all exceptions in the request pipeline
- Maps exceptions to appropriate HTTP status codes
- Formats error responses consistently
- Logs errors appropriately
- Handles custom business exceptions with error codes

**Exception Mapping**:

| Exception Type | HTTP Status | Error Code | Use Case |
|---|---|---|---|
| `BusinessException` | 409 Conflict | Custom (from exception) | Business rule violations |
| `TenantException` | 403 Forbidden | TENANT_ERROR | Tenant access denied |
| `InventoryException` | 409 Conflict | INVENTORY_ERROR | Inventory violations |
| `ArgumentException` | 400 Bad Request | INVALID_ARGUMENT | Validation errors |
| `KeyNotFoundException` | 404 Not Found | RESOURCE_NOT_FOUND | Resource not found |
| `InvalidOperationException` | 409 Conflict | BUSINESS_RULE_VIOLATION | Business rule violations |
| `UnauthorizedAccessException` | 401 Unauthorized | UNAUTHORIZED_ACCESS | Authorization failures |
| All other exceptions | 500 Internal Server Error | INTERNAL_SERVER_ERROR | Unexpected errors |

**Registration**: Registered in `Program.cs` as early middleware in the pipeline:
```csharp
app.UseExceptionHandlingMiddleware();
```

### 2. ErrorResponse Model (`VietCommerce.Core/Models/ErrorResponse.cs`)

**Purpose**: Standardized error response format with error codes for programmatic handling.

**Properties**:
- `Success` (bool): Always false for error responses
- `Message` (string): Human-readable error message
- `ErrorCode` (string): Machine-readable error code for programmatic handling
- `Errors` (Dictionary<string, string[]>): Field-level validation errors
- `StatusCode` (int): HTTP status code
- `Timestamp` (DateTime): When the error occurred

**Factory Methods**:
- `ValidationError()`: Create 400 validation error response
- `NotFound()`: Create 404 not found error response
- `Conflict()`: Create 409 conflict error response
- `Unauthorized()`: Create 401 unauthorized error response
- `Forbidden()`: Create 403 forbidden error response
- `ServerError()`: Create 500 server error response

### 3. ApiResponse Model (`VietCommerce.Core/Models/ApiResponse.cs`)

**Purpose**: Standardized success response format.

**Properties**:
- `Success` (bool): Always true for success responses
- `Data` (T): Response data
- `Message` (string): Human-readable message
- `Errors` (string[]): Optional error messages

**Factory Methods**:
- `SuccessResponse()`: Create successful response with data
- `FailureResponse()`: Create failure response with errors

### 4. Response Format Consistency Tests (`VietCommerce.Tests/Api/ResponseFormatConsistencyTests.cs`)

**Purpose**: Verify that all API responses follow the consistent format.

**Test Coverage**:
- Property 28: Success response format consistency
- Property 29: Validation error response format
- Property 30: Not found error response
- Property 31: Business rule violation response
- Property 32: Authorization error response
- Additional tests for error response structure

### 5. Exception Handling Middleware Tests (`VietCommerce.Tests/Api/ExceptionHandlingMiddlewareTests.cs`)

**Purpose**: Verify that the middleware correctly handles all exception types.

**Test Coverage**:
- BusinessException → 409 Conflict
- TenantException → 403 Forbidden
- InventoryException → 409 Conflict
- ArgumentException → 400 Bad Request
- KeyNotFoundException → 404 Not Found
- InvalidOperationException → 409 Conflict
- UnauthorizedAccessException → 401 Unauthorized
- Unknown exceptions → 500 Internal Server Error
- Error response always has timestamp
- Middleware passes through successful requests

## Error Response Examples

### Success Response (200)
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "campaignName": "Summer Sale"
  },
  "message": "Campaign created successfully"
}
```

### Validation Error (400)
```json
{
  "success": false,
  "message": "Validation failed",
  "errorCode": "INVALID_ARGUMENT",
  "errors": {
    "campaignName": ["Campaign name is required"],
    "budget": ["Budget must be non-negative"]
  },
  "statusCode": 400,
  "timestamp": "2025-01-13T10:30:00Z"
}
```

### Not Found Error (404)
```json
{
  "success": false,
  "message": "Campaign not found",
  "errorCode": "RESOURCE_NOT_FOUND",
  "errors": {},
  "statusCode": 404,
  "timestamp": "2025-01-13T10:30:00Z"
}
```

### Business Rule Violation (409)
```json
{
  "success": false,
  "message": "Cannot add promotion to non-DRAFT campaign",
  "errorCode": "CAMPAIGN_NOT_DRAFT",
  "errors": {},
  "statusCode": 409,
  "timestamp": "2025-01-13T10:30:00Z"
}
```

### Unauthorized (401)
```json
{
  "success": false,
  "message": "User not authenticated",
  "errorCode": "UNAUTHORIZED_ACCESS",
  "errors": {},
  "statusCode": 401,
  "timestamp": "2025-01-13T10:30:00Z"
}
```

### Forbidden (403)
```json
{
  "success": false,
  "message": "Access denied to tenant",
  "errorCode": "TENANT_ERROR",
  "errors": {},
  "statusCode": 403,
  "timestamp": "2025-01-13T10:30:00Z"
}
```

### Internal Server Error (500)
```json
{
  "success": false,
  "message": "An unexpected error occurred. Please try again later.",
  "errorCode": "INTERNAL_SERVER_ERROR",
  "errors": {},
  "statusCode": 500,
  "timestamp": "2025-01-13T10:30:00Z"
}
```

## Logging

The middleware logs all exceptions with the following format:
```
❌ Unhandled exception: {ExceptionType} - {ExceptionMessage}
```

Example:
```
❌ Unhandled exception: BusinessException - Campaign not in DRAFT status
```

## Integration Points

### Controllers
All controllers can throw exceptions, which will be automatically caught and formatted by the middleware:

```csharp
[HttpPost]
public async Task<IActionResult> CreateCampaign(CreateCampaignDto dto)
{
    // If validation fails, ArgumentException is thrown
    if (dto.EndDate <= dto.StartDate)
        throw new ArgumentException("EndDate must be greater than StartDate");
    
    // If business rule is violated, BusinessException is thrown
    if (dto.Budget < 0)
        throw new BusinessException("Budget cannot be negative", "NEGATIVE_BUDGET");
    
    // If resource not found, KeyNotFoundException is thrown
    var campaign = await _campaignService.GetCampaignAsync(id);
    if (campaign == null)
        throw new KeyNotFoundException("Campaign not found");
    
    // ... rest of implementation
}
```

### Services
Services can throw custom exceptions which will be caught by the middleware:

```csharp
public async Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto)
{
    // Validation
    if (dto.EndDate <= dto.StartDate)
        throw new ArgumentException("Invalid date range");
    
    // Business logic
    if (dto.Budget < 0)
        throw new BusinessException("Budget must be non-negative", "NEGATIVE_BUDGET");
    
    // ... rest of implementation
}
```

## Testing

### Running Tests

To run the response format consistency tests:
```bash
dotnet test VietCommerce.Tests/VietCommerce.Tests.csproj --filter "ResponseFormatConsistencyTests"
```

To run the exception handling middleware tests:
```bash
dotnet test VietCommerce.Tests/VietCommerce.Tests.csproj --filter "ExceptionHandlingMiddlewareTests"
```

To run all API tests:
```bash
dotnet test VietCommerce.Tests/VietCommerce.Tests.csproj --filter "Api"
```

## Correctness Properties Validated

1. **Property 28: Success Response Format Consistency** - All successful responses contain success=true, data, and message
2. **Property 29: Validation Error Response Format** - Validation failures return 400 with field-level errors
3. **Property 30: Not Found Error Response** - Missing resources return 404 with descriptive message
4. **Property 31: Business Rule Violation Response** - Business rule violations return 409 with error code
5. **Property 32: Authorization Error Response** - Unauthorized requests return 401 or 403

## Summary

The error handling middleware implementation provides:
- ✅ Global exception handling for all unhandled exceptions
- ✅ Consistent error response format across all endpoints
- ✅ Proper HTTP status code mapping
- ✅ Machine-readable error codes for programmatic handling
- ✅ Field-level validation error details
- ✅ Comprehensive logging
- ✅ Support for custom business exceptions
- ✅ Full test coverage with unit and property-based tests

All requirements (8.1-8.5) are fully implemented and tested.
