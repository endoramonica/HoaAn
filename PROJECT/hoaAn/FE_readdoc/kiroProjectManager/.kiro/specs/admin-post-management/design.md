# Design Document - Admin Post Management System

## Overview

Admin Post Management System cung cấp RESTful API để quản lý bài viết marketing. Hệ thống được thiết kế theo Clean Architecture với các layer rõ ràng: Controller → Service → Repository → Database. Hệ thống sử dụng JWT authentication, soft delete pattern, và optimistic concurrency control cho analytics.

## Architecture

### Layer Architecture

```
┌─────────────────────────────────────┐
│   API Layer (Controllers)           │
│   - AdminPostController              │
│   - Request/Response DTOs            │
│   - Authorization Filters            │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Service Layer                      │
│   - IPostService                     │
│   - PostService Implementation       │
│   - Business Logic                   │
│   - Validation                       │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Repository Layer                   │
│   - IPostRepository                  │
│   - Entity Framework Core            │
│   - Query Optimization               │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Database Layer                     │
│   - Post Entity                      │
│   - Indexes                          │
│   - Relationships                    │
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

#### AdminPostController

```csharp
[ApiController]
[Route("api/admin/posts")]
[Authorize(Roles = "Admin")]
public class AdminPostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ICurrentUser _currentUser;
    
    // CRUD Operations
    [HttpGet]
    Task<ApiResponse<PaginatedResult<PostListDto>>> GetPosts([FromQuery] GetPostsQuery query);
    
    [HttpGet("{id}")]
    Task<ApiResponse<PostDetailDto>> GetPostById(Guid id);
    
    [HttpPost]
    Task<ApiResponse<PostResponseDto>> CreatePost([FromBody] CreatePostDto dto);
    
    [HttpPut("{id}")]
    Task<ApiResponse<PostResponseDto>> UpdatePost(Guid id, [FromBody] UpdatePostDto dto);
    
    [HttpDelete("{id}")]
    Task<ApiResponse<bool>> DeletePost(Guid id);
    
    // Publishing Operations
    [HttpPost("{id}/publish")]
    Task<ApiResponse<PostResponseDto>> PublishPost(Guid id);
    
    [HttpPost("{id}/schedule")]
    Task<ApiResponse<PostResponseDto>> SchedulePost(Guid id, [FromBody] SchedulePostDto dto);
    
    [HttpPost("{id}/unpublish")]
    Task<ApiResponse<PostResponseDto>> UnpublishPost(Guid id);
    
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
    Task<ApiResponse<PostResponseDto>> DuplicatePost(Guid id);
    
    [HttpGet("statistics")]
    Task<ApiResponse<PostStatisticsDto>> GetStatistics();
    
    [HttpGet("by-product/{productId}")]
    Task<ApiResponse<List<PostListDto>>> GetPostsByProduct(Guid productId);
    
    // Bulk Operations
    [HttpPost("bulk/delete")]
    Task<ApiResponse<BulkOperationResult>> BulkDelete([FromBody] BulkDeleteDto dto);
    
    [HttpPost("bulk/publish")]
    Task<ApiResponse<BulkOperationResult>> BulkPublish([FromBody] BulkPublishDto dto);
    
    [HttpPost("bulk/status")]
    Task<ApiResponse<BulkOperationResult>> BulkUpdateStatus([FromBody] BulkUpdateStatusDto dto);
}
```

### 2. Service Layer

#### IPostService Interface

```csharp
public interface IPostService
{
    // CRUD
    Task<ApiResponse<PaginatedResult<PostListDto>>> GetPostsAsync(GetPostsQuery query);
    Task<ApiResponse<PostDetailDto>> GetPostByIdAsync(Guid id);
    Task<ApiResponse<PostResponseDto>> CreatePostAsync(CreatePostDto dto);
    Task<ApiResponse<PostResponseDto>> UpdatePostAsync(Guid id, UpdatePostDto dto);
    Task<ApiResponse<bool>> DeletePostAsync(Guid id);
    Task<ApiResponse<bool>> RestorePostAsync(Guid id);
    
    // Publishing
    Task<ApiResponse<PostResponseDto>> PublishPostAsync(Guid id);
    Task<ApiResponse<PostResponseDto>> SchedulePostAsync(Guid id, DateTime scheduledDate);
    Task<ApiResponse<PostResponseDto>> UnpublishPostAsync(Guid id);
    
    // Analytics
    Task<ApiResponse<bool>> IncrementViewsAsync(Guid id);
    Task<ApiResponse<bool>> IncrementClicksAsync(Guid id);
    Task<ApiResponse<bool>> IncrementSharesAsync(Guid id);
    
    // Utility
    Task<ApiResponse<PostResponseDto>> DuplicatePostAsync(Guid id);
    Task<ApiResponse<PostStatisticsDto>> GetStatisticsAsync();
    Task<ApiResponse<List<PostListDto>>> GetPostsByProductAsync(Guid productId);
    
    // Bulk Operations
    Task<ApiResponse<BulkOperationResult>> BulkDeleteAsync(List<Guid> postIds);
    Task<ApiResponse<BulkOperationResult>> BulkPublishAsync(List<Guid> postIds);
    Task<ApiResponse<BulkOperationResult>> BulkUpdateStatusAsync(List<Guid> postIds, PostStatus status);
}
```

### 3. Repository Layer

#### IPostRepository Interface

```csharp
public interface IPostRepository
{
    Task<PaginatedResult<Post>> GetPostsAsync(
        int pageNumber,
        int pageSize,
        PostStatus? status = null,
        Guid? productId = null,
        string? searchTerm = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDeleted = false
    );
    
    Task<Post?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<Post> CreateAsync(Post post);
    Task<Post> UpdateAsync(Post post);
    Task<bool> DeleteAsync(Guid id); // Soft delete
    Task<bool> RestoreAsync(Guid id);
    Task<List<Post>> GetByProductIdAsync(Guid productId);
    Task<PostStatistics> GetStatisticsAsync();
    Task<bool> IncrementViewsAsync(Guid id);
    Task<bool> IncrementClicksAsync(Guid id);
    Task<bool> IncrementSharesAsync(Guid id);
    Task<List<Post>> GetScheduledPostsDueAsync(DateTime currentTime);
}
```

## Data Models

### Database Entity

```csharp
public class Post
{
    // Primary Key
    public Guid Id { get; set; }
    
    // Basic Content
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    
    // Media
    public string? ImageUrl { get; set; }
    public string? ImageData { get; set; } // Base64 for backward compatibility
    public List<string>? ImageUrls { get; set; } // JSON array
    
    // Product Relation
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; } // Denormalized
    
    // Content Metadata
    public string? Topic { get; set; }
    public string? Platform { get; set; }
    public string? Tone { get; set; }
    public List<string>? Hashtags { get; set; } // JSON array
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; } // JSON array
    
    // Social Media Variants (JSON)
    public string? FacebookPost { get; set; }
    public string? InstagramPost { get; set; }
    public string? TwitterPost { get; set; }
    public string? LinkedInPost { get; set; }
    
    // Publishing
    public PostStatus Status { get; set; } // Enum: Draft, Published, Scheduled
    public DateTime? ScheduledDate { get; set; }
    public DateTime? PublishedDate { get; set; }
    
    // Analytics
    public int Views { get; set; }
    public int Clicks { get; set; }
    public int Shares { get; set; }
    
    // Audit Fields
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Concurrency
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public enum PostStatus
{
    Draft = 0,
    Published = 1,
    Scheduled = 2
}
```

### DTOs

#### CreatePostDto

```csharp
public class CreatePostDto
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
    
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; }
    
    public SocialMediaPostsDto? SocialPosts { get; set; }
    
    public PostStatus Status { get; set; } = PostStatus.Draft;
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

#### UpdatePostDto

```csharp
public class UpdatePostDto
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
    
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<string>? MetaKeywords { get; set; }
    
    public SocialMediaPostsDto? SocialPosts { get; set; }
    
    public PostStatus? Status { get; set; }
    public DateTime? ScheduledDate { get; set; }
}
```

#### PostResponseDto

```csharp
public class PostResponseDto
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

#### PostListDto (for pagination)

```csharp
public class PostListDto
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

#### PostDetailDto (full details)

```csharp
public class PostDetailDto : PostResponseDto
{
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
```

#### GetPostsQuery

```csharp
public class GetPostsQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    
    public PostStatus? Status { get; set; }
    public Guid? ProductId { get; set; }
    public string? Platform { get; set; }
    public string? SearchTerm { get; set; }
    
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    
    public bool IncludeDeleted { get; set; } = false;
}
```

#### PostStatisticsDto

```csharp
public class PostStatisticsDto
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
public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");
        
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
        
        // Full-text search index (SQL Server)
        // CREATE FULLTEXT INDEX ON Posts(Title, Content) KEY INDEX PK_Posts;
        
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
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
            );
            
        builder.Property(p => p.MetaKeywords)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
            );
            
        builder.Property(p => p.ImageUrls)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
            );
        
        // Concurrency token
        builder.Property(p => p.RowVersion).IsRowVersion();
        
        // Default values
        builder.Property(p => p.Views).HasDefaultValue(0);
        builder.Property(p => p.Clicks).HasDefaultValue(0);
        builder.Property(p => p.Shares).HasDefaultValue(0);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.Status).HasDefaultValue(PostStatus.Draft);
        
        // Audit fields
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        
        // Query filters (global filter for soft delete)
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
```

## Co
rrectness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Acceptance Criteria Testing Prework

#### 1.1 WHEN an admin creates a new post THEN the Admin Post System SHALL accept title, content, shortDescription, image, hashtags, productId, platform, tone, and SEO metadata
**Thoughts:** This is about validating that the create endpoint accepts all required fields. We can test this by generating random valid post data with all fields populated and ensuring the API accepts it without errors.
**Testable:** yes - property

#### 1.2 WHEN an admin creates a post THEN the Admin Post System SHALL automatically set createdAt, updatedAt timestamps and initialize analytics counters to zero
**Thoughts:** This is an invariant that should hold for all newly created posts. We can generate random post data, create it, then verify the timestamps are set and counters are zero.
**Testable:** yes - property

#### 1.3 WHEN an admin updates a post THEN the Admin Post System SHALL update the specified fields and refresh the updatedAt timestamp
**Thoughts:** This tests that updates work correctly and timestamps are maintained. We can create a post, update it with random data, and verify the updatedAt changed.
**Testable:** yes - property

#### 1.4 WHEN an admin provides a productId THEN the Admin Post System SHALL validate the product exists and store the product relationship
**Thoughts:** This is testing validation logic. We can test with both valid and invalid product IDs to ensure proper validation.
**Testable:** yes - property

#### 2.1 WHEN an admin creates a post without specifying status THEN the Admin Post System SHALL set the status to draft
**Thoughts:** This is a default value test. For any post created without status, it should default to draft.
**Testable:** yes - property

#### 2.2 WHEN an admin publishes a draft post THEN the Admin Post System SHALL change status to published and set publishedDate to current timestamp
**Thoughts:** This is a state transition property. For any draft post, publishing should change status and set the date.
**Testable:** yes - property

#### 2.3 WHEN an admin schedules a post with a future date THEN the Admin Post System SHALL set status to scheduled and store the scheduledDate
**Thoughts:** This tests scheduling logic. For any valid future date, the post should be scheduled correctly.
**Testable:** yes - property

#### 3.1 WHEN an admin requests posts with pagination THEN the Admin Post System SHALL return posts with pageNumber, pageSize, totalCount, and totalPages
**Thoughts:** This tests pagination structure. For any pagination request, the response should contain all required pagination metadata.
**Testable:** yes - property

#### 3.2 WHEN an admin filters by status THEN the Admin Post System SHALL return only posts matching the specified status
**Thoughts:** This is a filtering property. For any status filter, all returned posts should have that status.
**Testable:** yes - property

#### 3.3 WHEN an admin searches by keyword THEN the Admin Post System SHALL search in title, content, and productName fields
**Thoughts:** This tests search functionality. For any keyword, if a post is returned, it should contain that keyword in one of the searchable fields.
**Testable:** yes - property

#### 4.1 WHEN an admin deletes a post THEN the Admin Post System SHALL perform a soft delete by marking the post as deleted
**Thoughts:** This tests soft delete behavior. For any post, deleting it should set IsDeleted=true and preserve data.
**Testable:** yes - property

#### 4.2 WHEN an admin deletes a post THEN the Admin Post System SHALL set deletedAt timestamp and preserve all post data
**Thoughts:** This is part of soft delete verification. Deleting should set the timestamp and keep all data intact.
**Testable:** yes - property

#### 4.3 WHEN an admin requests posts THEN the Admin Post System SHALL exclude soft-deleted posts from results by default
**Thoughts:** This tests the query filter. For any query without includeDeleted flag, no deleted posts should appear.
**Testable:** yes - property

#### 5.1 WHEN a post view is recorded THEN the Admin Post System SHALL increment the views counter by one
**Thoughts:** This tests analytics increment. For any post, recording a view should increase the counter by exactly 1.
**Testable:** yes - property

#### 5.2 WHEN a post click is recorded THEN the Admin Post System SHALL increment the clicks counter by one
**Thoughts:** Similar to views, this tests click tracking.
**Testable:** yes - property

#### 5.3 WHEN a post share is recorded THEN the Admin Post System SHALL increment the shares counter by one
**Thoughts:** Similar to views and clicks, this tests share tracking.
**Testable:** yes - property

#### 5.4 WHEN analytics are updated THEN the Admin Post System SHALL ensure counters never decrease
**Thoughts:** This is a monotonicity property. For any sequence of analytics updates, counters should only increase or stay the same.
**Testable:** yes - property

#### 6.1 WHEN an admin requests statistics THEN the Admin Post System SHALL return total count of all posts
**Thoughts:** This tests statistics calculation. The total should equal the count of all non-deleted posts.
**Testable:** yes - property

#### 6.2 WHEN an admin requests statistics THEN the Admin Post System SHALL return counts grouped by status
**Thoughts:** This tests status grouping. The sum of status counts should equal the total.
**Testable:** yes - property

#### 6.3 WHEN an admin requests statistics THEN the Admin Post System SHALL return sum of all views across all posts
**Thoughts:** This tests aggregation. The total views should equal the sum of individual post views.
**Testable:** yes - property

#### 7.1 WHEN an admin duplicates a post THEN the Admin Post System SHALL create a new post with all content copied from the original
**Thoughts:** This tests duplication logic. The duplicated post should have the same content fields as the original.
**Testable:** yes - property

#### 7.2 WHEN duplicating a post THEN the Admin Post System SHALL append "(Copy)" to the title
**Thoughts:** This is a specific transformation. For any duplicated post, the title should end with " (Copy)".
**Testable:** yes - property

#### 7.3 WHEN duplicating a post THEN the Admin Post System SHALL set the new post status to draft
**Thoughts:** This tests default status on duplication. All duplicated posts should be drafts.
**Testable:** yes - property

#### 7.4 WHEN duplicating a post THEN the Admin Post System SHALL reset analytics counters to zero
**Thoughts:** This tests analytics reset. All duplicated posts should have zero views, clicks, and shares.
**Testable:** yes - property

#### 11.1 WHEN an admin performs any operation THEN the Admin Post System SHALL verify the user has admin role from JWT token
**Thoughts:** This is an authorization property. For any protected endpoint, non-admin users should be rejected.
**Testable:** yes - property

#### 12.1 WHEN an admin creates a post THEN the Admin Post System SHALL require title with minimum 3 characters and maximum 200 characters
**Thoughts:** This tests validation boundaries. Titles outside this range should be rejected.
**Testable:** yes - property

#### 12.2 WHEN an admin creates a post THEN the Admin Post System SHALL require content with minimum 10 characters
**Thoughts:** This tests content validation. Content shorter than 10 characters should be rejected.
**Testable:** yes - property

#### 12.3 WHEN an admin provides hashtags THEN the Admin Post System SHALL validate each hashtag contains only alphanumeric characters and underscores
**Thoughts:** This tests hashtag validation. Invalid hashtags should be rejected.
**Testable:** yes - property

#### 12.4 WHEN an admin provides a scheduledDate THEN the Admin Post System SHALL validate the date is in the future
**Thoughts:** This tests date validation. Past dates should be rejected for scheduling.
**Testable:** yes - property

### Property Reflection

After reviewing all testable properties, I identify the following consolidations:

1. **Analytics properties (5.1, 5.2, 5.3)** can be combined into a single property: "Analytics increment operations"
2. **Statistics properties (6.1, 6.2, 6.3)** can be combined into: "Statistics aggregation consistency"
3. **Duplication properties (7.1, 7.2, 7.3, 7.4)** can be combined into: "Post duplication correctness"
4. **Soft delete properties (4.1, 4.2, 4.3)** can be combined into: "Soft delete behavior"

### Correctness Properties

**Property 1: Post creation initializes required fields**
*For any* valid CreatePostDto, creating a post should result in a post with createdAt and updatedAt timestamps set to current time, and analytics counters (views, clicks, shares) initialized to zero.
**Validates: Requirements 1.2**

**Property 2: Post updates refresh timestamp**
*For any* existing post and valid UpdatePostDto, updating the post should result in the updatedAt timestamp being greater than the original updatedAt timestamp.
**Validates: Requirements 1.3**

**Property 3: Product validation on post creation**
*For any* CreatePostDto with a productId, if the product does not exist in the catalog, the post creation should fail with a validation error.
**Validates: Requirements 1.4, 10.1**

**Property 4: Default status is draft**
*For any* CreatePostDto without an explicit status field, the created post should have status set to Draft.
**Validates: Requirements 2.1**

**Property 5: Publishing state transition**
*For any* post with status Draft, calling publish should result in status changing to Published and publishedDate being set to current timestamp.
**Validates: Requirements 2.2**

**Property 6: Scheduling state transition**
*For any* post and future datetime, calling schedule should result in status changing to Scheduled and scheduledDate being set to the provided datetime.
**Validates: Requirements 2.3**

**Property 7: Pagination structure completeness**
*For any* GetPostsQuery with valid pageNumber and pageSize, the response should contain pageNumber, pageSize, totalCount, and totalPages fields.
**Validates: Requirements 3.1**

**Property 8: Status filter correctness**
*For any* GetPostsQuery with a status filter, all posts in the response should have the specified status.
**Validates: Requirements 3.2**

**Property 9: Search keyword matching**
*For any* GetPostsQuery with a searchTerm, all posts in the response should contain the searchTerm in either title, content, or productName fields (case-insensitive).
**Validates: Requirements 3.3**

**Property 10: Soft delete behavior**
*For any* post, calling delete should result in IsDeleted being set to true, deletedAt being set to current timestamp, and all other post data remaining unchanged.
**Validates: Requirements 4.1, 4.2**

**Property 11: Soft delete query filter**
*For any* GetPostsQuery without includeDeleted flag, the response should not contain any posts where IsDeleted is true.
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
*For any* CreatePostDto or UpdatePostDto, if title length is less than 3 or greater than 200 characters, the request should be rejected with a validation error.
**Validates: Requirements 12.1**

**Property 17: Content validation minimum length**
*For any* CreatePostDto, if content length is less than 10 characters, the request should be rejected with a validation error.
**Validates: Requirements 12.2**

**Property 18: Hashtag format validation**
*For any* CreatePostDto or UpdatePostDto with hashtags, if any hashtag contains characters other than alphanumeric and underscores, the request should be rejected with a validation error.
**Validates: Requirements 12.3**

**Property 19: Scheduled date future validation**
*For any* SchedulePostDto with a scheduledDate, if the date is not in the future, the request should be rejected with a validation error.
**Validates: Requirements 12.4**

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

```csharp
// Example: Invalid title length
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

```csharp
// Example: Post not found
{
    "success": false,
    "data": null,
    "message": "Post not found",
    "errors": ["Post with ID 'abc-123' does not exist"],
    "statusCode": 404
}
```

#### 3. Authorization Errors (401/403)

```csharp
// Example: Unauthorized
{
    "success": false,
    "data": null,
    "message": "Unauthorized",
    "errors": ["Admin role required"],
    "statusCode": 403
}
```

#### 4. Conflict Errors (409 Conflict)

```csharp
// Example: Concurrency conflict
{
    "success": false,
    "data": null,
    "message": "Conflict",
    "errors": ["Post was modified by another user. Please refresh and try again."],
    "statusCode": 409
}
```

#### 5. Server Errors (500 Internal Server Error)

```csharp
// Example: Database error
{
    "success": false,
    "data": null,
    "message": "Internal server error",
    "errors": ["An unexpected error occurred. Please try again later."],
    "statusCode": 500
}
```

### Exception Handling Strategy

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var response = exception switch
        {
            ValidationException ex => new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = ex.Errors.Select(e => e.ErrorMessage).ToList(),
                StatusCode = 400
            },
            
            NotFoundException ex => new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
                StatusCode = 404
            },
            
            UnauthorizedAccessException ex => new ApiResponse<object>
            {
                Success = false,
                Message = "Unauthorized",
                Errors = new List<string> { ex.Message },
                StatusCode = 403
            },
            
            DbUpdateConcurrencyException ex => new ApiResponse<object>
            {
                Success = false,
                Message = "Conflict",
                Errors = new List<string> { "Resource was modified by another user" },
                StatusCode = 409
            },
            
            _ => new ApiResponse<object>
            {
                Success = false,
                Message = "Internal server error",
                Errors = new List<string> { "An unexpected error occurred" },
                StatusCode = 500
            }
        };
        
        httpContext.Response.StatusCode = response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        
        return true;
    }
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

**Example Unit Test:**

```csharp
public class PostServiceTests
{
    [Fact]
    public async Task CreatePost_WithValidData_ShouldInitializeAnalyticsToZero()
    {
        // Arrange
        var mockRepo = new Mock<IPostRepository>();
        var mockCurrentUser = new Mock<ICurrentUser>();
        var service = new PostService(mockRepo.Object, mockCurrentUser.Object);
        
        var dto = new CreatePostDto
        {
            Title = "Test Post",
            Content = "Test content with enough characters"
        };
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Post>()))
            .ReturnsAsync((Post p) => p);
        
        // Act
        var result = await service.CreatePostAsync(dto);
        
        // Assert
        result.Data.Should().NotBeNull();
        result.Data!.Views.Should().Be(0);
        result.Data.Clicks.Should().Be(0);
        result.Data.Shares.Should().Be(0);
    }
    
    [Theory]
    [InlineData("AB")] // Too short
    [InlineData("")] // Empty
    public async Task CreatePost_WithInvalidTitle_ShouldReturnValidationError(string title)
    {
        // Arrange
        var service = CreateService();
        var dto = new CreatePostDto { Title = title, Content = "Valid content" };
        
        // Act
        var result = await service.CreatePostAsync(dto);
        
        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Title"));
    }
}
```

### Property-Based Testing

**Framework:** FsCheck (for C#) or Hedgehog

**Configuration:** Minimum 100 iterations per property test

**Example Property Test:**

```csharp
[Property(MaxTest = 100)]
public Property CreatePost_ShouldAlwaysInitializeTimestamps()
{
    return Prop.ForAll(
        Arb.Generate<CreatePostDto>().Where(dto => 
            dto.Title.Length >= 3 && 
            dto.Title.Length <= 200 &&
            dto.Content.Length >= 10),
        async dto =>
        {
            // Arrange
            var service = CreateService();
            var beforeCreate = DateTime.UtcNow;
            
            // Act
            var result = await service.CreatePostAsync(dto);
            var afterCreate = DateTime.UtcNow;
            
            // Assert
            return result.Success &&
                   result.Data.CreatedAt >= beforeCreate &&
                   result.Data.CreatedAt <= afterCreate &&
                   result.Data.UpdatedAt >= beforeCreate &&
                   result.Data.UpdatedAt <= afterCreate;
        });
}

[Property(MaxTest = 100)]
public Property Analytics_ShouldNeverDecrease()
{
    return Prop.ForAll(
        Arb.Generate<Guid>(),
        Arb.Generate<List<AnalyticsOperation>>(),
        async (postId, operations) =>
        {
            // Arrange
            var service = CreateService();
            await CreateTestPost(postId);
            
            var previousViews = 0;
            var previousClicks = 0;
            var previousShares = 0;
            
            // Act & Assert
            foreach (var op in operations)
            {
                switch (op)
                {
                    case AnalyticsOperation.View:
                        await service.IncrementViewsAsync(postId);
                        break;
                    case AnalyticsOperation.Click:
                        await service.IncrementClicksAsync(postId);
                        break;
                    case AnalyticsOperation.Share:
                        await service.IncrementSharesAsync(postId);
                        break;
                }
                
                var post = await service.GetPostByIdAsync(postId);
                
                if (post.Data.Views < previousViews ||
                    post.Data.Clicks < previousClicks ||
                    post.Data.Shares < previousShares)
                {
                    return false;
                }
                
                previousViews = post.Data.Views;
                previousClicks = post.Data.Clicks;
                previousShares = post.Data.Shares;
            }
            
            return true;
        });
}
```

### Integration Testing

**Framework:** WebApplicationFactory + TestContainers

**Test Coverage:**
- Full API endpoint testing
- Database integration
- Authentication/Authorization
- Pagination
- Filtering and search

**Example Integration Test:**

```csharp
public class PostsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    [Fact]
    public async Task GetPosts_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await SeedTestPosts(50);
        
        // Act
        var response = await _client.GetAsync("/api/admin/posts?pageNumber=2&pageSize=10");
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedResult<PostListDto>>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.PageNumber.Should().Be(2);
        result.Data.PageSize.Should().Be(10);
        result.Data.Items.Count.Should().Be(10);
        result.Data.TotalCount.Should().Be(50);
        result.Data.TotalPages.Should().Be(5);
    }
}
```

## Performance Considerations

### Database Optimization

1. **Indexes:** Create indexes on frequently queried columns (Status, ProductId, CreatedAt, IsDeleted)
2. **Pagination:** Use efficient pagination with OFFSET/FETCH or keyset pagination for large datasets
3. **Query Filters:** Use global query filters for soft delete to avoid repetitive WHERE clauses
4. **Denormalization:** Store ProductName to avoid JOIN on every query

### Caching Strategy

```csharp
public class CachedPostService : IPostService
{
    private readonly IPostService _innerService;
    private readonly IDistributedCache _cache;
    
    public async Task<ApiResponse<PostDetailDto>> GetPostByIdAsync(Guid id)
    {
        var cacheKey = $"post:{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
        {
            return JsonSerializer.Deserialize<ApiResponse<PostDetailDto>>(cached);
        }
        
        var result = await _innerService.GetPostByIdAsync(id);
        
        if (result.Success)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }
            );
        }
        
        return result;
    }
}
```

### Analytics Optimization

For high-traffic scenarios, use a queue-based approach:

```csharp
public class QueuedAnalyticsService
{
    private readonly IBackgroundTaskQueue _queue;
    
    public async Task IncrementViewsAsync(Guid postId)
    {
        await _queue.QueueBackgroundWorkItemAsync(async token =>
        {
            await _repository.IncrementViewsAsync(postId);
        });
    }
}
```

## Security Considerations

1. **JWT Validation:** Validate token signature, expiration, and claims
2. **Role-Based Access:** Enforce admin role on all endpoints
3. **Input Sanitization:** Sanitize HTML content to prevent XSS
4. **SQL Injection Prevention:** Use parameterized queries (EF Core handles this)
5. **Rate Limiting:** Implement rate limiting on analytics endpoints
6. **CORS:** Configure CORS appropriately for frontend domain

## Deployment Considerations

1. **Database Migrations:** Use EF Core migrations for schema changes
2. **Background Jobs:** Set up scheduled job for auto-publishing scheduled posts
3. **Monitoring:** Log all operations and set up alerts for errors
4. **Backup Strategy:** Regular database backups
5. **Scaling:** Consider read replicas for high read traffic

## API Documentation

Generate OpenAPI/Swagger documentation with:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Admin Post Management API",
        Version = "v1",
        Description = "API for managing marketing posts"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```
