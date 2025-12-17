# Campaign & Marketing Implementation Guide

**Status:** ✅ Complete  
**Date:** December 14, 2025  
**Version:** 1.0

---

## 📋 Overview

Complete implementation of Campaign & Promotion API and Marketing Post API in the frontend with:
- API services for both systems
- React components for UI
- Custom hooks for state management
- Full integration with Orval and OpenAPI

---

## 📁 File Structure

```
src/
├── lib/
│   ├── services/
│   │   ├── campaignService.ts          ✅ Campaign API service
│   │   └── marketingPostService.ts     ✅ Marketing Post API service
│   └── hooks/
│       ├── useCampaigns.ts             ✅ Campaign management hook
│       └── useMarketingPosts.ts        ✅ Marketing posts hook
├── components/
│   ├── campaign/
│   │   ├── CampaignPopup.tsx           ✅ Campaign popup component
│   │   └── index.ts                    ✅ Campaign exports
│   └── marketing/
│       ├── PostCard.tsx                ✅ Post card component
│       ├── PostsFeed.tsx               ✅ Posts feed component
│       └── index.ts                    ✅ Marketing exports
└── pages/
    └── MarketingPage.tsx               ✅ Marketing page
```

---

## 🔧 Services

### Campaign Service (`campaignService.ts`)

**Functions:**
- `getCampaigns()` - Fetch campaigns with filtering
- `getActiveCampaigns()` - Get active campaigns only
- `getCampaignById()` - Get campaign by ID
- `trackCampaignImpression()` - Track when campaign is shown
- `trackCampaignClick()` - Track when user clicks
- `getCampaignStats()` - Get campaign analytics
- `applyVoucher()` - Apply voucher code
- `removeVoucher()` - Remove voucher from cart
- `shouldShowCampaign()` - Check if campaign should be shown
- `calculateDiscount()` - Calculate discount amount
- `formatPrice()` - Format price with currency
- `getDiscountBadgeText()` - Get discount badge text
- `getSessionId()` - Generate or get session ID

**Usage:**
```typescript
import { getCampaigns, applyVoucher, trackCampaignImpression } from '@/lib/services/campaignService';

// Fetch campaigns
const campaigns = await getCampaigns({ status: 'ACTIVE' });

// Apply voucher
const result = await applyVoucher('CODE123');

// Track impression
await trackCampaignImpression(campaignId, {
  sessionId: 'session-123',
  page: 'home',
  timestamp: new Date().toISOString()
});
```

---

### Marketing Post Service (`marketingPostService.ts`)

**Functions:**
- `getMarketingPosts()` - Fetch posts with filtering
- `getPublishedPosts()` - Get published posts only
- `getFeaturedPosts()` - Get featured posts
- `getPostById()` - Get post by ID
- `searchPosts()` - Search posts by keyword
- `getPostsByProduct()` - Get posts for a product
- `getPostsByPlatform()` - Get posts by platform
- `trackPostView()` - Track post view
- `trackPostClick()` - Track post click
- `trackPostShare()` - Track post share
- `hasValidProduct()` - Check if post has valid product
- `getProductLink()` - Get product link from post
- `isFeaturedPost()` - Check if post is featured
- `setupPostViewTracking()` - Setup IntersectionObserver for tracking

**Usage:**
```typescript
import { getMarketingPosts, trackPostView, getPublishedPosts } from '@/lib/services/marketingPostService';

// Fetch posts
const posts = await getMarketingPosts({ status: 'Published', pageSize: 20 });

// Get published posts
const published = await getPublishedPosts();

// Track view
await trackPostView(postId);

// Track click
await trackPostClick(postId);
```

---

## 🎣 Hooks

### useCampaigns Hook

**Options:**
```typescript
interface UseCampaignsOptions {
  currentPage?: string;      // Current page for targeting
  autoTrack?: boolean;       // Auto-track impressions
}
```

**Returns:**
```typescript
{
  campaigns: Campaign[];
  isLoading: boolean;
  error: string | null;
  shownCampaigns: Set<string>;
  getApplicableCampaigns(): Campaign[];
  trackImpression(campaignId: string): Promise<void>;
  trackClick(campaignId: string): Promise<void>;
  reload(): Promise<void>;
}
```

**Usage:**
```typescript
import { useCampaigns } from '@/lib/hooks/useCampaigns';

function MyComponent() {
  const { campaigns, isLoading, getApplicableCampaigns, trackImpression } = 
    useCampaigns({ currentPage: 'home' });

  const applicable = getApplicableCampaigns();
  
  return (
    <div>
      {applicable.map(campaign => (
        <div key={campaign.campaignId}>
          {campaign.campaignName}
        </div>
      ))}
    </div>
  );
}
```

---

### useMarketingPosts Hook

**Options:**
```typescript
interface UseMarketingPostsOptions {
  query?: GetMarketingPostsQuery;
  autoLoad?: boolean;
}
```

**Returns:**
```typescript
{
  posts: MarketingPost[];
  isLoading: boolean;
  error: string | null;
  pageNumber: number;
  totalPages: number;
  totalItems: number;
  setPageNumber(page: number): void;
  loadPosts(): Promise<void>;
  loadPublished(pageSize?: number): Promise<void>;
  loadFeatured(pageSize?: number): Promise<void>;
  loadByProduct(productId: string, pageSize?: number): Promise<void>;
  loadByPlatform(platform: string, pageSize?: number): Promise<void>;
  search(searchTerm: string, pageSize?: number): Promise<void>;
}
```

**Usage:**
```typescript
import { useMarketingPosts } from '@/lib/hooks/useMarketingPosts';

function MyComponent() {
  const { posts, isLoading, loadFeatured } = useMarketingPosts();

  useEffect(() => {
    loadFeatured();
  }, []);

  return (
    <div>
      {posts.map(post => (
        <div key={post.id}>{post.title}</div>
      ))}
    </div>
  );
}
```

---

## 🎨 Components

### CampaignPopup Component

**Props:**
```typescript
interface CampaignPopupProps {
  campaign: Campaign;
  promotions?: Promotion[];
  onClose: () => void;
  onVoucherApplied?: (result: any) => void;
}
```

**Features:**
- Display campaign information
- Show promotions with discount info
- Voucher code input
- Apply/remove voucher
- Error handling
- Success messages

**Usage:**
```typescript
import { CampaignPopup } from '@/components/campaign';

function MyComponent() {
  const [showPopup, setShowPopup] = useState(false);

  return (
    <>
      <button onClick={() => setShowPopup(true)}>Show Campaign</button>
      {showPopup && (
        <CampaignPopup
          campaign={campaign}
          onClose={() => setShowPopup(false)}
          onVoucherApplied={(result) => console.log(result)}
        />
      )}
    </>
  );
}
```

---

### PostCard Component

**Props:**
```typescript
interface PostCardProps {
  post: MarketingPost;
  onProductClick?: (productId: string) => void;
}
```

**Features:**
- Display post image
- Show title and description
- Display product info (TaggedProduct)
- Show discount badge
- Display hashtags
- Show analytics (views, clicks, shares)
- Like and share buttons
- Auto-track views with IntersectionObserver

**Usage:**
```typescript
import { PostCard } from '@/components/marketing';

function MyComponent() {
  return (
    <PostCard
      post={post}
      onProductClick={(productId) => navigate(`/products/${productId}`)}
    />
  );
}
```

---

### PostsFeed Component

**Props:**
```typescript
interface PostsFeedProps {
  query?: GetMarketingPostsQuery;
  onProductClick?: (productId: string) => void;
}
```

**Features:**
- Display posts in grid
- Pagination support
- Loading state
- Error handling
- Empty state
- Responsive design

**Usage:**
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

---

## 📄 Pages

### MarketingPage

**Features:**
- Display active campaigns
- Show featured posts
- Campaign popup on load
- Product navigation
- Responsive design

**Usage:**
```typescript
import { MarketingPage } from '@/pages/MarketingPage';

// In your router
<Route path="/marketing" element={<MarketingPage />} />
```

---

## 🚀 Integration Steps

### Step 1: Update API Types

Ensure your `src/lib/api/types.ts` includes all Campaign and Marketing Post types:

```typescript
export interface Campaign {
  campaignId: string;
  campaignName: string;
  // ... other fields
}

export interface MarketingPost {
  id: string;
  title: string;
  // ... other fields
}
```

### Step 2: Update Router

Add the marketing page to your router:

```typescript
import { MarketingPage } from '@/pages/MarketingPage';

const routes = [
  // ... other routes
  { path: '/marketing', element: <MarketingPage /> },
];
```

### Step 3: Add Navigation

Add link to marketing page in your navigation:

```typescript
<Link to="/marketing">Marketing & Promotions</Link>
```

### Step 4: Test Features

1. Navigate to `/marketing`
2. See campaign popup
3. Apply voucher code
4. View marketing posts
5. Click on products
6. Check analytics tracking

---

## 🧪 Testing

### Campaign Testing

```typescript
import { getCampaigns, applyVoucher } from '@/lib/services/campaignService';

// Test fetching campaigns
const campaigns = await getCampaigns({ status: 'ACTIVE' });
console.log('Campaigns:', campaigns);

// Test applying voucher
try {
  const result = await applyVoucher('TEST-CODE');
  console.log('Voucher applied:', result);
} catch (error) {
  console.error('Voucher error:', error);
}
```

### Marketing Post Testing

```typescript
import { getMarketingPosts, trackPostView } from '@/lib/services/marketingPostService';

// Test fetching posts
const posts = await getMarketingPosts({ status: 'Published' });
console.log('Posts:', posts);

// Test tracking view
await trackPostView(posts[0].id);
```

### Component Testing

```typescript
import { render, screen } from '@testing-library/react';
import { PostCard } from '@/components/marketing';

test('renders post card', () => {
  render(<PostCard post={mockPost} />);
  expect(screen.getByText(mockPost.title)).toBeInTheDocument();
});
```

---

## 🔐 Error Handling

All services include error handling:

```typescript
try {
  const result = await applyVoucher(code);
} catch (error: any) {
  const message = error.response?.data?.message || 'Unknown error';
  const errorCode = error.response?.data?.errorCode;
  
  if (errorCode === 'INVALID_VOUCHER') {
    // Handle invalid voucher
  } else if (errorCode === 'VOUCHER_LIMIT_EXCEEDED') {
    // Handle limit exceeded
  }
}
```

---

## 📊 Analytics Tracking

### Campaign Tracking

```typescript
import { trackCampaignImpression, trackCampaignClick } from '@/lib/services/campaignService';

// Track impression
await trackCampaignImpression(campaignId, {
  sessionId: getSessionId(),
  page: 'home',
  timestamp: new Date().toISOString()
});

// Track click
await trackCampaignClick(campaignId, {
  sessionId: getSessionId(),
  page: 'home',
  timestamp: new Date().toISOString()
});
```

### Post Tracking

```typescript
import { trackPostView, trackPostClick, trackPostShare } from '@/lib/services/marketingPostService';

// Track view (automatic with IntersectionObserver)
await trackPostView(postId);

// Track click
await trackPostClick(postId);

// Track share
await trackPostShare(postId);
```

---

## 🎯 Key Features

✅ **Campaign Management**
- Fetch active campaigns
- Filter by status, date range
- Pagination support
- Campaign targeting logic
- Analytics tracking

✅ **Voucher System**
- Apply voucher codes
- Remove vouchers
- Discount calculation
- Error handling

✅ **Marketing Posts**
- Fetch posts with filtering
- Search functionality
- Featured posts
- Product integration (TaggedProduct)
- Analytics tracking

✅ **Analytics**
- Campaign impressions and clicks
- Post views, clicks, shares
- Automatic view tracking
- Session management

✅ **UI Components**
- Campaign popup
- Post cards
- Posts feed with pagination
- Responsive design
- Error states
- Loading states

---

## 📝 Configuration

### Environment Variables

```env
VITE_API_URL=http://localhost:5000
```

### Orval Configuration

Already configured in `orval.config.js`:
```javascript
{
  vietCommerce: {
    input: './swagger.json',
    output: {
      target: './Api/generated-orval/index.ts',
      client: 'axios',
      baseUrl: true,
    }
  }
}
```

---

## 🔄 API Generation

To regenerate API clients:

```bash
npm run api:generate
```

This will:
1. Generate Orval client from Swagger
2. Generate OpenAPI TypeScript client
3. Update all type definitions

---

## 📚 Documentation

- **API Reference:** `.kiro/specs/Fe read/FRONTEND_API_REFERENCE.md`
- **Integration Guide:** `.kiro/specs/Fe read/FRONTEND_INTEGRATION_GUIDE.md`
- **Quick Start:** `.kiro/specs/Fe read/FRONTEND_QUICK_START.md`
- **TypeScript Types:** `.kiro/specs/Fe read/frontend-types.ts`

---

## ✅ Checklist

- ✅ Campaign service created
- ✅ Marketing post service created
- ✅ useCampaigns hook created
- ✅ useMarketingPosts hook created
- ✅ CampaignPopup component created
- ✅ PostCard component created
- ✅ PostsFeed component created
- ✅ MarketingPage created
- ✅ API generation completed
- ✅ Orval configuration updated
- ✅ OpenAPI types generated

---

## 🚀 Next Steps

1. **Test the implementation:**
   - Navigate to `/marketing`
   - Test campaign popup
   - Test voucher application
   - Test post display

2. **Integrate with existing pages:**
   - Add campaign popup to homepage
   - Add marketing posts to product pages
   - Add featured posts to sidebar

3. **Customize styling:**
   - Update colors to match brand
   - Adjust component sizes
   - Add animations

4. **Monitor analytics:**
   - Check campaign impressions
   - Monitor post engagement
   - Track voucher usage

---

## 📞 Support

For issues or questions:
1. Check the API Reference documentation
2. Review component examples
3. Check error messages in console
4. Verify API is running

---

**Implementation Complete!** 🎉

All campaign and marketing features are now ready to use in your frontend application.
