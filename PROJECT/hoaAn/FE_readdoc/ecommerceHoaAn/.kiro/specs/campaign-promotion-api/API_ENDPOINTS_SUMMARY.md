# Campaign & Promotion API - Endpoints Summary

## 1. Campaign Management

### 1.1 Create Campaign
```
POST /api/v1/campaigns
Status: 200, 400, 401, 403

Request:
{
  "campaignName": "Đại hạ giá Tết Nguyên Đán 2025",
  "budget": 50000000,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "status": "DRAFT",
  "description": "Mâm cúng gia tiên, hương nến cao cấp",
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
    "startDate": "2025-01-01T00:00:00Z",
    "endDate": "2025-02-15T23:59:59Z",
    "status": "DRAFT",
    "targeting": {...},
    "createdAt": "2025-01-01T10:00:00Z"
  }
}

Error (400): EndDate <= StartDate or Budget < 0
Error (409): Campaign status conflict
```

### 1.2 Get Campaigns (Paginated)
```
GET /api/v1/campaigns?status=ACTIVE&pageNumber=1&pageSize=10
Status: 200, 401, 403

Query Params:
- status: DRAFT|ACTIVE|PAUSED|COMPLETED|ARCHIVED
- pageNumber: integer (default: 1)
- pageSize: integer (default: 10)
- fromDate: ISO 8601 date
- toDate: ISO 8601 date

Response (200):
{
  "success": true,
  "data": {
    "items": [
      {
        "campaignId": "camp-001",
        "campaignName": "...",
        "budget": 50000000,
        "status": "ACTIVE",
        ...
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalItems": 25,
    "totalPages": 3
  }
}
```

### 1.3 Update Campaign
```
PUT /api/v1/campaigns/{campaignId}
Status: 200, 400, 404, 409

Request:
{
  "campaignName": "Updated Name",
  "budget": 60000000,
  "status": "ACTIVE"
}

Response (200): Updated campaign object
Error (404): Campaign not found
Error (409): Cannot transition from current status
```

### 1.4 Delete Campaign
```
DELETE /api/v1/campaigns/{campaignId}
Status: 204, 404, 409

Response (204): No content
Error (404): Campaign not found
Error (409): Cannot delete active campaign
```

---

## 2. Promotion Management

### 2.1 Create Promotion
```
POST /api/v1/campaigns/{campaignId}/promotions
Status: 200, 400, 404, 409

Request:
{
  "discountValue": 40,
  "discountType": "percentage",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "minOrderValue": 500000,
  "maxUsage": 1000,
  "description": "40% off on all altar offerings"
}

Response (200):
{
  "success": true,
  "data": {
    "promotionId": "promo-001",
    "campaignId": "camp-001",
    "discountValue": 40,
    "discountType": "percentage",
    "currentUsage": 0,
    "maxUsage": 1000,
    ...
  }
}

Error (409): Campaign is not in DRAFT status
Error (400): EndDate <= StartDate
```

### 2.2 Get Promotions for Campaign
```
GET /api/v1/campaigns/{campaignId}/promotions
Status: 200, 404

Response (200):
{
  "success": true,
  "data": [
    {
      "promotionId": "promo-001",
      "campaignId": "camp-001",
      "discountValue": 40,
      "discountType": "percentage",
      "currentUsage": 150,
      "maxUsage": 1000,
      ...
    }
  ]
}
```

### 2.3 Update Promotion
```
PUT /api/v1/promotions/{promotionId}
Status: 200, 400, 404

Request:
{
  "discountValue": 50,
  "maxUsage": 2000
}

Response (200): Updated promotion object
```

### 2.4 Delete Promotion
```
DELETE /api/v1/promotions/{promotionId}
Status: 204, 404, 409

Error (409): Cannot delete promotion with active vouchers
```

---

## 3. Promotion-Product Linking

### 3.1 Link Products to Promotion
```
POST /api/v1/promotions/{promotionId}/link-products
Status: 200, 400, 404

Request:
{
  "productIds": ["prod-001", "prod-002", "prod-003"]
}

Response (200):
{
  "success": true,
  "data": {
    "promotionId": "promo-001",
    "linkedProducts": 3,
    "productIds": ["prod-001", "prod-002", "prod-003"]
  }
}
```

### 3.2 Get Linked Products
```
GET /api/v1/promotions/{promotionId}/products
Status: 200, 404

Response (200):
{
  "success": true,
  "data": {
    "promotionId": "promo-001",
    "products": [
      {
        "productId": "prod-001",
        "productName": "Mâm cúng gia tiên cao cấp",
        "linkedAt": "2025-01-01T10:00:00Z"
      }
    ]
  }
}
```

---

## 4. Voucher Management

### 4.1 Generate Voucher Codes
```
POST /api/v1/promotions/{promotionId}/vouchers/generate
Status: 200, 400, 404

Request:
{
  "quantity": 100,
  "prefix": "TET2025",
  "expiryDate": "2025-02-15T23:59:59Z"
}

Response (200):
{
  "success": true,
  "data": {
    "vouchersGenerated": 100,
    "promotionId": "promo-001",
    "codes": [
      "TET2025-ABC123",
      "TET2025-DEF456",
      "TET2025-GHI789"
    ]
  }
}
```

### 4.2 Apply Voucher to Cart
```
POST /api/v1/cart/apply-voucher
Status: 200, 400, 409

Request:
{
  "couponCode": "TET2025-ABC123"
}

Response (200):
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
  }
}

Error (400): Invalid or expired voucher
Error (409): Voucher limit exceeded
Error (400): Minimum order value not met
```

### 4.3 Remove Voucher from Cart
```
POST /api/v1/cart/remove-voucher
Status: 200

Response (200):
{
  "success": true,
  "data": {
    "cartId": "cart-001",
    "appliedCoupon": null,
    "subtotal": 1000000,
    "discountAmount": 0,
    "totalAmount": 1000000
  }
}
```

---

## 5. Campaign Analytics & Tracking

### 5.1 Track Campaign Impression
```
POST /api/v1/campaigns/{campaignId}/track-impression
Status: 200

Request:
{
  "sessionId": "sess-12345",
  "page": "home",
  "timestamp": "2025-01-15T10:30:00Z"
}

Response (200):
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "impressionId": "imp-001",
    "recorded": true
  }
}
```

### 5.2 Track Campaign Click
```
POST /api/v1/campaigns/{campaignId}/track-click
Status: 200

Request:
{
  "sessionId": "sess-12345",
  "page": "home",
  "timestamp": "2025-01-15T10:30:05Z"
}

Response (200):
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "clickId": "click-001",
    "recorded": true
  }
}
```

### 5.3 Get Campaign Statistics
```
GET /api/v1/campaigns/{campaignId}/stats?fromDate=2025-01-01&toDate=2025-02-15
Status: 200, 404

Query Params:
- fromDate: ISO 8601 date
- toDate: ISO 8601 date

Response (200):
{
  "success": true,
  "data": {
    "campaignId": "camp-001",
    "campaignName": "Đại hạ giá Tết Nguyên Đán 2025",
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
  }
}
```

---

## Response Format Standards

### Success Response
```json
{
  "success": true,
  "data": { /* entity or array */ },
  "message": "Operation completed successfully"
}
```

### Error Response
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

### Validation Error (400)
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "endDate": ["End date must be after start date"],
    "budget": ["Budget must be greater than or equal to 0"]
  }
}
```

### Business Rule Violation (409)
```json
{
  "success": false,
  "message": "Cannot add promotions to a non-DRAFT campaign",
  "errorCode": "CAMPAIGN_NOT_DRAFT"
}
```

---

## Status Codes Reference

| Code | Meaning | When Used |
|------|---------|-----------|
| 200 | OK | Successful GET, POST, PUT |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Validation error, invalid input |
| 401 | Unauthorized | Missing/invalid authentication |
| 403 | Forbidden | User lacks permission |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Business rule violation |
| 500 | Server Error | Unexpected server error |

---

## Validation Rules Quick Reference

### Campaign
- ✓ CampaignName: Required, max 255 chars
- ✓ Budget: Required, >= 0
- ✓ StartDate < EndDate
- ✓ Status: DRAFT|ACTIVE|PAUSED|COMPLETED|ARCHIVED

### Promotion
- ✓ DiscountValue: Required, > 0
- ✓ DiscountType: percentage|fixed
- ✓ StartDate < EndDate
- ✓ Campaign must be DRAFT to add promotions

### Voucher
- ✓ Code: Unique, required
- ✓ ExpiryDate: Future date
- ✓ Cannot exceed MaxUsage
- ✓ Promotion must be active

