# Campaign & Promotion API - Quick Reference Card

## 🚀 Quick Start

### 1. Create a Campaign
```bash
POST /api/v1/campaigns
{
  "campaignName": "Tết 2025 Sale",
  "budget": 50000000,
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "status": "DRAFT"
}
```

### 2. Create a Promotion
```bash
POST /api/v1/campaigns/{campaignId}/promotions
{
  "discountValue": 40,
  "discountType": "percentage",
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-02-15T23:59:59Z",
  "maxUsage": 1000
}
```

### 3. Generate Voucher Codes
```bash
POST /api/v1/promotions/{promotionId}/vouchers/generate
{
  "quantity": 100,
  "prefix": "TET2025",
  "expiryDate": "2025-02-15T23:59:59Z"
}
```

### 4. Apply Voucher to Cart
```bash
POST /api/v1/cart/apply-voucher
{
  "couponCode": "TET2025-ABC123"
}
```

---

## 📊 All Endpoints at a Glance

| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/v1/campaigns` | Create campaign |
| GET | `/api/v1/campaigns` | List campaigns |
| PUT | `/api/v1/campaigns/{id}` | Update campaign |
| DELETE | `/api/v1/campaigns/{id}` | Delete campaign |
| POST | `/api/v1/campaigns/{id}/promotions` | Create promotion |
| GET | `/api/v1/campaigns/{id}/promotions` | List promotions |
| PUT | `/api/v1/promotions/{id}` | Update promotion |
| DELETE | `/api/v1/promotions/{id}` | Delete promotion |
| POST | `/api/v1/promotions/{id}/link-products` | Link products |
| GET | `/api/v1/promotions/{id}/products` | Get linked products |
| POST | `/api/v1/promotions/{id}/vouchers/generate` | Generate vouchers |
| POST | `/api/v1/cart/apply-voucher` | Apply voucher |
| POST | `/api/v1/cart/remove-voucher` | Remove voucher |
| POST | `/api/v1/campaigns/{id}/track-impression` | Track impression |
| POST | `/api/v1/campaigns/{id}/track-click` | Track click |
| GET | `/api/v1/campaigns/{id}/stats` | Get statistics |

---

## ✅ Validation Checklist

### Campaign
- [ ] CampaignName: Required, max 255 chars
- [ ] Budget: >= 0
- [ ] EndDate > StartDate
- [ ] Status: DRAFT|ACTIVE|PAUSED|COMPLETED|ARCHIVED

### Promotion
- [ ] DiscountValue: > 0
- [ ] DiscountType: percentage|fixed
- [ ] EndDate > StartDate
- [ ] Campaign is DRAFT (to add promotions)

### Voucher
- [ ] Code: Unique
- [ ] ExpiryDate: Future date
- [ ] Usage <= MaxUsage

---

## 🔴 Common Errors

| Error | Status | Cause | Fix |
|-------|--------|-------|-----|
| EndDate <= StartDate | 400 | Invalid date range | Ensure EndDate > StartDate |
| Budget < 0 | 400 | Negative budget | Use budget >= 0 |
| CAMPAIGN_NOT_DRAFT | 409 | Adding promotion to non-DRAFT | Campaign must be DRAFT |
| INVALID_VOUCHER | 400 | Invalid/expired code | Check code and expiry |
| VOUCHER_LIMIT_EXCEEDED | 409 | Usage limit reached | Code has been used too many times |
| MIN_ORDER_VALUE_NOT_MET | 400 | Cart too small | Increase cart total |

---

## 💰 Discount Calculation

### Percentage Discount
```
Discount = Price × (DiscountValue / 100)
Example: 1,000,000 × (40 / 100) = 400,000
Final: 1,000,000 - 400,000 = 600,000
```

### Fixed Discount
```
Discount = DiscountValue
Example: 1,000,000 - 100,000 = 900,000
```

---

## 📈 Campaign Status Flow

```
DRAFT ──→ ACTIVE ──→ PAUSED ──→ ACTIVE
  ↓                              ↓
  └──────────────→ ARCHIVED ←────┘
                      ↑
                      │
                   (auto when
                   end date
                   reached)
                      │
                   COMPLETED
```

---

## 🎯 Targeting Options

### Pages
```json
"pages": ["home", "products", "cart", "profile", "services", "about"]
```

### Frequency
- `once-per-session` - Show once per user session
- `once-per-page` - Show once per page per session
- `always` - Show every time

### Timing
```json
"delayMs": 3000,        // Show after 3 seconds
"autoDismissMs": 15000  // Auto-close after 15 seconds
```

---

## 📊 Response Format

### Success (200)
```json
{
  "success": true,
  "data": { /* entity */ },
  "message": "Operation successful"
}
```

### Error (400/409)
```json
{
  "success": false,
  "message": "Error description",
  "errorCode": "ERROR_CODE",
  "errors": {
    "field": ["Field error message"]
  }
}
```

---

## 🔑 Key IDs

| ID | Format | Example |
|----|--------|---------|
| Campaign | `camp-{guid}` | `camp-tet-2025` |
| Promotion | `promo-{guid}` | `promo-40off` |
| Voucher | `vouch-{guid}` | `vouch-001` |
| Impression | `imp-{guid}` | `imp-12345` |
| Click | `click-{guid}` | `click-12345` |

---

## 🧪 Test Scenarios

### Scenario 1: Create and Activate Campaign
```
1. POST /campaigns → DRAFT
2. POST /campaigns/{id}/promotions → Add promotion
3. PUT /campaigns/{id} → Change status to ACTIVE
4. POST /promotions/{id}/vouchers/generate → Generate codes
5. POST /cart/apply-voucher → Apply code
```

### Scenario 2: Track Campaign Performance
```
1. POST /campaigns/{id}/track-impression → User sees campaign
2. POST /campaigns/{id}/track-click → User clicks CTA
3. POST /cart/apply-voucher → User applies voucher
4. GET /campaigns/{id}/stats → View performance
```

### Scenario 3: Error Handling
```
1. POST /campaigns with EndDate < StartDate → 400 error
2. POST /promotions to ACTIVE campaign → 409 error
3. POST /cart/apply-voucher with invalid code → 400 error
4. POST /cart/apply-voucher with expired code → 400 error
```

---

## 🔗 Related Services

### Cart Service
- Apply voucher: `POST /api/v1/cart/apply-voucher`
- Remove voucher: `POST /api/v1/cart/remove-voucher`
- Get cart: `GET /api/v1/cart`

### Order Service
- Create order: `POST /api/v1/checkout/process`
- Track redemption: Automatic when order created with voucher

### Product Service
- Link products: `POST /api/v1/promotions/{id}/link-products`
- Get products: `GET /api/v1/promotions/{id}/products`

---

## 📱 Frontend Integration

### AdPopup Component Expects
1. Campaign data with targeting rules
2. Impression tracking endpoint
3. Click tracking endpoint
4. Voucher application endpoint

### Example Flow
```
1. FE fetches campaigns: GET /api/v1/campaigns
2. FE shows campaign based on targeting rules
3. FE tracks impression: POST /api/v1/campaigns/{id}/track-impression
4. User clicks CTA
5. FE tracks click: POST /api/v1/campaigns/{id}/track-click
6. User applies voucher: POST /api/v1/cart/apply-voucher
7. FE shows discount in cart
```

---

## 🛠️ Debugging Tips

1. **Check campaign status** - Can only add promotions to DRAFT campaigns
2. **Verify date ranges** - EndDate must be > StartDate
3. **Check voucher expiry** - Voucher must not be expired
4. **Verify usage limits** - Check if voucher has reached maxUsage
5. **Check minimum order value** - Cart total must meet minimum
6. **Verify targeting rules** - Campaign must target current page
7. **Check frequency** - Campaign may have already been shown this session

---

## 📞 Common Questions

**Q: Can I add promotions to an ACTIVE campaign?**
A: No, only DRAFT campaigns can have promotions added. Change status to DRAFT first.

**Q: Can I use multiple vouchers on one order?**
A: No, only one voucher per order. The highest discount is applied.

**Q: What happens when a campaign end date is reached?**
A: Status automatically transitions from DRAFT to COMPLETED.

**Q: Can I reuse a voucher code?**
A: No, each voucher code can only be used once (unless marked as reusable).

**Q: How do I track campaign performance?**
A: Use GET /api/v1/campaigns/{id}/stats to get impressions, clicks, and redemptions.

---

## 📚 Full Documentation

- **README.md** - Overview and key features
- **requirements.md** - Detailed requirements
- **API_ENDPOINTS_SUMMARY.md** - Complete endpoint documentation
- **MOCK_DATA_EXAMPLES.md** - Example data and calculations
- **IMPLEMENTATION_GUIDE.md** - Database schema and implementation details
- **api-contract.json** - JSON API contract

---

**Last Updated**: January 2025
**Version**: 1.0
