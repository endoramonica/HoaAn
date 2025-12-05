# 📢 Thông Báo Cho Frontend Team - Mixed Feed API v2.0

## 🎉 Tin Vui!

Backend đã hoàn thành **Mixed Feed API v2.0** - hệ thống kết hợp Community Posts và Marketing Posts với thuật toán trộn thông minh!

---

## ⚡ TL;DR (Too Long; Didn't Read)

### Những Gì Bạn Cần Biết Ngay

1. **API mới**: `GET /api/v1/posts/feed?marketingRatio=4`
2. **Breaking change**: Response có thêm field `postType` (phải check trước khi render)
3. **Field đổi tên**: `postId` → `id`, `photoUrl` → `imageUrl`, `postedOn` → `createdAt`
4. **Backward compatible**: API cũ vẫn hoạt động bình thường
5. **Migration time**: Ước tính 2-3 ngày

---

## 🔥 Tính Năng Mới

### 1. Mixed Feed (Trộn Community + Marketing Posts)

```javascript
// Endpoint mới
GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4

// marketingRatio = 4 nghĩa là:
// 1 marketing post mỗi 4 community posts (20% marketing, 80% community)
```

**Pattern**:
```
Community Post 1
Community Post 2
Community Post 3
Community Post 4
⭐ Marketing Post 1  ← Xuất hiện sau mỗi 4 community posts
Community Post 5
Community Post 6
Community Post 7
Community Post 8
⭐ Marketing Post 2
...
```

### 2. Location-Based Posts (Banner, Sidebar, etc.)

```javascript
GET /api/v1/posts/location/homepage_banner?pageSize=5
```

**Use case**: Homepage banner carousel, sidebar ads, featured section

### 3. Featured Posts (Priority > 80)

```javascript
GET /api/v1/posts/featured?pageSize=10
```

**Use case**: Featured promotions section

### 4. Product Related Posts

```javascript
GET /api/v1/posts/related/{productId}?pageSize=20
```

**Use case**: Product detail page - show promotions + reviews

### 5. Analytics Tracking (PUBLIC - No Auth)

```javascript
// Track view
POST /api/v1/posts/{postId}/interactions/view?postType=marketing

// Track click
POST /api/v1/posts/{postId}/interactions/click?postType=marketing

// Track share
POST /api/v1/posts/{postId}/interactions/share?postType=marketing
```

---

## ⚠️ Breaking Changes

### 1. Response Structure

**Trước đây** (chỉ community posts):
```json
{
  "items": [
    {
      "postId": "...",
      "customerId": "...",
      "content": "...",
      "photoUrl": "..."
    }
  ]
}
```

**Bây giờ** (mixed posts):
```json
{
  "items": [
    {
      "id": "...",
      "postType": "community",  // ← NEW: PHẢI CHECK
      "customerId": "...",
      "content": "...",
      "imageUrl": "..."
    },
    {
      "id": "...",
      "postType": "marketing",  // ← NEW: PHẢI CHECK
      "title": "Flash Sale",
      "content": "...",
      "imageUrl": "...",
      "priorityScore": 95,
      "isFeatured": true,
      "productId": "...",
      "viewsCount": 12500
    }
  ]
}
```

### 2. Field Name Changes

| Old | New |
|-----|-----|
| `postId` | `id` |
| `photoUrl` | `imageUrl` |
| `postedOn` | `createdAt` |

### 3. Render Logic

**PHẢI CHECK `postType` trước khi render**:

```tsx
const PostCard = ({ post }) => {
  if (post.postType === 'community') {
    return <CommunityPostCard post={post} />;
  } else {
    return <MarketingPostCard post={post} />;
  }
};
```

---

## 📦 Những Gì Cần Làm

### Bước 1: Đọc Documentation (30 phút)

1. **Quick Start**: `FRONTEND_QUICK_REFERENCE.md` ← BẮT ĐẦU TỪ ĐÂY
2. **Migration Guide**: `FRONTEND_MIGRATION_GUIDE.md`
3. **API Docs**: `MIXED_FEED_API_DOCUMENTATION.md`
4. **Changelog**: `CHANGELOG_MIXED_FEED_API.md`

### Bước 2: Update Code (1-2 ngày)

1. **Type Definitions** (copy từ `FRONTEND_QUICK_REFERENCE.md`)
2. **API Service** (copy từ `FRONTEND_QUICK_REFERENCE.md`)
3. **Components**:
   - Update `PostCard` để handle cả 2 types
   - Tạo `MarketingPostCard` component
   - Tạo `HomepageBanner` component
   - Tạo `FeaturedSection` component
4. **Analytics**:
   - Implement view tracking (Intersection Observer)
   - Implement click tracking
   - Implement share tracking

### Bước 3: Testing (1 ngày)

1. Test mixed feed
2. Test location-based posts
3. Test featured posts
4. Test product related posts
5. Test analytics tracking
6. Test mobile responsive

---

## 🚀 Quick Start Code

### Copy & Paste Này Để Bắt Đầu

```typescript
// types/post.ts
export type MixedPost = CommunityPost | MarketingPost;

export interface CommunityPost {
  id: string;
  postType: 'community';
  customerId: string;
  customerName: string;
  content?: string;
  imageUrl?: string;
  likesCount: number;
  commentsCount: number;
}

export interface MarketingPost {
  id: string;
  postType: 'marketing';
  title: string;
  content?: string;
  imageUrl?: string;
  productId?: string;
  productName?: string;
  priorityScore: number;
  isFeatured: boolean;
  hashtags?: string[];
  viewsCount: number;
  clicksCount: number;
  sharesCount: number;
}

// services/postService.ts
export const getPostFeed = async (
  pageNumber: number = 1,
  pageSize: number = 20,
  marketingRatio: number = 4
) => {
  return axios.get('/api/v1/posts/feed', {
    params: { pageNumber, pageSize, marketingRatio }
  });
};

export const trackInteraction = async (
  postId: string,
  action: 'view' | 'click' | 'share',
  postType: 'community' | 'marketing'
) => {
  return axios.post(
    `/api/v1/posts/${postId}/interactions/${action}?postType=${postType}`,
    { action, source: 'feed', deviceType: 'mobile' }
  );
};

// components/PostCard.tsx
const PostCard = ({ post }: { post: MixedPost }) => {
  if (post.postType === 'community') {
    return <CommunityPostCard post={post} />;
  }
  return <MarketingPostCard post={post} />;
};
```

**Full code trong**: `FRONTEND_QUICK_REFERENCE.md`

---

## 🎯 Marketing Ratio Guide

| Ratio | Marketing % | Khi Nào Dùng |
|-------|-------------|--------------|
| 2 | 33% | Flash Sale, Black Friday |
| 3 | 25% | High promotion period |
| **4** | **20%** | **Normal (Recommended)** |
| 5 | 17% | Regular operation |
| 6 | 14% | User-focused |

---

## 📊 Analytics Tracking

### View Tracking (Tự động khi post hiển thị)

```typescript
// hooks/useViewTracking.ts
export const useViewTracking = (postId: string, postType: string) => {
  const ref = useRef<HTMLDivElement>(null);
  
  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          trackInteraction(postId, 'view', postType);
        }
      },
      { threshold: 0.5 }
    );
    
    if (ref.current) observer.observe(ref.current);
    return () => observer.disconnect();
  }, []);
  
  return ref;
};

// Usage
const PostCard = ({ post }) => {
  const viewRef = useViewTracking(post.id, post.postType);
  return <div ref={viewRef}>...</div>;
};
```

### Click Tracking

```typescript
const handleClick = () => {
  trackInteraction(post.id, 'click', 'marketing');
  // Navigate to product or detail
};
```

---

## 🎨 UI Examples

### Marketing Post Card

```
┌─────────────────────────────────────┐
│ ⭐ SPONSORED      🔥 FEATURED       │
├─────────────────────────────────────┤
│                                     │
│  🔥 Flash Sale - Giảm 70%          │
│                                     │
│  Chương trình flash sale đặc biệt   │
│  với giảm giá lên đến 70%...        │
│                                     │
│  [📷 Large Banner Image]            │
│                                     │
│  📱 iPhone 15 Pro Max               │
│  #FlashSale #DienThoai #GiamGia     │
│                                     │
│  👁️ 12.5K  🖱️ 3.4K  📤 890        │
│                                     │
│  [🛒 MUA NGAY]                      │
│                                     │
└─────────────────────────────────────┘
```

---

## ✅ Checklist

### Day 1: Preparation
- [ ] Đọc `FRONTEND_QUICK_REFERENCE.md`
- [ ] Đọc `FRONTEND_MIGRATION_GUIDE.md`
- [ ] Test API endpoints trong Postman/Swagger
- [ ] Update type definitions
- [ ] Create service functions

### Day 2: Implementation
- [ ] Update feed component
- [ ] Create marketing post card
- [ ] Implement view tracking
- [ ] Implement click tracking
- [ ] Create homepage banner
- [ ] Create featured section

### Day 3: Testing & Polish
- [ ] Unit tests
- [ ] Integration tests
- [ ] Mobile responsive testing
- [ ] Performance testing
- [ ] Deploy to staging

---

## 🆘 Cần Giúp Đỡ?

### Documentation
- **Quick Reference**: `FRONTEND_QUICK_REFERENCE.md` ← Copy-paste code
- **Migration Guide**: `FRONTEND_MIGRATION_GUIDE.md` ← Step-by-step
- **API Docs**: `MIXED_FEED_API_DOCUMENTATION.md` ← Full API reference
- **Visual Guide**: `MIXED_FEED_VISUAL_GUIDE.md` ← UI examples

### Testing
- **Swagger UI**: `https://localhost:7001/swagger`
- **Test Endpoints**: Xem trong `FRONTEND_QUICK_REFERENCE.md`

### Contact
- **Backend Team**: Slack #backend-team
- **API Support**: Slack #api-support
- **Questions**: Tạo issue trong Jira hoặc ping @backend-team

---

## 🎉 Summary

**Mixed Feed API v2.0** đã sẵn sàng! 🚀

**Những gì bạn nhận được**:
- ✅ Unified feed (community + marketing)
- ✅ Smart mixing algorithm
- ✅ Location-based posts
- ✅ Featured posts
- ✅ Product related posts
- ✅ Analytics tracking
- ✅ Backward compatible

**Estimated time**: 2-3 ngày để integrate hoàn chỉnh

**Status**: ✅ Production Ready

**Next steps**: 
1. Đọc `FRONTEND_QUICK_REFERENCE.md`
2. Copy code examples
3. Start coding!

---

**Questions?** Ping @backend-team trên Slack! 💬

**Happy Coding!** 🎨✨
