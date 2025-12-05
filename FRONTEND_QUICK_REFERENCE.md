# ⚡ Frontend Quick Reference - Mixed Feed API

## 🚀 Quick Start

### 1. Basic Feed (Replace Old API)

```typescript
// ❌ OLD
const response = await axios.get('/api/v1/posts/feed', {
  params: { pageNumber: 1, pageSize: 20 }
});

// ✅ NEW
const response = await axios.get('/api/v1/posts/feed', {
  params: { 
    pageNumber: 1, 
    pageSize: 20,
    marketingRatio: 4  // 1 marketing mỗi 4 community posts
  }
});
```

---

## 📝 Type Definitions (Copy & Paste)

```typescript
// types/post.ts
export interface BasePost {
  id: string;
  postType: 'community' | 'marketing';
  content?: string;
  imageUrl?: string;
  createdAt: string;
  isLikedByCurrentUser: boolean;
  isBookmarkedByCurrentUser: boolean;
}

export interface CommunityPost extends BasePost {
  postType: 'community';
  customerId: string;
  customerName: string;
  customerAvatar?: string;
  likesCount: number;
  commentsCount: number;
}

export interface MarketingPost extends BasePost {
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

export type MixedPost = CommunityPost | MarketingPost;
```

---

## 🔌 API Service (Copy & Paste)

```typescript
// services/postService.ts
import axios from 'axios';

const API_BASE = '/api/v1/posts';

// Mixed Feed
export const getPostFeed = async (
  pageNumber: number = 1,
  pageSize: number = 20,
  marketingRatio: number = 4
) => {
  return axios.get(`${API_BASE}/feed`, {
    params: { pageNumber, pageSize, marketingRatio }
  });
};

// Location-based posts
export const getPostsByLocation = async (
  location: 'homepage_banner' | 'product_section' | 'featured_section' | 'sidebar',
  pageSize: number = 10
) => {
  return axios.get(`${API_BASE}/location/${location}`, {
    params: { pageSize }
  });
};

// Featured posts
export const getFeaturedPosts = async (pageSize: number = 10) => {
  return axios.get(`${API_BASE}/featured`, {
    params: { pageSize }
  });
};

// Related posts by product
export const getRelatedPosts = async (
  productId: string,
  pageSize: number = 20
) => {
  return axios.get(`${API_BASE}/related/${productId}`, {
    params: { pageSize }
  });
};

// Track interaction (NO AUTH REQUIRED)
export const trackInteraction = async (
  postId: string,
  action: 'view' | 'click' | 'share',
  postType: 'community' | 'marketing',
  source: string = 'feed',
  deviceType: string = 'mobile'
) => {
  return axios.post(
    `${API_BASE}/${postId}/interactions/${action}?postType=${postType}`,
    { action, source, deviceType }
  );
};
```

---

## 🎨 React Component (Copy & Paste)

```tsx
// components/PostCard.tsx
import React from 'react';
import { MixedPost, CommunityPost, MarketingPost } from '@/types/post';
import { trackInteraction } from '@/services/postService';

export const PostCard: React.FC<{ post: MixedPost }> = ({ post }) => {
  if (post.postType === 'community') {
    return <CommunityPostCard post={post} />;
  }
  return <MarketingPostCard post={post} />;
};

const CommunityPostCard: React.FC<{ post: CommunityPost }> = ({ post }) => {
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

const MarketingPostCard: React.FC<{ post: MarketingPost }> = ({ post }) => {
  const handleClick = () => {
    trackInteraction(post.id, 'click', 'marketing');
    if (post.productId) {
      window.location.href = `/products/${post.productId}`;
    }
  };

  return (
    <div className="post-card marketing" onClick={handleClick}>
      <div className="marketing-badge">⭐ SPONSORED</div>
      {post.isFeatured && <div className="featured-badge">🔥 FEATURED</div>}
      
      <h3>{post.title}</h3>
      <p>{post.shortDescription}</p>
      
      {post.imageUrl && <img src={post.imageUrl} alt={post.title} />}
      
      {post.productName && (
        <div className="product-info">📱 {post.productName}</div>
      )}
      
      {post.hashtags && (
        <div className="hashtags">
          {post.hashtags.map(tag => <span key={tag}>#{tag}</span>)}
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

## 👁️ View Tracking Hook (Copy & Paste)

```typescript
// hooks/useViewTracking.ts
import { useEffect, useRef } from 'react';
import { trackInteraction } from '@/services/postService';

export const useViewTracking = (
  postId: string,
  postType: 'community' | 'marketing'
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
          }
        });
      },
      { threshold: 0.5 }
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

// Usage in component
const PostCard = ({ post }) => {
  const viewRef = useViewTracking(post.id, post.postType);
  
  return (
    <div ref={viewRef} className="post-card">
      {/* content */}
    </div>
  );
};
```

---

## 🎯 Common Use Cases

### 1. Homepage Feed

```tsx
const HomePage = () => {
  const [posts, setPosts] = useState<MixedPost[]>([]);
  const [page, setPage] = useState(1);

  useEffect(() => {
    loadFeed();
  }, [page]);

  const loadFeed = async () => {
    const response = await getPostFeed(page, 20, 4);
    setPosts(prev => [...prev, ...response.data.data.items]);
  };

  return (
    <div>
      {posts.map(post => <PostCard key={post.id} post={post} />)}
      <button onClick={() => setPage(p => p + 1)}>Load More</button>
    </div>
  );
};
```

### 2. Homepage Banner

```tsx
const HomepageBanner = () => {
  const [banners, setBanners] = useState<MarketingPost[]>([]);

  useEffect(() => {
    getPostsByLocation('homepage_banner', 5)
      .then(res => setBanners(res.data.data.posts));
  }, []);

  return (
    <div className="banner-carousel">
      {banners.map(banner => (
        <div key={banner.id} className="banner-slide">
          <img src={banner.imageUrl} alt={banner.title} />
          <h2>{banner.title}</h2>
          <button onClick={() => {
            trackInteraction(banner.id, 'click', 'marketing', 'banner');
            window.location.href = `/products/${banner.productId}`;
          }}>
            MUA NGAY
          </button>
        </div>
      ))}
    </div>
  );
};
```

### 3. Product Page

```tsx
const ProductPage = ({ productId }: { productId: string }) => {
  const [relatedPosts, setRelatedPosts] = useState<any>();

  useEffect(() => {
    getRelatedPosts(productId, 10)
      .then(res => setRelatedPosts(res.data.data));
  }, [productId]);

  return (
    <div>
      {/* Product details */}
      
      {/* Marketing promotions */}
      <section>
        <h3>📢 Khuyến Mãi</h3>
        {relatedPosts?.marketingPosts.map(post => (
          <MarketingPostCard key={post.id} post={post} />
        ))}
      </section>

      {/* Community reviews */}
      <section>
        <h3>💬 Đánh Giá</h3>
        {relatedPosts?.communityPosts.map(post => (
          <CommunityPostCard key={post.id} post={post} />
        ))}
      </section>
    </div>
  );
};
```

### 4. Featured Section

```tsx
const FeaturedSection = () => {
  const [featured, setFeatured] = useState<MarketingPost[]>([]);

  useEffect(() => {
    getFeaturedPosts(6)
      .then(res => setFeatured(res.data.data.posts));
  }, []);

  return (
    <section className="featured-section">
      <h2>⭐ Khuyến Mãi Nổi Bật</h2>
      <div className="featured-grid">
        {featured.map(post => (
          <div key={post.id} className="featured-card">
            <img src={post.imageUrl} alt={post.title} />
            <h3>{post.title}</h3>
            <button onClick={() => {
              trackInteraction(post.id, 'click', 'marketing', 'featured');
              window.location.href = `/products/${post.productId}`;
            }}>
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

## 🎨 CSS Styles (Copy & Paste)

```css
/* styles/post.css */

/* Community Post */
.post-card.community {
  background: white;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 16px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.post-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}

.post-header img {
  width: 40px;
  height: 40px;
  border-radius: 50%;
}

.post-content {
  margin-bottom: 12px;
  line-height: 1.5;
}

.post-actions {
  display: flex;
  gap: 16px;
}

.post-actions button {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 14px;
}

/* Marketing Post */
.post-card.marketing {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 16px;
  cursor: pointer;
  transition: transform 0.2s;
}

.post-card.marketing:hover {
  transform: translateY(-4px);
}

.marketing-badge {
  display: inline-block;
  background: rgba(255,255,255,0.2);
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  margin-bottom: 12px;
}

.featured-badge {
  display: inline-block;
  background: #ff6b6b;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  margin-left: 8px;
}

.post-card.marketing h3 {
  font-size: 20px;
  margin-bottom: 8px;
}

.marketing-image {
  width: 100%;
  border-radius: 8px;
  margin: 12px 0;
}

.product-info {
  background: rgba(255,255,255,0.1);
  padding: 8px 12px;
  border-radius: 8px;
  margin: 12px 0;
}

.hashtags {
  display: flex;
  gap: 8px;
  margin: 12px 0;
}

.hashtags span {
  background: rgba(255,255,255,0.2);
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.marketing-stats {
  display: flex;
  gap: 16px;
  margin: 12px 0;
  font-size: 14px;
}

.cta-button {
  width: 100%;
  background: white;
  color: #667eea;
  border: none;
  padding: 12px;
  border-radius: 8px;
  font-weight: bold;
  cursor: pointer;
  margin-top: 12px;
}

.cta-button:hover {
  background: #f0f0f0;
}
```

---

## 📊 Marketing Ratio Guide

| Ratio | Marketing % | Use Case |
|-------|-------------|----------|
| 2 | 33% | Flash Sale, Black Friday |
| 3 | 25% | High promotion period |
| **4** | **20%** | **Normal (Recommended)** |
| 5 | 17% | Regular operation |
| 6 | 14% | User-focused |
| 10 | 9% | Minimal marketing |

---

## 🔍 Type Guards (Helper Functions)

```typescript
// utils/typeGuards.ts
export function isCommunityPost(post: MixedPost): post is CommunityPost {
  return post.postType === 'community';
}

export function isMarketingPost(post: MixedPost): post is MarketingPost {
  return post.postType === 'marketing';
}

// Usage
if (isCommunityPost(post)) {
  console.log(post.customerName); // ✅ TypeScript knows this exists
}

if (isMarketingPost(post)) {
  console.log(post.priorityScore); // ✅ TypeScript knows this exists
}
```

---

## ⚠️ Important Notes

### 1. Field Name Changes

| Old | New |
|-----|-----|
| `postId` | `id` |
| `photoUrl` | `imageUrl` |
| `postedOn` | `createdAt` |

### 2. New Required Field

- **`postType`**: MUST check before rendering
  - `"community"` → Render as community post
  - `"marketing"` → Render as marketing post

### 3. Analytics Tracking

- **View**: Track when post is 50% visible
- **Click**: Track when user clicks post
- **Share**: Track when user shares post
- **NO AUTH REQUIRED** for tracking endpoints

### 4. Marketing Post Features

- `isFeatured`: Auto-set when `priorityScore > 80`
- `displayLocation`: Array of locations where post should appear
- `hashtags`: Array of hashtags for the post
- `productId`: Link to product (optional)

---

## 🚀 Quick Test

```bash
# Test mixed feed
curl "http://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4"

# Test location-based
curl "http://localhost:7001/api/v1/posts/location/homepage_banner?pageSize=5"

# Test featured
curl "http://localhost:7001/api/v1/posts/featured?pageSize=10"

# Test related posts
curl "http://localhost:7001/api/v1/posts/related/12345678-1234-1234-1234-123456789012"

# Test tracking (no auth)
curl -X POST "http://localhost:7001/api/v1/posts/{postId}/interactions/view?postType=marketing" \
  -H "Content-Type: application/json" \
  -d '{"action":"view","source":"feed","deviceType":"mobile"}'
```

---

## 📚 Full Documentation

- **API Docs**: `MIXED_FEED_API_DOCUMENTATION.md`
- **Migration Guide**: `FRONTEND_MIGRATION_GUIDE.md`
- **Algorithm Explanation**: `MIXED_FEED_ALGORITHM_EXPLANATION.md`
- **Visual Guide**: `MIXED_FEED_VISUAL_GUIDE.md`

---

## 💡 Tips

1. **Always check `postType`** before rendering
2. **Use TypeScript** for type safety
3. **Track views** with Intersection Observer
4. **Track clicks** on marketing posts
5. **Use lazy loading** for images
6. **Cache featured posts** (5-10 minutes)
7. **Handle errors gracefully**
8. **Test on mobile and desktop**

---

## 🎉 Ready to Go!

Copy the code snippets above và bắt đầu integrate ngay! 🚀

Nếu có vấn đề, check full documentation hoặc contact Backend team.
