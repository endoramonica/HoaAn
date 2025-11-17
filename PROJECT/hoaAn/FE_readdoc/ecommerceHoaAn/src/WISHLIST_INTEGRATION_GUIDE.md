# 🎯 Wishlist Integration Guide

## Tổng quan

Hệ thống **Wishlist (Danh sách yêu thích)** đã được tích hợp hoàn toàn vào ứng dụng với kiến trúc 3 lớp tương thích với backend ASP.NET Core (.NET 8).

---

## 📁 Cấu trúc File

```
├── lib/
│   ├── api/
│   │   └── types.ts              # WishlistItemDto, AddToWishlistRequest types
│   ├── services/
│   │   └── wishlistService.ts    # Service layer - gọi API wishlist
│   └── hooks/
│       └── useWishlist.ts        # Custom hook - state management
├── components/
│   ├── WishlistPage.tsx          # Trang hiển thị wishlist
│   ├── ProductsPage.tsx          # Đã tích hợp nút wishlist
│   └── Header.tsx                # Đã thêm icon wishlist
└── App.tsx                       # Đã thêm route /wishlist
```

---

## 🔌 Backend API Endpoints

### 1. GET /api/v1/wishlist
Lấy danh sách wishlist của user hiện tại

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "wishlist-item-id",
      "userId": "user-id",
      "productId": "product-id",
      "product": {
        "id": "product-id",
        "name": "Hương trầm cao cấp",
        "price": 150000,
        "thumbnailUrl": "https://...",
        ...
      },
      "createdAt": "2025-10-16T10:30:00Z"
    }
  ]
}
```

### 2. POST /api/v1/wishlist
Thêm sản phẩm vào wishlist

**Request Body:**
```json
{
  "productId": "product-id"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "wishlist-item-id",
    "userId": "user-id",
    "productId": "product-id",
    "product": {...},
    "createdAt": "2025-10-16T10:30:00Z"
  }
}
```

### 3. DELETE /api/v1/wishlist/{id}
Xóa wishlist item theo ID

**Response:**
```json
{
  "success": true,
  "message": "Đã xóa khỏi danh sách yêu thích"
}
```

### 4. DELETE /api/v1/wishlist/product/{productId}
Xóa sản phẩm khỏi wishlist theo productId

### 5. GET /api/v1/wishlist/check/{productId}
Kiểm tra sản phẩm có trong wishlist không

**Response:**
```json
{
  "success": true,
  "data": {
    "isInWishlist": true
  }
}
```

### 6. POST /api/v1/wishlist/toggle
Toggle wishlist - thêm nếu chưa có, xóa nếu đã có

**Request Body:**
```json
{
  "productId": "product-id"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isInWishlist": true
  }
}
```

### 7. DELETE /api/v1/wishlist/clear
Xóa toàn bộ wishlist

### 8. POST /api/v1/wishlist/move-to-cart
Di chuyển tất cả wishlist items vào giỏ hàng

---

## 💻 Frontend Usage

### 1. Sử dụng Hook `useWishlist`

```tsx
import { useWishlist } from '../lib/hooks/useWishlist';

function MyComponent() {
  const {
    wishlistItems,      // Danh sách items trong wishlist
    loading,            // Loading state
    totalItems,         // Tổng số items
    isEmpty,            // Kiểm tra wishlist trống
    
    // Actions
    addToWishlist,      // Thêm vào wishlist
    removeFromWishlist, // Xóa khỏi wishlist
    toggleWishlist,     // Toggle wishlist
    isInWishlist,       // Check item có trong wishlist
    clearWishlist,      // Xóa toàn bộ
    moveAllToCart,      // Chuyển tất cả vào giỏ hàng
    refresh,            // Làm mới dữ liệu
  } = useWishlist();

  return (
    <div>
      <p>Tổng: {totalItems} sản phẩm</p>
      {wishlistItems.map(item => (
        <div key={item.id}>
          {item.product.name}
          <button onClick={() => removeFromWishlist(item.id)}>
            Xóa
          </button>
        </div>
      ))}
    </div>
  );
}
```

### 2. Thêm sản phẩm vào Wishlist

```tsx
const { addToWishlist, isInWishlist } = useWishlist();

// Thêm sản phẩm
const handleAdd = async () => {
  try {
    await addToWishlist('product-id', 'Tên sản phẩm');
    // Toast success tự động hiển thị
  } catch (error) {
    // Toast error tự động hiển thị
  }
};

// Kiểm tra sản phẩm có trong wishlist
const inWishlist = isInWishlist('product-id');
```

### 3. Toggle Wishlist Button

```tsx
const { toggleWishlist, isInWishlist } = useWishlist();

<button 
  onClick={() => toggleWishlist(product.id, product.name)}
  className={isInWishlist(product.id) ? 'active' : ''}
>
  <Heart className={isInWishlist(product.id) ? 'fill-current' : ''} />
</button>
```

### 4. Xóa khỏi Wishlist

```tsx
const { removeFromWishlist, removeByProductId } = useWishlist();

// Xóa theo wishlist item ID
await removeFromWishlist('wishlist-item-id');

// Xóa theo product ID
await removeByProductId('product-id');
```

### 5. Hook đơn giản chỉ check status

```tsx
import { useWishlistStatus } from '../lib/hooks/useWishlist';

function ProductCard({ productId }: { productId: string }) {
  const { isInWishlist, loading } = useWishlistStatus(productId);
  
  return (
    <div>
      {isInWishlist ? '❤️ Đã thích' : '🤍 Thích'}
    </div>
  );
}
```

---

## 🎨 UI Components

### WishlistPage Component

Trang hiển thị đầy đủ wishlist với:
- ✅ Grid view responsive
- ✅ Remove individual items
- ✅ Clear all wishlist
- ✅ Move all to cart
- ✅ Empty state
- ✅ Loading state
- ✅ Error handling
- ✅ Confirmation dialogs

**Usage:**
```tsx
import WishlistPage from './components/WishlistPage';

// Trong App.tsx routing
case 'wishlist':
  return <WishlistPage />;
```

### Header Integration

Header đã được cập nhật với:
- Icon wishlist (Heart icon)
- Badge hiển thị số lượng items
- Navigation đến trang wishlist

---

## 🔄 Mock Data Mode

Khi backend chưa sẵn sàng, service tự động chuyển sang mock mode:

```typescript
// Trong .env
VITE_USE_MOCK_DATA=true
```

Mock mode cung cấp:
- ✅ Giả lập API delay
- ✅ Local state management
- ✅ Validation giống backend
- ✅ Error handling

---

## 🎯 Backend Implementation (C#)

### WishlistController.cs

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;
    
    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WishlistItemDto>>>> GetWishlist()
    {
        var userId = User.GetUserId();
        var items = await _wishlistService.GetWishlistAsync(userId);
        return Ok(new ApiResponse<List<WishlistItemDto>>
        {
            Success = true,
            Data = items
        });
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<WishlistItemDto>>> AddToWishlist(
        [FromBody] AddToWishlistRequest request)
    {
        var userId = User.GetUserId();
        var item = await _wishlistService.AddToWishlistAsync(userId, request.ProductId);
        return Ok(new ApiResponse<WishlistItemDto>
        {
            Success = true,
            Data = item
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveFromWishlist(string id)
    {
        var userId = User.GetUserId();
        await _wishlistService.RemoveFromWishlistAsync(userId, id);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Đã xóa khỏi danh sách yêu thích"
        });
    }
    
    [HttpDelete("product/{productId}")]
    public async Task<ActionResult<ApiResponse<object>>> RemoveByProductId(string productId)
    {
        var userId = User.GetUserId();
        await _wishlistService.RemoveByProductIdAsync(userId, productId);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Đã xóa khỏi danh sách yêu thích"
        });
    }
    
    [HttpGet("check/{productId}")]
    public async Task<ActionResult<ApiResponse<IsInWishlistResponse>>> CheckIsInWishlist(
        string productId)
    {
        var userId = User.GetUserId();
        var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
        return Ok(new ApiResponse<IsInWishlistResponse>
        {
            Success = true,
            Data = new IsInWishlistResponse { IsInWishlist = isInWishlist }
        });
    }
    
    [HttpPost("toggle")]
    public async Task<ActionResult<ApiResponse<ToggleWishlistResponse>>> ToggleWishlist(
        [FromBody] AddToWishlistRequest request)
    {
        var userId = User.GetUserId();
        var isInWishlist = await _wishlistService.ToggleWishlistAsync(userId, request.ProductId);
        return Ok(new ApiResponse<ToggleWishlistResponse>
        {
            Success = true,
            Data = new ToggleWishlistResponse { IsInWishlist = isInWishlist }
        });
    }
    
    [HttpDelete("clear")]
    public async Task<ActionResult<ApiResponse<object>>> ClearWishlist()
    {
        var userId = User.GetUserId();
        await _wishlistService.ClearWishlistAsync(userId);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Đã xóa toàn bộ danh sách yêu thích"
        });
    }
    
    [HttpPost("move-to-cart")]
    public async Task<ActionResult<ApiResponse<object>>> MoveAllToCart()
    {
        var userId = User.GetUserId();
        await _wishlistService.MoveAllToCartAsync(userId);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Đã di chuyển tất cả vào giỏ hàng"
        });
    }
}
```

### WishlistItem Entity

```csharp
public class WishlistItem : BaseEntity
{
    public string UserId { get; set; }
    public User User { get; set; }
    
    public string ProductId { get; set; }
    public Product Product { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
```

### IWishlistService Interface

```csharp
public interface IWishlistService
{
    Task<List<WishlistItemDto>> GetWishlistAsync(string userId);
    Task<WishlistItemDto> AddToWishlistAsync(string userId, string productId);
    Task RemoveFromWishlistAsync(string userId, string wishlistItemId);
    Task RemoveByProductIdAsync(string userId, string productId);
    Task<bool> IsInWishlistAsync(string userId, string productId);
    Task<bool> ToggleWishlistAsync(string userId, string productId);
    Task ClearWishlistAsync(string userId);
    Task MoveAllToCartAsync(string userId);
}
```

---

## 📊 State Management Flow

```
User Action (Click Heart)
    ↓
useWishlist Hook
    ↓
wishlistService.ts (API Call)
    ↓
apiClient.ts (Axios + JWT)
    ↓
Backend ASP.NET Core API
    ↓
Database (SQL Server / PostgreSQL)
    ↓
Response → Update Local State
    ↓
UI Re-render + Toast Notification
```

---

## ✅ Features

### ✨ Frontend Features
- ✅ Add to wishlist
- ✅ Remove from wishlist  
- ✅ Toggle wishlist (add/remove với 1 click)
- ✅ Check if product in wishlist
- ✅ View wishlist page
- ✅ Clear all wishlist
- ✅ Move all to cart
- ✅ Real-time UI updates
- ✅ Toast notifications
- ✅ Loading states
- ✅ Error handling
- ✅ Mock data mode
- ✅ Responsive design
- ✅ Empty state
- ✅ Confirmation dialogs

### 🎨 UI/UX Features
- ✅ Heart icon với fill animation
- ✅ Badge count trong header
- ✅ Grid view responsive
- ✅ Hover effects
- ✅ Smooth transitions
- ✅ Vietnamese translations
- ✅ Cultural design (màu nâu ấm, vàng, đỏ)

---

## 🧪 Testing

### Test Add to Wishlist
```typescript
// Mock mode - test trong browser console
const { addToWishlist } = useWishlist();
await addToWishlist('product-123', 'Hương trầm cao cấp');
```

### Test Toggle Wishlist
```typescript
const { toggleWishlist, isInWishlist } = useWishlist();
await toggleWishlist('product-123');
console.log(isInWishlist('product-123')); // true hoặc false
```

---

## 🚀 Next Steps

1. **Connect to Real Backend**: Tắt mock mode khi backend ready
2. **Add Analytics**: Track wishlist events
3. **Add Sharing**: Share wishlist với bạn bè
4. **Email Notifications**: Thông báo giảm giá cho wishlist items
5. **Wishlist Sync**: Đồng bộ giữa devices

---

## 📝 Notes

- Wishlist yêu cầu user đăng nhập (JWT required)
- Mock mode tự động bật khi `VITE_USE_MOCK_DATA=true`
- Toast notifications tự động với mọi action
- Icon wishlist trong header tự động update count
- Tích hợp hoàn toàn với existing ProductsPage

---

## 🤝 Support

Nếu có vấn đề, check:
1. JWT token có hợp lệ không
2. API endpoint có đúng không
3. Mock mode có đang bật không
4. Console có error gì không

---

**Happy coding! 💖**
