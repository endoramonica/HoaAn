# Campaign & Promotion API - Mock Data & Examples

## Campaign Examples

### Example 1: Tết 2025 Sale Campaign
```json
{
  "campaignId": "camp-tet-2025",
  "campaignName": "Đại hạ giá Tết Nguyên Đán 2025",
  "budget": 50000000,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "status": "ACTIVE",
  "description": "Mâm cúng gia tiên, hương nến cao cấp, giấy tiền vàng bạc. Giao hàng tận nhà trong ngày, tư vấn ngày giờ tốt miễn phí.",
  "targeting": {
    "pages": ["home", "products"],
    "frequency": "once-per-session",
    "delayMs": 3000,
    "autoDismissMs": 15000
  },
  "createdAt": "2024-12-15T10:00:00Z",
  "updatedAt": "2025-01-01T08:00:00Z"
}
```

### Example 2: VIP Membership Campaign
```json
{
  "campaignId": "camp-vip-member",
  "campaignName": "Trở thành thành viên VIP",
  "budget": 30000000,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-12-31T23:59:59Z",
  "status": "ACTIVE",
  "description": "Giảm giá 15% mọi đơn hàng, miễn phí vận chuyển, tư vấn phong thủy 24/7",
  "targeting": {
    "pages": ["cart", "profile"],
    "frequency": "once-per-session",
    "delayMs": 5000,
    "autoDismissMs": 20000
  },
  "createdAt": "2024-12-01T10:00:00Z",
  "updatedAt": "2024-12-01T10:00:00Z"
}
```

### Example 3: Lunar Calendar Banner Campaign
```json
{
  "campaignId": "camp-lunar-calendar",
  "campaignName": "Xem lịch âm - Chọn ngày tốt",
  "budget": 10000000,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-12-31T23:59:59Z",
  "status": "ACTIVE",
  "description": "Tư vấn miễn phí ngày giờ tốt cho mọi nghi lễ. Cập nhật lịch âm 2025 đầy đủ.",
  "targeting": {
    "pages": ["services", "about"],
    "frequency": "once-per-page",
    "delayMs": 2000,
    "autoDismissMs": 10000
  },
  "createdAt": "2024-12-01T10:00:00Z",
  "updatedAt": "2024-12-01T10:00:00Z"
}
```

---

## Promotion Examples

### Example 1: 40% Off Promotion
```json
{
  "promotionId": "promo-tet-40off",
  "campaignId": "camp-tet-2025",
  "discountValue": 40,
  "discountType": "percentage",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "minOrderValue": 500000,
  "maxUsage": 1000,
  "currentUsage": 150,
  "description": "40% off on all altar offerings",
  "createdAt": "2024-12-15T10:00:00Z"
}
```

### Example 2: 15% VIP Discount
```json
{
  "promotionId": "promo-vip-15off",
  "campaignId": "camp-vip-member",
  "discountValue": 15,
  "discountType": "percentage",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-12-31T23:59:59Z",
  "minOrderValue": 0,
  "maxUsage": null,
  "currentUsage": 0,
  "description": "15% off for VIP members on all purchases",
  "createdAt": "2024-12-01T10:00:00Z"
}
```

### Example 3: Fixed Amount Discount
```json
{
  "promotionId": "promo-fixed-100k",
  "campaignId": "camp-tet-2025",
  "discountValue": 100000,
  "discountType": "fixed",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "minOrderValue": 1000000,
  "maxUsage": 500,
  "currentUsage": 75,
  "description": "100,000 VND off orders over 1,000,000 VND",
  "createdAt": "2024-12-15T10:00:00Z"
}
```

---

## Voucher Examples

### Generated Voucher Codes
```json
{
  "vouchersGenerated": 100,
  "promotionId": "promo-tet-40off",
  "codes": [
    "TET2025-ABC123",
    "TET2025-DEF456",
    "TET2025-GHI789",
    "TET2025-JKL012",
    "TET2025-MNO345",
    "TET2025-PQR678",
    "TET2025-STU901",
    "TET2025-VWX234",
    "TET2025-YZA567",
    "TET2025-BCD890"
  ],
  "createdAt": "2025-01-01T10:00:00Z"
}
```

### Voucher Record
```json
{
  "voucherId": "vouch-001",
  "promotionId": "promo-tet-40off",
  "code": "TET2025-ABC123",
  "expiryDate": "2025-02-15T23:59:59Z",
  "isUsed": true,
  "usedAt": "2025-01-15T14:30:00Z",
  "usedBy": "cust-12345",
  "createdAt": "2025-01-01T10:00:00Z"
}
```

---

## Cart with Applied Voucher

### Before Voucher
```json
{
  "cartId": "cart-001",
  "items": [
    {
      "cartItemId": "item-001",
      "productId": "prod-001",
      "productName": "Mâm cúng gia tiên cao cấp",
      "quantity": 1,
      "unitPrice": 1000000,
      "totalPrice": 1000000
    }
  ],
  "subtotal": 1000000,
  "taxAmount": 0,
  "shippingFee": 50000,
  "discountAmount": 0,
  "totalAmount": 1050000
}
```

### After Applying 40% Voucher
```json
{
  "cartId": "cart-001",
  "items": [
    {
      "cartItemId": "item-001",
      "productId": "prod-001",
      "productName": "Mâm cúng gia tiên cao cấp",
      "quantity": 1,
      "unitPrice": 1000000,
      "totalPrice": 1000000
    }
  ],
  "subtotal": 1000000,
  "taxAmount": 0,
  "shippingFee": 50000,
  "discountAmount": 400000,
  "appliedCoupon": "TET2025-ABC123",
  "totalAmount": 650000
}
```

---

## Campaign Statistics Example

```json
{
  "campaignId": "camp-tet-2025",
  "campaignName": "Đại hạ giá Tết Nguyên Đán 2025",
  "status": "ACTIVE",
  "impressions": 5000,
  "clicks": 450,
  "clickThroughRate": 9.0,
  "redemptions": 150,
  "redemptionRate": 3.0,
  "totalRevenue": 60000000,
  "totalDiscount": 24000000,
  "conversionRate": 33.3,
  "averageOrderValue": 400000,
  "fromDate": "2025-01-01",
  "toDate": "2025-02-15",
  "generatedAt": "2025-02-15T23:59:59Z"
}
```

---

## Discount Calculation Examples

### Percentage Discount
```
Original Price: 1,000,000 VND
Discount: 40%
Calculation: 1,000,000 × (40 / 100) = 400,000 VND
Final Price: 1,000,000 - 400,000 = 600,000 VND
```

### Fixed Amount Discount
```
Original Price: 1,000,000 VND
Discount: 100,000 VND
Calculation: 1,000,000 - 100,000 = 900,000 VND
Final Price: 900,000 VND
```

### Multiple Items with Discount
```
Item 1: 500,000 VND
Item 2: 500,000 VND
Subtotal: 1,000,000 VND
Discount (40%): 400,000 VND
Shipping: 50,000 VND
Total: 1,000,000 - 400,000 + 50,000 = 650,000 VND
```

---

## Error Response Examples

### Invalid Voucher Code
```json
{
  "success": false,
  "message": "Voucher code is invalid or expired",
  "errorCode": "INVALID_VOUCHER",
  "errors": {
    "couponCode": ["The voucher code 'INVALID123' does not exist or has expired"]
  }
}
```

### Voucher Limit Exceeded
```json
{
  "success": false,
  "message": "Voucher code has reached its usage limit",
  "errorCode": "VOUCHER_LIMIT_EXCEEDED",
  "errors": {
    "couponCode": ["The voucher code 'TET2025-ABC123' has been used 1000 times and cannot be used again"]
  }
}
```

### Minimum Order Value Not Met
```json
{
  "success": false,
  "message": "Cart total must be at least 500,000 to use this voucher",
  "errorCode": "MIN_ORDER_VALUE_NOT_MET",
  "errors": {
    "cartTotal": ["Current cart total (300,000) is below minimum required (500,000)"]
  }
}
```

### Campaign Not in DRAFT Status
```json
{
  "success": false,
  "message": "Cannot add promotions to a non-DRAFT campaign",
  "errorCode": "CAMPAIGN_NOT_DRAFT",
  "errors": {
    "campaignStatus": ["Campaign must be in DRAFT status to add promotions. Current status: ACTIVE"]
  }
}
```

### Invalid Date Range
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "endDate": ["End date must be after start date"],
    "startDate": ["Start date must be before end date"]
  }
}
```

### Negative Budget
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "budget": ["Budget must be greater than or equal to 0"]
  }
}
```

---

## Pagination Example

### Request
```
GET /api/v1/campaigns?pageNumber=2&pageSize=5&status=ACTIVE
```

### Response
```json
{
  "success": true,
  "data": {
    "items": [
      { "campaignId": "camp-006", "campaignName": "Campaign 6", ... },
      { "campaignId": "camp-007", "campaignName": "Campaign 7", ... },
      { "campaignId": "camp-008", "campaignName": "Campaign 8", ... },
      { "campaignId": "camp-009", "campaignName": "Campaign 9", ... },
      { "campaignId": "camp-010", "campaignName": "Campaign 10", ... }
    ],
    "pageNumber": 2,
    "pageSize": 5,
    "totalItems": 25,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": true
  }
}
```

---

## Tracking Events Example

### Impression Tracking
```json
{
  "campaignId": "camp-tet-2025",
  "impressionId": "imp-12345",
  "sessionId": "sess-abc123",
  "page": "home",
  "recordedAt": "2025-01-15T10:30:00Z"
}
```

### Click Tracking
```json
{
  "campaignId": "camp-tet-2025",
  "clickId": "click-12345",
  "sessionId": "sess-abc123",
  "page": "home",
  "recordedAt": "2025-01-15T10:30:05Z"
}
```

### Redemption Tracking
```json
{
  "redemptionId": "redeem-12345",
  "voucherId": "vouch-001",
  "promotionId": "promo-tet-40off",
  "orderId": "order-12345",
  "discountAmount": 400000,
  "redeemedAt": "2025-01-15T14:30:00Z"
}
```

