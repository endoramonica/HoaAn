# 🎯 Mixed Feed API Documentation

## Tổng Quan

Mixed Feed API kết hợp **Community Posts** (từ customers) và **Marketing Posts** (từ admin) thành một feed thống nhất với thuật toán trộn thông minh.

### Đặc Điểm Chính

✅ **Thuật toán trộn thông minh**: Marketing posts xuất hiện mỗi N community posts (configurable)  
✅ **Priority-based sorting**: Marketing posts có priority score cao hơn được ưu tiên  
✅ **Location-based filtering**: Lọc posts theo vị trí hiển thị cụ thể  
✅ **Featured posts**: Tự động đánh dấu posts có priority > 80  
✅ **Product-related posts**: Tìm posts liên quan đến sản phẩm  
✅ **Analytics tracking**: Track views, clicks, shares (public endpoints)

---

## 🔌 API Endpoints

### Base URL
```
Development: https://localhost:7001/api/v1/posts
Production: https://api.vietcommerce.com/api/v1/posts
```

### Endpoints Summary

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/feed` | Mixed feed với tỷ lệ marketing tùy chỉnh | Optional |
| GET | `/location/{location}` | Posts theo vị trí cụ thể | Optional |
| GET | `/featured` | Featured posts (priority > 80) | Optional |
| GET | `/related/{productId}` | Posts liên quan sản phẩm | Optional |
| POST | `/{postId}/interactions/{action}` | Track tương tác | No |

---

## 📖 Chi Tiết Endpoints

### 1. Get Mixed Feed

**Endpoint**: `GET /api/v1/posts/feed`

**Description**: Lấy feed hỗn hợp với thuật toán trộn thông minh. Marketing posts xuất hiện đều đặn giữa community posts.

**Query Parameters**:
- `pageNumber` (int, default: 1) - Số trang
- `pageSize` (int, default: 20) - Số items mỗi trang
- `marketingRatio` (int, default: 4, range: 1-10) - Tỷ lệ marketing posts
  - `3` = 1 marketing post mỗi 3 community posts
  - `4` = 1 marketing post mỗi 4 community posts
  - `5` = 1 marketing post mỗi 5 community posts
- `location` (string, optional) - Filter marketing posts theo location
- `minPriorityScore` (int, optional) - Minimum priority score cho marketing posts
- `productId` (guid, optional) - Filter marketing posts theo product

**Example Request**:
```http
GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "11111111-1111-1111-1111-111111111111",
        "postType": "community",
        "content": "Vừa mua điện thoại mới, rất hài lòng!",
        "imageUrl": "https://cdn.example.com/photo1.jpg",
        "customerId": "22222222-2222-2222-2222-222222222222",
        "customerName": "Nguyễn Văn A",
        "customerAvatar": "https://cdn.example.com/avatar1.jpg",
        "createdAt": "2024-12-04T10:00:00Z",
        "likesCount": 45,
        "commentsCount": 12,
        "isLikedByCurrentUser": false,
        "isBookmarkedByCurrentUser": false
      },
      {
        "id": "33333333-3333-3333-3333-333333333333",
        "postType": "community",
        "content": "Review chi tiết về sản phẩm...",
        "imageUrl": "https://cdn.example.com/photo2.jpg",
        "customerId": "44444444-4444-4444-4444-444444444444",
        "customerName": "Trần Thị B",
        "createdAt": "2024-12-04T09:30:00Z",
        "likesCount": 23,
        "commentsCount": 5
      },
      {
        "id": "55555555-5555-5555-5555-555555555555",
        "postType": "community",
        "content": "Đang tìm mua laptop...",
        "customerId": "66666666-6666-6666-6666-666666666666",
        "customerName": "Lê Văn C",
        "createdAt": "2024-12-04T09:00:00Z",
        "likesCount": 8,
        "commentsCount": 3
      },
      {
        "id": "77777777-7777-7777-7777-777777777777",
        "postType": "community",
        "content": "Cảm ơn shop đã giao hàng nhanh!",
        "customerId": "88888888-8888-8888-8888-888888888888",
        "customerName": "Phạm Thị D",
        "createdAt": "2024-12-04T08:30:00Z",
        "likesCount": 67,
        "commentsCount": 15
      },
      {
        "id": "99999999-9999-9999-9999-999999999999",
        "postType": "marketing",
        "title": "🔥 Flash Sale - Giảm 70% Điện Thoại",
        "content": "Chương trình flash sale đặc biệt...",
        "shortDescription": "Flash sale điện thoại giảm đến 70%",
        "imageUrl": "https://cdn.example.com/flash-sale.jpg",
        "productId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
        "productName": "iPhone 15 Pro Max",
        "priorityScore": 95,
        "isFeatured": true,
        "displayLocation": ["homepage_banner", "featured_section"],
        "hashtags": ["FlashSale", "DienThoai", "GiamGia"],
        "publishedDate": "2024-12-04T08:00:00Z",
        "viewsCount": 12500,
        "clicksCount": 3400,
        "sharesCount": 890
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalCount": 1250,
    "totalPages": 63
  },
  "message": "Mixed feed retrieved successfully"
}
```

**Thuật Toán Trộn**:
```
marketingRatio = 4:
┌─────────────────────────────────┐
│ Community Post 1                │
│ Community Post 2                │
│ Community Post 3                │
│ Community Post 4                │
│ ⭐ Marketing Post 1 (Priority 95)│
│ Community Post 5                │
│ Community Post 6                │
│ Community Post 7                │
│ Community Post 8                │
│ ⭐ Marketing Post 2 (Priority 90)│
│ Community Post 9                │
│ ...                             │
└─────────────────────────────────┘
```

---

### 2. Get Posts by Location

**Endpoint**: `GET /api/v1/posts/location/{location}`

**Description**: Lấy marketing posts được tag cho vị trí hiển thị cụ thể.

**Path Parameters**:
- `location` (string, required) - Display location

**Valid Locations**:
- `homepage_banner` - Banner trang chủ
- `product_section` - Trong trang sản phẩm
- `featured_section` - Mục nổi bật
- `sidebar` - Thanh bên

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 20)

**Example Request**:
```http
GET /api/v1/posts/location/homepage_banner?pageNumber=1&pageSize=10
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "location": "homepage_banner",
    "posts": [
      {
        "id": "99999999-9999-9999-9999-999999999999",
        "postType": "marketing",
        "title": "🔥 Flash Sale - Giảm 70%",
        "priorityScore": 95,
        "isFeatured": true,
        "displayLocation": ["homepage_banner", "featured_section"],
        "viewsCount": 12500,
        "clicksCount": 3400
      },
      {
        "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
        "postType": "marketing",
        "title": "Black Friday Sale 2024",
        "priorityScore": 90,
        "isFeatured": true,
        "displayLocation": ["homepage_banner"],
        "viewsCount": 8900,
        "clicksCount": 2100
      }
    ],
    "totalCount": 15
  },
  "message": "Posts for location 'homepage_banner' retrieved successfully"
}
```

**Use Cases**:
- Homepage banner carousel
- Product page sidebar ads
- Featured section trong app
- Promotional banners

---

### 3. Get Featured Posts

**Endpoint**: `GET /api/v1/posts/featured`

**Description**: Lấy marketing posts có độ ưu tiên cao nhất (priority > 80).

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 10)

**Example Request**:
```http
GET /api/v1/posts/featured?pageNumber=1&pageSize=10
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "posts": [
      {
        "id": "99999999-9999-9999-9999-999999999999",
        "postType": "marketing",
        "title": "🔥 Flash Sale - Giảm 70%",
        "priorityScore": 95,
        "isFeatured": true,
        "viewsCount": 12500,
        "clicksCount": 3400,
        "sharesCount": 890
      },
      {
        "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
        "postType": "marketing",
        "title": "Black Friday Sale 2024",
        "priorityScore": 90,
        "isFeatured": true,
        "viewsCount": 8900,
        "clicksCount": 2100,
        "sharesCount": 450
      }
    ],
    "totalFeatured": 8
  },
  "message": "Featured posts retrieved successfully"
}
```

**Sorting**:
1. Priority Score (cao nhất trước)
2. Views (nhiều nhất trước)

**Use Cases**:
- Featured carousel
- Top promotions section
- Special offers page

---

### 4. Get Related Posts by Product

**Endpoint**: `GET /api/v1/posts/related/{productId}`

**Description**: Lấy posts liên quan đến một sản phẩm (cả marketing và community posts).

**Path Parameters**:
- `productId` (guid, required) - Product ID

**Query Parameters**:
- `pageNumber` (int, default: 1)
- `pageSize` (int, default: 20)

**Example Request**:
```http
GET /api/v1/posts/related/12345678-1234-1234-1234-123456789012
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "productId": "12345678-1234-1234-1234-123456789012",
    "productName": "iPhone 15 Pro Max",
    "marketingPosts": [
      {
        "id": "99999999-9999-9999-9999-999999999999",
        "postType": "marketing",
        "title": "iPhone 15 Pro Max - Giảm 20%",
        "productId": "12345678-1234-1234-1234-123456789012",
        "productName": "iPhone 15 Pro Max",
        "priorityScore": 85,
        "viewsCount": 5600
      }
    ],
    "communityPosts": [
      {
        "id": "11111111-1111-1111-1111-111111111111",
        "postType": "community",
        "content": "Vừa mua iPhone 15 Pro Max, rất hài lòng!",
        "customerId": "22222222-2222-2222-2222-222222222222",
        "customerName": "Nguyễn Văn A",
        "likesCount": 45,
        "commentsCount": 12
      }
    ],
    "totalCount": 12
  },
  "message": "Related posts retrieved successfully"
}
```

**Use Cases**:
- Product detail page
- Related content section
- Product reviews and promotions

---

### 5. Track Interaction

**Endpoint**: `POST /api/v1/posts/{postId}/interactions/{action}`

**Description**: Track user interactions (views, clicks, shares). Public endpoint, không cần authentication.

**Path Parameters**:
- `postId` (guid, required) - Post ID
- `action` (string, required) - Action type: `view`, `click`, `share`

**Query Parameters**:
- `postType` (string, optional, default: "marketing") - Post type: `community` hoặc `marketing`

**Request Body**:
```json
{
  "action": "view",
  "source": "feed",
  "deviceType": "mobile"
}
```

**Fields**:
- `action` (string, required) - Must match URL action
- `source` (string, optional) - Source: `feed`, `detail`, `search`, `related`
- `deviceType` (string, optional) - Device: `mobile`, `desktop`, `tablet`

**Example Requests**:

**Track View**:
```http
POST /api/v1/posts/99999999-9999-9999-9999-999999999999/interactions/view?postType=marketing
Content-Type: application/json

{
  "action": "view",
  "source": "feed",
  "deviceType": "mobile"
}
```

**Track Click**:
```http
POST /api/v1/posts/99999999-9999-9999-9999-999999999999/interactions/click?postType=marketing
Content-Type: application/json

{
  "action": "click",
  "source": "detail",
  "deviceType": "desktop"
}
```

**Track Share**:
```http
POST /api/v1/posts/99999999-9999-9999-9999-999999999999/interactions/share?postType=marketing
Content-Type: application/json

{
  "action": "share",
  "source": "feed",
  "deviceType": "mobile"
}
```

**Example Response**:
```json
{
  "success": true,
  "data": {
    "success": true,
    "action": "view",
    "newCount": 12501
  },
  "message": "view tracked successfully"
}
```

**Actions**:
- `view`: User xem post trong feed hoặc detail page
- `click`: User click vào post để xem chi tiết hoặc click product link
- `share`: User share post lên social media

**Note**: Endpoint này là **PUBLIC**, không cần JWT token.

---

## 🎨 Use Cases & Examples

### Use Case 1: Homepage Feed

```javascript
// Lấy mixed feed cho homepage với tỷ lệ 1:4 (1 marketing mỗi 4 community)
GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4

// Khi user scroll, load thêm
GET /api/v1/posts/feed?pageNumber=2&pageSize=20&marketingRatio=4
```

### Use Case 2: Homepage Banner Carousel

```javascript
// Lấy top 5 marketing posts cho banner
GET /api/v1/posts/location/homepage_banner?pageNumber=1&pageSize=5

// Hoặc lấy featured posts
GET /api/v1/posts/featured?pageNumber=1&pageSize=5
```

### Use Case 3: Product Detail Page

```javascript
// Lấy posts liên quan đến product
GET /api/v1/posts/related/12345678-1234-1234-1234-123456789012?pageSize=10

// Track view khi user xem product page
POST /api/v1/posts/{marketingPostId}/interactions/view?postType=marketing
{
  "action": "view",
  "source": "product_page",
  "deviceType": "desktop"
}
```

### Use Case 4: Analytics Tracking

```javascript
// User xem post trong feed
POST /api/v1/posts/{postId}/interactions/view?postType=marketing
{ "action": "view", "source": "feed", "deviceType": "mobile" }

// User click vào post
POST /api/v1/posts/{postId}/interactions/click?postType=marketing
{ "action": "click", "source": "feed", "deviceType": "mobile" }

// User share post
POST /api/v1/posts/{postId}/interactions/share?postType=marketing
{ "action": "share", "source": "detail", "deviceType": "mobile" }
```

---

## 🔧 Configuration

### Marketing Ratio Guidelines

| Ratio | Description | Use Case |
|-------|-------------|----------|
| 2 | 1 marketing mỗi 2 community | Aggressive marketing |
| 3 | 1 marketing mỗi 3 community | High promotion period |
| 4 | 1 marketing mỗi 4 community | **Default - Balanced** |
| 5 | 1 marketing mỗi 5 community | Normal operation |
| 6-10 | 1 marketing mỗi 6-10 community | Low marketing mode |

### Priority Score Guidelines

| Score | Category | Auto-Featured | Use Case |
|-------|----------|---------------|----------|
| 90-100 | Critical | ✅ Yes | Flash sales, major events |
| 81-89 | High | ✅ Yes | Important promotions |
| 71-80 | Medium-High | ❌ No | Regular promotions |
| 51-70 | Medium | ❌ No | Standard content |
| 1-50 | Low | ❌ No | Background content |

---

## 📊 Response Format

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful",
  "statusCode": 200
}
```

### Error Response
```json
{
  "success": false,
  "data": null,
  "message": "Error message",
  "errors": ["Detailed error 1", "Detailed error 2"],
  "statusCode": 400
}
```

---

## 🚀 Performance Tips

1. **Caching**: Cache featured posts và location-based posts (TTL: 5-10 minutes)
2. **Pagination**: Sử dụng pageSize hợp lý (10-20 items)
3. **Lazy Loading**: Load thêm khi user scroll
4. **Image Optimization**: Sử dụng CDN và lazy load images
5. **Analytics Batching**: Batch analytics requests nếu có nhiều interactions

---

## 🔐 Security Notes

- Mixed feed endpoints: **Optional authentication** (enhanced experience khi logged in)
- Interaction tracking: **Public** (không cần auth)
- Admin marketing post management: **Admin role required**
- Soft delete: Deleted posts không xuất hiện trong feed

---

## 📱 Frontend Integration Example

```typescript
// React/Next.js example
const MixedFeed = () => {
  const [posts, setPosts] = useState([]);
  const [page, setPage] = useState(1);

  useEffect(() => {
    fetchMixedFeed();
  }, [page]);

  const fetchMixedFeed = async () => {
    const response = await fetch(
      `/api/v1/posts/feed?pageNumber=${page}&pageSize=20&marketingRatio=4`
    );
    const data = await response.json();
    setPosts([...posts, ...data.data.items]);
  };

  const trackView = async (postId, postType) => {
    await fetch(
      `/api/v1/posts/${postId}/interactions/view?postType=${postType}`,
      {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          action: 'view',
          source: 'feed',
          deviceType: 'mobile'
        })
      }
    );
  };

  return (
    <div>
      {posts.map(post => (
        <PostCard 
          key={post.id} 
          post={post} 
          onView={() => trackView(post.id, post.postType)}
        />
      ))}
    </div>
  );
};
```

---

## 📞 Support

For API support:
- Email: support@vietcommerce.com
- Documentation: https://docs.vietcommerce.com
- Swagger UI: https://localhost:7001/swagger
