# 📝 Changelog - Mixed Feed API

## Version 2.0.0 - Mixed Feed Release (December 5, 2024)

### 🎉 New Features

#### 1. Mixed Feed API
- **Endpoint**: `GET /api/v1/posts/feed`
- **Description**: Kết hợp Community Posts và Marketing Posts với thuật toán trộn thông minh
- **New Parameters**:
  - `marketingRatio` (int, default: 4) - Tỷ lệ marketing posts (1-10)
  - `location` (string, optional) - Filter theo display location
  - `minPriorityScore` (int, optional) - Minimum priority score
  - `productId` (guid, optional) - Filter theo product

#### 2. Location-Based Posts
- **Endpoint**: `GET /api/v1/posts/location/{location}`
- **Description**: Lấy marketing posts theo vị trí hiển thị cụ thể
- **Locations**: 
  - `homepage_banner` - Banner trang chủ
  - `product_section` - Trong trang sản phẩm
  - `featured_section` - Mục nổi bật
  - `sidebar` - Thanh bên

#### 3. Featured Posts
- **Endpoint**: `GET /api/v1/posts/featured`
- **Description**: Lấy marketing posts có priority > 80
- **Sorting**: Priority DESC → Views DESC

#### 4. Related Posts by Product
- **Endpoint**: `GET /api/v1/posts/related/{productId}`
- **Description**: Lấy posts liên quan đến một sản phẩm
- **Returns**: 
  - Marketing posts linked to product
  - Community posts mentioning product (future)

#### 5. Interaction Tracking
- **Endpoint**: `POST /api/v1/posts/{postId}/interactions/{action}`
- **Description**: Track user interactions (views, clicks, shares)
- **Actions**: `view`, `click`, `share`
- **Auth**: PUBLIC - Không cần authentication
- **Query Params**: `postType` (marketing/community)

---

### 🔄 Breaking Changes

#### Field Name Changes

| Old Field | New Field | Type | Notes |
|-----------|-----------|------|-------|
| `postId` | `id` | string | Renamed for consistency |
| `photoUrl` | `imageUrl` | string | Renamed for consistency |
| `postedOn` | `createdAt` | DateTime | Renamed for consistency |

#### New Required Fields

| Field | Type | Description |
|-------|------|-------------|
| `postType` | enum | `"community"` or `"marketing"` - MUST check before rendering |

#### Response Structure Changes

**Old Response** (Community Posts Only):
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

**New Response** (Mixed Posts):
```json
{
  "items": [
    {
      "id": "...",
      "postType": "community",
      "customerId": "...",
      "content": "...",
      "imageUrl": "..."
    },
    {
      "id": "...",
      "postType": "marketing",
      "title": "...",
      "content": "...",
      "imageUrl": "...",
      "priorityScore": 95,
      "isFeatured": true
    }
  ]
}
```

---

### ✨ Enhancements

#### Marketing Post Features

| Feature | Description |
|---------|-------------|
| `priorityScore` | Score 1-100 để control display order |
| `isFeatured` | Auto-set khi priority > 80 |
| `displayLocation` | Array of locations where post appears |
| `hashtags` | Array of hashtags |
| `productId` | Link to product (optional) |
| `productName` | Denormalized product name |
| `viewsCount` | Number of views |
| `clicksCount` | Number of clicks |
| `sharesCount` | Number of shares |

#### Smart Mixing Algorithm

```
marketingRatio = 4 (default)
→ 1 marketing post mỗi 4 community posts
→ 20% marketing, 80% community

Pattern:
C C C C M C C C C M C C C C M
```

#### Priority-Based Sorting

Marketing posts được sort theo:
1. Priority Score (DESC) - Cao nhất trước
2. Published Date (DESC) - Mới nhất trước

#### Auto-Featured Detection

```
IF priorityScore > 80 THEN
    isFeatured = true
ELSE
    isFeatured = false
```

---

### 🔧 Migration Required

#### Frontend Changes Needed

1. **Update Type Definitions**
   - Add `MixedPost`, `CommunityPost`, `MarketingPost` types
   - Add `postType` discriminator

2. **Update API Calls**
   - Add `marketingRatio` parameter to feed endpoint
   - Handle new response structure

3. **Update Components**
   - Check `postType` before rendering
   - Create separate components for community vs marketing posts
   - Add view tracking with Intersection Observer
   - Add click tracking on marketing posts

4. **Update Field Names**
   - `postId` → `id`
   - `photoUrl` → `imageUrl`
   - `postedOn` → `createdAt`

#### Backward Compatibility

- ✅ Old endpoint `/api/v1/posts/feed` still works
- ✅ Returns community posts only if `marketingRatio` not specified
- ✅ Gradual migration possible

---

### 📊 New Analytics Capabilities

#### Tracking Metrics

| Metric | Description | Endpoint |
|--------|-------------|----------|
| Views | Post appears in viewport | `POST /interactions/view` |
| Clicks | User clicks post/product | `POST /interactions/click` |
| Shares | User shares post | `POST /interactions/share` |

#### Analytics Dashboard (Admin)

Marketing posts now include:
- Total views
- Total clicks
- Total shares
- Click-through rate (CTR)
- Share rate

---

### 🎨 New UI Components Needed

#### 1. Marketing Post Card
- Display title, description, image
- Show product info if linked
- Display hashtags
- Show analytics (views, clicks, shares)
- CTA button
- Sponsored badge
- Featured badge (if priority > 80)

#### 2. Homepage Banner Carousel
- Display top 5 marketing posts
- Auto-rotate every 5 seconds
- Click tracking
- Responsive design

#### 3. Featured Section
- Grid layout for featured posts
- Priority-based sorting
- Click tracking

#### 4. Product Related Posts
- Marketing promotions section
- Community reviews section
- Tabbed or stacked layout

---

### 🚀 Performance Improvements

#### Optimizations

1. **Pagination**: Efficient pagination with configurable page size
2. **Indexes**: Database indexes on priority, published date, location
3. **Caching**: Recommended caching for featured and location-based posts
4. **Lazy Loading**: Support for lazy loading images
5. **Virtualization**: Support for virtual scrolling in long feeds

#### Recommended Caching Strategy

| Endpoint | TTL | Reason |
|----------|-----|--------|
| Featured posts | 5-10 min | Changes infrequently |
| Location-based | 5-10 min | Changes infrequently |
| Mixed feed | No cache | Real-time updates |
| Related posts | 10-15 min | Product-specific |

---

### 🔐 Security Updates

#### Authentication

- Mixed feed endpoints: **Optional** (enhanced when logged in)
- Interaction tracking: **Public** (no auth required)
- Admin marketing post management: **Admin role required**

#### Authorization

- Community posts: Owner can edit/delete
- Marketing posts: Admin only
- Interactions: Public tracking

---

### 📚 Documentation Added

1. **MIXED_FEED_API_DOCUMENTATION.md**
   - Complete API reference
   - Request/response examples
   - Error handling
   - Best practices

2. **MIXED_FEED_ALGORITHM_EXPLANATION.md**
   - Algorithm details
   - Priority score system
   - Display location system
   - Performance metrics

3. **MIXED_FEED_IMPLEMENTATION_SUMMARY.md**
   - Implementation overview
   - Files created
   - Configuration guidelines

4. **MIXED_FEED_VISUAL_GUIDE.md**
   - Visual examples
   - UI/UX mockups
   - Layout examples

5. **FRONTEND_MIGRATION_GUIDE.md**
   - Step-by-step migration
   - Code examples
   - Breaking changes
   - Migration checklist

6. **FRONTEND_QUICK_REFERENCE.md**
   - Quick start guide
   - Copy-paste code snippets
   - Common use cases

---

### 🐛 Bug Fixes

- N/A (New feature release)

---

### ⚠️ Deprecations

- None (Old API still supported for backward compatibility)

---

### 🔮 Future Enhancements (Planned)

#### Phase 2 (Q1 2025)

- [ ] Community posts mention product detection
- [ ] Advanced filtering (by hashtags, date range)
- [ ] Personalized feed based on user preferences
- [ ] A/B testing for marketing ratio
- [ ] Real-time analytics dashboard

#### Phase 3 (Q2 2025)

- [ ] Machine learning for optimal post placement
- [ ] Sentiment analysis for community posts
- [ ] Automated content moderation
- [ ] Multi-language support
- [ ] Video post support

---

### 📞 Support & Resources

#### Documentation
- API Docs: `MIXED_FEED_API_DOCUMENTATION.md`
- Migration Guide: `FRONTEND_MIGRATION_GUIDE.md`
- Quick Reference: `FRONTEND_QUICK_REFERENCE.md`
- Visual Guide: `MIXED_FEED_VISUAL_GUIDE.md`

#### Testing
- Swagger UI: `https://localhost:7001/swagger`
- Postman Collection: Available on request

#### Contact
- Backend Team: backend@vietcommerce.com
- API Support: api-support@vietcommerce.com
- Slack: #api-mixed-feed

---

### ✅ Migration Checklist for Frontend

#### Phase 1: Preparation (Day 1)
- [ ] Read documentation
- [ ] Review breaking changes
- [ ] Update type definitions
- [ ] Create new service functions
- [ ] Test new endpoints in Postman/Swagger

#### Phase 2: Implementation (Day 2-3)
- [ ] Update feed component
- [ ] Create marketing post card component
- [ ] Implement view tracking
- [ ] Implement click tracking
- [ ] Create homepage banner component
- [ ] Create featured section component
- [ ] Create product related posts component

#### Phase 3: Testing (Day 4)
- [ ] Unit tests for new components
- [ ] Integration tests for API calls
- [ ] E2E tests for user flows
- [ ] Performance testing
- [ ] Mobile responsive testing

#### Phase 4: Deployment (Day 5)
- [ ] Deploy to staging
- [ ] QA testing
- [ ] Monitor analytics
- [ ] Deploy to production
- [ ] Monitor production metrics

---

### 📈 Success Metrics

#### KPIs to Track

| Metric | Target | Description |
|--------|--------|-------------|
| CTR | > 5% | Click-through rate on marketing posts |
| Engagement | > 10% | Overall engagement rate |
| Share Rate | > 1% | Share rate on marketing posts |
| User Satisfaction | > 4.0/5 | User feedback score |
| Page Load Time | < 2s | Feed load time |

---

### 🎉 Summary

**Version 2.0.0** introduces **Mixed Feed API** - a major enhancement that combines community and marketing content intelligently.

**Key Benefits**:
- ✅ Unified feed experience
- ✅ Smart content distribution
- ✅ Priority-based control
- ✅ Location targeting
- ✅ Real-time analytics
- ✅ Flexible configuration
- ✅ Backward compatible

**Estimated Migration Time**: 2-3 days for full implementation

**Status**: ✅ Ready for Production

---

## Version 1.0.0 - Initial Release

### Features
- Community posts feed
- Like/unlike functionality
- Bookmark functionality
- Comment system
- User profiles
- Search functionality

---

**Last Updated**: December 5, 2024  
**Version**: 2.0.0  
**Status**: Production Ready 🚀
