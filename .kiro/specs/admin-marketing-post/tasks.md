# Implementation Plan - Admin Marketing Post Management System

## Overview

This implementation plan breaks down the Admin Marketing Post Management System into discrete, manageable tasks. Each task builds incrementally on previous steps, ensuring a working system at each checkpoint.

---

## Tasks

- [x] 1. Create MarketingPost Entity and Database Configuration








  - Create `MarketingPost` entity in `VietCommerce.Core/Entities/Marketing/MarketingPost.cs`
  - Create `MarketingPostStatus` enum
  - Create `MarketingPostConfiguration` for EF Core
  - Add relationship to Product entity
  - Create database migration
  - _Requirements: 1.1, 1.2, 2.1, 4.1, 4.2, 5.1, 5.2, 5.3, 10.1, 10.2_

- [x] 2. Create DTOs and Query Models





  - Create `CreateMarketingPostDto` with validation attributes
  - Create `UpdateMarketingPostDto`
  - Create `MarketingPostResponseDto`
  - Create `MarketingPostListDto`
  - Create `MarketingPostDetailDto`
  - Create `GetMarketingPostsQuery`
  - Create `MarketingPostStatisticsDto`
  - Create `SocialMediaPostsDto`
  - Create `BulkOperationResult` and `BulkOperationError`
  - Create `SchedulePostDto`
  - _Requirements: 1.1, 3.1, 6.1, 6.2, 6.3, 8.1, 9.1, 13.4_

- [x] 3. Create Repository Interface and Implementation





  - Create `IMarketingPostRepository` interface in `VietCommerce.Data/Repositories/Interfaces`
  - Implement `MarketingPostRepository` extending `GenericRepository<MarketingPost>`
  - Implement `GetPostsAsync` with filtering, pagination, and search
  - Implement `GetByIdAsync` with includeDeleted parameter
  - Implement `CreateAsync`
  - Implement `UpdateAsync`
  - Implement `DeleteAsync` (soft delete)
  - Implement `RestoreAsync`
  - Implement `GetByProductIdAsync`
  - Implement `GetStatisticsAsync`
  - Implement `IncrementViewsAsync`, `IncrementClicksAsync`, `IncrementSharesAsync`
  - Implement `GetScheduledPostsDueAsync`
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 4.1, 4.3, 4.5, 5.1, 5.2, 5.3, 6.1, 6.2, 6.3, 10.3_

- [x] 4. Add Repository to UnitOfWork





  - Add `IMarketingPostRepository MarketingPosts { get; }` property to `IUnitOfWork`
  - Initialize repository in `UnitOfWork` constructor
  - _Requirements: All_

- [x] 5. Create Service Interface and Base Implementation





  - Create `IMarketingPostService` interface in `VietCommerce.Application/Services/Services/Interfaces`
  - Create `MarketingPostService` class extending `BaseService`
  - Inject dependencies: `IUnitOfWork`, `IMapper`, `ICurrentUser`, `ILogger`, `ICacheService`
  - Implement helper methods: `GetCurrentAdminId()`, `ValidateProductExists()`
  - _Requirements: 11.1, 11.4, 11.5_

- [x] 6. Implement CRUD Operations in Service





- [x] 6.1 Implement CreatePostAsync


  - Validate admin authentication
  - Validate input data (title, content, hashtags)
  - Validate product exists if productId provided
  - Set default values (status = Draft, analytics = 0, timestamps)
  - Apply SEO metadata defaults (metaTitle from title, metaDescription from shortDescription)
  - Save to database
  - Map to response DTO
  - _Requirements: 1.1, 1.2, 1.4, 2.1, 9.2, 9.3, 10.1, 11.1, 11.4, 12.1, 12.2, 12.3_

- [ ]* 6.2 Write property test for post creation initialization
  - **Property 1: Post creation initializes required fields**
  - **Validates: Requirements 1.2**

- [ ]* 6.2.1 Write property test for create response excludes TaggedProduct
  - **Property 28: Create post response excludes TaggedProduct**
  - **Validates: Requirements 16.7**

- [ ]* 6.3 Write property test for default status
  - **Property 4: Default status is draft**
  - **Validates: Requirements 2.1**

- [x] 6.3.5 Create TaggedProductDto and mapping logic




  done -  Created `TaggedProductDto` in `VietCommerce.Core/DTOs/Product`
  - Create helper method to build TaggedProductDto from Product entity
  - Handle null product case (return null TaggedProduct)
  - Handle missing product case (return null TaggedProduct)
  - _Requirements: 16.5_

- [x] 6.4 check Implement GetPostsAsync with filtering and pagination

  - Validate pagination parameters
  - Apply filters (status, productId, platform, searchTerm, date range)
  - Apply sorting (default: updatedAt DESC)
  - Return paginated result with metadata
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6_

- [ ]* 6.4.1 Enrich GetPostsAsync response with TaggedProduct
  - For each post in paginated results, fetch live product data if productId exists
  - Build TaggedProductDto for each post
  - Handle null/missing products gracefully
  - _Requirements: 16.8, 16.3, 16.5, 16.6_

- [ ]* 6.5 Write property test for pagination structure
  - **Property 7: Pagination structure completeness**
  - **Validates: Requirements 3.1**

- [ ]* 6.6 Write property test for status filter
  - **Property 8: Status filter correctness**
  - **Validates: Requirements 3.2**

- [ ]* 6.7 Write property test for search functionality
  - **Property 9: Search keyword matching**
  - **Validates: Requirements 3.3**

- [ ]* 6.7.1 Write property test for list view includes TaggedProduct
  - **Property 29: List view includes TaggedProduct**
  - **Validates: Requirements 16.8**

- [x] 6.8 Implement GetPostByIdAsync

  - Validate post ID
  - Retrieve post with details
  - Return 404 if not found
  - Map to detail DTO
  - Enrich response with TaggedProduct if productId exists
  - _Requirements: 14.1, 16.3, 16.5, 16.6_

- [ ]* 6.8.1 Write property test for TaggedProduct null when no productId
  - **Property 24: TaggedProduct null when no productId**
  - **Validates: Requirements 16.4**

- [ ]* 6.8.2 Write property test for TaggedProduct null when product not found
  - **Property 25: TaggedProduct null when product not found**
  - **Validates: Requirements 16.3**

- [ ]* 6.8.3 Write property test for TaggedProduct contains required fields
  - **Property 26: TaggedProduct contains required fields**
  - **Validates: Requirements 16.5**

- [x] 6.9 Implement UpdatePostAsync

  - Validate admin authentication and authorization
  - Validate input data
  - Validate product exists if productId changed
  - Update only provided fields
  - Refresh updatedAt timestamp
  - Handle concurrency conflicts
  - _Requirements: 1.3, 1.4, 9.5, 11.1, 12.1, 12.2, 12.3, 14.4_

- [ ]* 6.10 Write property test for update timestamp refresh
  - **Property 2: Post updates refresh timestamp**
  - **Validates: Requirements 1.3**

- [ ]* 6.11 Write property test for product validation
  - **Property 3: Product validation on post creation**
  - **Validates: Requirements 1.4, 10.1**

- [x] 6.12 Implement DeletePostAsync (soft delete)

  - Validate admin authentication
  - Validate post exists
  - Set IsDeleted = true, DeletedAt = now, DeletedBy = currentUserId
  - Preserve all post data
  - _Requirements: 4.1, 4.2, 11.1_

- [ ]* 6.13 Write property test for soft delete
  - **Property 10: Soft delete behavior**
  - **Validates: Requirements 4.1, 4.2**

- [ ]* 6.14 Write property test for soft delete query filter
  - **Property 11: Soft delete query filter**
  - **Validates: Requirements 4.3**

- [x] 6.15 Implement RestorePostAsync

  - Validate admin authentication
  - Validate post exists (includeDeleted = true)
  - Clear IsDeleted, DeletedAt, DeletedBy
  - _Requirements: 4.5, 11.1_

- [ ]* 6.16 Write unit tests for CRUD operations
  - Test CreatePostAsync with valid data
  - Test CreatePostAsync with invalid data
  - Test GetPostsAsync with various filters
  - Test UpdatePostAsync
  - Test DeletePostAsync
  - Test RestorePostAsync
  - _Requirements: All CRUD requirements_

- [x] 7. Implement Publishing Operations in Service





- [x] 7.1 Implement PublishPostAsync


  - Validate admin authentication
  - Validate post exists and is Draft or Scheduled
  - Change status to Published
  - Set publishedDate to current timestamp
  - Clear scheduledDate if exists
  - _Requirements: 2.2, 11.1_

- [ ]* 7.2 Write property test for publishing state transition
  - **Property 5: Publishing state transition**
  - **Validates: Requirements 2.2**

- [x] 7.3 Implement SchedulePostAsync

  - Validate admin authentication
  - Validate scheduledDate is in the future
  - Change status to Scheduled
  - Set scheduledDate
  - Clear publishedDate if exists
  - _Requirements: 2.3, 11.1, 12.4_

- [ ]* 7.4 Write property test for scheduling state transition
  - **Property 6: Scheduling state transition**
  - **Validates: Requirements 2.3**

- [ ]* 7.5 Write property test for scheduled date validation
  - **Property 19: Scheduled date future validation**
  - **Validates: Requirements 12.4**

- [x] 7.6 Implement UnpublishPostAsync

  - Validate admin authentication
  - Validate post exists and is Published
  - Change status to Draft
  - Clear publishedDate
  - _Requirements: 2.4, 11.1_

- [ ]* 7.7 Write unit tests for publishing operations
  - Test PublishPostAsync
  - Test SchedulePostAsync with valid future date
  - Test SchedulePostAsync with past date (should fail)
  - Test UnpublishPostAsync
  - _Requirements: 2.2, 2.3, 2.4_

- [x] 8. Implement Analytics Operations in Service





- [x] 8.1 Implement IncrementViewsAsync


  - Validate post exists
  - Increment views counter atomically
  - Ensure counter never decreases
  - _Requirements: 5.1, 5.4, 5.5_

- [x] 8.2 Implement IncrementClicksAsync


  - Validate post exists
  - Increment clicks counter atomically
  - Ensure counter never decreases
  - _Requirements: 5.2, 5.4, 5.5_

- [x] 8.3 Implement IncrementSharesAsync


  - Validate post exists
  - Increment shares counter atomically
  - Ensure counter never decreases
  - _Requirements: 5.3, 5.4, 5.5_

- [ ]* 8.4 Write property test for analytics monotonicity
  - **Property 12: Analytics increment monotonicity**
  - **Validates: Requirements 5.1, 5.2, 5.3, 5.4**

- [ ]* 8.5 Write unit tests for analytics operations
  - Test IncrementViewsAsync
  - Test IncrementClicksAsync
  - Test IncrementSharesAsync
  - Test concurrent increments
  - _Requirements: 5.1, 5.2, 5.3, 5.5_


- [ ] 8.6 Write property test for TaggedProduct reflects live product data



  - **Property 27: TaggedProduct reflects live product data**
  - **Validates: Requirements 16.6**


- [x] 9. Implement Statistics and Utility Operations




- [x] 9.1 Implement GetStatisticsAsync


  - Calculate total count (exclude deleted)
  - Calculate counts by status (draft, published, scheduled)
  - Calculate sum of views, clicks, shares
  - Calculate averages
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

- [ ]* 9.2 Write property test for statistics aggregation
  - **Property 13: Statistics aggregation consistency**
  - **Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5, 6.6**

- [x] 9.3 Implement GetPostsByProductAsync


  - Validate product ID
  - Retrieve all posts linked to product
  - Return list of posts
  - _Requirements: 10.3_

- [x] 9.4 Implement DuplicatePostAsync


  - Validate admin authentication
  - Validate source post exists
  - Copy all content fields
  - Append " (Copy)" to title
  - Set status to Draft
  - Reset analytics counters to zero
  - Generate new ID and timestamps
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 11.1_

- [ ]* 9.5 Write property test for post duplication
  - **Property 14: Post duplication correctness**
  - **Validates: Requirements 7.1, 7.2, 7.3, 7.4, 7.5**

- [ ]* 9.6 Write unit tests for utility operations
  - Test GetStatisticsAsync
  - Test GetPostsByProductAsync
  - Test DuplicatePostAsync
  - _Requirements: 6.1-6.6, 7.1-7.5, 10.3_

- [x] 10. Implement Bulk Operations in Service




- [x] 10.1 Implement BulkDeleteAsync


  - Validate admin authentication
  - Process each post ID in transaction
  - Soft delete all specified posts
  - Track successes and failures
  - Return detailed results
  - _Requirements: 11.1, 13.1, 13.4, 13.5_

- [x] 10.2 Implement BulkPublishAsync

  - Validate admin authentication
  - Process each post ID in transaction
  - Publish only draft posts
  - Track successes and failures
  - Return detailed results
  - _Requirements: 11.1, 13.3, 13.4, 13.5_

- [x] 10.3 Implement BulkUpdateStatusAsync

  - Validate admin authentication
  - Process each post ID in transaction
  - Update status for all specified posts
  - Track successes and failures
  - Return detailed results
  - _Requirements: 11.1, 13.2, 13.4, 13.5_

- [ ]* 10.4 Write unit tests for bulk operations
  - Test BulkDeleteAsync with valid IDs
  - Test BulkDeleteAsync with some invalid IDs
  - Test BulkPublishAsync
  - Test BulkUpdateStatusAsync
  - Test transaction rollback on failure
  - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5_

- [x] 11. Create AutoMapper Profile




  - Create `MarketingPostMappingProfile` in `VietCommerce.Application/Mappings`
  - Map `MarketingPost` → `MarketingPostResponseDto`
  - Map `MarketingPost` → `MarketingPostListDto`
  - Map `MarketingPost` → `MarketingPostDetailDto`
  - Map `CreateMarketingPostDto` → `MarketingPost`
  - Map `UpdateMarketingPostDto` → `MarketingPost` (ignore null values)
  - Handle enum to string conversions
  - _Requirements: All_

- [x] 12. Create FluentValidation Validators





  - Create `CreateMarketingPostDtoValidator`
    - Validate Title: Required, 3-200 characters
    - Validate Content: Required, min 10 characters
    - Validate Hashtags: Alphanumeric and underscores only
    - Validate ScheduledDate: Must be future date if provided
    - Validate Status: Must be valid enum value
  - Create `UpdateMarketingPostDtoValidator`
    - Same rules as Create but all fields optional
  - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.6_

- [ ]* 12.1 Write property test for title validation
  - **Property 16: Title validation boundaries**
  - **Validates: Requirements 12.1**

- [ ]* 12.2 Write property test for content validation
  - **Property 17: Content validation minimum length**
  - **Validates: Requirements 12.2**

- [ ]* 12.3 Write property test for hashtag validation
  - **Property 18: Hashtag format validation**
  - **Validates: Requirements 12.3**

- [x] 13. Create API Controller








  - Create `AdminMarketingPostController` in `VietCommerce.AdminApi/Controllers`
  - Add `[Authorize]` attribute
  - Implement all CRUD endpoints
  - Implement publishing endpoints
  - Implement analytics endpoints (AllowAnonymous)
  - Implement utility endpoints
  - Implement bulk operation endpoints
  - Add proper HTTP status codes and response types
  - Add XML documentation comments
  - _Requirements: 11.1, 11.2, 11.3_

- [ ]* 13.1 Write property test for authorization
  - **Property 15: Authorization enforcement**
  - **Validates: Requirements 11.1, 11.2, 11.3**

- [ ]* 13.2 Write integration tests for API endpoints
  - Test all CRUD endpoints
  - Test publishing endpoints
  - Test analytics endpoints
  - Test bulk operations
  - Test authorization (401/403 responses)
  - Test validation errors (400 responses)
  - Test not found errors (404 responses)
  - _Requirements: All_

- [ ] 14. Create Custom Exceptions
  - Create `MarketingPostNotFoundException` in `VietCommerce.Core/Exceptions`
  - Create `InvalidMarketingPostStatusException`
  - Create `MarketingPostConcurrencyException`
  - Update global exception handler to handle new exceptions
  - _Requirements: 14.1, 14.2, 14.3, 14.5_

- [x] 15. Register Services in DI Container





  - Register `IMarketingPostService` → `MarketingPostService` in `Program.cs`
  - Register `IMarketingPostRepository` → `MarketingPostRepository`
  - Register validators
  - Register AutoMapper profile
  - _Requirements: All_

- [ ] 16. Create and Apply Database Migration
  - Run `Add-Migration AddMarketingPostEntity`
  - Review migration file
  - Apply migration to database
  - Verify table structure and indexes
  - _Requirements: All_

- [ ] 17. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 18. Create Background Job for Auto-Publishing
  - Create `MarketingPostPublishingJob` background service
  - Query for posts with Status = Scheduled and ScheduledDate <= now
  - Publish each post automatically
  - Run job every minute
  - Log publishing activities
  - _Requirements: 2.5_

- [ ]* 18.1 Write unit tests for background job
  - Test auto-publishing logic
  - Test job scheduling
  - _Requirements: 2.5_

- [ ] 19. Update API Documentation




  - Update Swagger configuration to include new controller
  - Add XML documentation to all endpoints
  - Add example requests and responses
  - Document authentication requirements
  - _Requirements: All_

- [ ] 20. Final Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

---

## Summary

**Total Tasks:** 20 main tasks with 40+ sub-tasks
**Optional Tasks:** 20 test-related sub-tasks (marked with *)
**Checkpoints:** 2 checkpoints to ensure quality

**Key Milestones:**
1. Database setup (Tasks 1-4)
2. Core CRUD operations (Tasks 5-6)
3. Publishing workflow (Task 7)
4. Analytics tracking (Task 8)
5. Advanced features (Tasks 9-10)
6. API layer (Tasks 11-13)
7. Infrastructure (Tasks 14-16)
8. Background jobs (Task 18)
9. Documentation (Task 19)
