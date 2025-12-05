# 🔄 Frontend Migration Guide - Mixed Feed API

## 📋 Tổng Quan

Guide này hướng dẫn Frontend team migrate từ **Community Posts API cũ** sang **Mixed Feed API mới** - kết hợp cả Community Posts và Marketing Posts.

---

## 🔍 So Sánh API Cũ vs Mới

### ❌ API Cũ (Community Posts Only)

```javascript
// Endpoint cũ
GET /api/v1/posts/feed?pageNumber=1&pageSize=20

// Response cũ
{
  "success": true,
  "data": {
    "items": [
      {
        "postId": "...",
        "customerId": "...",
        "customerName": "...",
        "content": "...",
        "photoUrl": "...",
        "likesCount": 45,
        "commentsCount": 12,
        "isLikedByCurrentUser": false
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 150
  }
}
```

### ✅ API Mới (Mixed Feed)

```javascript
// Endpoint mới
GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4

// Response mới
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "...",
        "postType": "community",  // ← NEW: Phân biệt loại post
        "customerId": "...",
        "customerName": "...",
        "content": "...",
        "imageUrl": "...",
        "likesCount": 45,
        "commentsCount": 12,
        "isLikedByCurrentUser": false
      },
      {
        "id": "...",
        "postType": "marketing",  // ← NEW: Marketing post
        "title": "Flash Sale - Giảm 70%",
        "content": "...",
        "shortDescription": "...",
        "imageUrl": "...",
        "productId": "...",
        "productName": "iPhone 15 Pro Max",
        "priorityScore": 95,
        "isFeatured": true,
        "hashtags": ["FlashSale", "DienThoai"],
        "viewsCount": 12500,
        "clicksCount": 3400,
        "sharesCount": 890
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 1250
  }
}
```

---

## 🔧 Migration Steps

### Step 1: Cập Nhật API Endpoint

#### ❌ Code Cũ

```typescript
// services/postService.ts
export const getPostFeed = async (pageNumber: number, pageSize: number) => {
  const response = await axios.get('/api/v1/posts/feed', {
    params: { pageNumber, pageSize }
  });
  return response.data;
};
```

#### ✅ Code Mới

```typescript
// services/postService.ts
export const getPostFeed = async (
  pageNumber: number, 
  pageSize: number,
  marketingRatio: number = 4  // ← NEW: Tỷ lệ marketing posts
) => {
  const response = await axios.get('/api/v1/posts/feed', {
    params: { pageNumber, pageSize, marketingRatio }
  });
  return response.data;
};
```

---

### Step 2: Cập Nhật Type Definitions

#### ❌ Type Cũ

```typescript
// types/post.ts
interface Post {
  postId: string;
  customerId: string;
  customerName: string;
  customerAvatar?: string;
  content: string;
  photoUrl?: string;
  postedOn: string;
  likesCount: number;
  commentsCount: number;
  isLikedByCurrentUser: boolean;
  isBookmarkedByCurrentUser: boolean;
}
```

#### ✅ Type Mới

```typescript
// types/post.ts
interface BasePost {
  id: string;
  postType: 'community' | 'marketing';  // ← NEW
  content?: string;
  imageUrl?: string;
  createdAt: string;
  isLikedByCurrentUser: boolean;
  isBookmarkedByCurrentUser: boolean;
}

interface CommunityPost extends BasePost {
  postType: 'community';
  customerId: string;
  customerName: string;
  customerAvatar?: string;
  likesCount: number;
  commentsCount: number;
}

interface MarketingPost extends BasePost {
  postType: 'marketing';
  title: string;
  shortDescription?: string;
  imageUrls?: string[];
  productId?: string;
  productName?: string;
  priorityScore: number;
  isFeatured: boolean;
  displayLocation?: string[];
  hashtags?: string[];
  publishedDate?: string;
  viewsCount: number;
  clicksCount: number;
  sharesCount: number;
}

type MixedPost = CommunityPost | MarketingPost;

interface FeedResponse {
  items: MixedPost[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
```

---

### Step 3: Cập Nhật Component Render

#### ❌ Component Cũ

```tsx
// components/PostFeed.tsx
const PostFeed = () => {
  const [posts, setPosts] = useState<Post[]>([]);

  return (
    <div>
      {posts.map(post => (
        <PostCard key={post.postId} post={post} />
      ))}
    </div>
  );
};

const PostCard = ({ post }: { post: Post }) => {
  return (
    <div className="post-card">
      <div className="post-header">
        <img src={post.customerAvatar} alt={post.customerName} />
        <span>{post.customerName}</span>
      </div>
      <div className="post-content">{post.content}</div>
      {post.photoUrl && <img src={post.photoUrl} alt="Post" />}
      <div className="post-actions">
        <button>❤️ {post.likesCount}</button>
        <button>💬 {post.commentsCount}</button>
      </div>
    </div>
  );
};
```

#### ✅ Component Mới

```tsx
// components/PostFeed.tsx
const PostFeed = () => {
  const [posts, setPosts] = useState<MixedPost[]>([]);
  const [marketingRatio, setMarketingRatio] = useState(4);

  useEffect(() => {
    loadFeed();
  }, []);

  const loadFeed = async () => {
    const response = await getPostFeed(1, 20, marketingRatio);
    setPosts(response.data.items);
  };

  return (
    <div>
      {posts.map(post => (
        <PostCard key={post.id} post={post} />
      ))}
    </div>
  );
};

const PostCard = ({ post }: { post: MixedPost }) => {
  // ← NEW: Phân biệt render theo postType
  if (post.postType === 'community') {
    return <CommunityPostCard post={post} />;
  } else {
    return <MarketingPostCard post={post} />;
  }
};

const CommunityPostCard = ({ post }: { post: CommunityPost }) => {
  return (
    <div className="post-card community">
      <div className="post-header">
        <img src={post.customerAvatar} alt={post.customerName} />
        <span>{post.customerName}</span>
      </div>
      <div className="post-content">{post.content}</div>
      {post.imageUrl && <img src={post.imageUrl} alt="Post" />}
      <div className="post-actions">
        <button>❤️ {post.likesCount}</button>
        <button>💬 {post.commentsCount}</button>
      </div>
    </div>
  );
};

const MarketingPostCard = ({ post }: { post: MarketingPost }) => {
  const handleClick = () => {
    // Track click
    trackInteraction(post.id, 'click', 'marketing');
    // Navigate to product or detail
    if (post.productId) {
      router.push(`/products/${post.productId}`);
    }
  };

  return (
    <div className="post-card marketing" onClick={handleClick}>
      <div className="marketing-badge">⭐ SPONSORED</div>
      {post.isFeatured && <div className="featured-badge">🔥 FEATURED</div>}
      
      <h3>{post.title}</h3>
      <p>{post.shortDescription}</p>
      
      {post.imageUrl && (
        <img src={post.imageUrl} alt={post.title} className="marketing-image" />
      )}
      
      {post.productName && (
        <div className="product-info">
          📱 {post.productName}
        </div>
      )}
      
      {post.hashtags && (
        <div className="hashtags">
          {post.hashtags.map(tag => (
            <span key={tag}>#{tag}</span>
          ))}
        </div>
      )}
      
      <div className="marketing-stats">
        <span>👁️ {post.viewsCount.toLocaleString()}</span>
        <span>🖱️ {post.clicksCount.toLocaleString()}</span>
        <span>📤 {post.sharesCount.toLocaleString()}</span>
      </div>
      
      <button className="cta-button">🛒 MUA NGAY</button>
    </div>
  );
};
```

---

### Step 4: Thêm Analytics Tracking

#### ✅ Tracking Service

```typescript
// services/analyticsService.ts
export const trackInteraction = async (
  postId: string,
  action: 'view' | 'click' | 'share',
  postType: 'community' | 'marketing',
  source: string = 'feed',
  deviceType: string = 'mobile'
) => {
  try {
    await axios.post(
      `/api/v1/posts/${postId}/interactions/${action}?postType=${postType}`,
      {
        action,
        source,
        deviceType
      }
    );
  } catch (error) {
    console.error('Failed to track interaction:', error);
  }
};
```

#### ✅ Intersection Observer cho View Tracking

```typescript
// hooks/useViewTracking.ts
import { useEffect, useRef } from 'react';

export const useViewTracking = (
  postId: string,
  postType: 'community' | 'marketing',
  onView?: () => void
) => {
  const ref = useRef<HTMLDivElement>(null);
  const hasTracked = useRef(false);

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting && !hasTracked.current) {
            hasTracked.current = true;
            trackInteraction(postId, 'view', postType);
            onView?.();
          }
        });
      },
      { threshold: 0.5 } // 50% visible
    );

    if (ref.current) {
      observer.observe(ref.current);
    }

    return () => {
      if (ref.current) {
        observer.unobserve(ref.current);
      }
    };
  }, [postId, postType]);

  return ref;
};
```

#### ✅ Sử Dụng trong Component

```tsx
const PostCard = ({ post }: { post: MixedPost }) => {
  const viewRef = useViewTracking(post.id, post.postType);

  return (
    <div ref={viewRef} className="post-card">
      {/* Post content */}
    </div>
  );
};
```

---

### Step 5: Thêm Các Endpoint Mới

#### ✅ Location-Based Posts

```typescript
// services/postService.ts
export const getPostsByLocation = async (
  location: 'homepage_banner' | 'product_section' | 'featured_section' | 'sidebar',
  pageSize: number = 10
) => {
  const response = await axios.get(`/api/v1/posts/location/${location}`, {
    params: { pageSize }
  });
  return response.data;
};
```

#### ✅ Featured Posts

```typescript
export const getFeaturedPosts = async (pageSize: number = 10) => {
  const response = await axios.get('/api/v1/posts/featured', {
    params: { pageSize }
  });
  return response.data;
};
```

#### ✅ Related Posts by Product

```typescript
export const getRelatedPosts = async (productId: string, pageSize: number = 20) => {
  const response = await axios.get(`/api/v1/posts/related/${productId}`, {
    params: { pageSize }
  });
  return response.data;
};
```

---

## 🎨 UI Components Examples

### Homepage Banner Carousel

```tsx
// components/HomepageBanner.tsx
const HomepageBanner = () => {
  const [bannerPosts, setBannerPosts] = useState<MarketingPost[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);

  useEffect(() => {
    loadBannerPosts();
  }, []);

  const loadBannerPosts = async () => {
    const response = await getPostsByLocation('homepage_banner', 5);
    setBannerPosts(response.data.posts);
  };

  return (
    <div className="banner-carousel">
      {bannerPosts.map((post, index) => (
        <div
          key={post.id}
          className={`banner-slide ${index === currentIndex ? 'active' : ''}`}
          onClick={() => {
            trackInteraction(post.id, 'click', 'marketing', 'banner');
            window.location.href = `/products/${post.productId}`;
          }}
        >
          <img src={post.imageUrl} alt={post.title} />
          <div className="banner-content">
            <h2>{post.title}</h2>
            <p>{post.shortDescription}</p>
            <button className="cta-button">🛒 MUA NGAY</button>
          </div>
        </div>
      ))}
      
      <div className="carousel-dots">
        {bannerPosts.map((_, index) => (
          <button
            key={index}
            className={index === currentIndex ? 'active' : ''}
            onClick={() => setCurrentIndex(index)}
          />
        ))}
      </div>
    </div>
  );
};
```

### Product Page Related Posts

```tsx
// components/ProductRelatedPosts.tsx
const ProductRelatedPosts = ({ productId }: { productId: string }) => {
  const [relatedPosts, setRelatedPosts] = useState<{
    marketingPosts: MarketingPost[];
    communityPosts: CommunityPost[];
  }>();

  useEffect(() => {
    loadRelatedPosts();
  }, [productId]);

  const loadRelatedPosts = async () => {
    const response = await getRelatedPosts(productId, 10);
    setRelatedPosts(response.data);
  };

  return (
    <div className="related-posts">
      {/* Marketing Promotions */}
      {relatedPosts?.marketingPosts.length > 0 && (
        <section>
          <h3>📢 Khuyến Mãi Đặc Biệt</h3>
          <div className="promotions-grid">
            {relatedPosts.marketingPosts.map(post => (
              <MarketingPostCard key={post.id} post={post} />
            ))}
          </div>
        </section>
      )}

      {/* Community Reviews */}
      {relatedPosts?.communityPosts.length > 0 && (
        <section>
          <h3>💬 Đánh Giá Từ Khách Hàng</h3>
          <div className="reviews-list">
            {relatedPosts.communityPosts.map(post => (
              <CommunityPostCard key={post.id} post={post} />
            ))}
          </div>
        </section>
      )}
    </div>
  );
};
```

### Featured Posts Section

```tsx
// components/FeaturedSection.tsx
const FeaturedSection = () => {
  const [featuredPosts, setFeaturedPosts] = useState<MarketingPost[]>([]);

  useEffect(() => {
    loadFeaturedPosts();
  }, []);

  const loadFeaturedPosts = async () => {
    const response = await getFeaturedPosts(6);
    setFeaturedPosts(response.data.posts);
  };

  return (
    <section className="featured-section">
      <h2>⭐ Khuyến Mãi Nổi Bật</h2>
      <div className="featured-grid">
        {featuredPosts.map(post => (
          <div key={post.id} className="featured-card">
            <div className="featured-badge">🔥 HOT</div>
            <img src={post.imageUrl} alt={post.title} />
            <h3>{post.title}</h3>
            <p>{post.shortDescription}</p>
            <div className="stats">
              <span>👁️ {post.viewsCount.toLocaleString()}</span>
              <span>Priority: {post.priorityScore}</span>
            </div>
            <button
              onClick={() => {
                trackInteraction(post.id, 'click', 'marketing', 'featured');
                window.location.href = `/products/${post.productId}`;
              }}
            >
              Xem Chi Tiết
            </button>
          </div>
        ))}
      </div>
    </section>
  );
};
```

---

## 🎯 Configuration Options

### Marketing Ratio Control

```tsx
// components/FeedSettings.tsx (Admin/Debug)
const FeedSettings = () => {
  const [ratio, setRatio] = useState(4);

  return (
    <div className="feed-settings">
      <label>Marketing Ratio: {ratio}</label>
      <input
        type="range"
        min="2"
        max="10"
        value={ratio}
        onChange={(e) => setRatio(Number(e.target.value))}
      />
      <p>1 marketing post mỗi {ratio} community posts</p>
    </div>
  );
};
```

---

## 📊 Analytics Dashboard

```tsx
// components/PostAnalytics.tsx (Admin)
const PostAnalytics = ({ post }: { post: MarketingPost }) => {
  return (
    <div className="post-analytics">
      <h4>📊 Analytics</h4>
      <div className="metrics">
        <div className="metric">
          <span className="label">Views</span>
          <span className="value">{post.viewsCount.toLocaleString()}</span>
        </div>
        <div className="metric">
          <span className="label">Clicks</span>
          <span className="value">{post.clicksCount.toLocaleString()}</span>
        </div>
        <div className="metric">
          <span className="label">Shares</span>
          <span className="value">{post.sharesCount.toLocaleString()}</span>
        </div>
        <div className="metric">
          <span className="label">CTR</span>
          <span className="value">
            {((post.clicksCount / post.viewsCount) * 100).toFixed(2)}%
          </span>
        </div>
      </div>
    </div>
  );
};
```

---

## ✅ Migration Checklist

### Phase 1: Backward Compatible (Không Breaking)
- [ ] Thêm type definitions mới
- [ ] Thêm services cho endpoints mới
- [ ] Giữ nguyên API cũ hoạt động
- [ ] Test song song cả 2 APIs

### Phase 2: Implement New Features
- [ ] Tạo components cho Marketing Posts
- [ ] Implement view tracking
- [ ] Implement click tracking
- [ ] Implement share tracking
- [ ] Tạo Homepage Banner component
- [ ] Tạo Featured Section component
- [ ] Tạo Product Related Posts component

### Phase 3: Switch to New API
- [ ] Update feed endpoint từ cũ sang mới
- [ ] Update PostCard component để handle cả 2 types
- [ ] Test thoroughly
- [ ] Deploy to staging
- [ ] Monitor analytics

### Phase 4: Cleanup (Optional)
- [ ] Remove old API code (nếu không còn dùng)
- [ ] Update documentation
- [ ] Optimize performance

---

## 🚨 Breaking Changes

### Field Name Changes

| Old Field | New Field | Notes |
|-----------|-----------|-------|
| `postId` | `id` | Renamed |
| `photoUrl` | `imageUrl` | Renamed |
| `postedOn` | `createdAt` | Renamed |
| - | `postType` | NEW - Required for rendering |

### Response Structure

- **Old**: Chỉ có community posts
- **New**: Mixed array của community + marketing posts
- **Action**: Phải check `postType` trước khi render

---

## 💡 Best Practices

### 1. Type Guards

```typescript
function isCommunityPost(post: MixedPost): post is CommunityPost {
  return post.postType === 'community';
}

function isMarketingPost(post: MixedPost): post is MarketingPost {
  return post.postType === 'marketing';
}

// Usage
if (isCommunityPost(post)) {
  console.log(post.customerName); // TypeScript knows this exists
}
```

### 2. Error Handling

```typescript
const loadFeed = async () => {
  try {
    const response = await getPostFeed(1, 20, 4);
    setPosts(response.data.items);
  } catch (error) {
    console.error('Failed to load feed:', error);
    // Fallback to old API if new API fails
    const fallbackResponse = await getOldPostFeed(1, 20);
    setPosts(fallbackResponse.data.items.map(convertToMixedPost));
  }
};
```

### 3. Performance Optimization

```typescript
// Lazy load images
<img 
  src={post.imageUrl} 
  loading="lazy" 
  alt={post.title}
/>

// Virtualize long lists
import { FixedSizeList } from 'react-window';

<FixedSizeList
  height={600}
  itemCount={posts.length}
  itemSize={200}
>
  {({ index, style }) => (
    <div style={style}>
      <PostCard post={posts[index]} />
    </div>
  )}
</FixedSizeList>
```

---

## 📞 Support

Nếu có vấn đề trong quá trình migration:

1. Check documentation: `MIXED_FEED_API_DOCUMENTATION.md`
2. Check examples: `MIXED_FEED_VISUAL_GUIDE.md`
3. Contact Backend team
4. Check Swagger UI: `https://localhost:7001/swagger`

---

## 🎉 Summary

**Migration từ Community Posts sang Mixed Feed API**:

✅ **Backward Compatible**: API cũ vẫn hoạt động  
✅ **New Features**: Marketing posts, analytics, location-based  
✅ **Type Safe**: Full TypeScript support  
✅ **Performance**: Optimized với lazy loading, virtualization  
✅ **Analytics**: Track views, clicks, shares  
✅ **Flexible**: Configurable marketing ratio

**Estimated Migration Time**: 2-3 days cho full implementation

Good luck! 🚀
