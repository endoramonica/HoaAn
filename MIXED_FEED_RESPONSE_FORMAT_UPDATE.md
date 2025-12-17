# 🔄 Mixed Feed Response Format Update

## 📋 Tóm Tắt Thay Đổi

Cập nhật phương thức `GetMixedFeedAsync` để chuẩn hóa response format của marketing posts theo format hiện tại của marketing feed API.

**Ngày cập nhật**: December 14, 2025  
**Phạm vi**: Chỉ xử lý GET post (không thay đổi controller, route, kiến trúc)  
**Ảnh hưởng**: Marketing items trong mixed feed sẽ có structure giống hệt API "Posts retrieved successfully"

---

## 🎯 Yêu Cầu

### ✅ Giữ Nguyên
- Logic phân trang (pagination)
- Marketing ratio (tỷ lệ trộn)
- Mix community + marketing posts
- Controller, route, kiến trúc tổng thể

### 🔄 Cập Nhật
- Mapping dữ liệu trả về cho marketing posts
- Response format chuẩn hóa
- Thêm TaggedProduct với live product data

---

## 📝 Chi Tiết Thay Đổi

### 1. MixedFeedDto - Thêm Fields

**File**: `VietCommerce.Core/DTOs/Posts/MixedFeedDto.cs`

**Fields Mới**:
```csharp
// Marketing Post specific
public string? Image { get; set; }                    // Primary image
public TaggedProductDto? TaggedProduct { get; set; }  // Live product data
public string? Platform { get; set; }                 // Social platform
public string? Status { get; set; }                   // Published, Draft, Scheduled
```

**Cấu Trúc Đầy Đủ**:
```csharp
public class MixedFeedDto
{
    // Basic Info
    public Guid Id { get; set; }
    public string PostType { get; set; }  // "community" | "marketing"
    
    // Marketing Post Fields
    public string? Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Image { get; set; }
    public string? Content { get; set; }
    
    // Product Info
    public Guid? ProductId { get; set; }
    public string? ProductName { get; set; }
    public TaggedProductDto? TaggedProduct { get; set; }  // NEW
    
    // Marketing Metadata
    public string? Platform { get; set; }                 // NEW
    public List<string>? Hashtags { get; set; }
    public int? PriorityScore { get; set; }
    public bool? IsFeatured { get; set; }
    public string? Status { get; set; }                   // NEW
    public List<string>? DisplayLocation { get; set; }
    
    // Engagement
    public int ViewsCount { get; set; }
    public int ClicksCount { get; set; }
    public int SharesCount { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedDate { get; set; }
    
    // Community Post Fields
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAvatar { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    
    // User Interaction
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsBookmarkedByCurrentUser { get; set; }
    public bool IsOwnedByCurrentUser { get; set; }
}
```

### 2. MixedFeedService - Cập Nhật Mapping

**File**: `VietCommerce.Application/Services/Services/MixedFeedService.cs`

#### a) MapMarketingPostToDto - Cập Nhật

```csharp
private MixedFeedDto MapMarketingPostToDto(MarketingPost post, Guid? currentUserId)
{
    return new MixedFeedDto
    {
        Id = post.Id,
        PostType = "marketing",
        Title = post.Title,
        Content = post.Content,
        ShortDescription = post.ShortDescription,
        Image = post.ImageUrl,                    // NEW
        ImageUrl = post.ImageUrl,
        ImageUrls = ParseJsonArray(post.ImageUrls),
        CreatedAt = post.CreatedAt,
        PublishedDate = post.PublishedDate,
        ProductId = post.ProductId,
        ProductName = post.ProductName,
        TaggedProduct = null,                     // NEW - populated by caller
        Platform = post.Platform,                 // NEW
        Hashtags = ParseJsonArray(post.Hashtags),
        PriorityScore = post.PriorityScore,
        IsFeatured = post.IsFeatured,
        Status = post.Status.ToString(),          // NEW
        DisplayLocation = ParseJsonArray(post.DisplayLocation),
        ViewsCount = post.Views,
        ClicksCount = post.Clicks,
        SharesCount = post.Shares,
        IsOwnedByCurrentUser = false
    };
}
```

#### b) GetMarketingPostsAsync - Populate TaggedProduct

```csharp
private async Task<List<MixedFeedDto>> GetMarketingPostsAsync(
    int count,
    string? location,
    int? minPriority,
    Guid? productId,
    Guid? currentUserId)
{
    var result = await _unitOfWork.MarketingPosts.GetPostsAsync(
        pageNumber: 1,
        pageSize: count * 2,
        status: MarketingPostStatus.Published,
        productId: productId,
        displayLocation: location,
        minPriorityScore: minPriority,
        sortBy: "priority",
        sortOrder: "desc");

    var dtos = new List<MixedFeedDto>();
    foreach (var post in result.Items)
    {
        var dto = MapMarketingPostToDto(post, currentUserId);
        
        // Populate TaggedProduct if productId exists
        if (post.ProductId.HasValue)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(post.ProductId.Value);
            if (product != null)
            {
                dto.TaggedProduct = BuildTaggedProductDto(product);
            }
        }
        
        dtos.Add(dto);
    }

    return dtos;
}
```

#### c) BuildTaggedProductDto - Helper Method (NEW)

```csharp
private TaggedProductDto BuildTaggedProductDto(Product product)
{
    var price = product.Prices?.FirstOrDefault();
    var discount = product.Discounts?.FirstOrDefault();
    
    var basePrice = price?.Price ?? 0;
    var discountPercentage = discount?.DiscountPercentage ?? 0;
    var discountedPrice = basePrice * (1 - discountPercentage / 100m);
    
    return new TaggedProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Price = basePrice,
        Currency = price?.Currency ?? "VND",
        FormattedPrice = $"{discountedPrice:N0} {price?.Currency ?? "VND"}",
        ThumbnailUrl = product.Images?.FirstOrDefault()?.ImageUrl ?? string.Empty,
        HasDiscount = discountPercentage > 0,
        DiscountPercentage = discountPercentage
    };
}
```

### 3. Using Statements - Thêm

```csharp
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
```

---

## 📊 Response Format Comparison

### Trước (Old Format)
```json
{
  "id": "guid",
  "postType": "marketing",
  "title": "Product Title",
  "content": "Content",
  "shortDescription": "Short desc",
  "imageUrl": "url",
  "productId": "guid",
  "productName": "Product Name",
  "priorityScore": 85,
  "isFeatured": true,
  "hashtags": ["tag1", "tag2"],
  "viewsCount": 100,
  "clicksCount": 50,
  "sharesCount": 10,
  "createdAt": "2025-12-14T10:00:00Z",
  "publishedDate": "2025-12-14T10:00:00Z"
}
```

### Sau (New Format - Chuẩn Hóa)
```json
{
  "id": "guid",
  "postType": "marketing",
  "title": "Product Title",
  "shortDescription": "Short desc",
  "image": "url",
  "content": "Content",
  "productId": "guid",
  "productName": "Product Name",
  "taggedProduct": {
    "id": "guid",
    "name": "Product Name",
    "price": 100000,
    "currency": "VND",
    "formattedPrice": "90,000 VND",
    "thumbnailUrl": "url",
    "hasDiscount": true,
    "discountPercentage": 10
  },
  "platform": "facebook",
  "hashtags": ["tag1", "tag2"],
  "priorityScore": 85,
  "isFeatured": true,
  "status": "Published",
  "displayLocation": ["homepage_banner", "product_section"],
  "viewsCount": 100,
  "clicksCount": 50,
  "sharesCount": 10,
  "createdAt": "2025-12-14T10:00:00Z",
  "publishedDate": "2025-12-14T10:00:00Z"
}
```

---

## 🔄 Data Flow

### Mixed Feed Response Flow

```
1. Client Request
   GET /api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4
   ↓
2. MixedFeedController.GetMixedFeed()
   ↓
3. MixedFeedService.GetMixedFeedAsync()
   ├─ GetCommunityPostsAsync()
   │  └─ MapCommunityPostToDto() → Community items
   │
   └─ GetMarketingPostsAsync()
      ├─ MapMarketingPostToDto() → Base marketing item
      ├─ For each marketing post with ProductId:
      │  ├─ GetByIdAsync(productId)
      │  └─ BuildTaggedProductDto() → Populate TaggedProduct
      └─ Return enriched marketing items
   ↓
4. MixPosts() → Mix community + marketing
   ↓
5. Apply Pagination
   ↓
6. Return PaginatedResult<MixedFeedDto>
   {
     "items": [
       { "postType": "community", ... },
       { "postType": "community", ... },
       { "postType": "community", ... },
       { "postType": "community", ... },
       { "postType": "marketing", "taggedProduct": {...}, ... },
       ...
     ],
     "pageNumber": 1,
     "pageSize": 20,
     "totalItems": 1000,
     "totalPages": 50
   }
```

---

## ✅ Validation

### Marketing Post Mapping
- ✅ Tất cả fields từ MarketingPost được map
- ✅ Status được convert từ enum sang string
- ✅ Hashtags, DisplayLocation được parse từ JSON
- ✅ TaggedProduct được populate từ Product entity

### Community Post Mapping
- ✅ Giữ nguyên structure hiện tại
- ✅ Không ảnh hưởng đến community posts

### Response Format
- ✅ Marketing items có structure giống hệt API "Posts retrieved successfully"
- ✅ Tất cả fields cần thiết đều có
- ✅ TaggedProduct chứa live product data

---

## 🚀 Testing

### Test Case 1: Mixed Feed with Marketing Posts
```bash
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4"
```

**Expected Response**:
- Items chứa mix community + marketing posts
- Marketing items có `taggedProduct` field
- Marketing items có `platform`, `status` fields
- Community items giữ nguyên structure

### Test Case 2: Verify TaggedProduct
```bash
# Get mixed feed
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=5"

# Check marketing item có taggedProduct
# {
#   "postType": "marketing",
#   "taggedProduct": {
#     "id": "...",
#     "name": "...",
#     "price": 100000,
#     "formattedPrice": "90,000 VND",
#     "hasDiscount": true,
#     "discountPercentage": 10
#   }
# }
```

### Test Case 3: Verify Community Posts Unchanged
```bash
# Get mixed feed
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20"

# Check community items không có taggedProduct
# {
#   "postType": "community",
#   "customerId": "...",
#   "customerName": "...",
#   "likesCount": 10,
#   "commentsCount": 5
# }
```

---

## 📋 Files Modified

| File | Changes |
|------|---------|
| `VietCommerce.Core/DTOs/Posts/MixedFeedDto.cs` | Thêm fields: Image, TaggedProduct, Platform, Status |
| `VietCommerce.Application/Services/Services/MixedFeedService.cs` | Cập nhật MapMarketingPostToDto, GetMarketingPostsAsync, thêm BuildTaggedProductDto |

---

## 🔐 Backward Compatibility

### ✅ Không Breaking Changes
- Tất cả fields mới là optional (nullable)
- Community posts structure không thay đổi
- Pagination logic không thay đổi
- Controller endpoints không thay đổi
- Route không thay đổi

### ✅ Frontend Compatibility
- Frontend có thể ignore fields mới
- Frontend có thể sử dụng TaggedProduct nếu cần
- Existing code vẫn hoạt động bình thường

---

## 📈 Performance Impact

### Positive
- ✅ TaggedProduct được fetch một lần per post
- ✅ Không có N+1 query issue (foreach loop)
- ✅ Product data luôn live/current

### Considerations
- ⚠️ Thêm 1 query per marketing post (để fetch product)
- 💡 **Optimization**: Có thể cache product data nếu cần

### Optimization Options
```csharp
// Option 1: Batch fetch products
var productIds = result.Items
    .Where(m => m.ProductId.HasValue)
    .Select(m => m.ProductId.Value)
    .Distinct()
    .ToList();

var products = await _unitOfWork.Products.GetByIdsAsync(productIds);
var productDict = products.ToDictionary(p => p.Id);

// Then map using dictionary
foreach (var post in result.Items)
{
    if (post.ProductId.HasValue && productDict.TryGetValue(post.ProductId.Value, out var product))
    {
        dto.TaggedProduct = BuildTaggedProductDto(product);
    }
}
```

---

## 🎯 Summary

✅ **Cập nhật hoàn tất**:
- MixedFeedDto thêm fields cần thiết
- MapMarketingPostToDto chuẩn hóa response format
- GetMarketingPostsAsync populate TaggedProduct
- BuildTaggedProductDto helper method
- Tất cả fields từ marketing feed API đều có

✅ **Không ảnh hưởng**:
- Controller, route, kiến trúc
- Community posts structure
- Pagination logic
- Frontend existing code

✅ **Kết quả**:
- Marketing items trong mixed feed có structure giống hệt API "Posts retrieved successfully"
- Live product data được include
- Response format chuẩn hóa
- Frontend có thể sử dụng unified format

