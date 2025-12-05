# Requirements Document - Admin Post Management System

## Introduction

Hệ thống quản lý bài viết marketing cho Admin, cho phép tạo, chỉnh sửa, xuất bản và theo dõi hiệu suất của các bài viết marketing. Hệ thống tích hợp với Product catalog và cung cấp analytics để đo lường hiệu quả.

## Glossary

- **Admin Post System**: Hệ thống quản lý bài viết marketing dành cho quản trị viên
- **Post**: Bài viết marketing có thể chứa tiêu đề, nội dung, hình ảnh, hashtags và metadata SEO
- **Publishing Status**: Trạng thái xuất bản của bài viết (draft, published, scheduled)
- **Analytics Metrics**: Các chỉ số đo lường hiệu suất (views, clicks, shares)
- **Product Integration**: Liên kết bài viết với sản phẩm trong catalog
- **Social Media Variants**: Các phiên bản nội dung được tối ưu cho từng nền tảng mạng xã hội
- **SEO Metadata**: Thông tin meta để tối ưu hóa công cụ tìm kiếm
- **Soft Delete**: Xóa logic không xóa vật lý dữ liệu khỏi database

## Requirements

### Requirement 1: Post Creation and Management

**User Story:** As an admin, I want to create and manage marketing posts with rich content, so that I can effectively promote products and engage customers.

#### Acceptance Criteria

1. WHEN an admin creates a new post THEN the Admin Post System SHALL accept title, content, shortDescription, image, hashtags, productId, platform, tone, and SEO metadata
2. WHEN an admin creates a post THEN the Admin Post System SHALL automatically set createdAt, updatedAt timestamps and initialize analytics counters to zero
3. WHEN an admin updates a post THEN the Admin Post System SHALL update the specified fields and refresh the updatedAt timestamp
4. WHEN an admin provides a productId THEN the Admin Post System SHALL validate the product exists and store the product relationship
5. WHEN an admin uploads an image THEN the Admin Post System SHALL accept and store the image data

### Requirement 2: Publishing and Scheduling

**User Story:** As an admin, I want to control when posts are published, so that I can schedule content strategically.

#### Acceptance Criteria

1. WHEN an admin creates a post without specifying status THEN the Admin Post System SHALL set the status to draft
2. WHEN an admin publishes a draft post THEN the Admin Post System SHALL change status to published and set publishedDate to current timestamp
3. WHEN an admin schedules a post with a future date THEN the Admin Post System SHALL set status to scheduled and store the scheduledDate
4. WHEN an admin changes a published post back to draft THEN the Admin Post System SHALL update the status and clear the publishedDate
5. WHEN the scheduledDate arrives THEN the Admin Post System SHALL automatically change status from scheduled to published

### Requirement 3: Post Retrieval and Filtering

**User Story:** As an admin, I want to search and filter posts efficiently, so that I can quickly find and manage specific content.

#### Acceptance Criteria

1. WHEN an admin requests posts with pagination THEN the Admin Post System SHALL return posts with pageNumber, pageSize, totalCount, and totalPages
2. WHEN an admin filters by status THEN the Admin Post System SHALL return only posts matching the specified status
3. WHEN an admin searches by keyword THEN the Admin Post System SHALL search in title, content, and productName fields
4. WHEN an admin filters by productId THEN the Admin Post System SHALL return only posts linked to that product
5. WHEN an admin filters by date range THEN the Admin Post System SHALL return posts created within the specified fromDate and toDate
6. WHEN an admin requests posts THEN the Admin Post System SHALL sort results by updatedAt in descending order by default

### Requirement 4: Post Deletion

**User Story:** As an admin, I want to delete posts safely, so that I can remove outdated or incorrect content while maintaining data integrity.

#### Acceptance Criteria

1. WHEN an admin deletes a post THEN the Admin Post System SHALL perform a soft delete by marking the post as deleted
2. WHEN an admin deletes a post THEN the Admin Post System SHALL set deletedAt timestamp and preserve all post data
3. WHEN an admin requests posts THEN the Admin Post System SHALL exclude soft-deleted posts from results by default
4. WHEN an admin requests deleted posts explicitly THEN the Admin Post System SHALL return only soft-deleted posts
5. WHEN an admin restores a deleted post THEN the Admin Post System SHALL clear the deletedAt timestamp

### Requirement 5: Analytics Tracking

**User Story:** As an admin, I want to track post performance metrics, so that I can measure content effectiveness and make data-driven decisions.

#### Acceptance Criteria

1. WHEN a post view is recorded THEN the Admin Post System SHALL increment the views counter by one
2. WHEN a post click is recorded THEN the Admin Post System SHALL increment the clicks counter by one
3. WHEN a post share is recorded THEN the Admin Post System SHALL increment the shares counter by one
4. WHEN analytics are updated THEN the Admin Post System SHALL ensure counters never decrease
5. WHEN multiple analytics updates occur concurrently THEN the Admin Post System SHALL handle race conditions correctly

### Requirement 6: Statistics and Reporting

**User Story:** As an admin, I want to view aggregate statistics, so that I can understand overall content performance.

#### Acceptance Criteria

1. WHEN an admin requests statistics THEN the Admin Post System SHALL return total count of all posts
2. WHEN an admin requests statistics THEN the Admin Post System SHALL return counts grouped by status (draft, published, scheduled)
3. WHEN an admin requests statistics THEN the Admin Post System SHALL return sum of all views across all posts
4. WHEN an admin requests statistics THEN the Admin Post System SHALL return sum of all clicks across all posts
5. WHEN an admin requests statistics THEN the Admin Post System SHALL return sum of all shares across all posts
6. WHEN calculating statistics THEN the Admin Post System SHALL exclude soft-deleted posts

### Requirement 7: Post Duplication

**User Story:** As an admin, I want to duplicate existing posts, so that I can quickly create similar content without starting from scratch.

#### Acceptance Criteria

1. WHEN an admin duplicates a post THEN the Admin Post System SHALL create a new post with all content copied from the original
2. WHEN duplicating a post THEN the Admin Post System SHALL append "(Copy)" to the title
3. WHEN duplicating a post THEN the Admin Post System SHALL set the new post status to draft
4. WHEN duplicating a post THEN the Admin Post System SHALL reset analytics counters to zero
5. WHEN duplicating a post THEN the Admin Post System SHALL generate a new unique ID and set new timestamps

### Requirement 8: Social Media Variants

**User Story:** As an admin, I want to store platform-specific content variants, so that I can optimize posts for different social media platforms.

#### Acceptance Criteria

1. WHEN an admin provides social media variants THEN the Admin Post System SHALL store separate content for facebook, instagram, twitter, and linkedin
2. WHEN an admin retrieves a post THEN the Admin Post System SHALL return all stored social media variants
3. WHEN an admin updates social variants THEN the Admin Post System SHALL allow updating individual platform content independently
4. WHEN social variants are not provided THEN the Admin Post System SHALL store null for those platforms

### Requirement 9: SEO Metadata Management

**User Story:** As an admin, I want to manage SEO metadata for posts, so that content can be optimized for search engines.

#### Acceptance Criteria

1. WHEN an admin provides SEO metadata THEN the Admin Post System SHALL store metaTitle, metaDescription, and metaKeywords
2. WHEN metaTitle is not provided THEN the Admin Post System SHALL use the post title as default
3. WHEN metaDescription is not provided THEN the Admin Post System SHALL use the shortDescription or first 160 characters of content
4. WHEN an admin retrieves a post THEN the Admin Post System SHALL return all SEO metadata fields
5. WHEN an admin updates SEO metadata THEN the Admin Post System SHALL allow updating fields independently

### Requirement 10: Product Integration

**User Story:** As an admin, I want to link posts to products, so that marketing content is connected to the items being promoted.

#### Acceptance Criteria

1. WHEN an admin links a post to a product THEN the Admin Post System SHALL validate the productId exists in the Product catalog
2. WHEN a product is linked THEN the Admin Post System SHALL store both productId and productName for denormalization
3. WHEN an admin retrieves posts by product THEN the Admin Post System SHALL return all posts linked to that productId
4. WHEN a product is deleted THEN the Admin Post System SHALL maintain the post but mark the product relationship as invalid
5. WHEN an admin removes product link THEN the Admin Post System SHALL clear productId and productName fields

### Requirement 11: Authorization and Security

**User Story:** As a system administrator, I want secure access control, so that only authorized admins can manage posts.

#### Acceptance Criteria

1. WHEN an admin performs any operation THEN the Admin Post System SHALL verify the user has admin role from JWT token
2. WHEN an unauthorized user attempts access THEN the Admin Post System SHALL return 403 Forbidden status
3. WHEN JWT token is missing or invalid THEN the Admin Post System SHALL return 401 Unauthorized status
4. WHEN an admin creates or updates a post THEN the Admin Post System SHALL record the admin's userId as createdBy or updatedBy
5. WHEN validating permissions THEN the Admin Post System SHALL use ICurrentUser service to extract user information from JWT

### Requirement 12: Data Validation

**User Story:** As a system administrator, I want robust input validation, so that data integrity is maintained.

#### Acceptance Criteria

1. WHEN an admin creates a post THEN the Admin Post System SHALL require title with minimum 3 characters and maximum 200 characters
2. WHEN an admin creates a post THEN the Admin Post System SHALL require content with minimum 10 characters
3. WHEN an admin provides hashtags THEN the Admin Post System SHALL validate each hashtag contains only alphanumeric characters and underscores
4. WHEN an admin provides a scheduledDate THEN the Admin Post System SHALL validate the date is in the future
5. WHEN an admin provides invalid data THEN the Admin Post System SHALL return 400 Bad Request with detailed validation errors
6. WHEN an admin provides a status THEN the Admin Post System SHALL validate it is one of: draft, published, scheduled

### Requirement 13: Bulk Operations

**User Story:** As an admin, I want to perform bulk operations on posts, so that I can manage content efficiently at scale.

#### Acceptance Criteria

1. WHEN an admin requests bulk delete THEN the Admin Post System SHALL soft delete all specified post IDs
2. WHEN an admin requests bulk status change THEN the Admin Post System SHALL update status for all specified post IDs
3. WHEN an admin requests bulk publish THEN the Admin Post System SHALL publish all specified draft posts
4. WHEN bulk operations fail partially THEN the Admin Post System SHALL return detailed results indicating which operations succeeded and failed
5. WHEN performing bulk operations THEN the Admin Post System SHALL process operations within a transaction

### Requirement 14: Error Handling

**User Story:** As a developer, I want comprehensive error handling, so that issues can be diagnosed and resolved quickly.

#### Acceptance Criteria

1. WHEN a post is not found THEN the Admin Post System SHALL return 404 Not Found with a descriptive message
2. WHEN a database error occurs THEN the Admin Post System SHALL return 500 Internal Server Error and log the error details
3. WHEN validation fails THEN the Admin Post System SHALL return 400 Bad Request with all validation errors
4. WHEN a product reference is invalid THEN the Admin Post System SHALL return 400 Bad Request indicating the product does not exist
5. WHEN an error response is returned THEN the Admin Post System SHALL follow the ApiResponse format with success=false and error details
