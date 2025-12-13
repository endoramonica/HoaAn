# Swagger/OpenAPI Documentation Guide

## Overview

The Campaign & Promotion Management System API is fully documented with Swagger/OpenAPI 3.0 specifications. This guide explains the API structure, endpoints, and how to use the Swagger UI.

## Accessing Swagger UI

**Development Environment:**
- URL: `http://localhost:5000/swagger/ui` or `http://localhost:5000/swagger`
- Available when running in Development mode
- Automatically disabled in Production

## API Structure

### Base URL
```
http://localhost:5000/api/v1
```

### Authentication
All endpoints (except analytics tracking) require JWT Bearer token authentication.

**Header Format:**
```
Authorization: Bearer <your_jwt_token>
```

## API Endpoints Overview

### Campaign Management (`/campaigns`)

#### Create Campaign
- **Method:** POST
- **Endpoint:** `/api/v1/campaigns`
- **Description:** Create a new marketing campaign
- **Requirements:** 1.1, 1.2, 1.3
- **Request Body:**
  ```json
  {
    "campaignName": "Summer Sale 2025",
    "description": "50% off all summer items",
    "campaignType": 1,
    "startDate": "2025-06-01T00:00:00Z",
    "endDate": "2025-08-31T23:59:59Z",
    "budget": 10000.00,
    "targetingRules": "{\"pages\": [\"home\", \"products\"], \"frequency\": \"once-per-session\"}"
  }
  ```
- **Response:** CampaignDto with all fields persisted
- **Status Codes:**
  - 200: Campaign created successfully
  - 400: Validation error (invalid dates, negative budget)
  - 401: Unauthorized

#### Get Campaigns
- **Method:** GET
- **Endpoint:** `/api/v1/campaigns`
- **Description:** Retrieve campaigns with pagination and filtering
- **Requirements:** 1.5
- **Query Parameters:**
  - `pageNumber` (int, default: 1): Page number
  - `pageSize` (int, default: 10, max: 100): Items per page
  - `status` (int, optional): Filter by status (0=DRAFT, 1=ACTIVE, 2=PAUSED, 3=COMPLETED, 4=ARCHIVED)
  - `fromDate` (datetime, optional): Filter campaigns from this date
  - `toDate` (datetime, optional): Filter campaigns to this date
  - `searchTerm` (string, optional): Search by campaign name
  - `sortBy` (string, default: "CreatedAt"): Sort field
  - `isDescending` (bool, default: true): Sort order
- **Response:** PaginatedResult<CampaignDto>
- **Status Codes:**
  - 200: Campaigns retrieved successfully
  - 400: Invalid pagination parameters
  - 401: Unauthorized

#### Get Campaign by ID
- **Method:** GET
- **Endpoint:** `/api/v1/campaigns/{id}`
- **Description:** Retrieve a specific campaign
- **Path Parameters:**
  - `id` (GUID): Campaign ID
- **Response:** CampaignDto
- **Status Codes:**
  - 200: Campaign retrieved successfully
  - 404: Campaign not found
  - 401: Unauthorized

#### Update Campaign
- **Method:** PUT
- **Endpoint:** `/api/v1/campaigns/{id}`
- **Description:** Update campaign details
- **Requirements:** 1.4
- **Path Parameters:**
  - `id` (GUID): Campaign ID
- **Request Body:** UpdateCampaignDto (all fields optional)
- **Response:** Updated CampaignDto
- **Status Codes:**
  - 200: Campaign updated successfully
  - 400: Validation error
  - 404: Campaign not found
  - 401: Unauthorized

#### Delete Campaign
- **Method:** DELETE
- **Endpoint:** `/api/v1/campaigns/{id}`
- **Description:** Delete a campaign (soft delete)
- **Path Parameters:**
  - `id` (GUID): Campaign ID
- **Response:** Success status
- **Status Codes:**
  - 200: Campaign deleted successfully
  - 404: Campaign not found
  - 401: Unauthorized

#### Change Campaign Status
- **Method:** PATCH
- **Endpoint:** `/api/v1/campaigns/{id}/status`
- **Description:** Transition campaign to new status
- **Requirements:** 6.1, 6.2, 6.3, 6.4, 6.5
- **Path Parameters:**
  - `id` (GUID): Campaign ID
- **Request Body:**
  ```json
  {
    "newStatus": 1
  }
  ```
- **Status Values:**
  - 0 = DRAFT
  - 1 = ACTIVE
  - 2 = PAUSED
  - 3 = COMPLETED
  - 4 = ARCHIVED
- **Response:** Updated CampaignDto
- **Status Codes:**
  - 200: Status changed successfully
  - 400: Invalid status or validation error
  - 404: Campaign not found
  - 409: Business rule violation
  - 401: Unauthorized

### Promotion Management (`/campaigns/{campaignId}/promotions`)

#### Create Promotion
- **Method:** POST
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions`
- **Description:** Create a promotion for a campaign
- **Requirements:** 2.1, 2.2, 2.3, 2.4
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
- **Request Body:**
  ```json
  {
    "promotionName": "50% Off Summer Items",
    "promotionType": 1,
    "discountValue": 50,
    "discountType": "percentage",
    "startDate": "2025-06-01T00:00:00Z",
    "endDate": "2025-08-31T23:59:59Z",
    "minOrderAmount": 100000,
    "usageLimit": 1000
  }
  ```
- **Response:** PromotionDto
- **Status Codes:**
  - 200: Promotion created successfully
  - 400: Validation error
  - 404: Campaign not found
  - 409: Campaign not in DRAFT status
  - 401: Unauthorized

#### Get Promotions
- **Method:** GET
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions`
- **Description:** Retrieve promotions for a campaign
- **Requirements:** 2.5
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
- **Query Parameters:** Same as Get Campaigns
- **Response:** PaginatedResult<PromotionDto>
- **Status Codes:**
  - 200: Promotions retrieved successfully
  - 400: Invalid pagination parameters
  - 404: Campaign not found
  - 401: Unauthorized

#### Get Promotion by ID
- **Method:** GET
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions/{id}`
- **Description:** Retrieve a specific promotion
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
  - `id` (GUID): Promotion ID
- **Response:** PromotionDto
- **Status Codes:**
  - 200: Promotion retrieved successfully
  - 404: Promotion not found
  - 401: Unauthorized

#### Update Promotion
- **Method:** PUT
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions/{id}`
- **Description:** Update promotion details
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
  - `id` (GUID): Promotion ID
- **Request Body:** UpdatePromotionDto (all fields optional)
- **Response:** Updated PromotionDto
- **Status Codes:**
  - 200: Promotion updated successfully
  - 400: Validation error
  - 404: Promotion not found
  - 401: Unauthorized

#### Delete Promotion
- **Method:** DELETE
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions/{id}`
- **Description:** Delete a promotion (soft delete)
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
  - `id` (GUID): Promotion ID
- **Response:** Success status
- **Status Codes:**
  - 200: Promotion deleted successfully
  - 404: Promotion not found
  - 401: Unauthorized

#### Link Products to Promotion
- **Method:** POST
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions/{promotionId}/products`
- **Description:** Link products to a promotion
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
  - `promotionId` (GUID): Promotion ID
- **Request Body:**
  ```json
  {
    "productIds": ["550e8400-e29b-41d4-a716-446655440000", "550e8400-e29b-41d4-a716-446655440001"]
  }
  ```
- **Response:** Success status
- **Status Codes:**
  - 200: Products linked successfully
  - 400: Validation error
  - 404: Promotion or product not found
  - 401: Unauthorized

#### Get Linked Products
- **Method:** GET
- **Endpoint:** `/api/v1/campaigns/{campaignId}/promotions/{promotionId}/products`
- **Description:** Retrieve products linked to a promotion
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
  - `promotionId` (GUID): Promotion ID
- **Response:** List of ProductDto
- **Status Codes:**
  - 200: Products retrieved successfully
  - 404: Promotion not found
  - 401: Unauthorized

### Voucher Management (`/promotions/{promotionId}/vouchers`)

#### Generate Vouchers
- **Method:** POST
- **Endpoint:** `/api/v1/promotions/{promotionId}/vouchers/generate`
- **Description:** Generate voucher codes for a promotion
- **Requirements:** 3.1
- **Path Parameters:**
  - `promotionId` (GUID): Promotion ID
- **Request Body:**
  ```json
  {
    "quantity": 100,
    "expiryDate": "2025-12-31T23:59:59Z",
    "prefix": "SUMMER25"
  }
  ```
- **Response:** GenerateVouchersResultDto with generated codes
- **Status Codes:**
  - 200: Vouchers generated successfully
  - 400: Validation error
  - 404: Promotion not found
  - 401: Unauthorized

#### Get Vouchers
- **Method:** GET
- **Endpoint:** `/api/v1/promotions/{promotionId}/vouchers`
- **Description:** Retrieve vouchers for a promotion
- **Requirements:** 3.1
- **Path Parameters:**
  - `promotionId` (GUID): Promotion ID
- **Query Parameters:**
  - `pageNumber` (int, default: 1): Page number
  - `pageSize` (int, default: 10, max: 100): Items per page
- **Response:** PaginatedResult<VoucherDto>
- **Status Codes:**
  - 200: Vouchers retrieved successfully
  - 400: Invalid pagination parameters
  - 404: Promotion not found
  - 401: Unauthorized

#### Delete Voucher
- **Method:** DELETE
- **Endpoint:** `/api/v1/promotions/{promotionId}/vouchers/{voucherId}`
- **Description:** Delete a voucher (soft delete)
- **Path Parameters:**
  - `promotionId` (GUID): Promotion ID
  - `voucherId` (GUID): Voucher ID
- **Response:** Success status
- **Status Codes:**
  - 200: Voucher deleted successfully
  - 404: Voucher not found
  - 401: Unauthorized

### Campaign Analytics (`/analytics`)

#### Track Impression
- **Method:** POST
- **Endpoint:** `/api/v1/analytics/{campaignId}/track-impression`
- **Description:** Track campaign impression (when shown to user)
- **Requirements:** 7.1
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
- **Request Body:**
  ```json
  {
    "sessionId": "sess_abc123def456",
    "page": "home",
    "timestamp": "2025-01-15T10:30:00Z"
  }
  ```
- **Response:** Success status
- **Status Codes:**
  - 200: Impression tracked successfully
  - 400: Validation error
  - 404: Campaign not found
- **Note:** No authentication required

#### Track Click
- **Method:** POST
- **Endpoint:** `/api/v1/analytics/{campaignId}/track-click`
- **Description:** Track campaign click (when user interacts)
- **Requirements:** 7.2
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
- **Request Body:**
  ```json
  {
    "sessionId": "sess_abc123def456",
    "page": "home",
    "timestamp": "2025-01-15T10:30:15Z"
  }
  ```
- **Response:** Success status
- **Status Codes:**
  - 200: Click tracked successfully
  - 400: Validation error
  - 404: Campaign not found
- **Note:** No authentication required

#### Get Campaign Stats
- **Method:** GET
- **Endpoint:** `/api/v1/analytics/{campaignId}/stats`
- **Description:** Retrieve campaign statistics
- **Requirements:** 7.4, 7.5
- **Path Parameters:**
  - `campaignId` (GUID): Campaign ID
- **Query Parameters:**
  - `fromDate` (datetime, optional): Start date for filtering
  - `toDate` (datetime, optional): End date for filtering
- **Response:** CampaignStatsDto with calculated metrics
- **Status Codes:**
  - 200: Statistics retrieved successfully
  - 400: Validation error
  - 404: Campaign not found
- **Note:** No authentication required

## Response Format

### Success Response
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {
    // Response data here
  }
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

## HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful request |
| 400 | Bad Request | Validation error or invalid input |
| 401 | Unauthorized | Missing or invalid JWT token |
| 403 | Forbidden | User lacks required permissions |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Business rule violation |
| 500 | Internal Server Error | Unexpected server error |

## Error Codes

| Code | Meaning |
|------|---------|
| INVALID_DATE_RANGE | EndDate <= StartDate |
| NEGATIVE_BUDGET | Budget < 0 |
| CAMPAIGN_NOT_DRAFT | Cannot add promotion to non-DRAFT campaign |
| INVALID_VOUCHER | Voucher code invalid or expired |
| VOUCHER_LIMIT_EXCEEDED | Voucher usage limit reached |
| MIN_ORDER_VALUE_NOT_MET | Cart total below minimum |
| PROMOTION_NOT_FOUND | Promotion doesn't exist |
| CAMPAIGN_NOT_FOUND | Campaign doesn't exist |
| INVALID_STATUS_TRANSITION | Cannot transition to requested status |

## Campaign Status Lifecycle

```
DRAFT (0)
  ↓
  ├─→ ACTIVE (1)
  │     ├─→ PAUSED (2)
  │     │     └─→ ACTIVE (1)
  │     └─→ COMPLETED (3) [automatic when end date reached]
  │
  └─→ ARCHIVED (4)
```

**Status Rules:**
- DRAFT: Initial state, allows adding/removing promotions
- ACTIVE: Campaign is live, no new promotions can be added
- PAUSED: Campaign is temporarily paused
- COMPLETED: Campaign end date has passed (automatic transition)
- ARCHIVED: Campaign is archived for historical reference

## Discount Types

| Type | Format | Example |
|------|--------|---------|
| percentage | Percentage value (0-100) | 50 = 50% off |
| fixed | Fixed amount in currency | 100000 = 100,000 VND off |

## Campaign Types

| Value | Type |
|-------|------|
| 0 | Modal Popup |
| 1 | Banner |
| 2 | Voucher |

## Promotion Types

| Value | Type |
|-------|------|
| 0 | Percentage Discount |
| 1 | Fixed Amount Discount |
| 2 | Free Shipping |

## Promotion Status

| Value | Status |
|-------|--------|
| 0 | ACTIVE |
| 1 | INACTIVE |
| 2 | EXPIRED |

## Testing with Swagger UI

1. **Authenticate:**
   - Click "Authorize" button
   - Enter your JWT token in format: `Bearer <token>`
   - Click "Authorize"

2. **Try Endpoints:**
   - Click on any endpoint to expand it
   - Click "Try it out"
   - Fill in required parameters
   - Click "Execute"
   - View response

3. **View Response:**
   - Response body shows the returned data
   - Response headers show HTTP headers
   - Response code shows HTTP status

## Example Workflow

### 1. Create Campaign
```
POST /api/v1/campaigns
{
  "campaignName": "Summer Sale",
  "budget": 10000,
  "startDate": "2025-06-01T00:00:00Z",
  "endDate": "2025-08-31T23:59:59Z",
  "campaignType": 1
}
```

### 2. Create Promotion
```
POST /api/v1/campaigns/{campaignId}/promotions
{
  "promotionName": "50% Off",
  "discountValue": 50,
  "discountType": "percentage",
  "startDate": "2025-06-01T00:00:00Z",
  "endDate": "2025-08-31T23:59:59Z"
}
```

### 3. Generate Vouchers
```
POST /api/v1/promotions/{promotionId}/vouchers/generate
{
  "quantity": 100,
  "expiryDate": "2025-12-31T23:59:59Z",
  "prefix": "SUMMER25"
}
```

### 4. Change Campaign Status
```
PATCH /api/v1/campaigns/{campaignId}/status
{
  "newStatus": 1
}
```

### 5. Track Impression
```
POST /api/v1/analytics/{campaignId}/track-impression
{
  "sessionId": "sess_123",
  "page": "home"
}
```

### 6. Get Campaign Stats
```
GET /api/v1/analytics/{campaignId}/stats?fromDate=2025-06-01&toDate=2025-08-31
```

## Additional Resources

- **OpenAPI Specification:** Available at `/swagger/v1.json`
- **Swagger UI:** Available at `/swagger/ui`
- **Requirements Document:** See `requirements.md`
- **Design Document:** See `design.md`
- **Implementation Guide:** See `IMPLEMENTATION_GUIDE.md`

