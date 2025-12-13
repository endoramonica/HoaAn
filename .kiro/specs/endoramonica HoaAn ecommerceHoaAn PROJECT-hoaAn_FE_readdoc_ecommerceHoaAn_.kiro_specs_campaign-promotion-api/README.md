# Campaign & Promotion Management System - API Specification

## 📋 Overview

This specification defines the complete API contract for a Campaign & Promotion Management System. The frontend already has UI components (AdPopup with modal/banner types) ready to consume these APIs. This document provides backend developers with everything needed to implement the system.

## 📁 Specification Files

1. **requirements.md** - Detailed requirements using EARS patterns and INCOSE quality rules
2. **api-contract.json** - Complete JSON API contract with all endpoints, mock data, and responses
3. **API_ENDPOINTS_SUMMARY.md** - Human-readable endpoint documentation
4. **MOCK_DATA_EXAMPLES.md** - Real-world examples and mock data
5. **IMPLEMENTATION_GUIDE.md** - Database schema, validation rules, and implementation checklist

## 🎯 Key Features

### Campaign Management
- Create, read, update, delete campaigns
- Campaign status lifecycle (DRAFT → ACTIVE → PAUSED → COMPLETED)
- Budget tracking and validation
- Targeting rules (pages, frequency, delays)
- Date range validation (EndDate > StartDate)

### Promotion Management
- Create promotions linked to campaigns
- Support for percentage and fixed amount discounts
- Minimum order value requirements
- Usage limits and tracking
- Product-specific promotions via PromotionProduct junction table

### Voucher System
- Generate unique voucher codes
- Apply vouchers to shopping carts
- Validate voucher expiry and usage limits
- Track redemptions and revenue

### Analytics & Tracking
- Track campaign impressions (when shown to users)
- Track campaign clicks (when users click CTA)
- Track voucher redemptions
- Generate campaign statistics (CTR, conversion rate, ROI)

## 🔌 API Endpoints (13 Total)

### Campaign Management (4 endpoints)
- `POST /api/v1/campaigns` - Create campaign
- `GET /api/v1/campaigns` - List campaigns (paginated)
- `PUT /api/v1/campaigns/{campaignId}` - Update campaign
- `DELETE /api/v1/campaigns/{campaignId}` - Delete campaign

### Promotion Management (4 endpoints)
- `POST /api/v1/campaigns/{campaignId}/promotions` - Create promotion
- `GET /api/v1/campaigns/{campaignId}/promotions` - List promotions
- `PUT /api/v1/promotions/{promotionId}` - Update promotion
- `DELETE /api/v1/promotions/{promotionId}` - Delete promotion

### Promotion-Product Linking (2 endpoints)
- `POST /api/v1/promotions/{promotionId}/link-products` - Link products
- `GET /api/v1/promotions/{promotionId}/products` - Get linked products

### Voucher Management (3 endpoints)
- `POST /api/v1/promotions/{promotionId}/vouchers/generate` - Generate codes
- `POST /api/v1/cart/apply-voucher` - Apply voucher to cart
- `POST /api/v1/cart/remove-voucher` - Remove voucher from cart

### Analytics & Tracking (3 endpoints)
- `POST /api/v1/campaigns/{campaignId}/track-impression` - Track impression
- `POST /api/v1/campaigns/{campaignId}/track-click` - Track click
- `GET /api/v1/campaigns/{campaignId}/stats` - Get statistics

## 📊 Data Models

### Campaign
```
- campaignId (PK)
- campaignName
- budget (>= 0)
- startDate
- endDate (> startDate)
- status (DRAFT|ACTIVE|PAUSED|COMPLETED|ARCHIVED)
- description
- targeting (pages, frequency, delays)
- createdAt, updatedAt
```

### Promotion
```
- promotionId (PK)
- campaignId (FK)
- discountValue (> 0)
- discountType (percentage|fixed)
- startDate
- endDate (> startDate)
- minOrderValue
- maxUsage
- currentUsage
- description
```

### PromotionProduct
```
- promotionProductId (PK)
- promotionId (FK)
- productId (FK)
```

### Voucher
```
- voucherId (PK)
- promotionId (FK)
- code (unique)
- expiryDate
- isUsed
- usedAt, usedBy
```

## ✅ Validation Rules

### Campaign
- ✓ CampaignName: Required, max 255 characters
- ✓ Budget: Required, must be >= 0
- ✓ StartDate < EndDate (error 400 if violated)
- ✓ Status: Must be one of DRAFT, ACTIVE, PAUSED, COMPLETED, ARCHIVED
- ✓ Targeting.Pages: Array of valid page names
- ✓ Targeting.Frequency: once-per-session, once-per-page, or always

### Promotion
- ✓ DiscountValue: Required, must be > 0
- ✓ DiscountType: Required, must be 'percentage' or 'fixed'
- ✓ StartDate < EndDate (error 400 if violated)
- ✓ Campaign must be in DRAFT status (error 409 if not)
- ✓ MinOrderValue: Optional, if set must be > 0
- ✓ MaxUsage: Optional, if set must be > 0

### Voucher
- ✓ Code: Required, must be unique
- ✓ ExpiryDate: Required, must be in future
- ✓ Cannot exceed promotion's MaxUsage (error 409 if exceeded)
- ✓ Must not be expired (error 400 if expired)

## 🔄 Business Rules

### Campaign Status Transitions
```
DRAFT → ACTIVE (requires validation)
ACTIVE → PAUSED
PAUSED → ACTIVE
Any status → ARCHIVED
Auto: DRAFT → COMPLETED (when end date reached)
```

### Promotion Rules
- Can only add promotions to DRAFT campaigns
- Cannot add promotions to ACTIVE campaigns
- Promotion dates must be within campaign dates
- No discount stacking (apply highest discount only)

### Discount Calculation
```
Percentage: discount = originalPrice × (discountValue / 100)
Fixed: discount = discountValue
Applied to: subtotal (before shipping/tax)
```

## 🚨 Error Handling

### HTTP Status Codes
- `200 OK` - Successful request
- `400 Bad Request` - Validation error (invalid dates, negative budget, etc.)
- `401 Unauthorized` - Missing/invalid authentication
- `403 Forbidden` - User lacks permission
- `404 Not Found` - Resource not found
- `409 Conflict` - Business rule violation (e.g., adding promotion to non-DRAFT campaign)
- `500 Internal Server Error` - Server error

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

## 📝 Example Requests & Responses

### Create Campaign
```bash
POST /api/v1/campaigns
Content-Type: application/json

{
  "campaignName": "Đại hạ giá Tết Nguyên Đán 2025",
  "budget": 50000000,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "status": "DRAFT",
  "targeting": {
    "pages": ["home", "products"],
    "frequency": "once-per-session",
    "delayMs": 3000,
    "autoDismissMs": 15000
  }
}

Response (200):
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "campaignName": "Đại hạ giá Tết Nguyên Đán 2025",
    "budget": 50000000,
    "status": "DRAFT",
    "createdAt": "2025-01-01T10:00:00Z"
  }
}
```

### Apply Voucher to Cart
```bash
POST /api/v1/cart/apply-voucher
Content-Type: application/json

{
  "couponCode": "TET2025-ABC123"
}

Response (200):
{
  "success": true,
  "data": {
    "cartId": "cart-001",
    "appliedCoupon": "TET2025-ABC123",
    "discountAmount": 400000,
    "subtotal": 1000000,
    "totalAmount": 600000
  }
}
```

## 🗄️ Database Schema

See **IMPLEMENTATION_GUIDE.md** for complete SQL schema including:
- Campaign table
- Promotion table
- PromotionProduct junction table
- Voucher table
- CampaignImpressions table
- CampaignClicks table
- VoucherRedemptions table

## 📋 Implementation Checklist

- [ ] Create Campaign entity and DbContext
- [ ] Create Promotion entity and DbContext
- [ ] Create PromotionProduct junction entity
- [ ] Create Voucher entity and DbContext
- [ ] Create analytics tables (Impressions, Clicks, Redemptions)
- [ ] Implement Campaign CRUD endpoints
- [ ] Implement Promotion CRUD endpoints
- [ ] Implement Voucher generation endpoint
- [ ] Implement Voucher application logic in Cart service
- [ ] Implement Campaign tracking endpoints
- [ ] Implement Campaign statistics endpoint
- [ ] Add validation for all business rules
- [ ] Add error handling and logging
- [ ] Add unit tests
- [ ] Add integration tests
- [ ] Document API in Swagger/OpenAPI
- [ ] Test with FE AdPopup component

## 🔗 Frontend Integration

The FE AdPopup component expects:

1. **Campaign Fetching**: GET /api/v1/campaigns with targeting filters
2. **Impression Tracking**: POST /api/v1/campaigns/{id}/track-impression when shown
3. **Click Tracking**: POST /api/v1/campaigns/{id}/track-click when CTA clicked
4. **Voucher Application**: POST /api/v1/cart/apply-voucher when code applied
5. **Analytics**: GET /api/v1/campaigns/{id}/stats for admin dashboard

## 📚 Additional Resources

- **api-contract.json** - Complete JSON contract with all endpoints
- **API_ENDPOINTS_SUMMARY.md** - Detailed endpoint documentation
- **MOCK_DATA_EXAMPLES.md** - Real-world examples and test data
- **IMPLEMENTATION_GUIDE.md** - Database schema and implementation details

## 🎓 Key Concepts

### Campaign Lifecycle
1. **DRAFT** - Campaign created, promotions can be added
2. **ACTIVE** - Campaign is live, no new promotions can be added
3. **PAUSED** - Campaign temporarily stopped
4. **COMPLETED** - Campaign ended (auto-transition when end date reached)
5. **ARCHIVED** - Campaign archived for historical reference

### Discount Types
- **Percentage**: Discount as % of original price (e.g., 40% off)
- **Fixed**: Discount as fixed amount (e.g., 100,000 VND off)

### Targeting Frequency
- **once-per-session**: Show campaign only once per user session
- **once-per-page**: Show campaign only once per page per session
- **always**: Show campaign every time conditions are met

## 📞 Support

For questions or clarifications about this specification, refer to:
1. requirements.md - For business requirements
2. IMPLEMENTATION_GUIDE.md - For technical implementation details
3. MOCK_DATA_EXAMPLES.md - For example data and calculations

---

**Last Updated**: January 2025
**Version**: 1.0
**Status**: Ready for Implementation
