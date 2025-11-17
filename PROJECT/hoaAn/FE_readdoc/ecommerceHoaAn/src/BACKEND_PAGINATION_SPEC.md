# Backend Pagination Specification cho VietCommerce API
## Yêu cầu từ Frontend Developer

> **⚠️ DEPRECATED:** Document này đã lỗi thời. Vui lòng tham khảo cấu trúc thực tế từ Backend .NET 8 dưới đây.

---

## 📋 MỤC ĐÍCH
Document này cung cấp specification chi tiết về cấu trúc pagination response THỰC TẾ từ Backend .NET 8 API (namespace VietCommerce.Core.Models).

---

## 🎯 ENDPOINT: GET /api/v1/product

### Request Parameters (Query String)

```typescript
interface ProductFilterDto {
  pageNumber?: number;        // Trang hiện tại (bắt đầu từ 1, mặc định: 1) [CHANGED: page → pageNumber]
  pageSize?: number;          // Số items mỗi trang (mặc định: 12, max: 100)
  searchTerm?: string;        // Từ khóa tìm kiếm (tìm trong name, description)
  categoryId?: string;        // Lọc theo danh mục
  storeId?: string;           // Lọc theo cửa hàng
  isActive?: boolean;         // Chỉ lấy sản phẩm active (mặc định: true)
  minPrice?: number;          // Giá tối thiểu (VND)
  maxPrice?: number;          // Giá tối đa (VND)
  sortBy?: string;            // Trường sort: 'name' | 'price' | 'createdAt'
  isDescending?: boolean;     // true = DESC, false = ASC (mặc định: true)
}
```

### Request Examples

```bash
# Trang 1, 12 items
GET /api/v1/product?pageNumber=1&pageSize=12

# Tìm kiếm "hương", sắp xếp theo giá tăng dần
GET /api/v1/product?pageNumber=1&pageSize=12&searchTerm=hương&sortBy=price&isDescending=false

# Lọc theo category, giá từ 100k-500k
GET /api/v1/product?pageNumber=1&pageSize=24&categoryId=cat-1&minPrice=100000&maxPrice=500000

# Trang 2, chỉ sản phẩm active, sắp xếp mới nhất
GET /api/v1/product?pageNumber=2&pageSize=12&isActive=true&sortBy=createdAt&isDescending=true
```

---

## ✅ RESPONSE STRUCTURE

### 1. C# Model Definitions

```csharp
// File: DTOs/Common/PaginatedResult.cs
namespace VietCommerce.DTOs.Common
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}

// File: DTOs/Common/ApiResponse.cs
namespace VietCommerce.DTOs.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}

// File: DTOs/Product/ProductListDto.cs
namespace VietCommerce.DTOs.Product
{
    public class ProductListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ThumbnailUrl { get; set; }
        public string CategoryName { get; set; }
        public string StoreName { get; set; }
    }
}
```

### 2. Controller Implementation Example

```csharp
// File: Controllers/ProductController.cs
[ApiController]
[Route("api/v1/product")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ProductListDto>>>> GetProducts(
        [FromQuery] ProductFilterDto filter)
    {
        try
        {
            // Validate & set defaults
            filter.Page = filter.Page ?? 1;
            filter.PageSize = Math.Min(filter.PageSize ?? 12, 100);
            filter.IsActive = filter.IsActive ?? true;
            
            // Get paginated data from service
            var result = await _productService.GetProductsAsync(filter);
            
            return Ok(new ApiResponse<PaginatedResult<ProductListDto>>
            {
                Success = true,
                Data = result,
                Message = null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<PaginatedResult<ProductListDto>>
            {
                Success = false,
                Data = null,
                Message = $"Lỗi khi tải danh sách sản phẩm: {ex.Message}"
            });
        }
    }
}
```

### 3. Service Implementation Example

```csharp
// File: Services/ProductService.cs
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public async Task<PaginatedResult<ProductListDto>> GetProductsAsync(ProductFilterDto filter)
    {
        // Build query
        var query = _context.Products.AsQueryable();

        // Apply filters
        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchLower = filter.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchLower) || 
                p.Description.ToLower().Contains(searchLower)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.CategoryId))
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        if (!string.IsNullOrWhiteSpace(filter.StoreId))
            query = query.Where(p => p.StoreId == filter.StoreId);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.IsDescending == true 
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => filter.IsDescending == true 
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "createdat" => filter.IsDescending == true 
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt) // Default
        };

        // Get total count
        var totalCount = await query.CountAsync();

        // Calculate pagination
        var page = filter.Page ?? 1;
        var pageSize = filter.PageSize ?? 12;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var skip = (page - 1) * pageSize;

        // Get paginated items
        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .Select(p => new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                Stock = p.Stock,
                ThumbnailUrl = p.ThumbnailUrl,
                CategoryName = p.Category.Name,
                StoreName = p.Store.Name
            })
            .ToListAsync();

        // Return paginated result
        return new PaginatedResult<ProductListDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasPreviousPage = page > 1,
            HasNextPage = page < totalPages
        };
    }
}
```

---

## 📄 JSON RESPONSE EXAMPLE

### Success Response

```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440001",
        "name": "Hương Trầm Cao Cấp Huế",
        "slug": "huong-tram-cao-cap-hue",
        "price": 150000,
        "stock": 50,
        "thumbnailUrl": "https://storage.vietcommerce.com/products/huong-tram-001.jpg",
        "categoryName": "Hương & Nến",
        "storeName": "Cửa Hàng Đồ Cúng Truyền Thống"
      },
      {
        "id": "550e8400-e29b-41d4-a716-446655440002",
        "name": "Nến Thờ Đỏ 24H",
        "slug": "nen-tho-do-24h",
        "price": 85000,
        "stock": 120,
        "thumbnailUrl": "https://storage.vietcommerce.com/products/nen-tho-002.jpg",
        "categoryName": "Hương & Nến",
        "storeName": "Cửa Hàng Đồ Cúng Truyền Thống"
      }
    ],
    "totalCount": 95,
    "page": 1,
    "pageSize": 12,
    "totalPages": 8,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "message": null
}
```

### Error Response Example

```json
{
  "success": false,
  "data": null,
  "message": "Lỗi khi tải danh sách sản phẩm: Database connection failed"
}
```

---

## ⚠️ QUAN TRỌNG - CHECKLIST

### ✅ Pagination Logic
- [ ] `page` bắt đầu từ **1** (không phải 0)
- [ ] `pageSize` có giá trị mặc định là 12
- [ ] `pageSize` tối đa là 100 (validate để tránh query quá lớn)
- [ ] `totalPages = Math.Ceiling((double)totalCount / pageSize)`
- [ ] `hasPreviousPage = page > 1`
- [ ] `hasNextPage = page < totalPages`
- [ ] Skip/Take logic: `skip = (page - 1) * pageSize`

### ✅ Filtering & Sorting
- [ ] `searchTerm` tìm trong cả `name` và `description`
- [ ] `categoryId` lọc chính xác theo category
- [ ] `minPrice` và `maxPrice` filter theo range
- [ ] `isActive` mặc định là `true` (chỉ hiển thị sản phẩm active)
- [ ] `sortBy` hỗ trợ: `name`, `price`, `createdAt`
- [ ] `isDescending` cho phép đảo chiều sort

### ✅ JSON Serialization
- [ ] Field names phải là **camelCase** trong JSON response
- [ ] Cấu hình `JsonSerializerOptions` với `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`
- [ ] Hoặc sử dụng `[JsonPropertyName("camelCaseName")]` attribute

### ✅ Performance
- [ ] Sử dụng `AsNoTracking()` cho read-only queries
- [ ] Index database cho các trường thường xuyên filter/sort (Name, Price, CategoryId, CreatedAt)
- [ ] Limit maximum pageSize để tránh query quá lớn
- [ ] Cân nhắc caching cho categories và filters phổ biến

---

## 🔍 TESTING EXAMPLES

### Test Case 1: Basic Pagination
```bash
GET /api/v1/product?page=1&pageSize=12
```
**Expected:**
- 12 sản phẩm
- totalCount = tổng số sản phẩm trong DB
- hasPreviousPage = false
- hasNextPage = true (nếu có > 12 sản phẩm)

### Test Case 2: Search
```bash
GET /api/v1/product?searchTerm=hương&page=1&pageSize=10
```
**Expected:**
- Chỉ sản phẩm có "hương" trong name hoặc description
- Pagination đúng với số lượng kết quả tìm được

### Test Case 3: Sort by Price (Low to High)
```bash
GET /api/v1/product?sortBy=price&isDescending=false&page=1&pageSize=12
```
**Expected:**
- Sản phẩm sắp xếp từ giá thấp đến cao
- Item đầu tiên có price thấp nhất

### Test Case 4: Category Filter
```bash
GET /api/v1/product?categoryId=cat-1&page=1&pageSize=12
```
**Expected:**
- Chỉ sản phẩm thuộc category "cat-1"
- totalCount = số sản phẩm trong category đó

### Test Case 5: Price Range
```bash
GET /api/v1/product?minPrice=100000&maxPrice=500000&page=1&pageSize=12
```
**Expected:**
- Chỉ sản phẩm có giá từ 100k-500k
- Không có sản phẩm nào ngoài range

---

## 🚀 FRONTEND INTEGRATION

Frontend đã implement sẵn service call như sau:

```typescript
// File: /lib/services/vietCommerceProductService.ts
const response = await apiRequest.get<ApiResponse<PaginatedResult<ProductListDto>>>(
  `/product${queryString}`
);
return response.data; // Trả về PaginatedResult<ProductListDto>
```

Frontend đã xử lý:
- ✅ Build query string từ ProductFilterDto
- ✅ Parse PaginatedResult response
- ✅ Hiển thị pagination UI (số trang, next/prev buttons)
- ✅ Loading states và error handling
- ✅ Responsive design cho mobile/desktop

---

## 📞 SUPPORT

Nếu có thắc mắc hoặc cần clarification, vui lòng liên hệ Frontend Team.

**Endpoint Base URL:**
```
Development: https://hbh1z72d-7131.asse.devtunnels.ms/api/v1
Production: TBD
```

**Document Version:** 1.0
**Last Updated:** November 7, 2025
