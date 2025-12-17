# 🎯 Mixed Feed Implementation Summary

## ✅ Hoàn Thành

Đã implement thành công **Mixed Feed API** - hệ thống kết hợp Community Posts và Marketing Posts với thuật toán trộn thông minh.

---

## 📁 Files Created

### 1. DTOs (Data Transfer Objects)
**File**: `VietCommerce.Core/DTOs/Posts/MixedFeedDto.cs`

**Classes**:
- `MixedFeedDto` - DTO chính cho mixed feed items
- `MixedFeedQuery` - Query parameters cho mixed feed
- `LocationPostsDto` - Response cho location-based posts
- `FeaturedPostsDto` - Response cho featured posts
- `RelatedPostsDto` - Response cho product-related posts
- `PostInteractionDto` - DTO cho tracking interactions
- `PostInteractionResult` - Result của interaction tracking

### 2. Service Interface
**File**: `VietCommerce.Application/Services/Services/Interfaces/IMixedFeedService.cs`

**Methods**:
- `GetMixedFeedAsync()` - Mixed feed với thuật toán trộn
- `GetPostsByLocationAsync()` - Posts theo vị trí
- `GetFeaturedPostsAsync()` - Featured posts (priority > 80)
- `GetRelatedPostsByProductAsync()` - Posts liên quan product
- `TrackInteractionAsync()` - Track views/clicks/shares

### 3. Service Implementation
**File**: `VietCommerce.Application/Services/Services/MixedFeedService.cs`

**Features**:
- ✅ Smart mixing algorithm
- ✅ Priority-based sorting
- ✅ Location filtering
- ✅ Featured posts detection
- ✅ Product-related posts
- ✅ Analytics tracking
- ✅ Pagination support
- ✅ Current user context
- ✅ **[NEW] Marketing post mapping với TaggedProduct**
- ✅ **[NEW] Chuẩn hóa response format theo marketing feed API**
- ✅ **[NEW] Sử dụng TaggedProductHelper cho consistency**

### 4. API Controller
**File**: `VietCommerce.Api/Controllers/MixedFeedController.cs`

**Endpoints**:
```
GET  /api/v1/posts/feed
GET  /api/v1/posts/location/{location}
GET  /api/v1/posts/featured
GET  /api/v1/posts/related/{productId}
POST /api/v1/posts/{postId}/interactions/{action}
```

### 5. Documentation
**Files**:
- `MIXED_FEED_API_DOCUMENTATION.md` - Complete API documentation
- `MIXED_FEED_ALGORITHM_EXPLANATION.md` - Algorithm explanation

### 6. Service Registration
**File**: `VietCommerce.Application/Extensions/ServiceCollectionExtensions.cs`

**Added**:
```csharp
services.AddScoped<IMixedFeedService, MixedFeedService>();
```

---

## 🎯 Key Features

### 1. Smart Mixing Algorithm
```
marketingRatio = 4 (default)
→ 1 marketing post mỗi 4 community posts

Pattern:
C C C C M C C C C M C C C C M
█ █ █ █ ⭐ █ █ █ █ ⭐ █ █ █ █ ⭐
```

### 2. Priority Score System
```
90-100: CRITICAL (Featured)
81-89:  HIGH (Featured)
71-80:  MEDIUM-HIGH
51-70:  MEDIUM
1-50:   LOW
```

### 3. Display Locations
- `homepage_banner` - Banner trang chủ
- `product_section` - Trong trang sản phẩm
- `featured_section` - Mục nổi bật
- `sidebar` - Thanh bên

### 4. Analytics Tracking
- **Views**: Post appears in viewport
- **Clicks**: User clicks post/product link
- **Shares**: User shares post

---

## 🔌 API Endpoints Detail

### 1. GET /api/v1/posts/feed
**Purpose**: Mixed feed với tỷ lệ marketing tùy chỉnh

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 20)
- `marketingRatio` (int, default: 4, range: 1-10)
- `location` (string, optional)
- `minPriorityScore` (int, optional)
- `productId` (guid, optional)

**Example**:
```http
GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4
```

**Response**: Paginated list of mixed posts (community + marketing)

---

### 2. GET /api/v1/posts/location/{location}
**Purpose**: Lấy marketing posts theo vị trí hiển thị

**Path Parameters**:
- `location` (string) - homepage_banner, product_section, featured_section, sidebar

**Example**:
```http
GET /api/v1/posts/location/homepage_banner?pageSize=10
```

**Response**: Marketing posts filtered by location

---

### 3. GET /api/v1/posts/featured
**Purpose**: Lấy featured posts (priority > 80)

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)

**Example**:
```http
GET /api/v1/posts/featured?pageSize=5
```

**Response**: Top featured marketing posts

---

### 4. GET /api/v1/posts/related/{productId}
**Purpose**: Lấy posts liên quan đến product

**Path Parameters**:
- `productId` (guid)

**Example**:
```http
GET /api/v1/posts/related/12345678-1234-1234-1234-123456789012
```

**Response**: Marketing posts + community posts related to product

---

### 5. POST /api/v1/posts/{postId}/interactions/{action}
**Purpose**: Track user interactions (PUBLIC endpoint)

**Path Parameters**:
- `postId` (guid)
- `action` (string) - view, click, share

**Query Parameters**:
- `postType` (string) - marketing, community

**Request Body**:
```json
{
  "action": "view",
  "source": "feed",
  "deviceType": "mobile"
}
```

**Example**:
```http
POST /api/v1/posts/{postId}/interactions/view?postType=marketing
Content-Type: application/json

{
  "action": "view",
  "source": "feed",
  "deviceType": "mobile"
}
```

**Response**: Interaction result with new count

---

## 🎨 Use Cases

### Use Case 1: Homepage Feed
```javascript
// Load mixed feed
GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4

// Infinite scroll - load more
GET /api/v1/posts/feed?pageNumber=2&pageSize=20&marketingRatio=4
```

### Use Case 2: Homepage Banner
```javascript
// Get top 5 marketing posts for banner carousel
GET /api/v1/posts/location/homepage_banner?pageSize=5

// Or get featured posts
GET /api/v1/posts/featured?pageSize=5
```

### Use Case 3: Product Page
```javascript
// Get related posts
GET /api/v1/posts/related/{productId}?pageSize=10

// Track view when user lands on page
POST /api/v1/posts/{postId}/interactions/view?postType=marketing
```

### Use Case 4: Analytics
```javascript
// Track view (when post appears in viewport)
POST /api/v1/posts/{postId}/interactions/view?postType=marketing

// Track click (when user clicks post)
POST /api/v1/posts/{postId}/interactions/click?postType=marketing

// Track share (when user shares post)
POST /api/v1/posts/{postId}/interactions/share?postType=marketing
```

---

## 🔧 Configuration Guidelines

### Marketing Ratio Recommendations

| Scenario | Ratio | Description |
|----------|-------|-------------|
| Flash Sale / Black Friday | 2-3 | Aggressive marketing |
| Promotion Period | 3-4 | High visibility |
| **Normal Operation** | **4-5** | **Balanced (Recommended)** |
| User-Focused | 6-8 | Low marketing |
| Community-First | 9-10 | Minimal marketing |

### Priority Score Strategy

| Priority | Use Case | Auto-Featured |
|----------|----------|---------------|
| 90-100 | Flash sales, major events | ✅ Yes |
| 81-89 | Important promotions | ✅ Yes |
| 71-80 | Regular promotions | ❌ No |
| 51-70 | Standard content | ❌ No |
| 1-50 | Background content | ❌ No |

---

## 📊 Data Flow

### Mixed Feed Flow
```
1. Client Request
   ↓
2. MixedFeedController.GetMixedFeed()
   ↓
3. MixedFeedService.GetMixedFeedAsync()
   ├─ Fetch Community Posts (sorted by PostedOn DESC)
   ├─ Fetch Marketing Posts (sorted by Priority DESC, PublishedDate DESC)
   └─ Mix using algorithm
   ↓
4. Apply Pagination
   ↓
5. Return PaginatedResult<MixedFeedDto>
```

### Interaction Tracking Flow
```
1. User Action (view/click/share)
   ↓
2. Frontend Event
   ↓
3. POST /api/v1/posts/{postId}/interactions/{action}
   ↓
4. MixedFeedService.TrackInteractionAsync()
   ├─ Identify post type (marketing/community)
   ├─ Increment counter (views++, clicks++, shares++)
   └─ Save to database
   ↓
5. Return PostInteractionResult
```

---

## 🚀 Testing

### Manual Testing

**1. Test Mixed Feed**:
```bash
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4"
```

**2. Test Location-Based**:
```bash
curl -X GET "https://localhost:7001/api/v1/posts/location/homepage_banner?pageSize=10"
```

**3. Test Featured Posts**:
```bash
curl -X GET "https://localhost:7001/api/v1/posts/featured?pageSize=5"
```

**4. Test Related Posts**:
```bash
curl -X GET "https://localhost:7001/api/v1/posts/related/12345678-1234-1234-1234-123456789012"
```

**5. Test Interaction Tracking**:
```bash
curl -X POST "https://localhost:7001/api/v1/posts/{postId}/interactions/view?postType=marketing" \
  -H "Content-Type: application/json" \
  -d '{"action":"view","source":"feed","deviceType":"mobile"}'
```

---

## 📈 Performance Considerations

### Optimization Tips

1. **Caching**:
   - Cache featured posts (TTL: 5-10 minutes)
   - Cache location-based posts (TTL: 5-10 minutes)
   - Cache product-related posts (TTL: 10-15 minutes)

2. **Database Indexes**:
   - `MarketingPosts.PriorityScore` (DESC)
   - `MarketingPosts.PublishedDate` (DESC)
   - `MarketingPosts.DisplayLocation` (JSON index)
   - `Posts.PostedOn` (DESC)

3. **Pagination**:
   - Use reasonable page sizes (10-20 items)
   - Implement cursor-based pagination for large datasets

4. **Analytics**:
   - Batch analytics updates if high traffic
   - Consider queue-based processing for analytics

---

## 🔐 Security

### Authentication
- Mixed feed endpoints: **Optional** (enhanced experience when logged in)
- Interaction tracking: **Public** (no auth required)
- Admin marketing post management: **Admin role required**

### Authorization
- Community posts: Owner can edit/delete
- Marketing posts: Admin only
- Interactions: Public tracking

### Data Protection
- Soft delete for posts
- Audit trails (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- Concurrency control with RowVersion

---

## 📚 Documentation Files

1. **MIXED_FEED_API_DOCUMENTATION.md**
   - Complete API reference
   - Request/response examples
   - Error handling
   - Frontend integration examples

2. **MIXED_FEED_ALGORITHM_EXPLANATION.md**
   - Algorithm details
   - Priority score system
   - Display location system
   - Performance metrics
   - Best practices

3. **MIXED_FEED_IMPLEMENTATION_SUMMARY.md** (this file)
   - Implementation overview
   - Files created
   - Configuration guidelines
   - Testing instructions

---

## ✅ Checklist

- [x] DTOs created
- [x] Service interface defined
- [x] Service implementation completed
- [x] API controller created
- [x] Service registered in DI
- [x] Documentation written
- [x] Algorithm explained
- [x] Use cases documented
- [x] Testing guide provided
- [x] **[NEW] Marketing post mapping cập nhật**
- [x] **[NEW] TaggedProduct integration**
- [x] **[NEW] Response format chuẩn hóa**
- [x] **[NEW] TaggedProductHelper integration**

---

## 🎉 Summary

**Mixed Feed API** đã được implement hoàn chỉnh với các tính năng:

✅ **Smart Mixing Algorithm**: Trộn community và marketing posts thông minh  
✅ **Priority-Based Sorting**: Sắp xếp theo priority score  
✅ **Location Filtering**: Filter theo vị trí hiển thị  
✅ **Featured Posts**: Auto-detect posts có priority > 80  
✅ **Product Relations**: Tìm posts liên quan product  
✅ **Analytics Tracking**: Track views, clicks, shares  
✅ **Flexible Configuration**: Tùy chỉnh marketing ratio  
✅ **Public Endpoints**: Interaction tracking không cần auth  
✅ **Comprehensive Documentation**: Đầy đủ docs và examples

**Default Configuration**:
- Marketing Ratio: 4 (1 marketing mỗi 4 community posts)
- Page Size: 20 items
- Priority Score: 50 (medium)
- Auto-Featured: Priority > 80

**API Ready**: Tất cả endpoints đã sẵn sàng để sử dụng! 🚀
