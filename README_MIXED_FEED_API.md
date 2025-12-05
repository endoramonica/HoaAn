# 🎯 Mixed Feed API v2.0

> Hệ thống kết hợp Community Posts và Marketing Posts với thuật toán trộn thông minh

[![Status](https://img.shields.io/badge/status-production%20ready-brightgreen)]()
[![Version](https://img.shields.io/badge/version-2.0.0-blue)]()
[![API](https://img.shields.io/badge/API-RESTful-orange)]()
[![Auth](https://img.shields.io/badge/auth-JWT%20%2B%20Public-yellow)]()

---

## 📋 Tổng Quan

**Mixed Feed API** là hệ thống mới kết hợp **Community Posts** (từ customers) và **Marketing Posts** (từ admin) thành một feed thống nhất với thuật toán trộn thông minh.

### ✨ Tính Năng Chính

- 🎯 **Smart Mixing Algorithm**: Marketing posts xuất hiện đều đặn mỗi N community posts
- 📊 **Priority-Based Sorting**: Control display order với priority score (1-100)
- 📍 **Location Targeting**: Hiển thị posts theo vị trí cụ thể (banner, sidebar, etc.)
- ⭐ **Auto-Featured**: Tự động đánh dấu posts có priority > 80
- 🔗 **Product Integration**: Link posts với products trong catalog
- 📈 **Analytics Tracking**: Track views, clicks, shares (public endpoints)
- 🔄 **Backward Compatible**: API cũ vẫn hoạt động bình thường

---

## 🚀 Quick Start

### For Frontend Developers

```typescript
// 1. Install dependencies (if needed)
npm install axios

// 2. Copy type definitions
import { MixedPost, CommunityPost, MarketingPost } from '@/types/post';

// 3. Call API
const response = await axios.get('/api/v1/posts/feed', {
  params: { 
    pageNumber: 1, 
    pageSize: 20,
    marketingRatio: 4  // 1 marketing mỗi 4 community posts
  }
});

// 4. Render posts
response.data.data.items.map(post => {
  if (post.postType === 'community') {
    return <CommunityPostCard post={post} />;
  } else {
    return <MarketingPostCard post={post} />;
  }
});
```

**Full code examples**: See `FRONTEND_QUICK_REFERENCE.md`

---

## 📚 Documentation

### 🎯 Start Here

| Document | Description | Read Time | Audience |
|----------|-------------|-----------|----------|
| **[FRONTEND_NOTIFICATION.md](FRONTEND_NOTIFICATION.md)** ⭐ | Quick notification for frontend team | 5 min | All |
| **[FRONTEND_QUICK_REFERENCE.md](FRONTEND_QUICK_REFERENCE.md)** ⭐ | Copy-paste code to start quickly | 15 min | Frontend |
| **[MIXED_FEED_API_INDEX.md](MIXED_FEED_API_INDEX.md)** | Documentation index & reading path | 5 min | All |

### 📖 Complete Guides

| Document | Description | Read Time | Audience |
|----------|-------------|-----------|----------|
| [FRONTEND_MIGRATION_GUIDE.md](FRONTEND_MIGRATION_GUIDE.md) | Step-by-step migration guide | 30 min | Frontend |
| [MIXED_FEED_API_DOCUMENTATION.md](MIXED_FEED_API_DOCUMENTATION.md) | Complete API reference | 1 hour | All Dev |
| [MIXED_FEED_VISUAL_GUIDE.md](MIXED_FEED_VISUAL_GUIDE.md) | UI/UX examples & mockups | 30 min | UI/UX, Frontend |
| [MIXED_FEED_ALGORITHM_EXPLANATION.md](MIXED_FEED_ALGORITHM_EXPLANATION.md) | Algorithm details | 45 min | PM, Backend |
| [MIXED_FEED_IMPLEMENTATION_SUMMARY.md](MIXED_FEED_IMPLEMENTATION_SUMMARY.md) | Backend implementation | 20 min | Backend |
| [CHANGELOG_MIXED_FEED_API.md](CHANGELOG_MIXED_FEED_API.md) | Version history | 15 min | All |

---

## 🔌 API Endpoints

### Mixed Feed
```
GET /api/v1/posts/feed
```
Kết hợp community và marketing posts với tỷ lệ tùy chỉnh

**Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 20)
- `marketingRatio` (int, default: 4, range: 1-10)

---

### Location-Based Posts
```
GET /api/v1/posts/location/{location}
```
Lấy marketing posts theo vị trí hiển thị

**Locations**: `homepage_banner`, `product_section`, `featured_section`, `sidebar`

---

### Featured Posts
```
GET /api/v1/posts/featured
```
Lấy marketing posts có priority > 80

---

### Related Posts by Product
```
GET /api/v1/posts/related/{productId}
```
Lấy posts liên quan đến một sản phẩm

---

### Interaction Tracking (PUBLIC)
```
POST /api/v1/posts/{postId}/interactions/{action}
```
Track user interactions: `view`, `click`, `share`

**No authentication required**

---

## 🎨 Example Response

```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "...",
        "postType": "community",
        "customerId": "...",
        "customerName": "Nguyễn Văn A",
        "content": "Vừa mua iPhone 15, rất hài lòng!",
        "imageUrl": "...",
        "likesCount": 45,
        "commentsCount": 12
      },
      {
        "id": "...",
        "postType": "marketing",
        "title": "🔥 Flash Sale - Giảm 70%",
        "content": "Chương trình flash sale đặc biệt...",
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
    "totalCount": 1250,
    "totalPages": 63
  }
}
```

---

## 🎯 Marketing Ratio

Control tỷ lệ marketing posts trong feed:

```
marketingRatio = 4 (default)
→ 1 marketing post mỗi 4 community posts
→ 20% marketing, 80% community

Pattern:
C C C C M C C C C M C C C C M
█ █ █ █ ⭐ █ █ █ █ ⭐ █ █ █ █ ⭐
```

### Recommended Ratios

| Ratio | Marketing % | Use Case |
|-------|-------------|----------|
| 2 | 33% | Flash Sale, Black Friday |
| 3 | 25% | High promotion period |
| **4** | **20%** | **Normal (Recommended)** |
| 5 | 17% | Regular operation |
| 6 | 14% | User-focused |
| 10 | 9% | Minimal marketing |

---

## 📊 Priority Score System

Marketing posts được sort theo priority score:

```
90-100: CRITICAL (Featured) - Flash sales, major events
81-89:  HIGH (Featured) - Important promotions
71-80:  MEDIUM-HIGH - Regular promotions
51-70:  MEDIUM - Standard content
1-50:   LOW - Background content
```

**Auto-Featured**: Posts với priority > 80 tự động được đánh dấu featured

---

## 🔄 Migration Guide

### Breaking Changes

1. **Field name changes**:
   - `postId` → `id`
   - `photoUrl` → `imageUrl`
   - `postedOn` → `createdAt`

2. **New required field**:
   - `postType`: Must check before rendering (`"community"` or `"marketing"`)

3. **Response structure**:
   - Mixed array of community + marketing posts
   - Must handle both types in render logic

### Migration Steps

```
Day 1: Understanding (1 hour)
├─ Read FRONTEND_NOTIFICATION.md
├─ Read FRONTEND_QUICK_REFERENCE.md
└─ Test API in Postman/Swagger

Day 2-3: Implementation (2 days)
├─ Update type definitions
├─ Update API service
├─ Update components
├─ Implement analytics tracking
└─ Create new UI components

Day 4: Testing (1 day)
├─ Unit tests
├─ Integration tests
├─ Mobile responsive testing
└─ Performance testing
```

**Full migration guide**: See `FRONTEND_MIGRATION_GUIDE.md`

---

## 📈 Analytics Tracking

### View Tracking (Automatic)

```typescript
// Use Intersection Observer
const useViewTracking = (postId, postType) => {
  // Track when post is 50% visible
  // POST /api/v1/posts/{postId}/interactions/view
};
```

### Click Tracking

```typescript
const handleClick = () => {
  trackInteraction(postId, 'click', 'marketing');
  // Navigate to product or detail
};
```

### Share Tracking

```typescript
const handleShare = () => {
  trackInteraction(postId, 'share', 'marketing');
  // Share to social media
};
```

**Full implementation**: See `FRONTEND_QUICK_REFERENCE.md`

---

## 🎨 UI Components

### Homepage Feed
- Mixed feed with community + marketing posts
- Infinite scroll
- View tracking
- Click tracking

### Homepage Banner
- Top 5 marketing posts
- Auto-rotate carousel
- Click tracking
- Responsive design

### Featured Section
- Grid layout for featured posts
- Priority-based sorting
- Click tracking

### Product Page
- Marketing promotions section
- Community reviews section
- Related posts

**Full examples**: See `MIXED_FEED_VISUAL_GUIDE.md`

---

## 🔐 Security

### Authentication

- **Mixed feed endpoints**: Optional (enhanced when logged in)
- **Interaction tracking**: Public (no auth required)
- **Admin marketing post management**: Admin role required

### Authorization

- **Community posts**: Owner can edit/delete
- **Marketing posts**: Admin only
- **Interactions**: Public tracking

---

## 🚀 Performance

### Optimization Tips

1. **Caching**:
   - Featured posts: 5-10 minutes TTL
   - Location-based posts: 5-10 minutes TTL
   - Product-related posts: 10-15 minutes TTL

2. **Lazy Loading**:
   - Use `loading="lazy"` for images
   - Implement virtual scrolling for long feeds

3. **Analytics**:
   - Batch analytics requests if high traffic
   - Use debounce for view tracking

4. **Database**:
   - Indexes on priority, published date, location
   - Efficient pagination

---

## 🧪 Testing

### Manual Testing

```bash
# Test mixed feed
curl "http://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4"

# Test location-based
curl "http://localhost:7001/api/v1/posts/location/homepage_banner?pageSize=5"

# Test featured
curl "http://localhost:7001/api/v1/posts/featured?pageSize=10"

# Test tracking
curl -X POST "http://localhost:7001/api/v1/posts/{postId}/interactions/view?postType=marketing" \
  -H "Content-Type: application/json" \
  -d '{"action":"view","source":"feed","deviceType":"mobile"}'
```

### Swagger UI

```
https://localhost:7001/swagger
```

---

## 📞 Support

### Documentation
- **Index**: `MIXED_FEED_API_INDEX.md`
- **Quick Reference**: `FRONTEND_QUICK_REFERENCE.md`
- **API Docs**: `MIXED_FEED_API_DOCUMENTATION.md`
- **Migration Guide**: `FRONTEND_MIGRATION_GUIDE.md`

### Contact
- **Slack**: #api-support, #backend-team
- **Email**: api-support@vietcommerce.com
- **Jira**: Create ticket with label "mixed-feed-api"

### Resources
- **Swagger UI**: https://localhost:7001/swagger
- **Postman Collection**: Available on request
- **GitHub**: Create issue for bugs/features

---

## ✅ Checklist

### Before Starting
- [ ] Read `FRONTEND_NOTIFICATION.md`
- [ ] Read `FRONTEND_QUICK_REFERENCE.md`
- [ ] Test API in Postman/Swagger
- [ ] Understand breaking changes

### Implementation
- [ ] Update type definitions
- [ ] Update API service
- [ ] Update components
- [ ] Implement view tracking
- [ ] Implement click tracking
- [ ] Create marketing post card
- [ ] Create homepage banner
- [ ] Create featured section

### Testing
- [ ] Unit tests
- [ ] Integration tests
- [ ] Mobile responsive
- [ ] Performance optimized
- [ ] Analytics working

### Deployment
- [ ] Deploy to staging
- [ ] QA testing
- [ ] Monitor metrics
- [ ] Deploy to production

---

## 🎉 Summary

**Mixed Feed API v2.0** provides:

✅ **Unified Feed**: Community + Marketing posts  
✅ **Smart Algorithm**: Configurable mixing ratio  
✅ **Priority Control**: Score-based sorting  
✅ **Location Targeting**: Display in specific areas  
✅ **Analytics**: Track views, clicks, shares  
✅ **Flexible**: Highly configurable  
✅ **Backward Compatible**: Old API still works  
✅ **Production Ready**: Fully tested and documented

**Status**: ✅ Production Ready  
**Version**: 2.0.0  
**Release Date**: December 5, 2024

---

## 📊 Stats

- **8 documentation files** (~138 KB)
- **5 API endpoints**
- **2 post types** (community, marketing)
- **4 display locations**
- **3 interaction types** (view, click, share)
- **100 priority levels** (1-100)
- **10 marketing ratios** (1-10)

---

## 🚀 Get Started

1. **Read**: `FRONTEND_NOTIFICATION.md` (5 min)
2. **Copy**: Code from `FRONTEND_QUICK_REFERENCE.md` (15 min)
3. **Code**: Start implementing! (2-3 days)
4. **Test**: Thoroughly test all features (1 day)
5. **Deploy**: Ship to production! 🎉

---

**Happy Coding!** 🎨✨

**Questions?** Check `MIXED_FEED_API_INDEX.md` or ping @backend-team on Slack!
