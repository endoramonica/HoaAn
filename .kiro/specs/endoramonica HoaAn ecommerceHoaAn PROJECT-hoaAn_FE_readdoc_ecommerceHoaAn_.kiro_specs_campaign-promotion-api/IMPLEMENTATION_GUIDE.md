# Campaign & Promotion API - Implementation Guide

## Overview

This guide provides backend developers with the complete API contract for implementing the Campaign & Promotion Management System. The FE already has UI components (AdPopup with modal/banner types) that will consume these APIs.

## API Endpoints Summary

### Campaign Management
- `POST /api/v1/campaigns` - Create campaign
- `GET /api/v1/campaigns` - List campaigns (paginated, filterable)
- `PUT /api/v1/campaigns/{campaignId}` - Update campaign
- `DELETE /api/v1/campaigns/{campaignId}` - Delete campaign (optional)

### Promotion Management
- `POST /api/v1/campaigns/{campaignId}/promotions` - Create promotion
- `GET /api/v1/campaigns/{campaignId}/promotions` - List promotions
- `PUT /api/v1/promotions/{promotionId}` - Update promotion (optional)
- `DELETE /api/v1/promotions/{promotionId}` - Delete promotion (optional)

### Promotion-Product Linking
- `POST /api/v1/promotions/{promotionId}/link-products` - Link products to promotion
- `GET /api/v1/promotions/{promotionId}/products` - Get linked products

### Voucher Management
- `POST /api/v1/promotions/{promotionId}/vouchers/generate` - Generate voucher codes
- `POST /api/v1/cart/apply-voucher` - Apply voucher to cart
- `POST /api/v1/cart/remove-voucher` - Remove voucher from cart

### Analytics & Tracking
- `POST /api/v1/campaigns/{campaignId}/track-impression` - Track campaign impression
- `POST /api/v1/campaigns/{campaignId}/track-click` - Track campaign click
- `GET /api/v1/campaigns/{campaignId}/stats` - Get campaign statistics

## Database Schema

### Campaign Table
```sql
CREATE TABLE Campaigns (
  CampaignId NVARCHAR(50) PRIMARY KEY,
  CampaignName NVARCHAR(255) NOT NULL,
  Budget DECIMAL(18,2) NOT NULL CHECK (Budget >= 0),
  StartDate DATETIME NOT NULL,
  EndDate DATETIME NOT NULL CHECK (EndDate > StartDate),
  Status NVARCHAR(50) NOT NULL DEFAULT 'DRAFT', -- DRAFT, ACTIVE, PAUSED, COMPLETED, ARCHIVED
  Description NVARCHAR(MAX),
  TargetingPages NVARCHAR(MAX), -- JSON array
  TargetingFrequency NVARCHAR(50), -- once-per-session, once-per-page, always
  TargetingDelayMs INT,
  TargetingAutoDismissMs INT,
  CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
  CreatedBy NVARCHAR(50),
  UpdatedBy NVARCHAR(50)
);
```

### Promotion Table
```sql
CREATE TABLE Promotions (
  PromotionId NVARCHAR(50) PRIMARY KEY,
  CampaignId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Campaigns(CampaignId),
  DiscountValue DECIMAL(18,2) NOT NULL,
  DiscountType NVARCHAR(50) NOT NULL, -- percentage, fixed
  StartDate DATETIME NOT NULL,
  EndDate DATETIME NOT NULL CHECK (EndDate > StartDate),
  MinOrderValue DECIMAL(18,2),
  MaxUsage INT,
  CurrentUsage INT DEFAULT 0,
  Description NVARCHAR(MAX),
  CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
  UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

### PromotionProduct Table
```sql
CREATE TABLE PromotionProducts (
  PromotionProductId NVARCHAR(50) PRIMARY KEY,
  PromotionId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Promotions(PromotionId),
  ProductId NVARCHAR(50) NOT NULL,
  CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

### Voucher Table
```sql
CREATE TABLE Vouchers (
  VoucherId NVARCHAR(50) PRIMARY KEY,
  PromotionId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Promotions(PromotionId),
  Code NVARCHAR(100) NOT NULL UNIQUE,
  ExpiryDate DATETIME NOT NULL,
  IsUsed BIT DEFAULT 0,
  UsedAt DATETIME,
  UsedBy NVARCHAR(50),
  CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

### Campaign Analytics Tables
```sql
CREATE TABLE CampaignImpressions (
  ImpressionId NVARCHAR(50) PRIMARY KEY,
  CampaignId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Campaigns(CampaignId),
  SessionId NVARCHAR(100),
  Page NVARCHAR(100),
  RecordedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE CampaignClicks (
  ClickId NVARCHAR(50) PRIMARY KEY,
  CampaignId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Campaigns(CampaignId),
  SessionId NVARCHAR(100),
  Page NVARCHAR(100),
  RecordedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE VoucherRedemptions (
  RedemptionId NVARCHAR(50) PRIMARY KEY,
  VoucherId NVARCHAR(50) NOT NULL FOREIGN KEY REFERENCES Vouchers(VoucherId),
  OrderId NVARCHAR(50),
  DiscountAmount DECIMAL(18,2),
  RedeemedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);
```

## Validation Rules

### Campaign Validation
- `CampaignName`: Required, max 255 characters
- `Budget`: Required, must be >= 0
- `StartDate`: Required, must be before EndDate
- `EndDate`: Required, must be after StartDate
- `Status`: Must be one of: DRAFT, ACTIVE, PAUSED, COMPLETED, ARCHIVED
- `Targeting.Pages`: Array of page names (home, products, cart, etc.)
- `Targeting.Frequency`: Must be one of: once-per-session, once-per-page, always

### Promotion Validation
- `DiscountValue`: Required, must be > 0
- `DiscountType`: Required, must be 'percentage' or 'fixed'
- `StartDate`: Required, must be before EndDate
- `EndDate`: Required, must be after StartDate
- `MinOrderValue`: Optional, if set must be > 0
- `MaxUsage`: Optional, if set must be > 0
- Campaign must be in DRAFT status to add promotions

### Voucher Validation
- `Code`: Required, must be unique
- `ExpiryDate`: Required, must be in future
- Promotion must exist
- Cannot exceed promotion's MaxUsage

## Business Rules

### Campaign Status Transitions
- DRAFT → ACTIVE (requires all required fields)
- ACTIVE → PAUSED
- PAUSED → ACTIVE
- Any status → ARCHIVED
- DRAFT → COMPLETED (auto when end date reached)

### Promotion Rules
- Can only add promotions to DRAFT campaigns
- Cannot add promotions to ACTIVE campaigns
- Promotion dates must be within campaign dates
- Discount calculation:
  - Percentage: `discount = originalPrice * (discountValue / 100)`
  - Fixed: `discount = discountValue`
- No discount stacking (apply highest discount only)
- Minimum order value must be met to apply voucher

### Voucher Rules
- Each voucher code is unique
- Voucher can be used only once (unless marked as reusable)
- Voucher must not be expired
- Promotion must be active
- Cart total must meet minimum order value

## Error Handling

### HTTP Status Codes
- `200 OK`: Successful request
- `400 Bad Request`: Validation error (invalid dates, negative budget, etc.)
- `401 Unauthorized`: Missing or invalid authentication
- `403 Forbidden`: User lacks permission
- `404 Not Found`: Resource not found
- `409 Conflict`: Business rule violation (e.g., adding promotion to non-DRAFT campaign)
- `500 Internal Server Error`: Server error

### Error Response Format
```json
{
  "success": false,
  "message": "Human-readable error message",
  "errorCode": "MACHINE_READABLE_CODE",
  "errors": {
    "fieldName": ["Field-specific error message"]
  }
}
```

## Frontend Integration Points

The FE AdPopup component expects campaigns to be fetched and displayed. Key integration points:

1. **Campaign Fetching**: GET /api/v1/campaigns with targeting filters
2. **Impression Tracking**: POST /api/v1/campaigns/{id}/track-impression when campaign is shown
3. **Click Tracking**: POST /api/v1/campaigns/{id}/track-click when user clicks CTA
4. **Voucher Application**: POST /api/v1/cart/apply-voucher when user applies code
5. **Analytics**: GET /api/v1/campaigns/{id}/stats for admin dashboard

## Implementation Checklist

- [ ] Create Campaign entity and DbContext
- [ ] Create Promotion entity and DbContext
- [ ] Create PromotionProduct junction entity
- [ ] Create Voucher entity and DbContext
- [ ] Create Campaign analytics tables (Impressions, Clicks, Redemptions)
- [ ] Implement Campaign CRUD endpoints
- [ ] Implement Promotion CRUD endpoints
- [ ] Implement Voucher generation endpoint
- [ ] Implement Voucher application logic in Cart service
- [ ] Implement Campaign tracking endpoints
- [ ] Implement Campaign statistics endpoint
- [ ] Add validation for all business rules
- [ ] Add error handling and logging
- [ ] Add unit tests for all endpoints
- [ ] Add integration tests
- [ ] Document API in Swagger/OpenAPI
- [ ] Test with FE AdPopup component

## Notes

- All timestamps should be in UTC (ISO 8601 format)
- All IDs should be generated as NVARCHAR(50) with format: `{entity-type}-{guid}`
- Implement soft deletes for campaigns and promotions (add IsDeleted flag)
- Add audit logging for all campaign/promotion changes
- Consider caching campaign data for performance
- Implement rate limiting for tracking endpoints
