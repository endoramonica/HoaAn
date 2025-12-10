# Fix: Product Price Inconsistency Between List and Detail Endpoints

## Vấn đề
- **GET /api/v1/Product** (danh sách): Trả về `price: 15000000` ✅
- **GET /api/v1/Product/{id}** (chi tiết): Trả về `price: 0` ❌

Ví dụ: Sản phẩm "Gói Dịch Vụ Lễ Cưới Hỏi Trọn Vẹn" (ID: 18503bfd-9434-42e1-875e-d8f362c79f59)

## Root Cause Analysis

### 1. GetByIdAsync() không filter Prices
```csharp
// ❌ BEFORE: Load toàn bộ Prices (có thể rỗng hoặc không active)
.Include(p => p.Prices)

// ✅ AFTER: Chỉ load Prices active và effective
.Include(p => p.Prices.Where(pr => pr.IsActive && pr.EffectiveFrom <= now))
```

### 2. PriceCalculationHelper.GetCurrentPrice() logic
Hàm này lấy giá từ collection `product.Prices`:
```csharp
var currentPrice = product.Prices?
    .Where(p => p.PriceType == priceType &&
                p.IsActive &&
                p.EffectiveFrom <= currentTime &&
                (p.EffectiveTo == null || p.EffectiveTo > currentTime))
    .OrderByDescending(p => p.EffectiveFrom)
    .FirstOrDefault();

return currentPrice?.Price ?? 0;  // ← Trả về 0 nếu không tìm thấy
```

### 3. Mapping sử dụng PriceCalculationHelper
```csharp
.ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
    PriceCalculationHelper.GetDisplayPrice(src).DiscountedPrice))
```

**Vấn đề:** Nếu `product.Prices` rỗng hoặc không có giá REGULAR active, hàm trả về 0.

## Giải pháp

### Fix 1: GetByIdAsync() - Filter Prices khi load
```csharp
public override async Task<Product?> GetByIdAsync(Guid id)
{
    var now = DateTime.UtcNow;
    return await _context.Products
        .Include(p => p.Prices.Where(pr => pr.IsActive && pr.EffectiveFrom <= now))
        .Include(p => p.Images)
        .Include(p => p.Category)
        .Include(p => p.Store)
        .Include(p => p.PromotionProducts)
            .ThenInclude(pp => pp.Promotion)
        .FirstOrDefaultAsync(p => p.Id == id);
}
```

### Fix 2: Đảm bảo ProductPrice được tạo đúng
Kiểm tra xem sản phẩm có ProductPrice record không:
- `PriceType = REGULAR`
- `IsActive = true`
- `EffectiveFrom <= DateTime.UtcNow`
- `EffectiveTo = null` hoặc `> DateTime.UtcNow`

## Kiểm tra Database
```sql
SELECT * FROM ProductPrices 
WHERE ProductId = '18503bfd-9434-42e1-875e-d8f362c79f59'
AND IsActive = 1
AND PriceType = 0  -- REGULAR
AND EffectiveFrom <= GETUTCDATE()
```

## Files thay đổi
- ✅ `VietCommerce.Data/Repositories/ProductRepository.cs`
  - `GetByIdAsync()` - Filter Prices khi load
  - `GetByCodeAsync()` - Filter Prices khi load
  - `GetByStoreIdAsync()` - Filter Prices khi load
  - `GetByCategoryIdAsync()` - Filter Prices khi load
  - `GetPromotedProductsAsync()` - Filter Prices khi load
  - `GetRelatedProductsAsync()` - Filter Prices khi load
  - `GetRecentlyViewedAsync()` - Filter Prices khi load
  - `GetFavoriteProductsAsync()` - Filter Prices khi load
  - `GetTrendingProductsAsync()` - Filter Prices khi load

- ✅ `VietCommerce.Core/Helpers/PriceCalculationHelper.cs` - Comment thêm

## Kết quả dự kiến
Sau khi sửa, cả hai endpoint sẽ trả về giá nhất quán:
- **GET /api/v1/Product** → price = 15000000 ✅
- **GET /api/v1/Product/{id}** → price = 15000000 ✅

## Cách kiểm tra
1. Rebuild solution
2. Test GET /api/v1/Product (danh sách)
3. Test GET /api/v1/Product/{id} (chi tiết)
4. Kiểm tra giá trả về có nhất quán không
