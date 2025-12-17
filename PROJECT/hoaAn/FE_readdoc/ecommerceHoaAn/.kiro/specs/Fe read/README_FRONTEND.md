# Frontend Documentation - Complete Integration Package

## 📦 What's Included

This package contains everything the frontend team needs to integrate with two backend systems:

1. **Campaign & Promotion Management API** - Marketing campaigns with vouchers
2. **Admin Marketing Post Management API** - Marketing posts with product integration

---

## 📚 Documentation Files

### 1. **FRONTEND_QUICK_START.md** ⚡
**Start here!** 5-minute overview with copy-paste ready code examples.

**Contains:**
- Quick integration examples
- Key data structures
- API endpoints table
- Testing checklist

**Best for:** Getting started quickly, understanding the basics

---

### 2. **FRONTEND_INTEGRATION_GUIDE.md** 📖
Complete integration guide with detailed workflows and component examples.

**Contains:**
- System architecture overview
- Complete workflows (4 detailed examples)
- UI component examples (React/TypeScript)
- Authentication setup
- Error handling patterns
- Testing checklist

**Best for:** Understanding how to integrate, building components

---

### 3. **FRONTEND_API_REFERENCE.md** 🔌
Complete API endpoint documentation with request/response examples.

**Contains:**
- All endpoints documented
- Request/response examples for each endpoint
- Query parameters explained
- Error codes reference
- HTTP status codes
- Rate limiting info
- Pagination details

**Best for:** API implementation, debugging, reference

---

### 4. **frontend-types.ts** 💻
TypeScript types file for type-safe frontend development.

**Contains:**
- All TypeScript interfaces
- Type guards
- Utility functions
- Helper functions
- Constants
- Error types

**Best for:** TypeScript projects, IDE autocomplete, type safety

---

## 🚀 Getting Started (3 Steps)

### Step 1: Read Quick Start (5 minutes)
```bash
Open: FRONTEND_QUICK_START.md
```

### Step 2: Copy TypeScript Types (1 minute)
```bash
Copy: frontend-types.ts to your project
```

### Step 3: Implement First Feature (30 minutes)
```bash
Follow: FRONTEND_INTEGRATION_GUIDE.md → Workflow 1
```

---

## 🎯 Key Concepts

### Two Separate Systems

**Campaign & Promotion API**
- Purpose: Display marketing campaigns with discount vouchers
- Auth: Public (no token needed)
- Main use: AdPopup component, voucher application

**Marketing Post API**
- Purpose: Display marketing posts with product information
- Auth: Public for analytics, Admin JWT for management
- Main use: Post feed, product promotion

### TaggedProduct - The Key Difference

Marketing Posts include **live product data** in responses:

```typescript
// When you fetch a post with productId:
{
  id: "post-001",
  productId: "prod-001",
  taggedProduct: {
    id: "prod-001",
    name: "Product Name",
    price: 1000000,           // LIVE - updates in real-time
    formattedPrice: "1,000,000 VND",
    thumbnailUrl: "https://...",
    hasDiscount: true,
    discountPercentage: 20    // LIVE - updates when discount changes
  }
}
```

**Important:** Product data is **always fresh** - no caching needed!

---

## 📊 API Endpoints Quick Reference

### Campaign & Promotion
| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/v1/campaigns` | GET | List campaigns |
| `/api/v1/campaigns/{id}/track-impression` | POST | Track view |
| `/api/v1/campaigns/{id}/track-click` | POST | Track click |
| `/api/v1/cart/apply-voucher` | POST | Apply code |
| `/api/v1/cart/remove-voucher` | POST | Remove code |
| `/api/v1/campaigns/{id}/stats` | GET | Get analytics |

### Marketing Posts
| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/admin/marketing-posts` | GET | List posts |
| `/api/admin/marketing-posts/{id}` | GET | Get post details |
| `/api/admin/marketing-posts/{id}/analytics/views` | POST | Track view |
| `/api/admin/marketing-posts/{id}/analytics/clicks` | POST | Track click |
| `/api/admin/marketing-posts/{id}/analytics/shares` | POST | Track share |

---

## 💡 Common Workflows

### 1. Display Campaign Popup
```typescript
// Fetch campaigns → Filter by targeting → Show popup → Track impression
See: FRONTEND_INTEGRATION_GUIDE.md → Workflow 1
```

### 2. Apply Voucher Code
```typescript
// User enters code → Apply voucher → Update cart → Show discount
See: FRONTEND_INTEGRATION_GUIDE.md → Workflow 2
```

### 3. Display Marketing Posts
```typescript
// Fetch posts → Display with product info → Track analytics
See: FRONTEND_INTEGRATION_GUIDE.md → Workflow 3
```

### 4. Track Post Analytics
```typescript
// Track views, clicks, shares as users interact
See: FRONTEND_INTEGRATION_GUIDE.md → Workflow 4
```

---

## 🔐 Authentication

### Public Endpoints (No Auth)
- Campaign fetching
- Campaign tracking (impressions, clicks)
- Voucher application
- Post analytics tracking

### Protected Endpoints (Admin JWT)
- Post management (create, update, delete)
- Post statistics

**How to authenticate:**
```typescript
const headers = {
  'Authorization': `Bearer ${jwtToken}`,
  'Content-Type': 'application/json'
};
```

---

## 🧪 Testing Checklist

- [ ] Fetch campaigns and display popup
- [ ] Apply valid voucher code
- [ ] Apply invalid voucher code (should error)
- [ ] Fetch marketing posts
- [ ] Verify TaggedProduct has product data
- [ ] Track post views/clicks/shares
- [ ] Check campaign analytics
- [ ] Test error handling
- [ ] Test pagination
- [ ] Test filtering

---

## 📱 Component Examples

### AdPopup Component
```typescript
See: FRONTEND_INTEGRATION_GUIDE.md → UI Component Integration
```

### Marketing Post Card Component
```typescript
See: FRONTEND_INTEGRATION_GUIDE.md → UI Component Integration
```

---

## 🔗 Live Documentation

### Swagger UI
### OpenAPI Specification

## 📋 File Organization

```
.kiro/specs/
├── README_FRONTEND.md                    ← You are here
├── FRONTEND_QUICK_START.md               ← Start here!
├── FRONTEND_INTEGRATION_GUIDE.md         ← Detailed guide
├── FRONTEND_API_REFERENCE.md             ← API docs
├── frontend-types.ts                     ← TypeScript types
│
├── campaign-promotion-api/               ← Campaign API spec
│   ├── README.md
│   ├── requirements.md
│   ├── API_ENDPOINTS_SUMMARY.md
│   └── ...
│
└── admin-marketing-post/                 ← Marketing Post spec
    ├── design.md
    ├── requirements.md
    └── ...
```

---

## 🎓 Learning Path

### For Quick Integration (1-2 hours)
1. Read FRONTEND_QUICK_START.md (5 min)
2. Copy frontend-types.ts (1 min)
3. Implement first workflow (30 min)
4. Test with Swagger UI (30 min)

### For Complete Understanding (4-6 hours)
1. Read FRONTEND_QUICK_START.md (5 min)
2. Read FRONTEND_INTEGRATION_GUIDE.md (1 hour)
3. Review FRONTEND_API_REFERENCE.md (1 hour)
4. Implement all workflows (2-3 hours)
5. Test thoroughly (1 hour)

### For Reference (ongoing)
- FRONTEND_API_REFERENCE.md - For endpoint details
- frontend-types.ts - For type definitions
- FRONTEND_INTEGRATION_GUIDE.md - For patterns

---

## ❓ FAQ

### Q: Do I need to cache product data?
**A:** No! TaggedProduct is always fresh. Backend fetches live data on every request.

### Q: Can I use both APIs together?
**A:** Yes! Campaign API for promotions, Marketing Post API for content. They work independently.

### Q: How do I track analytics?
**A:** Use the public analytics endpoints - no authentication needed. Call them directly from frontend.

### Q: What if a product is deleted?
**A:** Post remains, but TaggedProduct becomes null. Post data is preserved.

### Q: How do I handle errors?
**A:** Check `success` field in response. If false, read `message` and `errorCode` fields.

### Q: Do I need JWT for everything?
**A:** No! Only for admin operations (create/update/delete posts). Fetching and tracking are public.

---

## 🐛 Troubleshooting

### Campaign not showing?
- Check targeting.pages includes current page
- Check campaign status is ACTIVE
- Check current date is within startDate/endDate
- Check targeting.frequency rules

### Voucher not applying?
- Check voucher code is correct
- Check voucher is not expired
- Check cart total meets minimum order value
- Check voucher hasn't reached usage limit

### Product data not showing?
- Check post has productId
- Check product still exists in catalog
- Fetch post details (not list view) for full data
- Check TaggedProduct is not null

### Analytics not tracking?
- Check you're calling the correct endpoint
- Check post/campaign ID is correct
- Check endpoint is public (no auth required)
- Check response.success is true

---

## 📞 Support

### For API Issues
- Check FRONTEND_API_REFERENCE.md
- Check Swagger UI: http://localhost:5000/swagger/ui
- Check error codes in API_REFERENCE.md

### For Integration Issues
- Check FRONTEND_INTEGRATION_GUIDE.md
- Check example workflows
- Check component examples

### For Type Issues
- Check frontend-types.ts
- Check TypeScript interfaces
- Check type guards

---

## 🎉 You're Ready!

Everything you need is in this package. Start with FRONTEND_QUICK_START.md and you'll be integrating in minutes!

---

**Last Updated:** January 2025
**Version:** 1.0
**Status:** Ready for Frontend Integration

**Questions?** Check the relevant documentation file above! 📚
