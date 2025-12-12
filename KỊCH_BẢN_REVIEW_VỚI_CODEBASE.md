# 📋 REVIEW KỊCH BẢN PACKAGE/CUSTOMIZABLE PRODUCTS VỚI CODEBASE HIỆN TẠI

## 🎯 TÓNG QUAN

Kịch bản về Package Products (Mâm cúng, Combo) với tùy chỉnh là **hoàn toàn khả thi** với codebase hiện tại, nhưng cần **thêm các trường mới** vào entities và DTOs.

---

## ✅ ĐIỂM MẠNH CỦA CODEBASE HIỆN TẠI

### 1. **Kiến trúc sạch & modular**
- Tách biệt rõ: Controllers → Services → Repositories → Entities
- AutoMapper + FluentValidation sẵn có
- Permission-based authorization (AdminOnly, AdminOrModerator)

### 2. **Entities linh hoạt**
- `Product` entity có `Type` field (product/service) - có thể mở rộng
- `OrderItem` đã có snapshot fields (ProductName, ProductCode, UnitPrice)
- Soft delete support (ISoftDelete interface)

### 3. **Cart & Order flow**
- `CartItem` → `OrderItem` flow rõ ràng
- Có `CreatedAt`, `UpdatedAt` tracking
- Support multiple items per order

### 4. **API Response structure**
- Consistent `ApiResponse<T>` wrapper
- Proper HTTP status codes (200, 403, 404)
- Validation error handling

---

## ⚠️ ĐIỂM CẦN THÊM/SỬA

### 1. **Product Entity - Thiếu fields cho Package**

**Hiện tại:**
```csharp
public class Product : AuditableEntity, ISoftDelete
{
    public string Name { get; set; }
    public string Code { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    // ... không có details, customizable options
}
```

**Cần thêm:**
```csharp
public class Product : AuditableEntity, ISoftDelete
{
    // ... existing fields ...
    
    // 🔧 NEW: Package/Combo fields
    /// <summary>
    /// Danh sách items bao gồm trong package (JSON)
    /// Example: ["Cá chép giấy (3 con)", "Mũ giấy (3 cái)", ...]
    /// </summary>
    public string? DetailsJson { get; set; }
    
    /// <summary>
    /// Các options có thể tùy chỉnh (JSON)
    /// Example: [{"id":"opt-xoi","name":"Xôi gấc","unitPrice":45000,...}]
    /// </summary>
    public string? CustomizableOptionsJson { get; set; }
    
    /// <summary>
    /// Giá gốc của package (trước khi tùy chỉnh)
    /// </summary>
    public decimal BasePrice { get; set; }
}
```

### 2. **CartItem Entity - Thiếu customization**

**Hiện tại:**
```csharp
public class CartItem : AuditableEntity, ISoftDelete
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    // ... không lưu customization
}
```

**Cần thêm:**
```csharp
public class CartItem : AuditableEntity, ISoftDelete
{
    // ... existing fields ...
    
    // 🔧 NEW: Customization fields
    /// <summary>
    /// Customizations JSON (nếu product có options)
    /// Example: [{"optionId":"opt-xoi","quantity":10,"unitPrice":45000}]
    /// </summary>
    public string? CustomizationsJson { get; set; }
    
    /// <summary>
    /// Giá gốc của product (trước customization)
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Tổng phụ thu từ customization
    /// </summary>
    public decimal CustomizationPrice { get; set; }
    
    /// <summary>
    /// Tổng giá cuối cùng (BasePrice + CustomizationPrice) * Quantity
    /// </summary>
    public decimal FinalPrice { get; set; }
}
```

### 3. **OrderItem Entity - Cần lưu customization**

**Hiện tại:**
```csharp
public class OrderItem : AuditableEntity, ISoftDelete
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
```

**Cần thêm:**
```csharp
public class OrderItem : AuditableEntity, ISoftDelete
{
    // ... existing fields ...
    
    // 🔧 NEW: Customization snapshot
    /// <summary>
    /// Customizations snapshot (lưu trữ để xử lý đơn hàng)
    /// </summary>
    public string? CustomizationsJson { get; set; }
    
    /// <summary>
    /// Giá gốc product (snapshot)
    /// </summary>
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Tổng phụ thu customization (snapshot)
    /// </summary>
    public decimal CustomizationPrice { get; set; }
}
```

---

## 🔧 THAY ĐỔI CẦN THIẾT

### 1. **DTOs mới cần tạo**

#### A. `CustomizableOptionDto.cs`
```csharp
public class CustomizableOptionDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int BaseQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public string Unit { get; set; }
}
```

#### B. `ProductCustomizationDto.cs`
```csharp
public class ProductCustomizationDto
{
    public List<string> Details { get; set; }
    public List<CustomizableOptionDto> CustomizableOptions { get; set; }
}
```

#### C. `CartItemCustomizationDto.cs`
```csharp
public class CartItemCustomizationDto
{
    public string OptionId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
```

#### D. Cập nhật `AddToCartDto.cs`
```csharp
public class AddToCartDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    
    // 🔧 NEW: Customizations
    public List<CartItemCustomizationDto>? Customizations { get; set; }
}
```

#### E. Cập nhật `CartItemDetailDto.cs`
```csharp
public class CartItemDetailDto
{
    // ... existing fields ...
    
    // 🔧 NEW
    public decimal BasePrice { get; set; }
    public decimal CustomizationPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public List<CartItemCustomizationDto>? Customizations { get; set; }
}
```

#### F. Cập nhật `ProductDetailDto.cs`
```csharp
public class ProductDetailDto
{
    // ... existing fields ...
    
    // 🔧 NEW
    public List<string>? Details { get; set; }
    public List<CustomizableOptionDto>? CustomizableOptions { get; set; }
}
```

### 2. **ProductCreateDto & ProductUpdateDto - Thêm fields**

```csharp
public class ProductCreateDto
{
    // ... existing fields ...
    
    // 🔧 NEW: Package fields
    public List<string>? Details { get; set; }
    public List<CustomizableOptionDto>? CustomizableOptions { get; set; }
}

public class ProductUpdateDto
{
    // ... existing fields ...
    
    // 🔧 NEW
    public List<string>? Details { get; set; }
    public List<CustomizableOptionDto>? CustomizableOptions { get; set; }
}
```

---

## 📊 KỊCH BẢN MAPPING VỚI CODEBASE

### **KB1: Sản phẩm đơn giản** ✅
- Không có `DetailsJson`, `CustomizableOptionsJson`
- Hoạt động bình thường với flow hiện tại
- **Không cần thay đổi**

### **KB2: Combo cố định** ✅
- Có `DetailsJson` (danh sách items)
- Không có `CustomizableOptionsJson`
- `ProductDetailDto.Details` hiển thị danh sách
- **Cần thêm:** `Details` field vào DTOs

### **KB3: Package có tùy chỉnh** ⚠️
- Có `DetailsJson` + `CustomizableOptionsJson`
- `AddToCartDto` cần `Customizations` field
- `CartItem` cần lưu `CustomizationsJson`
- **Cần thêm:** Validation logic, price calculation

### **KB4: Admin quản lý** ✅
- `ProductCreateDto` + `ProductUpdateDto` hỗ trợ
- Permission check sẵn có (product.create, product.update)
- **Cần thêm:** Validation cho JSON structure

### **KB5: Thay đổi options** ✅
- Update `CustomizableOptionsJson` trong Product
- Đơn hàng cũ vẫn giữ snapshot
- **Không cần thay đổi** (OrderItem đã có snapshot)

### **KB6: Xử lý đơn hàng** ✅
- `OrderItem` lưu snapshot (ProductName, UnitPrice)
- **Cần thêm:** `CustomizationsJson`, `CustomizationPrice` snapshot

### **KB7: Báo cáo thống kê** ⚠️
- Cần query `OrderItem.CustomizationsJson`
- Deserialize JSON để phân tích
- **Cần thêm:** Analytics service

---

## 🛠️ IMPLEMENTATION ROADMAP

### **Phase 1: Database & Entities** (1-2 ngày)
1. Thêm fields vào `Product` entity
2. Thêm fields vào `CartItem` entity
3. Thêm fields vào `OrderItem` entity
4. Tạo migration

### **Phase 2: DTOs & Validators** (1 ngày)
1. Tạo `CustomizableOptionDto`
2. Cập nhật `ProductCreateDto`, `ProductUpdateDto`
3. Cập nhật `AddToCartDto`
4. Cập nhật `CartItemDetailDto`, `ProductDetailDto`
5. Tạo FluentValidation rules

### **Phase 3: Services** (2-3 ngày)
1. Cập nhật `IProductService`
   - Validate customizable options
   - Serialize/deserialize JSON
2. Cập nhật `ICartService`
   - Validate customizations khi add to cart
   - Calculate final price
   - Lưu customizations vào CartItem
3. Cập nhật `IOrderService`
   - Snapshot customizations từ CartItem
   - Lưu vào OrderItem

### **Phase 4: Controllers** (1 ngày)
1. Cập nhật `AdminProductController`
   - Support create/update package products
2. Cập nhật Cart API
   - Support customizations

### **Phase 5: Testing & Validation** (1-2 ngày)
1. Unit tests cho validation logic
2. Integration tests cho cart flow
3. E2E tests cho order flow

---

## 💡 BEST PRACTICES ĐỀ XUẤT

### 1. **JSON Validation**
```csharp
private bool IsValidCustomizableOptions(string json)
{
    try
    {
        var options = JsonSerializer.Deserialize<List<CustomizableOptionDto>>(json);
        return options?.All(o => 
            !string.IsNullOrEmpty(o.Id) && 
            !string.IsNullOrEmpty(o.Name) &&
            o.UnitPrice > 0 &&
            o.MinQuantity > 0) ?? false;
    }
    catch
    {
        return false;
    }
}
```

### 2. **Price Calculation**
```csharp
public decimal CalculateFinalPrice(
    decimal basePrice, 
    List<CartItemCustomizationDto> customizations)
{
    var customizationTotal = customizations?
        .Sum(c => c.TotalPrice) ?? 0;
    return basePrice + customizationTotal;
}
```

### 3. **Concurrent Updates**
```csharp
[ConcurrencyCheck]
public byte[] RowVersion { get; set; }
```

### 4. **Audit Trail**
```csharp
// Lưu lịch sử thay đổi giá option
public class ProductOptionHistory
{
    public Guid ProductId { get; set; }
    public string OptionId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public DateTime ChangedAt { get; set; }
}
```

---

## 📝 SUMMARY

| Kịch bản | Trạng thái | Công việc cần làm |
|---------|-----------|------------------|
| KB1: Sản phẩm đơn giản | ✅ OK | Không cần |
| KB2: Combo cố định | ⚠️ Cần thêm | Thêm `Details` field |
| KB3: Package tùy chỉnh | ⚠️ Cần thêm | Thêm customization fields + logic |
| KB4: Admin quản lý | ✅ OK | Validation JSON |
| KB5: Thay đổi options | ✅ OK | Không cần |
| KB6: Xử lý đơn hàng | ⚠️ Cần thêm | Snapshot customizations |
| KB7: Báo cáo | ⚠️ Cần thêm | Analytics service |

**Tổng công việc:** ~5-7 ngày (1 sprint)

---

## 🎯 NEXT STEPS

1. **Approve** kịch bản này
2. **Tạo migration** cho database changes
3. **Implement** DTOs & validators
4. **Implement** service logic
5. **Test** toàn bộ flow
