# Frontend API Reference - Complete Endpoint Documentation

## Campaign & Promotion API

### Base URL
## Campaign Endpoints

### GET /campaigns
Fetch active campaigns with filtering

**Query Parameters:**
```typescript
{
  status?: "DRAFT" | "ACTIVE" | "PAUSED" | "COMPLETED" | "ARCHIVED",
  pageNumber?: number, // default: 1
  pageSize?: number,   // default: 10
  fromDate?: string,   // ISO 8601
  toDate?: string      // ISO 8601
}
```

**Example Request:**
```bash
GET /api/v1/campaigns?status=ACTIVE&pageNumber=1&pageSize=10
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "campaignId": "camp-001",
        "campaignName": "Tết 2025 Sale",
        "budget": 50000000,
        "startDate": "2025-01-01T00:00:00Z",
        "endDate": "2025-02-15T23:59:59Z",
        "status": "ACTIVE",
        "description": "50% off all items",
        "targeting": {
          "pages": ["home", "products"],
          "frequency": "once-per-session",
          "delayMs": 3000,
          "autoDismissMs": 15000
        },
        "createdAt": "2024-12-15T10:00:00Z",
        "updatedAt": "2025-01-01T08:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalItems": 25,
    "totalPages": 3
  },
  "message": "Campaigns retrieved successfully"
}
```

**Error Response (400):**
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "pageSize": ["Page size must be between 1 and 100"]
  }
}
```

---

### POST /campaigns/{campaignId}/track-impression
Track when a campaign is shown to a user

**Path Parameters:**
- `campaignId` (string, required): Campaign ID

**Request Body:**
```json
{
  "sessionId": "sess-abc123def456",
  "page": "home",
  "timestamp": "2025-01-15T10:30:00Z"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "impressionId": "imp-001",
    "recorded": true
  },
  "message": "Impression tracked successfully"
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "Campaign not found",
  "errorCode": "RESOURCE_NOT_FOUND"
}
```

---

### POST /campaigns/{campaignId}/track-click
Track when a user clicks on a campaign

**Path Parameters:**
- `campaignId` (string, required): Campaign ID

**Request Body:**
```json
{
  "sessionId": "sess-abc123def456",
  "page": "home",
  "timestamp": "2025-01-15T10:30:05Z"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "clickId": "click-001",
    "recorded": true
  },
  "message": "Click tracked successfully"
}
```

---

### POST /cart/apply-voucher
Apply a voucher code to the shopping cart

**Request Body:**
```json
{
  "couponCode": "TET2025-ABC123"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "cartId": "cart-001",
    "appliedCoupon": "TET2025-ABC123",
    "promotionId": "promo-001",
    "discountValue": 40,
    "discountType": "percentage",
    "discountAmount": 400000,
    "subtotal": 1000000,
    "totalAmount": 600000
  },
  "message": "Voucher applied successfully"
}
```

**Error Response (400) - Invalid Voucher:**
```json
{
  "success": false,
  "message": "Voucher code is invalid or expired",
  "errorCode": "INVALID_VOUCHER"
}
```

**Error Response (409) - Limit Exceeded:**
```json
{
  "success": false,
  "message": "Voucher code has reached its usage limit",
  "errorCode": "VOUCHER_LIMIT_EXCEEDED"
}
```

**Error Response (400) - Minimum Order Not Met:**
```json
{
  "success": false,
  "message": "Cart total must be at least 500000 to use this voucher",
  "errorCode": "MIN_ORDER_VALUE_NOT_MET"
}
```

---

### POST /cart/remove-voucher
Remove applied voucher from cart

**Request Body:**
```json
{}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "cartId": "cart-001",
    "appliedCoupon": null,
    "subtotal": 1000000,
    "discountAmount": 0,
    "totalAmount": 1000000
  },
  "message": "Voucher removed successfully"
}
```

---

### GET /campaigns/{campaignId}/stats
Get campaign performance statistics

**Path Parameters:**
- `campaignId` (string, required): Campaign ID

**Query Parameters:**
```typescript
{
  fromDate?: string, // ISO 8601
  toDate?: string    // ISO 8601
}
```

**Example Request:**
```bash
GET /api/v1/campaigns/camp-001/stats?fromDate=2025-01-01&toDate=2025-02-15
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "campaignName": "Tết 2025 Sale",
    "impressions": 5000,
    "clicks": 450,
    "clickThroughRate": 9.0,
    "redemptions": 150,
    "redemptionRate": 3.0,
    "totalRevenue": 60000000,
    "totalDiscount": 24000000,
    "conversionRate": 33.3,
    "fromDate": "2025-01-01",
    "toDate": "2025-02-15"
  },
  "message": "Campaign statistics retrieved successfully"
}
```

---

## Marketing Post API

### Base URL
```
http://localhost:5000/api/admin/marketing-posts
```

---

### GET /
List marketing posts with pagination and filtering

**Query Parameters:**
```typescript
{
  pageNumber?: number,        // default: 1
  pageSize?: number,          // default: 20, max: 100
  status?: "Draft" | "Published" | "Scheduled",
  productId?: string,
  platform?: string,
  searchTerm?: string,
  displayLocation?: string,
  isFeatured?: boolean,
  minPriorityScore?: number,
  fromDate?: string,          // ISO 8601
  toDate?: string,            // ISO 8601
  sortBy?: string,            // priority, publishedDate, views, clicks, shares, updatedAt
  sortOrder?: "asc" | "desc"  // default: desc
}
```

**Example Request:**
```bash
GET /api/admin/marketing-posts?status=Published&pageSize=10&sortBy=priority
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "post-001",
        "title": "Amazing Product",
        "shortDescription": "Check out this amazing product",
        "image": "https://example.com/image.jpg",
        "productId": "prod-001",
        "productName": "Product Name",
        "taggedProduct": {
          "id": "prod-001",
          "name": "Product Name",
          "price": 1000000,
          "currency": "VND",
          "formattedPrice": "1,000,000 VND",
          "thumbnailUrl": "https://example.com/thumb.jpg",
          "hasDiscount": true,
          "discountPercentage": 20
        },
        "platform": "facebook",
        "hashtags": ["sale", "product"],
        "status": "Published",
        "publishedDate": "2025-01-15T10:00:00Z",
        "views": 150,
        "clicks": 25,
        "shares": 5,
        "createdAt": "2025-01-15T09:00:00Z",
        "updatedAt": "2025-01-15T10:00:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalItems": 45,
    "totalPages": 5
  },
  "message": "Posts retrieved successfully"
}
```

---

### GET /{id}
Get detailed information about a specific post

**Path Parameters:**
- `id` (string, required): Post ID

**Example Request:**
```bash
GET /api/admin/marketing-posts/post-001
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "post-001",
    "title": "Amazing Product",
    "content": "This is a detailed description of the amazing product...",
    "shortDescription": "Check out this amazing product",
    "image": "https://example.com/image.jpg",
    "images": [
      "https://example.com/image1.jpg",
      "https://example.com/image2.jpg"
    ],
    "productId": "prod-001",
    "productName": "Product Name",
    "taggedProduct": {
      "id": "prod-001",
      "name": "Product Name",
      "price": 1000000,
      "currency": "VND",
      "formattedPrice": "1,000,000 VND",
      "thumbnailUrl": "https://example.com/thumb.jpg",
      "hasDiscount": true,
      "discountPercentage": 20
    },
    "topic": "Product Launch",
    "platform": "facebook",
    "tone": "Professional",
    "hashtags": ["sale", "product", "new"],
    "priorityScore": 85,
    "displayLocation": ["homepage_banner", "featured_section"],
    "isFeatured": true,
    "metaTitle": "Amazing Product - Best Deal",
    "metaDescription": "Check out this amazing product with 20% discount",
    "metaKeywords": ["product", "sale", "discount"],
    "socialPosts": {
      "facebook": "Check out this amazing product!",
      "instagram": "Amazing product 🎉",
      "twitter": "New product available now!",
      "linkedin": "Introducing our latest product"
    },
    "status": "Published",
    "scheduledDate": null,
    "publishedDate": "2025-01-15T10:00:00Z",
    "views": 150,
    "clicks": 25,
    "shares": 5,
    "createdAt": "2025-01-15T09:00:00Z",
    "updatedAt": "2025-01-15T10:00:00Z"
  },
  "message": "Post retrieved successfully"
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "Marketing post not found",
  "errorCode": "RESOURCE_NOT_FOUND"
}
```

---

### POST /{id}/analytics/views
Track a post view (public endpoint, no auth required)

**Path Parameters:**
- `id` (string, required): Post ID

**Request Body:**
```json
{}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "postId": "post-001",
    "viewsCount": 151
  },
  "message": "View tracked successfully"
}
```

---

### POST /{id}/analytics/clicks
Track a post click (public endpoint, no auth required)

**Path Parameters:**
- `id` (string, required): Post ID

**Request Body:**
```json
{}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "postId": "post-001",
    "clicksCount": 26
  },
  "message": "Click tracked successfully"
}
```

---

### POST /{id}/analytics/shares
Track a post share (public endpoint, no auth required)

**Path Parameters:**
- `id` (string, required): Post ID

**Request Body:**
```json
{}
```

**Success Response (200):**
```json
{
  "success": true,
  "data": {
    "postId": "post-001",
    "sharesCount": 6
  },
  "message": "Share tracked successfully"
}
```

---

## Error Codes Reference

| Code | HTTP Status | Meaning |
|------|------------|---------|
| `INVALID_VOUCHER` | 400 | Voucher code invalid or expired |
| `VOUCHER_LIMIT_EXCEEDED` | 409 | Voucher usage limit reached |
| `MIN_ORDER_VALUE_NOT_MET` | 400 | Cart total below minimum |
| `RESOURCE_NOT_FOUND` | 404 | Post or campaign not found |
| `INVALID_ARGUMENT` | 400 | Validation error |
| `UNAUTHORIZED_ACCESS` | 401 | Missing or invalid JWT token |
| `INTERNAL_SERVER_ERROR` | 500 | Server error |

---

## Response Format

### Success Response
```json
{
  "success": true,
  "data": { /* response data */ },
  "message": "Operation successful"
}
```

### Error Response
```json
{
  "success": false,
  "message": "Human-readable error message",
  "errorCode": "ERROR_CODE",
  "errors": {
    "fieldName": ["Field-specific error message"]
  }
}
```

---

## HTTP Status Codes

| Status | Meaning |
|--------|---------|
| 200 | OK - Request successful |
| 400 | Bad Request - Validation error |
| 401 | Unauthorized - Missing/invalid JWT |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource doesn't exist |
| 409 | Conflict - Business rule violation |
| 500 | Internal Server Error |

---

## Authentication

### Public Endpoints (No Auth Required)
- GET `/api/v1/campaigns`
- POST `/api/v1/campaigns/{id}/track-impression`
- POST `/api/v1/campaigns/{id}/track-click`
- POST `/api/v1/cart/apply-voucher`
- POST `/api/v1/cart/remove-voucher`
- GET `/api/v1/campaigns/{id}/stats`
- POST `/api/admin/marketing-posts/{id}/analytics/views`
- POST `/api/admin/marketing-posts/{id}/analytics/clicks`
- POST `/api/admin/marketing-posts/{id}/analytics/shares`

### Protected Endpoints (Admin JWT Required)
- GET `/api/admin/marketing-posts`
- GET `/api/admin/marketing-posts/{id}`
- POST `/api/admin/marketing-posts`
- PUT `/api/admin/marketing-posts/{id}`
- DELETE `/api/admin/marketing-posts/{id}`
- GET `/api/admin/marketing-posts/statistics`

**Header Format:**
```
Authorization: Bearer <jwt_token>
```

---

## Rate Limiting

No rate limiting currently implemented. Contact backend team if needed.

---

## Pagination

All list endpoints support pagination:

```typescript
{
  pageNumber: number,  // 1-based
  pageSize: number,    // 1-100
  totalItems: number,  // Total count
  totalPages: number   // Total pages
}
```

---

## Sorting

Supported sort fields vary by endpoint:

**Marketing Posts:**
- `priority` - Priority score (1-100)
- `publishedDate` - Publication date
- `views` - View count
- `clicks` - Click count
- `shares` - Share count
- `updatedAt` - Last update date

**Sort Order:**
- `asc` - Ascending
- `desc` - Descending (default)

---

## Date Format

All dates use ISO 8601 format:
```
2025-01-15T10:30:00Z
```

---

## Swagger/OpenAPI

Interactive API documentation available at:
```
http://localhost:5000/swagger/ui
```

JSON specification:
```
http://localhost:5000/swagger/v1.json
```

---

**Last Updated:** January 2025
**Version:** 1.0
