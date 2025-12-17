# 📋 Kịch Bản Triển Khai: GetMixedFeedAsync Refactoring

## 🎯 Mục Tiêu

Cập nhật `GetMixedFeedAsync` để học hỏi từ pattern của `MarketingPostService.GetPostsAsync()`:
- ✅ Sử dụng `ExecuteAsApiResponseAsync` pattern từ BaseService
- ✅ Thêm logging chi tiết với emoji
- ✅ Cải thiện error handling
- ✅ Thêm validation parameters
- ✅ Sử dụng TaggedProductHelper thay vì BuildTaggedProductDto
- ✅ Tối ưu hóa performance

---

## 📊 So Sánh Pattern

### Pattern Hiện Tại (MixedFeedService)
```csharp
public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    try
    {
        // Validate marketing ratio
        if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
        {
            return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
                "Marketing ratio must be between 1 and 10");
        }

        var currentUserId = _currentUser.UserId;
        var mixedFeed = new List<MixedFeedDto>();

        // ... logic ...

        return ApiResponse<PaginatedResult<MixedFeedDto>>.SuccessResponse(
            result,
            "Mixed feed retrieved successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting mixed feed");
        return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
            "Failed to retrieve mixed feed");
    }
}
```

### Pattern Mục Tiêu (Học từ MarketingPostService)
```csharp
public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    return await ExecuteAsApiResponseAsync(async () =>
    {
        LogInfo($"📋 Getting mixed feed - Page: {query.PageNumber}, Size: {query.PageSize}, Ratio: {query.MarketingRatio}");

        // Validate pagination parameters
        if (query.PageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(query.PageNumber));
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100", nameof(query.PageSize));
        }

        // Validate marketing ratio
        if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
        {
            throw new ArgumentException("Marketing ratio must be between 1 and 10", nameof(query.MarketingRatio));
        }

        var currentUserId = _currentUser.UserId;

        // ... logic ...

        LogInfo($"✅ Retrieved {pagedItems.Count} mixed posts (Total: {totalCount})");
        return result;

    }, "GetMixedFeedAsync", "Mixed feed retrieved successfully");
}
```

---

## 🔄 Chi Tiết Thay Đổi

### Phase 1: Kế Thừa từ BaseService

**Hiện Tại**: MixedFeedService không kế thừa BaseService
```csharp
public class MixedFeedService : IMixedFeedService
{
    private IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MixedFeedService> _logger;
}
```

**Mục Tiêu**: Kế thừa BaseService
```csharp
public class MixedFeedService : BaseService, IMixedFeedService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public MixedFeedService(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<MixedFeedService> logger,
        ICacheService cacheService)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }
}
```

**Lợi Ích**:
- ✅ Sử dụng `ExecuteAsApiResponseAsync` pattern
- ✅ Sử dụng `LogInfo`, `LogWarning`, `LogError` methods
- ✅ Sử dụng `ValidateId`, `ValidateNotEmpty` helpers
- ✅ Tích hợp caching support
- ✅ Standardized error handling

---

### Phase 2: Refactor GetMixedFeedAsync

#### Step 1: Thay Đổi Cấu Trúc

**Trước**:
```csharp
public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    try
    {
        // ... logic ...
        return ApiResponse<PaginatedResult<MixedFeedDto>>.SuccessResponse(...);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting mixed feed");
        return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(...);
    }
}
```

**Sau**:
```csharp
public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    return await ExecuteAsApiResponseAsync(async () =>
    {
        // ... logic ...
        return result;
    }, "GetMixedFeedAsync", "Mixed feed retrieved successfully");
}
```

#### Step 2: Thêm Logging Chi Tiết

**Trước**: Không có logging
```csharp
var currentUserId = _currentUser.UserId;
var mixedFeed = new List<MixedFeedDto>();
```

**Sau**: Logging chi tiết
```csharp
LogInfo($"📋 Getting mixed feed - Page: {query.PageNumber}, Size: {query.PageSize}, Ratio: {query.MarketingRatio}");

var currentUserId = _currentUser.UserId;
LogDebug($"Current user: {currentUserId}");

var mixedFeed = new List<MixedFeedDto>();
LogDebug($"Calculated: {marketingCount} marketing, {communityCount} community posts");
```

#### Step 3: Cải Thiện Validation

**Trước**:
```csharp
if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
{
    return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
        "Marketing ratio must be between 1 and 10");
}
```

**Sau**:
```csharp
// Validate pagination parameters
if (query.PageNumber < 1)
{
    throw new ArgumentException("Page number must be greater than 0", nameof(query.PageNumber));
}

if (query.PageSize < 1 || query.PageSize > 100)
{
    throw new ArgumentException("Page size must be between 1 and 100", nameof(query.PageSize));
}

// Validate marketing ratio
if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
{
    throw new ArgumentException("Marketing ratio must be between 1 and 10", nameof(query.MarketingRatio));
}
```

#### Step 4: Thêm Logging Kết Quả

**Trước**: Không có logging kết quả
```csharp
return ApiResponse<PaginatedResult<MixedFeedDto>>.SuccessResponse(
    result,
    "Mixed feed retrieved successfully");
```

**Sau**: Logging kết quả chi tiết
```csharp
LogInfo($"✅ Retrieved {pagedItems.Count} mixed posts (Total: {totalCount})");
return result;
```

---

### Phase 3: Cập Nhật GetMarketingPostsAsync

#### Sử Dụng TaggedProductHelper

**Trước**:
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

**Sau** (Sử dụng TaggedProductHelper):
```csharp
// Enrich with TaggedProduct for each post with a valid productId
foreach (var dto in dtos)
{
    if (dto.ProductId.HasValue)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId.Value);
        dto.TaggedProduct = TaggedProductHelper.BuildTaggedProductDto(product);
    }
}
```

**Lợi Ích**:
- ✅ Sử dụng helper class chung (DRY principle)
- ✅ Tránh duplicate code
- ✅ Dễ maintain
- ✅ Consistent với MarketingPostService

---

### Phase 4: Tối Ưu Hóa Performance

#### Option 1: Batch Fetch Products (Recommended)

**Trước** (N+1 Query Problem):
```csharp
foreach (var post in result.Items)
{
    var dto = MapMarketingPostToDto(post, currentUserId);
    
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
```

**Sau** (Batch Fetch):
```csharp
// Map to DTOs
var dtos = result.Items.Select(m => MapMarketingPostToDto(m, currentUserId)).ToList();

// Batch fetch products
var productIds = dtos
    .Where(d => d.ProductId.HasValue)
    .Select(d => d.ProductId.Value)
    .Distinct()
    .ToList();

var products = await _unitOfWork.Products.GetByIdsAsync(productIds);
var productDict = products.ToDictionary(p => p.Id);

// Enrich with TaggedProduct
foreach (var dto in dtos)
{
    if (dto.ProductId.HasValue && productDict.TryGetValue(dto.ProductId.Value, out var product))
    {
        dto.TaggedProduct = TaggedProductHelper.BuildTaggedProductDto(product);
    }
}
```

**Performance Improvement**:
- ❌ Before: N+1 queries (1 + N products)
- ✅ After: 2 queries (1 posts + 1 batch products)

#### Option 2: Caching

```csharp
private const string MIXED_FEED_CACHE_KEY = "mixed_feed";
private static readonly TimeSpan MIXED_FEED_CACHE_DURATION = TimeSpan.FromMinutes(5);

public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    var cacheKey = CreateCacheKey(MIXED_FEED_CACHE_KEY, query.PageNumber, query.PageSize, query.MarketingRatio);
    
    return await ExecuteAsApiResponseAsync(async () =>
    {
        var result = await GetFromCacheOrExecuteAsync(
            cacheKey,
            async () => await GetMixedFeedInternalAsync(query),
            MIXED_FEED_CACHE_DURATION);
        
        return result;
    }, "GetMixedFeedAsync", "Mixed feed retrieved successfully");
}

private async Task<PaginatedResult<MixedFeedDto>> GetMixedFeedInternalAsync(MixedFeedQuery query)
{
    // ... actual logic ...
}
```

---

## 📝 Kịch Bản Triển Khai Chi Tiết

### Step 1: Cập Nhật Class Declaration

```csharp
// BEFORE
public class MixedFeedService : IMixedFeedService
{
    private IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MixedFeedService> _logger;

    public MixedFeedService(
       IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<MixedFeedService> logger)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }
}

// AFTER
public class MixedFeedService : BaseService, IMixedFeedService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public MixedFeedService(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<MixedFeedService> logger,
        ICacheService cacheService)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }
}
```

### Step 2: Refactor GetMixedFeedAsync

```csharp
// BEFORE
public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    try
    {
        // Validate marketing ratio
        if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
        {
            return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
                "Marketing ratio must be between 1 and 10");
        }

        var currentUserId = _currentUser.UserId;
        var mixedFeed = new List<MixedFeedDto>();

        // Calculate how many of each type we need
        var totalItems = query.PageSize;
        var marketingCount = totalItems / (query.MarketingRatio + 1);
        var communityCount = totalItems - marketingCount;

        // Get community posts
        var communityPosts = await GetCommunityPostsAsync(communityCount, currentUserId);

        // Get marketing posts
        var marketingPosts = await GetMarketingPostsAsync(
            marketingCount,
            query.Location,
            query.MinPriorityScore,
            query.ProductId,
            currentUserId);

        // Mix posts using smart algorithm
        mixedFeed = MixPosts(communityPosts, marketingPosts, query.MarketingRatio);

        // Apply pagination
        var skip = (query.PageNumber - 1) * query.PageSize;
        var pagedItems = mixedFeed.Skip(skip).Take(query.PageSize).ToList();

        // Get total count (approximate)
        var totalCommunity = await _unitOfWork.Posts.CountAsync(p => !p.IsDeleted && p.IsActive);
        var totalMarketing = await _unitOfWork.MarketingPosts.CountAsync(m => !m.IsDeleted && m.Status == MarketingPostStatus.Published);
        var totalCount = totalCommunity + totalMarketing;

        var result = new PaginatedResult<MixedFeedDto>
        {
            Items = pagedItems,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };

        return ApiResponse<PaginatedResult<MixedFeedDto>>.SuccessResponse(
            result,
            "Mixed feed retrieved successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting mixed feed");
        return ApiResponse<PaginatedResult<MixedFeedDto>>.FailureResponse(
            "Failed to retrieve mixed feed");
    }
}

// AFTER
public async Task<ApiResponse<PaginatedResult<MixedFeedDto>>> GetMixedFeedAsync(MixedFeedQuery query)
{
    return await ExecuteAsApiResponseAsync(async () =>
    {
        LogInfo($"📋 Getting mixed feed - Page: {query.PageNumber}, Size: {query.PageSize}, Ratio: {query.MarketingRatio}");

        // Validate pagination parameters
        if (query.PageNumber < 1)
        {
            throw new ArgumentException("Page number must be greater than 0", nameof(query.PageNumber));
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100", nameof(query.PageSize));
        }

        // Validate marketing ratio
        if (query.MarketingRatio < 1 || query.MarketingRatio > 10)
        {
            throw new ArgumentException("Marketing ratio must be between 1 and 10", nameof(query.MarketingRatio));
        }

        var currentUserId = _currentUser.UserId;
        LogDebug($"Current user: {currentUserId}");

        // Calculate how many of each type we need
        var totalItems = query.PageSize;
        var marketingCount = totalItems / (query.MarketingRatio + 1);
        var communityCount = totalItems - marketingCount;
        LogDebug($"Calculated: {marketingCount} marketing, {communityCount} community posts");

        // Get community posts
        var communityPosts = await GetCommunityPostsAsync(communityCount, currentUserId);
        LogDebug($"Retrieved {communityPosts.Count} community posts");

        // Get marketing posts
        var marketingPosts = await GetMarketingPostsAsync(
            marketingCount,
            query.Location,
            query.MinPriorityScore,
            query.ProductId,
            currentUserId);
        LogDebug($"Retrieved {marketingPosts.Count} marketing posts");

        // Mix posts using smart algorithm
        var mixedFeed = MixPosts(communityPosts, marketingPosts, query.MarketingRatio);
        LogDebug($"Mixed {mixedFeed.Count} total posts");

        // Apply pagination
        var skip = (query.PageNumber - 1) * query.PageSize;
        var pagedItems = mixedFeed.Skip(skip).Take(query.PageSize).ToList();

        // Get total count (approximate)
        var totalCommunity = await _unitOfWork.Posts.CountAsync(p => !p.IsDeleted && p.IsActive);
        var totalMarketing = await _unitOfWork.MarketingPosts.CountAsync(m => !m.IsDeleted && m.Status == MarketingPostStatus.Published);
        var totalCount = totalCommunity + totalMarketing;

        var result = new PaginatedResult<MixedFeedDto>
        {
            Items = pagedItems,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalItems = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };

        LogInfo($"✅ Retrieved {pagedItems.Count} mixed posts (Total: {totalCount})");
        return result;

    }, "GetMixedFeedAsync", "Mixed feed retrieved successfully");
}
```

### Step 3: Cập Nhật GetMarketingPostsAsync

```csharp
// BEFORE
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

// AFTER (Batch Fetch Optimization)
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

    // Map to DTOs
    var dtos = result.Items.Select(m => MapMarketingPostToDto(m, currentUserId)).ToList();

    // Batch fetch products for better performance
    var productIds = dtos
        .Where(d => d.ProductId.HasValue)
        .Select(d => d.ProductId.Value)
        .Distinct()
        .ToList();

    if (productIds.Any())
    {
        var products = await _unitOfWork.Products.GetByIdsAsync(productIds);
        var productDict = products.ToDictionary(p => p.Id);

        // Enrich with TaggedProduct
        foreach (var dto in dtos)
        {
            if (dto.ProductId.HasValue && productDict.TryGetValue(dto.ProductId.Value, out var product))
            {
                dto.TaggedProduct = TaggedProductHelper.BuildTaggedProductDto(product);
            }
        }
    }

    return dtos;
}
```

### Step 4: Xóa BuildTaggedProductDto (Không Cần Nữa)

```csharp
// DELETE THIS METHOD - Use TaggedProductHelper instead
// private TaggedProductDto BuildTaggedProductDto(Product product)
// {
//     ...
// }
```

### Step 5: Thêm Using Statements

```csharp
using VietCommerce.Application.Helpers;  // For TaggedProductHelper
using VietCommerce.Application.Services.Services;  // For BaseService
```

---

## 🧪 Testing Plan

### Unit Tests

```csharp
[TestClass]
public class MixedFeedServiceTests
{
    private MixedFeedService _service;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ICurrentUser> _currentUserMock;
    private Mock<ILogger<MixedFeedService>> _loggerMock;
    private Mock<ICacheService> _cacheServiceMock;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _loggerMock = new Mock<ILogger<MixedFeedService>>();
        _cacheServiceMock = new Mock<ICacheService>();

        _service = new MixedFeedService(
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _loggerMock.Object,
            _cacheServiceMock.Object);
    }

    [TestMethod]
    public async Task GetMixedFeedAsync_WithValidQuery_ReturnsSuccess()
    {
        // Arrange
        var query = new MixedFeedQuery { PageNumber = 1, PageSize = 20, MarketingRatio = 4 };
        
        // Act
        var result = await _service.GetMixedFeedAsync(query);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task GetMixedFeedAsync_WithInvalidPageNumber_ReturnsFail()
    {
        // Arrange
        var query = new MixedFeedQuery { PageNumber = 0, PageSize = 20, MarketingRatio = 4 };
        
        // Act
        var result = await _service.GetMixedFeedAsync(query);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains("Page number"));
    }

    [TestMethod]
    public async Task GetMixedFeedAsync_WithInvalidPageSize_ReturnsFail()
    {
        // Arrange
        var query = new MixedFeedQuery { PageNumber = 1, PageSize = 101, MarketingRatio = 4 };
        
        // Act
        var result = await _service.GetMixedFeedAsync(query);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains("Page size"));
    }

    [TestMethod]
    public async Task GetMixedFeedAsync_WithInvalidMarketingRatio_ReturnsFail()
    {
        // Arrange
        var query = new MixedFeedQuery { PageNumber = 1, PageSize = 20, MarketingRatio = 11 };
        
        // Act
        var result = await _service.GetMixedFeedAsync(query);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.Message.Contains("Marketing ratio"));
    }
}
```

### Integration Tests

```bash
# Test 1: Get mixed feed
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=4"

# Test 2: Invalid page number
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=0&pageSize=20&marketingRatio=4"

# Test 3: Invalid page size
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=101&marketingRatio=4"

# Test 4: Invalid marketing ratio
curl -X GET "https://localhost:7001/api/v1/posts/feed?pageNumber=1&pageSize=20&marketingRatio=11"
```

---

## ✅ Checklist Triển Khai

- [ ] **Phase 1: Kế Thừa BaseService**
  - [ ] Thay đổi class declaration
  - [ ] Cập nhật constructor
  - [ ] Thêm using statements

- [ ] **Phase 2: Refactor GetMixedFeedAsync**
  - [ ] Sử dụng ExecuteAsApiResponseAsync
  - [ ] Thêm logging chi tiết
  - [ ] Cải thiện validation
  - [ ] Thêm logging kết quả

- [ ] **Phase 3: Cập Nhật GetMarketingPostsAsync**
  - [ ] Implement batch fetch optimization
  - [ ] Sử dụng TaggedProductHelper
  - [ ] Xóa BuildTaggedProductDto

- [ ] **Phase 4: Testing**
  - [ ] Unit tests pass
  - [ ] Integration tests pass
  - [ ] No breaking changes
  - [ ] Performance improved

- [ ] **Phase 5: Documentation**
  - [ ] Update code comments
  - [ ] Update API documentation
  - [ ] Create migration guide

---

## 📊 Expected Improvements

### Code Quality
- ✅ Standardized error handling
- ✅ Better logging
- ✅ Improved validation
- ✅ DRY principle (reuse TaggedProductHelper)

### Performance
- ✅ Batch fetch products (N+1 → 2 queries)
- ✅ Optional caching support
- ✅ Reduced database calls

### Maintainability
- ✅ Consistent with MarketingPostService pattern
- ✅ Easier to debug with detailed logging
- ✅ Better error messages

### User Experience
- ✅ Better error messages
- ✅ Faster response times
- ✅ More reliable API

---

## 🚀 Deployment Steps

1. **Backup Current Code**
   ```bash
   git checkout -b feature/mixed-feed-refactoring
   ```

2. **Implement Changes**
   - Follow the script above
   - Test each phase

3. **Run Tests**
   ```bash
   dotnet test VietCommerce.Tests
   ```

4. **Code Review**
   - Review with team
   - Get approval

5. **Deploy**
   ```bash
   git commit -m "refactor: improve GetMixedFeedAsync with BaseService pattern"
   git push origin feature/mixed-feed-refactoring
   ```

6. **Monitor**
   - Check logs
   - Monitor performance
   - Verify no issues

---

## 📞 Support & Questions

Nếu có câu hỏi hoặc cần clarification:
1. Xem BaseService.cs để hiểu ExecuteAsApiResponseAsync
2. Xem MarketingPostService.cs để xem pattern
3. Xem TaggedProductHelper để hiểu helper class
4. Kiểm tra unit tests để xem expected behavior

---

**Status**: 📋 Ready for Review  
**Estimated Time**: 2-3 hours  
**Risk Level**: Low (Refactoring only, no logic changes)  
**Rollback Plan**: Revert to previous commit

