# Frontend Integration Guide - Campaign & Promotion + Marketing Posts

## 📋 Overview

This guide provides frontend developers with everything needed to integrate two backend systems:
1. **Campaign & Promotion Management API** - Marketing campaigns with promotions and vouchers
2. **Admin Marketing Post Management API** - Marketing posts with product integration

Both systems work together to provide a complete marketing solution with analytics tracking.

---

## 🎯 System Architecture

### Two Separate Systems

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend Application                      │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────┐      ┌──────────────────────┐    │
│  │  Campaign & Promo    │      │  Marketing Posts     │    │
│  │  Components          │      │  Components          │    │
│  │  - AdPopup           │      │  - Post Feed         │    │
│  │  - Voucher Apply     │      │  - Post Detail       │    │
│  │  - Analytics Track   │      │  - Post List         │    │
│  └──────────────────────┘      └──────────────────────┘    │
│           │                              │                   │
└───────────┼──────────────────────────────┼───────────────────┘
            │                              │
            ▼                              ▼
    ┌──────────────────┐          ┌──────────────────┐
    │  Campaign API    │          │  Marketing Post  │
    │  /api/v1/...     │          │  /api/admin/...  │
    └──────────────────┘          └──────────────────┘
            │                              │
            ▼                              ▼
    ┌──────────────────┐          ┌──────────────────┐
    │  Campaign DB     │          │  Marketing DB    │
    │  - Campaigns     │          │  - Posts         │
    │  - Promotions    │          │  - Analytics     │
    │  - Vouchers      │          │  - Product Links │
    │  - Analytics     │          │  - SEO Metadata  │
    └──────────────────┘          └──────────────────┘
```

### Key Difference: TaggedProduct

**Marketing Posts** include live product data in responses:
- When you fetch a post with `productId`, you get `TaggedProduct` with current product info
- Product data is **live** - price, discount, images update in real-time
- This allows displaying current product info without separate API calls

**Campaigns** do NOT include product data:
- Campaigns are simpler - just promotions and vouchers
- No product integration in campaign responses

---

## 🔌 API Endpoints Quick Reference

### Campaign & Promotion API

**Base URL:** `http://localhost:5000/api/v1`

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/campaigns` | List active campaigns |
| POST | `/campaigns/{id}/track-impression` | Track when campaign shown |
| POST | `/campaigns/{id}/track-click` | Track when user clicks |
| POST | `/cart/apply-voucher` | Apply discount code |
| POST | `/cart/remove-voucher` | Remove discount code |
| GET | `/campaigns/{id}/stats` | Get campaign analytics |

### Marketing Post API

**Base URL:** `http://localhost:5000/api/admin/marketing-posts`

**Note:** Requires Admin JWT token (except analytics endpoints)

| Method | Endpoint | Purpose |
|--------|----------|---------|
| GET | `/` | List posts (paginated) |
| GET | `/{id}` | Get post details |
| POST | `/` | Create post (admin only) |
| PUT | `/{id}` | Update post (admin only) |
| DELETE | `/{id}` | Delete post (admin only) |
| POST | `/{id}/publish` | Publish post (admin only) |
| POST | `/{id}/schedule` | Schedule post (admin only) |
| POST | `/{id}/analytics/views` | Track view (public) |
| POST | `/{id}/analytics/clicks` | Track click (public) |
| POST | `/{id}/analytics/shares` | Track share (public) |
| GET | `/statistics` | Get aggregate stats (admin only) |

---

## 📊 Data Models

### Campaign Response

```typescript
interface Campaign {
  campaignId: string;
  campaignName: string;
  budget: number;
  startDate: string; // ISO 8601
  endDate: string;
  status: "DRAFT" | "ACTIVE" | "PAUSED" | "COMPLETED" | "ARCHIVED";
  description: string;
  targeting: {
    pages: string[]; // ["home", "products", "cart"]
    frequency: "once-per-session" | "once-per-page" | "always";
    delayMs: number; // Show after X milliseconds
    autoDismissMs: number; // Auto-close after X milliseconds
  };
  createdAt: string;
  updatedAt: string;
}
```

### Promotion Response

```typescript
interface Promotion {
  promotionId: string;
  campaignId: string;
  discountValue: number;
  discountType: "percentage" | "fixed";
  startDate: string;
  endDate: string;
  minOrderValue: number;
  maxUsage: number;
  currentUsage: number;
  description: string;
  createdAt: string;
}
```

### Voucher Application Response

```typescript
interface VoucherApplicationResult {
  cartId: string;
  appliedCoupon: string;
  promotionId: string;
  discountValue: number;
  discountType: "percentage" | "fixed";
  discountAmount: number;
  subtotal: number;
  totalAmount: number;
}
```

### Marketing Post Response

```typescript
interface MarketingPost {
  id: string;
  title: string;
  content: string;
  shortDescription: string;
  image: string; // Primary image URL
  images: string[]; // Multiple images
  
  // Product Integration - LIVE DATA
  productId: string | null;
  productName: string | null;
  taggedProduct: {
    id: string;
    name: string;
    price: number;
    currency: string; // "VND"
    formattedPrice: string; // "1,000,000 VND"
    thumbnailUrl: string;
    hasDiscount: boolean;
    discountPercentage: number;
  } | null; // null if no product or product deleted
  
  // Content Metadata
  topic: string;
  platform: string;
  tone: string;
  hashtags: string[];
  
  // Priority & Display
  priorityScore: number; // 1-100
  displayLocation: string[]; // ["homepage_banner", "featured_section"]
  isFeatured: boolean; // true if priorityScore > 80
  
  // SEO
  metaTitle: string;
  metaDescription: string;
  metaKeywords: string[];
  
  // Social Media Variants
  socialPosts: {
    facebook: string;
    instagram: string;
    twitter: string;
    linkedin: string;
  };
  
  // Publishing
  status: "Draft" | "Published" | "Scheduled";
  scheduledDate: string | null;
  publishedDate: string | null;
  
  // Analytics
  views: number;
  clicks: number;
  shares: number;
  
  // Audit
  createdAt: string;
  updatedAt: string;
}
```

### Marketing Post List Response (Paginated)

```typescript
interface PaginatedMarketingPosts {
  items: MarketingPost[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}
```

---

## 🔄 Common Workflows

### Workflow 1: Display Campaign with AdPopup

```typescript
// 1. Fetch active campaigns
const campaigns = await fetch('/api/v1/campaigns?status=ACTIVE')
  .then(r => r.json());

// 2. Filter by targeting rules
const applicableCampaigns = campaigns.data.filter(c => 
  c.targeting.pages.includes(currentPage) &&
  shouldShowBasedOnFrequency(c.targeting.frequency)
);

// 3. Show popup with delay
for (const campaign of applicableCampaigns) {
  setTimeout(() => {
    showAdPopup(campaign);
    
    // 4. Track impression
    await fetch(`/api/v1/campaigns/${campaign.campaignId}/track-impression`, {
      method: 'POST',
      body: JSON.stringify({
        sessionId: getSessionId(),
        page: currentPage,
        timestamp: new Date().toISOString()
      })
    });
  }, campaign.targeting.delayMs);
  
  // 5. Auto-dismiss
  setTimeout(() => {
    closeAdPopup();
  }, campaign.targeting.autoDismissMs);
}
```

### Workflow 2: Apply Voucher to Cart

```typescript
// 1. User enters voucher code
const voucherCode = userInput; // "TET2025-ABC123"

// 2. Apply voucher
const result = await fetch('/api/v1/cart/apply-voucher', {
  method: 'POST',
  body: JSON.stringify({ couponCode: voucherCode })
}).then(r => r.json());

if (result.success) {
  // 3. Update cart display
  updateCartDisplay({
    subtotal: result.data.subtotal,
    discount: result.data.discountAmount,
    total: result.data.totalAmount
  });
  
  // 4. Show success message
  showNotification(`Discount applied: ${result.data.discountAmount} VND`);
} else {
  // 5. Show error
  showError(result.message); // "Voucher code is invalid or expired"
}
```

### Workflow 3: Display Marketing Posts with Live Product Data

```typescript
// 1. Fetch marketing posts
const posts = await fetch('/api/admin/marketing-posts?status=Published&pageSize=10')
  .then(r => r.json());

// 2. Display posts with product info
posts.data.items.forEach(post => {
  const productInfo = post.taggedProduct; // LIVE product data
  
  if (productInfo) {
    // Product exists and is current
    displayPost({
      title: post.title,
      image: post.image,
      productName: productInfo.name,
      productPrice: productInfo.formattedPrice,
      productImage: productInfo.thumbnailUrl,
      discount: productInfo.discountPercentage,
      productLink: `/products/${productInfo.id}`
    });
  } else if (post.productId) {
    // Product was deleted
    displayPost({
      title: post.title,
      image: post.image,
      productName: post.productName, // Fallback to stored name
      productDeleted: true
    });
  } else {
    // No product linked
    displayPost({
      title: post.title,
      image: post.image,
      noProduct: true
    });
  }
  
  // 3. Track analytics
  trackPostView(post.id);
});
```

### Workflow 4: Track Marketing Post Analytics

```typescript
// Track view when post becomes visible
const observer = new IntersectionObserver(entries => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      const postId = entry.target.dataset.postId;
      fetch(`/api/admin/marketing-posts/${postId}/analytics/views`, {
        method: 'POST'
      });
      observer.unobserve(entry.target);
    }
  });
});

// Track click when user clicks post
postElement.addEventListener('click', () => {
  fetch(`/api/admin/marketing-posts/${postId}/analytics/clicks`, {
    method: 'POST'
  });
});

// Track share when user shares
shareButton.addEventListener('click', () => {
  fetch(`/api/admin/marketing-posts/${postId}/analytics/shares`, {
    method: 'POST'
  });
});
```

---

## 🎨 UI Component Integration

### AdPopup Component (Campaign & Promotion)

```typescript
interface AdPopupProps {
  campaign: Campaign;
  onClose: () => void;
  onVoucherApply: (code: string) => void;
}

function AdPopup({ campaign, onClose, onVoucherApply }: AdPopupProps) {
  const [voucherCode, setVoucherCode] = useState('');
  
  return (
    <div className="ad-popup">
      <h2>{campaign.campaignName}</h2>
      <p>{campaign.description}</p>
      
      {/* Display promotions */}
      {campaign.promotions?.map(promo => (
        <div key={promo.promotionId}>
          <p>
            {promo.discountType === 'percentage' 
              ? `${promo.discountValue}% OFF` 
              : `${promo.discountValue} VND OFF`}
          </p>
          {promo.minOrderValue && (
            <p>Min order: {promo.minOrderValue} VND</p>
          )}
        </div>
      ))}
      
      {/* Voucher input */}
      <input 
        value={voucherCode}
        onChange={e => setVoucherCode(e.target.value)}
        placeholder="Enter voucher code"
      />
      <button onClick={() => onVoucherApply(voucherCode)}>
        Apply Discount
      </button>
      
      <button onClick={onClose}>Close</button>
    </div>
  );
}
```

### Marketing Post Card Component

```typescript
interface PostCardProps {
  post: MarketingPost;
  onViewTrack: (postId: string) => void;
  onClickTrack: (postId: string) => void;
}

function PostCard({ post, onViewTrack, onClickTrack }: PostCardProps) {
  useEffect(() => {
    // Track view when component mounts
    onViewTrack(post.id);
  }, [post.id]);
  
  return (
    <div className="post-card">
      <img src={post.image} alt={post.title} />
      <h3>{post.title}</h3>
      <p>{post.shortDescription}</p>
      
      {/* Display product if linked */}
      {post.taggedProduct && (
        <div className="product-section">
          <img 
            src={post.taggedProduct.thumbnailUrl} 
            alt={post.taggedProduct.name}
          />
          <div>
            <p className="product-name">{post.taggedProduct.name}</p>
            <p className="product-price">
              {post.taggedProduct.formattedPrice}
            </p>
            {post.taggedProduct.hasDiscount && (
              <span className="discount-badge">
                -{post.taggedProduct.discountPercentage}%
              </span>
            )}
          </div>
          <a 
            href={`/products/${post.taggedProduct.id}`}
            onClick={() => onClickTrack(post.id)}
          >
            View Product
          </a>
        </div>
      )}
      
      {/* Display hashtags */}
      <div className="hashtags">
        {post.hashtags?.map(tag => (
          <span key={tag}>#{tag}</span>
        ))}
      </div>
      
      {/* Analytics display */}
      <div className="analytics">
        <span>👁️ {post.views}</span>
        <span>👆 {post.clicks}</span>
        <span>📤 {post.shares}</span>
      </div>
    </div>
  );
}
```

---

## 🔐 Authentication

### JWT Token Setup

Both APIs require JWT authentication (except public analytics endpoints):

```typescript
// 1. Get JWT token from login
const token = localStorage.getItem('authToken');

// 2. Add to request headers
const headers = {
  'Authorization': `Bearer ${token}`,
  'Content-Type': 'application/json'
};

// 3. Make authenticated request
const response = await fetch('/api/admin/marketing-posts', {
  headers
});
```

### Public vs Protected Endpoints

**Public (No Auth Required):**
- GET `/api/v1/campaigns` - Fetch campaigns
- POST `/api/v1/campaigns/{id}/track-impression` - Track impression
- POST `/api/v1/campaigns/{id}/track-click` - Track click
- POST `/api/admin/marketing-posts/{id}/analytics/views` - Track view
- POST `/api/admin/marketing-posts/{id}/analytics/clicks` - Track click
- POST `/api/admin/marketing-posts/{id}/analytics/shares` - Track share

**Protected (Admin JWT Required):**
- POST `/api/admin/marketing-posts` - Create post
- PUT `/api/admin/marketing-posts/{id}` - Update post
- DELETE `/api/admin/marketing-posts/{id}` - Delete post
- GET `/api/admin/marketing-posts/statistics` - Get stats

---

## 🎯 Key Integration Points

### 1. Campaign Display Logic

```typescript
// Check if campaign should be shown
function shouldShowCampaign(campaign: Campaign, currentPage: string): boolean {
  // Check page targeting
  if (!campaign.targeting.pages.includes(currentPage)) {
    return false;
  }
  
  // Check frequency
  const sessionKey = `campaign_${campaign.campaignId}_shown`;
  if (campaign.targeting.frequency === 'once-per-session') {
    if (sessionStorage.getItem(sessionKey)) {
      return false;
    }
  }
  
  // Check date range
  const now = new Date();
  if (now < new Date(campaign.startDate) || now > new Date(campaign.endDate)) {
    return false;
  }
  
  return true;
}
```

### 2. Discount Calculation

```typescript
// Calculate discount amount
function calculateDiscount(
  promotion: Promotion,
  cartTotal: number
): number {
  // Check minimum order value
  if (promotion.minOrderValue && cartTotal < promotion.minOrderValue) {
    throw new Error(`Minimum order value: ${promotion.minOrderValue}`);
  }
  
  // Calculate discount
  if (promotion.discountType === 'percentage') {
    return cartTotal * (promotion.discountValue / 100);
  } else {
    return promotion.discountValue;
  }
}
```

### 3. Product Data Freshness

```typescript
// TaggedProduct is ALWAYS fresh
// When you fetch a post, product data is current
// No caching needed - backend fetches live data

// Example: Product price changed
// Old: post.taggedProduct.price = 1,000,000
// Fetch post again
// New: post.taggedProduct.price = 900,000 (updated)

// This means:
// - Always fetch post details before displaying
// - Don't cache post responses for product data
// - Product info updates in real-time
```

---

## 📱 Response Format

### Success Response

```json
{
  "success": true,
  "data": {
    // Response data here
  },
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

### Common Error Codes

| Code | Meaning | HTTP Status |
|------|---------|------------|
| `INVALID_VOUCHER` | Voucher code invalid or expired | 400 |
| `VOUCHER_LIMIT_EXCEEDED` | Voucher usage limit reached | 409 |
| `MIN_ORDER_VALUE_NOT_MET` | Cart total below minimum | 400 |
| `CAMPAIGN_NOT_FOUND` | Campaign doesn't exist | 404 |
| `RESOURCE_NOT_FOUND` | Post doesn't exist | 404 |
| `UNAUTHORIZED_ACCESS` | Missing/invalid JWT token | 401 |
| `INVALID_ARGUMENT` | Validation error | 400 |

---

## 🧪 Testing Checklist

- [ ] Campaign fetching and filtering
- [ ] Campaign impression tracking
- [ ] Campaign click tracking
- [ ] Voucher application with valid code
- [ ] Voucher application with invalid code
- [ ] Voucher application with expired code
- [ ] Voucher application with minimum order not met
- [ ] Marketing post fetching
- [ ] Marketing post with product (TaggedProduct populated)
- [ ] Marketing post without product (TaggedProduct null)
- [ ] Marketing post with deleted product (TaggedProduct null)
- [ ] Post view tracking
- [ ] Post click tracking
- [ ] Post share tracking
- [ ] Error handling for all endpoints
- [ ] Authentication/authorization

---

## 📚 Additional Resources

- **Campaign API Spec:** `.kiro/specs/campaign-promotion-api/README.md`
- **Marketing Post Spec:** `.kiro/specs/admin-marketing-post/design.md`
- **Swagger UI:** 
- **OpenAPI Spec:** 

---

## 🚀 Getting Started

1. **Review this guide** - Understand the two systems
2. **Check Swagger UI** - See live API documentation
3. **Implement Campaign Display** - Start with AdPopup component
4. **Implement Marketing Posts** - Add post feed component
5. **Add Analytics Tracking** - Track user interactions
6. **Test thoroughly** - Use testing checklist above

---

**Last Updated:** January 2025
**Version:** 1.0
**Status:** Ready for Frontend Integration
