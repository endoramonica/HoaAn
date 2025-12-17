# Campaign & Marketing - Quick Reference

**Quick lookup guide for campaign and marketing features**

---

## 🚀 Quick Start (Copy-Paste Ready)

### Display Campaign Popup
```typescript
import { CampaignPopup } from '@/components/campaign';
import { useCampaigns } from '@/lib/hooks/useCampaigns';

function MyComponent() {
  const { campaigns } = useCampaigns({ currentPage: 'home' });
  const [showPopup, setShowPopup] = useState(false);

  return (
    <>
      {showPopup && campaigns[0] && (
        <CampaignPopup
          campaign={campaigns[0]}
          onClose={() => setShowPopup(false)}
        />
      )}
    </>
  );
}
```

### Display Marketing Posts
```typescript
import { PostsFeed } from '@/components/marketing';

function MyComponent() {
  return (
    <PostsFeed
      query={{ status: 'Published', pageSize: 12 }}
      onProductClick={(productId) => navigate(`/products/${productId}`)}
    />
  );
}
```

### Apply Voucher
```typescript
import { applyVoucher } from '@/lib/services/campaignService';

async function handleApplyVoucher(code: string) {
  try {
    const result = await applyVoucher(code);
    console.log('Discount:', result.discountAmount);
  } catch (error) {
    console.error('Error:', error.response?.data?.message);
  }
}
```

### Track Post View
```typescript
import { trackPostView } from '@/lib/services/marketingPostService';

await trackPostView(postId);
```

---

## 📚 Services

### Campaign Service
```typescript
import {
  getCampaigns,
  getActiveCampaigns,
  getCampaignById,
  trackCampaignImpression,
  trackCampaignClick,
  getCampaignStats,
  applyVoucher,
  removeVoucher,
  shouldShowCampaign,
  calculateDiscount,
  formatPrice,
  getDiscountBadgeText,
  getSessionId,
} from '@/lib/services/campaignService';
```

### Marketing Post Service
```typescript
import {
  getMarketingPosts,
  getPublishedPosts,
  getFeaturedPosts,
  getPostById,
  searchPosts,
  getPostsByProduct,
  getPostsByPlatform,
  trackPostView,
  trackPostClick,
  trackPostShare,
  hasValidProduct,
  getProductLink,
  isFeaturedPost,
  setupPostViewTracking,
} from '@/lib/services/marketingPostService';
```

---

## 🎣 Hooks

### useCampaigns
```typescript
import { useCampaigns } from '@/lib/hooks/useCampaigns';

const {
  campaigns,
  isLoading,
  error,
  getApplicableCampaigns,
  trackImpression,
  trackClick,
} = useCampaigns({ currentPage: 'home' });
```

### useMarketingPosts
```typescript
import { useMarketingPosts } from '@/lib/hooks/useMarketingPosts';

const {
  posts,
  isLoading,
  pageNumber,
  totalPages,
  loadPublished,
  loadFeatured,
  search,
} = useMarketingPosts();
```

---

## 🎨 Components

### CampaignPopup
```typescript
import { CampaignPopup } from '@/components/campaign';

<CampaignPopup
  campaign={campaign}
  promotions={promotions}
  onClose={() => {}}
  onVoucherApplied={(result) => {}}
/>
```

### PostCard
```typescript
import { PostCard } from '@/components/marketing';

<PostCard
  post={post}
  onProductClick={(productId) => {}}
/>
```

### PostsFeed
```typescript
import { PostsFeed } from '@/components/marketing';

<PostsFeed
  query={{ status: 'Published', pageSize: 12 }}
  onProductClick={(productId) => {}}
/>
```

---

## 🔌 API Endpoints

### Campaign API
```
GET    /api/v1/campaigns
POST   /api/v1/campaigns/{id}/track-impression
POST   /api/v1/campaigns/{id}/track-click
POST   /api/v1/cart/apply-voucher
POST   /api/v1/cart/remove-voucher
GET    /api/v1/campaigns/{id}/stats
```

### Marketing Post API
```
GET    /api/admin/marketing-posts
GET    /api/admin/marketing-posts/{id}
POST   /api/admin/marketing-posts/{id}/analytics/views
POST   /api/admin/marketing-posts/{id}/analytics/clicks
POST   /api/admin/marketing-posts/{id}/analytics/shares
```

---

## 🎯 Common Tasks

### Fetch Active Campaigns
```typescript
const campaigns = await getActiveCampaigns();
```

### Get Featured Posts
```typescript
const posts = await getFeaturedPosts();
```

### Search Posts
```typescript
const results = await searchPosts('keyword');
```

### Get Posts by Product
```typescript
const posts = await getPostsByProduct(productId);
```

### Apply Voucher
```typescript
const result = await applyVoucher('CODE123');
```

### Track Analytics
```typescript
await trackPostView(postId);
await trackPostClick(postId);
await trackPostShare(postId);
```

### Check Product Validity
```typescript
if (hasValidProduct(post)) {
  const link = getProductLink(post);
}
```

---

## 🔍 Query Parameters

### Get Campaigns
```typescript
{
  status?: 'DRAFT' | 'ACTIVE' | 'PAUSED' | 'COMPLETED' | 'ARCHIVED',
  pageNumber?: number,
  pageSize?: number,
  fromDate?: string,
  toDate?: string,
}
```

### Get Marketing Posts
```typescript
{
  pageNumber?: number,
  pageSize?: number,
  status?: 'Draft' | 'Published' | 'Scheduled',
  productId?: string,
  platform?: string,
  searchTerm?: string,
  displayLocation?: string,
  isFeatured?: boolean,
  minPriorityScore?: number,
  fromDate?: string,
  toDate?: string,
  sortBy?: 'priority' | 'publishedDate' | 'views' | 'clicks' | 'shares' | 'updatedAt',
  sortOrder?: 'asc' | 'desc',
}
```

---

## ⚠️ Error Codes

| Code | Meaning |
|------|---------|
| INVALID_VOUCHER | Voucher code invalid or expired |
| VOUCHER_LIMIT_EXCEEDED | Voucher usage limit reached |
| MIN_ORDER_VALUE_NOT_MET | Cart total below minimum |
| RESOURCE_NOT_FOUND | Post or campaign not found |
| UNAUTHORIZED_ACCESS | Missing/invalid JWT token |

---

## 🧪 Testing

### Test Campaign
```typescript
const campaigns = await getCampaigns({ status: 'ACTIVE' });
console.log('Campaigns:', campaigns);
```

### Test Voucher
```typescript
try {
  const result = await applyVoucher('TEST-CODE');
  console.log('Success:', result);
} catch (error) {
  console.log('Error:', error.response?.data?.errorCode);
}
```

### Test Posts
```typescript
const posts = await getMarketingPosts({ status: 'Published' });
console.log('Posts:', posts);
```

---

## 📁 File Locations

| File | Location |
|------|----------|
| Campaign Service | `src/lib/services/campaignService.ts` |
| Marketing Service | `src/lib/services/marketingPostService.ts` |
| useCampaigns Hook | `src/lib/hooks/useCampaigns.ts` |
| useMarketingPosts Hook | `src/lib/hooks/useMarketingPosts.ts` |
| CampaignPopup | `src/components/campaign/CampaignPopup.tsx` |
| PostCard | `src/components/marketing/PostCard.tsx` |
| PostsFeed | `src/components/marketing/PostsFeed.tsx` |
| MarketingPage | `src/pages/MarketingPage.tsx` |

---

## 🔗 Related Documentation

- **Full Guide:** `CAMPAIGN_MARKETING_IMPLEMENTATION.md`
- **Summary:** `IMPLEMENTATION_SUMMARY.md`
- **API Reference:** `.kiro/specs/Fe read/FRONTEND_API_REFERENCE.md`
- **Integration Guide:** `.kiro/specs/Fe read/FRONTEND_INTEGRATION_GUIDE.md`

---

## 💡 Tips

1. **Always use hooks** for state management
2. **Track analytics** for all user interactions
3. **Handle errors** gracefully with user messages
4. **Use TypeScript** for type safety
5. **Test on mobile** for responsive design
6. **Check console** for debugging
7. **Verify API** is running before testing

---

## 🚀 Deploy

```bash
# Build
npm run build

# Preview
npm run preview

# Deploy to production
# (Your deployment command)
```

---

**Last Updated:** December 14, 2025  
**Version:** 1.0
