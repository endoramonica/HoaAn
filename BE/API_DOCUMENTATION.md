# VietCommerce Admin API Documentation

## Overview

The VietCommerce Admin API provides comprehensive endpoints for managing e-commerce operations including products, orders, inventory, and marketing posts. This documentation focuses on the Marketing Post Management System.

## Base URL

```
Development: https://localhost:7001
Production: https://api.vietcommerce.com/admin
```

## Authentication

All endpoints (except analytics endpoints) require JWT Bearer token authentication with Admin role.

### Getting a Token

1. **Login Endpoint**: `POST /api/admin/auth/login`
2. **Request Body**:
```json
{
  "email": "admin@vietcommerce.com",
  "password": "your-password"
}
```
3. **Response**:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "...",
    "expiresAt": "2024-12-04T10:00:00Z"
  }
}
```

### Using the Token

Include the token in the Authorization header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Marketing Post Management API

### Endpoints Overview

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/admin/marketing-posts` | Get paginated list of posts | Yes |
| GET | `/api/admin/marketing-posts/{id}` | Get post by ID | Yes |
| POST | `/api/admin/marketing-posts` | Create new post | Yes |
| PUT | `/api/admin/marketing-posts/{id}` | Update post | Yes |
| DELETE | `/api/admin/marketing-posts/{id}` | Soft delete post | Yes |
| POST | `/api/admin/marketing-posts/{id}/restore` | Restore deleted post | Yes |
| POST | `/api/admin/marketing-posts/{id}/publish` | Publish post | Yes |
| POST | `/api/admin/marketing-posts/{id}/schedule` | Schedule post | Yes |
| POST | `/api/admin/marketing-posts/{id}/unpublish` | Unpublish post | Yes |
| POST | `/api/admin/marketing-posts/{id}/analytics/views` | Increment views | No |
| POST | `/api/admin/marketing-posts/{id}/analytics/clicks` | Increment clicks | No |
| POST | `/api/admin/marketing-posts/{id}/analytics/shares` | Increment shares | No |
| POST | `/api/admin/marketing-posts/{id}/duplicate` | Duplicate post | Yes |
| GET | `/api/admin/marketing-posts/statistics` | Get statistics | Yes |
| GET | `/api/admin/marketing-posts/by-product/{productId}` | Get posts by product | Yes |
| POST | `/api/admin/marketing-posts/bulk/delete` | Bulk delete | Yes |
| POST | `/api/admin/marketing-posts/bulk/publish` | Bulk publish | Yes |
| POST | `/api/admin/marketing-posts/bulk/status` | Bulk status update | Yes |

---

## Detailed Endpoint Documentation

### 1. Get Marketing Posts (Paginated)

**Endpoint**: `GET /api/admin/marketing-posts`

**Description**: Retrieve a paginated list of marketing posts with filtering and search capabilities.

**Query Parameters**:
- `pageNumber` (int, default: 1) - Page number
- `pageSize` (int, default: 20) - Items per page
- `status` (enum, optional) - Filter by status (Draft=0, Published=1, Scheduled=2)
- `productId` (guid, optional) - Filter by product ID
- `platform` (string, optional) - Filter by platform
- `searchTerm` (string, optional) - Search in title, content, productName
- `displayLocation` (string, optional) - Filter by display location
- `isFeatured` (bool, optional) - Filter featured posts
- `minPriorityScore` (int, optional) - Minimum priority score
- `fromDate` (datetime, optional) - Filter from date
- `toDate` (datetime, optional) - Filter to date
- `sortBy` (string, default: "priority") - Sort field (priority, publishedDate, views, clicks, shares, updatedAt)
- `sortOrder` (string, default: "desc") - Sort order (asc, desc)
- `includeDeleted` (bool, default: false) - Include soft-deleted posts

**Example Request**:
```http
GET /api/admin/marketing-posts?pageNumber=1&pageSize=20&status=1&sortBy=views&sortOrder=desc
Authorization: Bearer {token}
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "12345678-1234-1234-1234-123456789012",
        "title": "Summer Sale 2024 - Up to 50% Off",
        "shortDescription": "Summer sale with up to 50% discount",
        "image": "https://example.com/images/summer-sale.jpg",
        "productId": "87654321-4321-4321-4321-210987654321",
        "productName": "Summer Collection Bundle",
        "platform": "All Platforms",
        "hashtags": ["SummerSale", "Discount"],
        "status": "Published",
        "publishedDate": "2024-12-01T10:00:00Z",
        "views": 1250,
        "clicks": 340,
        "shares": 85,
        "createdAt": "2024-11-28T10:00:00Z",
        "updatedAt": "2024-12-01T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 45,
    "totalPages": 3
  },
  "message": "Marketing posts retrieved successfully"
}
```

---

### 2. Create Marketing Post

**Endpoint**: `POST /api/admin/marketing-posts`

**Description**: Create a new marketing post.

**Request Body**:
```json
{
  "title": "Summer Sale 2024 - Up to 50% Off",
  "content": "Get ready for our biggest summer sale! Enjoy up to 50% off on selected items. Limited time offer.",
  "shortDescription": "Summer sale with up to 50% discount on selected items",
  "imageUrl": "https://example.com/images/summer-sale-2024.jpg",
  "productId": "12345678-1234-1234-1234-123456789012",
  "topic": "Seasonal Sale",
  "platform": "All Platforms",
  "tone": "Exciting",
  "hashtags": ["SummerSale", "Discount", "Shopping", "Sale2024"],
  "priorityScore": 85,
  "displayLocation": ["homepage_banner", "featured_section"],
  "metaTitle": "Summer Sale 2024 - Up to 50% Off | VietCommerce",
  "metaDescription": "Don't miss our summer sale with up to 50% discount on selected items.",
  "metaKeywords": ["summer sale", "discount", "shopping", "deals"],
  "socialPosts": {
    "facebook": "🌞 Summer Sale Alert! Get up to 50% OFF! 🎉 #SummerSale",
    "instagram": "☀️ SUMMER SALE ☀️\n50% OFF selected items!",
    "twitter": "🔥 Summer Sale is here! Up to 50% OFF!",
    "linkedIn": "Our Summer Sale 2024 is now live with up to 50% discount."
  },
  "status": 0
}
```

**Validation Rules**:
- `title`: Required, 3-200 characters
- `content`: Required, minimum 10 characters
- `hashtags`: Alphanumeric and underscores only
- `priorityScore`: 1-100 (default: 50)
- `scheduledDate`: Must be future date if provided

**Example Response**:
```json
{
  "success": true,
  "data": {
    "id": "12345678-1234-1234-1234-123456789012",
    "title": "Summer Sale 2024 - Up to 50% Off",
    "content": "Get ready for our biggest summer sale!...",
    "status": "Draft",
    "priorityScore": 85,
    "isFeatured": true,
    "views": 0,
    "clicks": 0,
    "shares": 0,
    "createdAt": "2024-12-03T10:00:00Z",
    "updatedAt": "2024-12-03T10:00:00Z"
  },
  "message": "Marketing post created successfully"
}
```

---

### 3. Publish Marketing Post

**Endpoint**: `POST /api/admin/marketing-posts/{id}/publish`

**Description**: Publish a draft or scheduled post immediately.

**Path Parameters**:
- `id` (guid) - Marketing post ID

**Example Request**:
```http
POST /api/admin/marketing-posts/12345678-1234-1234-1234-123456789012/publish
Authorization: Bearer {token}
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "id": "12345678-1234-1234-1234-123456789012",
    "title": "Summer Sale 2024 - Up to 50% Off",
    "status": "Published",
    "publishedDate": "2024-12-03T10:00:00Z",
    "scheduledDate": null
  },
  "message": "Marketing post published successfully"
}
```

---

### 4. Schedule Marketing Post

**Endpoint**: `POST /api/admin/marketing-posts/{id}/schedule`

**Description**: Schedule a post for future publication.

**Request Body**:
```json
{
  "scheduledDate": "2024-12-06T10:00:00Z"
}
```

**Validation**:
- `scheduledDate` must be in the future

**Example Response**:
```json
{
  "success": true,
  "data": {
    "id": "12345678-1234-1234-1234-123456789012",
    "status": "Scheduled",
    "scheduledDate": "2024-12-06T10:00:00Z"
  },
  "message": "Marketing post scheduled successfully"
}
```

---

### 5. Track Analytics

**Endpoints**:
- `POST /api/admin/marketing-posts/{id}/analytics/views`
- `POST /api/admin/marketing-posts/{id}/analytics/clicks`
- `POST /api/admin/marketing-posts/{id}/analytics/shares`

**Description**: Increment analytics counters. These are public endpoints (no authentication required).

**Example Request**:
```http
POST /api/admin/marketing-posts/12345678-1234-1234-1234-123456789012/analytics/views
```

**Example Response**:
```json
{
  "success": true,
  "data": true,
  "message": "View counter incremented successfully"
}
```

---

### 6. Get Statistics

**Endpoint**: `GET /api/admin/marketing-posts/statistics`

**Description**: Get aggregate statistics for all marketing posts.

**Example Response**:
```json
{
  "success": true,
  "data": {
    "total": 45,
    "draft": 12,
    "published": 28,
    "scheduled": 5,
    "totalViews": 125000,
    "totalClicks": 34500,
    "totalShares": 8900,
    "averageViews": 2777.78,
    "averageClicks": 766.67,
    "averageShares": 197.78
  },
  "message": "Statistics retrieved successfully"
}
```

---

### 7. Bulk Operations

#### Bulk Delete
**Endpoint**: `POST /api/admin/marketing-posts/bulk/delete`

**Request Body**:
```json
{
  "postIds": [
    "12345678-1234-1234-1234-123456789012",
    "22345678-1234-1234-1234-123456789012",
    "32345678-1234-1234-1234-123456789012"
  ]
}
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "totalRequested": 3,
    "successCount": 2,
    "failureCount": 1,
    "errors": [
      {
        "postId": "32345678-1234-1234-1234-123456789012",
        "errorMessage": "Marketing post not found"
      }
    ]
  },
  "message": "Bulk operation completed with some errors"
}
```

---

## Error Responses

### 400 Bad Request
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

### 401 Unauthorized
```json
{
  "success": false,
  "data": null,
  "message": "Unauthorized",
  "errors": ["JWT token is missing or invalid"],
  "statusCode": 401
}
```

### 403 Forbidden
```json
{
  "success": false,
  "data": null,
  "message": "Forbidden",
  "errors": ["Admin role required"],
  "statusCode": 403
}
```

### 404 Not Found
```json
{
  "success": false,
  "data": null,
  "message": "Marketing post not found",
  "errors": ["Marketing post with ID 'abc-123' does not exist"],
  "statusCode": 404
}
```

### 409 Conflict
```json
{
  "success": false,
  "data": null,
  "message": "Conflict",
  "errors": ["Post was modified by another user. Please refresh and try again."],
  "statusCode": 409
}
```

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request - Invalid input |
| 401 | Unauthorized - Missing or invalid token |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource doesn't exist |
| 409 | Conflict - Concurrency issue |
| 500 | Internal Server Error |

---

## Data Models

### MarketingPostStatus Enum
```
Draft = 0
Published = 1
Scheduled = 2
```

### Priority Score
- Range: 1-100
- Default: 50
- Posts with score > 80 are automatically marked as "featured"

### Display Locations
- `homepage_banner`
- `product_section`
- `featured_section`
- `sidebar`

---

## Best Practices

1. **Always validate input** before sending requests
2. **Handle errors gracefully** - check the `success` field in responses
3. **Use pagination** for list endpoints to avoid performance issues
4. **Implement retry logic** for 409 Conflict errors (concurrency)
5. **Cache tokens** - don't request a new token for every API call
6. **Use bulk operations** when performing multiple similar actions
7. **Monitor analytics** regularly to track post performance
8. **Set appropriate priority scores** to control display order

---

## Rate Limiting

- Analytics endpoints: 100 requests per minute per IP
- Other endpoints: 60 requests per minute per user

---

## Support

For API support, contact:
- Email: support@vietcommerce.com
- Documentation: https://docs.vietcommerce.com
- Swagger UI: https://localhost:7001/swagger (Development only)
