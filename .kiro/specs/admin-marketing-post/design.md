# Design Document - Admin Marketing Post Management System

## Overview

Admin Marketing Post Management System cung cấp RESTful API để quản lý bài viết marketing. Hệ thống được thiết kế theo Clean Architecture với các layer rõ ràng: Controller → Service → Repository → Database. Hệ thống sử dụng JWT authentication, soft delete pattern, và optimistic concurrency control cho analytics. Đây là hệ thống riêng biệt với Social Community Posts.

## Architecture

### Layer Architecture

```
┌─────────────────────────────────────┐
│   API Layer (Controllers)           │
│   - AdminMarketingPostController     │
│   - Request/Response DTOs            │
│   - Authorization Filters            │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Service Layer                      │
│   - IMarketingPostService            │
│   - MarketingPostService Impl        │
│   - Business Logic                   │
│   - Validation                       │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Repository Layer                   │
│   - IMarketingPostRepository         │
│   - Entity Framework Core            │
│   - Query Optimization               │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Database Layer                     │
│   - MarketingPost Entity             │
│   - Product Relationship             │
│   - Indexes                          │
└─────────────────────────────────────┘
```

### Technology Stack

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server / PostgreSQL
- **Authentication**: JWT Bearer Token
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **API Documentation**: Swagger/OpenAPI

## Components and Interfaces

### 1. API Controller

#### AdminMarketingPostController

```csharp
[ApiController]
[Route("api/admin/marketing-posts")]
[Authorize(Roles = "Admin")]
public class AdminMarketingPostController : ControllerBase
{
    private readonly IMarketingPostService _marketingPostService;
    private readonly ICurrentUser _currentUser;

    // CRUD Operations
    [HttpGet]
    Task<ApiResponse<PaginatedResult<MarketingPostListDto>>> GetPosts(
        [FromQuery] GetMarketingPostsQuery query);

    [HttpGet("{id}")]
    Task<ApiResponse<MarketingPostDetailDto>> GetPostById(Guid id);

    [HttpPost]
    Task<ApiResponse<MarketingPostResponseDto>> CreatePost(
        [FromBody] CreateMarketingPostDto dto);

    [HttpPut("{id}")]
    Task<ApiResponse<MarketingPostResponseDto>> UpdatePost(
        Guid id, 
        [FromBody] UpdateMarketingPostDto dto);

    [HttpDelete("{id}")]
    Task<ApiResponse<bool>> DeletePost(Guid id);

    [HttpPost("{id}/restore")]
    Task<ApiResponse<bool>> RestorePost(Guid id);

    // Publishing Operations
    [HttpPost("{id}/publish")]
    Task<ApiResponse<MarketingPostResponseDto>> PublishPost(Guid id);

    [HttpPost("{id}/schedule")]
    Task<ApiResponse<MarketingPostResponseDto>> SchedulePost(
        Guid id, 
        [FromBody] SchedulePostDto dto);

    [HttpPost("{id}/unpublish")]
    Task<ApiResponse<MarketingPostResponseDto>> UnpublishPost(Guid id);

    // Analytics Operations
    [HttpPost("{id}/analytics/views")]
    [AllowAnonymous] // Public can view
    Task<ApiResponse<bool>> IncrementViews(Guid id);

    [HttpPost("{id}/analytics/clicks")]
    [AllowAnonymous]
    Task<ApiResponse<bool>> IncrementClicks(Guid id);

    [HttpPost("{id}/analytics/shares")]
    [AllowAnonymous]
    Task<ApiResponse<bool>> IncrementShares(Guid id);

    // Utility Operations
    [HttpPost("{id}/duplicate")]
    Task<ApiResponse<MarketingPostResponseDto>> DuplicatePost(Guid id);

    [HttpGet("statistics")]
    Task<ApiResponse<MarketingPostStatisticsDto>> GetStatistics();

    [HttpGet("by-product/{productId}")]
    Task<ApiResponse<List<MarketingPostListDto>>> GetPostsByProduct(Guid productId);

    // Bulk Operations
    [HttpPost("bulk/delete")]
    Task<ApiResponse<BulkOperationResult>> BulkDelete(
        [FromBody] BulkDeleteDto dto);

    [HttpPost("bulk/publish")]
    Task<ApiResponse<BulkOperationResult>> BulkPublish(
        [FromBody] BulkPublishDto dto);

    [HttpPost("bulk/status")]
    Task<ApiResponse<BulkOperationResult>> BulkUpdateStatus(
        [FromBody] BulkUpdateStatusDto dto);
}
```

### 2. Service Layer

#### IMarketingPostService Interface

```csharp
public interface IMarketingPostService
{
    // CRUD
    Task<ApiResponse<PaginatedResult<MarketingPostListDto>>> GetPostsAsync(
        GetMarketingPostsQuery query);
    
    Task<ApiResponse<MarketingPostDetailDto>> GetPostByIdAsync(Guid id);
    
    Task<ApiResponse<MarketingPostResponseDto>> CreatePostAsync(
        CreateMarketingPostDto dto);
    
    Task<ApiResponse<MarketingPostResponseDto>> UpdatePostAsync(
        Guid id, 
        UpdateMarketingPostDto dto);
    
    Task<ApiResponse<bool>> DeletePostAsync(Guid id);
    
    Task<ApiResponse<bool>> RestorePostAsync(Guid id);

    // Publishing
    Task<ApiResponse<MarketingPostResponseDto>> PublishPostAsync(Guid id);
    
    Task<ApiResponse<MarketingPostResponseDto>> SchedulePostAsync(
        Guid id, 
        DateTime scheduledDate);
    
    Task<ApiResponse<MarketingPostResponseDto>> UnpublishPostAsync(Guid id);

    // Analytics
    Task<ApiResponse<bool>> IncrementViewsAsync(Guid id);
    Task<ApiResponse<bool>> IncrementClicksAsync(Guid id);
    Task<ApiResponse<bool>> IncrementSharesAsync(Guid id);

    // Utility
    Task<ApiResponse<MarketingPostResponseDto>> DuplicatePostAsync(Guid id);
    
    Task<ApiResponse<MarketingPostStatisticsDto>> GetStatisticsAsync();
    
    Task<ApiResponse<List<MarketingPostListDto>>> GetPostsByProductAsync(
        Guid productId);

    // Bulk Operations
    Task<ApiResponse<BulkOperationResult>> BulkDeleteAsync(List<Guid> postIds);
    
    Task<ApiResponse<BulkOperationResult>> BulkPublishAsync(List<Guid> postIds);
    
    Task<ApiResponse<BulkOperationResult>> BulkUpdateStatusAsync(
        List<Guid> postIds, 
        MarketingPostStatus status);
}
```

### 3. Repository Layer

#### IMarketingPostRepository Interface

```csharp
public interface IMarketingPostRepository
{
    Task<PaginatedResult<MarketingPost>> GetPostsAsync(
        int pageNumber,
        int pageSize,
        MarketingPostStatus? status = null,
        Guid? productId = null,
        string? searchTerm = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDeleted = false);

    Task<MarketingPost?> GetByIdAsync(Guid id, bool includeDeleted = false);
    
    Task<MarketingPost> CreateAsync(MarketingPost post);
    
    Task<MarketingPost> UpdateAsync(MarketingPost post);
    
    Task<bool> DeleteAsync(Guid id); // Soft delete
    
    Task<bool> RestoreAsync(Guid id);
    
    Task<List<MarketingPost>> GetByProductIdAsync(Guid productId);
    
    Task<MarketingPostStatistics> GetStatisticsAsync();
    
    Task<bool> IncrementViewsAsync(Guid id);
    
    Task<bool> IncrementClicksAsync(Guid id);
    
    Task<bool> IncrementSharesAsync(Guid id);
    
    Task<List<MarketingPost>> GetScheduledPostsDueAsync(DateTime currentTime);
}
```

## Data Models

### Database Entity

```csharp
public class MarketingPost : AuditableEntity, ISoftDelete
{
    // Primary Key
    public Guid Id { get; set; }

    // Basic Content
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(10000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ShortDescription { get; set; }

    // Media
    public string? ImageUrl { get; set; }
    public string? ImageData { get; set; } // Base64 for backward compatibility
    public List<string>? ImageUrls { get; set; } // JSON array

    // Product Relation
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; } // Denormalized
    public virtual Product? Product { get; set; }

    // Content Metadata
    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; } // JSON array

    // Priority and Display Control
    public int PriorityScore { get; set; } = 50; // 1-100, default 50
    public List<string>? DisplayLocation { get; set; } // JSON array
    public bool IsFeatured { get; set; } = false; // Auto-set when PriorityScore > 80

    // SEO
    [MaxLength(200)]
    public string? MetaTitle { get; set; }
    
    [MaxLength(500)]
    public string? MetaDescription { get; set; }
    
    public List<string>? MetaKeywords { get; set; } // JSON array

    // Social Media Variants (JSON)
    public string? FacebookPost { get; set; }
    public string? InstagramPost { get; set; }
    public string? TwitterPost { get; set; }
    public string? LinkedInPost { get; set; }

    // Publishing
    public MarketingPostStatus Status { get; set; } = MarketingPostStatus.Draft;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    // Analytics
    public int Views { get; set; } = 0;
    public int Clicks { get; set; } = 0;
    public int Shares { get; set; } = 0;

    // Audit Fields (from AuditableEntity)
    // public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedAt { get; set; }
    // public Guid CreatedBy { get; set; }
    // public Guid? UpdatedBy { get; set; }

    // Soft Delete (from ISoftDelete)
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Concurrency
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public enum MarketingPostStatus
{
    Draft = 0,
    Published = 1,
    Scheduled = 2
}
```

### DTOs

#### CreateMarketingPostDto

```csharp
public class CreateMarketingPostDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(10000, MinimumLength = 10)]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ShortDescription { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageData { get; set; }
    public List<string>? ImageUrls { get; set; }

    public Guid? ProductId { get; set; }

    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; }

    [Range(1, 100)]
    public int PriorityScore { get; set; } = 50;
    public List<string>? DisplayLocation { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; }

    public SocialMediaPostsDto? SocialPosts { get; set; }

    public MarketingPostStatus Status { get; set; } = MarketingPostStatus.Draft;
    public DateTime? ScheduledDate { get; set; }
}

public class SocialMediaPostsDto
{
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? Twitter { get; set; }
    public string? LinkedIn { get; set; }
}
```

#### UpdateMarketingPostDto

```csharp
public class UpdateMarketingPostDto
{
    [StringLength(200, MinimumLength = 3)]
    public string? Title { get; set; }

    [StringLength(10000, MinimumLength = 10)]
    public string? Content { get; set; }

    public string? ShortDescription { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageData { get; set; }
    public List<string>? ImageUrls { get; set; }

    public Guid? ProductId { get; set; }

    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; }

    [Range(1, 100)]
    public int? PriorityScore { get; set; }
    public List<string>? DisplayLocation { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; }

    public SocialMediaPostsDto? SocialPosts { get; set; }

    public MarketingPostStatus? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }

    public byte[]? RowVersion { get; set; } // For concurrency
}
```

#### MarketingPostResponseDto

```csharp
public class MarketingPostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }

    public string? Image { get; set; } // Primary image
    public List<string>? Images { get; set; }

    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }

    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; }

    public int PriorityScore { get; set; }
    public List<string>? DisplayLocation { get; set; }
    public bool IsFeatured { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; }

    public SocialMediaPostsDto? SocialPosts { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    public int Views { get; set; }
    public int Clicks { get; set; }
    public int Shares { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### MarketingPostListDto (for pagination)

```csharp
public class MarketingPostListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Image { get; set; }

    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }

    public string? Platform { get; set; }
    public List<string>? Hashtags { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }

    public int Views { get; set; }
    public int Clicks { get; set; }
    public int Shares { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

#### MarketingPostDetailDto (full details)

```csharp
public class MarketingPostDetailDto : MarketingPostResponseDto
{
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
```

#### GetMarketingPostsQuery

```csharp
public class GetMarketingPostsQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public MarketingPostStatus? Status { get; set; }
    public Guid? ProductId { get; set; }
    public string? Platform { get; set; }
    public string? SearchTerm { get; set; }
    public string? DisplayLocation { get; set; }
    public bool? IsFeatured { get; set; }
    public int? MinPriorityScore { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public string SortBy { get; set; } = "priority"; // priority, publishedDate, views, clicks, shares, updatedAt
    public string SortOrder { get; set; } = "desc"; // asc, desc

    public bool IncludeDeleted { get; set; } = false;
}
```

#### MarketingPostStatisticsDto

```csharp
public class MarketingPostStatisticsDto
{
    public int Total { get; set; }
    public int Draft { get; set; }
    public int Published { get; set; }
    public int Scheduled { get; set; }

    public int TotalViews { get; set; }
    public int TotalClicks { get; set; }
    public int TotalShares { get; set; }

    public double AverageViews { get; set; }
    public double AverageClicks { get; set; }
    public double AverageShares { get; set; }
}
```

#### BulkOperationResult

```csharp
public class BulkOperationResult
{
    public int TotalRequested { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<BulkOperationError> Errors { get; set; } = new();
}

public class BulkOperationError
{
    public Guid PostId { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
```

### Database Configuration

```csharp
public class MarketingPostConfiguration : IEntityTypeConfiguration<MarketingPost>
{
    public void Configure(EntityTypeBuilder<MarketingPost> builder)
    {
        builder.ToTable("MarketingPosts");
        builder.HasKey(p => p.Id);

        // Indexes
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.ProductId);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.PublishedDate);
        builder.HasIndex(p => p.ScheduledDate);
        builder.HasIndex(p => p.IsDeleted);

        // Composite indexes
        builder.HasIndex(p => new { p.Status, p.IsDeleted });
        builder.HasIndex(p => new { p.ProductId, p.IsDeleted });

        // String lengths
        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Content).HasMaxLength(10000).IsRequired();
        builder.Property(p => p.ShortDescription).HasMaxLength(500);
        builder.Property(p => p.MetaTitle).HasMaxLength(200);
        builder.Property(p => p.MetaDescription).HasMaxLength(500);

        // JSON columns
        builder.Property(p => p.Hashtags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null));

        builder.Property(p => p.MetaKeywords)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null));

        builder.Property(p => p.ImageUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null));

        // Concurrency token
        builder.Property(p => p.RowVersion).IsRowVersion();

        // Default values
        builder.Property(p => p.Views).HasDefaultValue(0);
        builder.Property(p => p.Clicks).HasDefaultValue(0);
        builder.Property(p => p.Shares).HasDefaultValue(0);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.Status).HasDefaultValue(MarketingPostStatus.Draft);

        // Relationships
        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        // Query filters (global filter for soft delete)
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

**Property 1: Post creation initializes required fields**

*For any* valid CreateMarketingPostDto, creating a post should result in a post with createdAt and updatedAt timestamps set to current time, and analytics counters (views, clicks, shares) initialized to zero.

**Validates: Requirements 1.2**

**Property 2: Post updates refresh timestamp**

*For any* existing post and valid UpdateMarketingPostDto, updating the post should result in the updatedAt timestamp being greater than the original updatedAt timestamp.

**Validates: Requirements 1.3**

**Property 3: Product validation on post creation**

*For any* CreateMarketingPostDto with a productId, if the product does not exist in the catalog, the post creation should fail with a validation error.

**Validates: Requirements 1.4, 10.1**

**Property 4: Default status is draft**

*For any* CreateMarketingPostDto without an explicit status field, the created post should have status set to Draft.

**Validates: Requirements 2.1**

**Property 5: Publishing state transition**

*For any* post with status Draft, calling publish should result in status changing to Published and publishedDate being set to current timestamp.

**Validates: Requirements 2.2**

**Property 6: Scheduling state transition**

*For any* post and future datetime, calling schedule should result in status changing to Scheduled and scheduledDate being set to the provided datetime.

**Validates: Requirements 2.3**

**Property 7: Pagination structure completeness**

*For any* GetMarketingPostsQuery with valid pageNumber and pageSize, the response should contain pageNumber, pageSize, totalCount, and totalPages fields.

**Validates: Requirements 3.1**

**Property 8: Status filter correctness**

*For any* GetMarketingPostsQuery with a status filter, all posts in the response should have the specified status.

**Validates: Requirements 3.2**

**Property 9: Search keyword matching**

*For any* GetMarketingPostsQuery with a searchTerm, all posts in the response should contain the searchTerm in either title, content, or productName fields (case-insensitive).

**Validates: Requirements 3.3**

**Property 10: Soft delete behavior**

*For any* post, calling delete should result in IsDeleted being set to true, deletedAt being set to current timestamp, and all other post data remaining unchanged.

**Validates: Requirements 4.1, 4.2**

**Property 11: Soft delete query filter**

*For any* GetMarketingPostsQuery without includeDeleted flag, the response should not contain any posts where IsDeleted is true.

**Validates: Requirements 4.3**

**Property 12: Analytics increment monotonicity**

*For any* post and any sequence of analytics operations (views, clicks, shares), the counter values should never decrease.

**Validates: Requirements 5.1, 5.2, 5.3, 5.4**

**Property 13: Statistics aggregation consistency**

*For any* statistics request, the sum of draft, published, and scheduled counts should equal the total count, and totalViews should equal the sum of views across all non-deleted posts.

**Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5, 6.6**

**Property 14: Post duplication correctness**

*For any* existing post, duplicating it should create a new post where: (1) title equals original title + " (Copy)", (2) status is Draft, (3) analytics counters are zero, (4) all content fields match the original, and (5) id and timestamps are new.

**Validates: Requirements 7.1, 7.2, 7.3, 7.4, 7.5**

**Property 15: Authorization enforcement**

*For any* protected endpoint, requests without a valid JWT token containing admin role should be rejected with 401 or 403 status.

**Validates: Requirements 11.1, 11.2, 11.3**

**Property 16: Title validation boundaries**

*For any* CreateMarketingPostDto or UpdateMarketingPostDto, if title length is less than 3 or greater than 200 characters, the request should be rejected with a validation error.

**Validates: Requirements 12.1**

**Property 17: Content validation minimum length**

*For any* CreateMarketingPostDto, if content length is less than 10 characters, the request should be rejected with a validation error.

**Validates: Requirements 12.2**

**Property 18: Hashtag format validation**

*For any* CreateMarketingPostDto or UpdateMarketingPostDto with hashtags, if any hashtag contains characters other than alphanumeric and underscores, the request should be rejected with a validation error.

**Validates: Requirements 12.3**

**Property 19: Scheduled date future validation**

*For any* SchedulePostDto with a scheduledDate, if the date is not in the future, the request should be rejected with a validation error.

**Validates: Requirements 12.4**

**Property 20: Priority score validation boundaries**

*For any* CreateMarketingPostDto or UpdateMarketingPostDto with priorityScore, if the value is less than 1 or greater than 100, the request should be rejected with a validation error.

**Validates: Requirements 1.4**

**Property 21: Default priority score**

*For any* CreateMarketingPostDto without an explicit priorityScore field, the created post should have priorityScore set to 50.

**Validates: Requirements 1.3**

**Property 22: Featured flag auto-set**

*For any* post, if priorityScore is greater than 80, the isFeatured flag should be automatically set to true; otherwise it should be false.

**Validates: Requirements 14.4**

**Property 23: Priority-based sorting**

*For any* GetMarketingPostsQuery with default sorting, posts should be ordered by priorityScore descending, then by publishedDate descending.

**Validates: Requirements 3.6, 14.1, 14.6**

## Error Handling

### Error Response Format

All errors follow the standard ApiResponse format:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }
}
```

### Error Categories

#### 1. Validation Errors (400 Bad Request)

```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": [
    "Title must be between 3 and 200 characters",
    "Content must be at least 10 characters"
  ],
  "statusCode": 400
}
```

#### 2. Not Found Errors (404 Not Found)

```json
{
  "success": false,
  "data": null,
  "message": "Marketing post not found",
  "errors": ["Marketing post with ID 'abc-123' does not exist"],
  "statusCode": 404
}
```

#### 3. Authorization Errors (401/403)

```json
{
  "success": false,
  "data": null,
  "message": "Unauthorized",
  "errors": ["Admin role required"],
  "statusCode": 403
}
```

#### 4. Conflict Errors (409 Conflict)

```json
{
  "success": false,
  "data": null,
  "message": "Conflict",
  "errors": ["Post was modified by another user. Please refresh and try again."],
  "statusCode": 409
}
```

#### 5. Server Errors (500 Internal Server Error)

```json
{
  "success": false,
  "data": null,
  "message": "Internal server error",
  "errors": ["An unexpected error occurred. Please try again later."],
  "statusCode": 500
}
```

## Testing Strategy

### Unit Testing

**Framework:** xUnit + Moq + FluentAssertions

**Test Coverage:**
- Service layer business logic
- Validation rules
- DTO mapping
- Repository queries
- Authorization logic

### Property-Based Testing

**Framework:** FsCheck (for C#)

**Configuration:** Minimum 100 iterations per property test

**Property Tests to Implement:**
- Property 1: Post creation initializes required fields
- Property 2: Post updates refresh timestamp
- Property 4: Default status is draft
- Property 5: Publishing state transition
- Property 8: Status filter correctness
- Property 10: Soft delete behavior
- Property 12: Analytics increment monotonicity
- Property 13: Statistics aggregation consistency
- Property 14: Post duplication correctness
- Property 16: Title validation boundaries
- Property 17: Content validation minimum length
- Property 18: Hashtag format validation
- Property 19: Scheduled date future validation

### Integration Testing

**Framework:** WebApplicationFactory + TestContainers

**Test Coverage:**
- Full API endpoint testing
- Database integration
- Authentication/Authorization
- Pagination
- Filtering and search

## Performance Considerations

### Database Optimization

1. **Indexes:** Create indexes on frequently queried columns (Status, ProductId, CreatedAt, IsDeleted)
2. **Pagination:** Use efficient pagination with OFFSET/FETCH
3. **Query Filters:** Use global query filters for soft delete
4. **Denormalization:** Store ProductName to avoid JOIN on every query

### Caching Strategy

Consider implementing caching for:
- Frequently accessed posts
- Statistics data
- Product information

### Analytics Optimization

For high-traffic scenarios, consider:
- Queue-based analytics updates
- Batch processing
- Eventual consistency

## Security Considerations

1. **JWT Validation:** Validate token signature, expiration, and claims
2. **Role-Based Access:** Enforce admin role on all endpoints
3. **Input Sanitization:** Sanitize HTML content to prevent XSS
4. **SQL Injection Prevention:** Use parameterized queries (EF Core handles this)
5. **Rate Limiting:** Implement rate limiting on analytics endpoints

## Deployment Considerations

1. **Database Migrations:** Use EF Core migrations for schema changes
2. **Background Jobs:** Set up scheduled job for auto-publishing scheduled posts
3. **Monitoring:** Log all operations and set up alerts for errors
4. **Backup Strategy:** Regular database backups
5. **Scaling:** Consider read replicas for high read traffic
