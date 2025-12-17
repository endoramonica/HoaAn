# Frontend Quick Start - 5 Minute Overview

## 🎯 Two Systems, One Goal

### System 1: Campaign & Promotion API
**Purpose:** Display marketing campaigns with discount vouchers
**Auth:** Public (no token needed)

### System 2: Marketing Post API
**Purpose:** Display marketing posts with product information
**Auth:** Public for analytics, Admin JWT for management

---

## 🚀 Quick Integration (Copy-Paste Ready)

### 1. Display Campaign Popup

```typescript
// Fetch campaigns
const campaigns = await fetch('/api/v1/campaigns?status=ACTIVE')
  .then(r => r.json());

// Show popup
campaigns.data.forEach(campaign => {
  if (campaign.targeting.pages.includes('home')) {
    setTimeout(() => {
      showPopup(campaign);
      
      // Track impression
      fetch(`/api/v1/campaigns/${campaign.campaignId}/track-impression`, {
        method: 'POST',
        body: JSON.stringify({
          sessionId: 'user-session-id',
          page: 'home',
          timestamp: new Date().toISOString()
        })
      });
    }, campaign.targeting.delayMs);
  }
});
```

### 2. Apply Voucher Code

```typescript
const applyVoucher = async (code: string) => {
  const result = await fetch('/api/v1/cart/apply-voucher', {
    method: 'POST',
    body: JSON.stringify({ couponCode: code })
  }).then(r => r.json());
  
  if (result.success) {
    // Update cart
    setCart({
      subtotal: result.data.subtotal,
      discount: result.data.discountAmount,
      total: result.data.totalAmount
    });
  } else {
    // Show error
    alert(result.message);
  }
};
```

### 3. Display Marketing Posts

```typescript
// Fetch posts
const posts = await fetch('/api/admin/marketing-posts?status=Published')
  .then(r => r.json());

// Display with product info
posts.data.items.forEach(post => {
  const product = post.taggedProduct; // LIVE product data!
  
  console.log({
    title: post.title,
    productName: product?.name,
    productPrice: product?.formattedPrice,
    productImage: product?.thumbnailUrl,
    discount: product?.discountPercentage
  });
});
```

### 4. Track Post Analytics

```typescript
// Track view
fetch(`/api/admin/marketing-posts/${postId}/analytics/views`, {
  method: 'POST'
});

// Track click
fetch(`/api/admin/marketing-posts/${postId}/analytics/clicks`, {
  method: 'POST'
});

// Track share
fetch(`/api/admin/marketing-posts/${postId}/analytics/shares`, {
  method: 'POST'
});
```

---

## 📊 Key Data Structures

### Campaign
```typescript
{
  campaignId: "camp-001",
  campaignName: "Tết 2025 Sale",
  status: "ACTIVE",
  targeting: {
    pages: ["home", "products"],
    frequency: "once-per-session",
    delayMs: 3000,
    autoDismissMs: 15000
  }
}
```

### Marketing Post (with TaggedProduct)
```typescript
{
  id: "post-001",
  title: "Amazing Product",
  image: "https://...",
  
  // LIVE product data
  taggedProduct: {
    id: "prod-001",
    name: "Product Name",
    price: 1000000,
    formattedPrice: "1,000,000 VND",
    thumbnailUrl: "https://...",
    hasDiscount: true,
    discountPercentage: 20
  },
  
  views: 150,
  clicks: 25,
  shares: 5
}
```

---

## ⚡ Key Points

### TaggedProduct is LIVE
- Product data updates in real-time
- Price, discount, images are current
- No caching needed
- Null if product deleted

### Campaign Targeting
- `pages`: Show only on specific pages
- `frequency`: once-per-session, once-per-page, always
- `delayMs`: Wait before showing
- `autoDismissMs`: Auto-close after X ms

### Analytics Endpoints (Public)
- No authentication needed
- Track views, clicks, shares
- Called from frontend directly

---

## 🔗 API Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/v1/campaigns` | GET | List campaigns |
| `/api/v1/campaigns/{id}/track-impression` | POST | Track view |
| `/api/v1/campaigns/{id}/track-click` | POST | Track click |
| `/api/v1/cart/apply-voucher` | POST | Apply code |
| `/api/admin/marketing-posts` | GET | List posts |
| `/api/admin/marketing-posts/{id}` | GET | Get post details |
| `/api/admin/marketing-posts/{id}/analytics/views` | POST | Track view |
| `/api/admin/marketing-posts/{id}/analytics/clicks` | POST | Track click |
| `/api/admin/marketing-posts/{id}/analytics/shares` | POST | Track share |

---

## 🧪 Test These

1. ✅ Fetch campaigns and display popup
2. ✅ Apply valid voucher code
3. ✅ Apply invalid voucher code (should error)
4. ✅ Fetch marketing posts
5. ✅ Verify TaggedProduct has product data
6. ✅ Track post views/clicks/shares
7. ✅ Check campaign analytics

---

## 📖 Full Documentation

See `FRONTEND_INTEGRATION_GUIDE.md` for:
- Complete workflows
- Component examples
- Error handling
- Authentication setup
- Testing checklist

---

**Ready to integrate?** Start with the Quick Integration section above! 🚀
