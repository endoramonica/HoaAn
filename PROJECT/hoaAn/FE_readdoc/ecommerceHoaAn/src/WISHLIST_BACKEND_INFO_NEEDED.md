# 📋 Wishlist Backend Information Needed

## Response Models Required

Vui lòng cung cấp C# DTO models hoặc JSON response examples cho các endpoints sau:

### 1. GET /api/v1/Wishlist

**Response structure cần:**

```typescript
// TypeScript interface tương ứng
interface WishlistResponse {
  // Cần biết structure chính xác
  items?: WishlistItem[];
  totalCount?: number;
  // Hoặc format khác?
}

interface WishlistItem {
  id: number;                    // Wishlist item ID
  productId: number;             // Product ID
  productName?: string;
  productCode?: string;
  price?: number;
  displayPrice?: number;
  primaryImage?: string;
  inStock?: boolean;
  stockQuantity?: number;
  averageRating?: number;
  viewCount?: number;
  favoriteCount?: number;
  addedDate?: string;            // ISO date
  // Còn fields nào khác?
}
```

**Example JSON Response:**
```json
{
  "success": true,
  "data": {
    // Điền example response ở đây
  }
}
```

---

### 2. POST /api/v1/Wishlist

**Request body:**
```json
{
  // Điền request structure
  "productId": 123
  // Còn fields nào?
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    // Điền response structure
  },
  "message": "Đã thêm vào danh sách yêu thích"
}
```

---

### 3. DELETE /api/v1/Wishlist/{id}

**Response:**
```json
{
  "success": true,
  "message": "Đã xóa khỏi danh sách yêu thích"
}
```

---

### 4. DELETE /api/v1/Wishlist/product/{productId}

**Response:**
```json
{
  "success": true,
  "message": "Đã xóa sản phẩm khỏi danh sách yêu thích"
}
```

---

### 5. GET /api/v1/Wishlist/check/{productId}

**Response:**
```json
{
  "success": true,
  "data": {
    "isInWishlist": true,
    "wishlistItemId": 1
    // Còn fields nào?
  }
}
```

---

### 6. POST /api/v1/Wishlist/toggle

**Request:**
```json
{
  "productId": 123
  // Còn fields nào?
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "isInWishlist": true,      // true = added, false = removed
    "wishlistItemId": 1,       // or null if removed
    "action": "added"          // or "removed"
    // Còn fields nào?
  },
  "message": "Đã thêm vào danh sách yêu thích"
}
```

---

### 7. DELETE /api/v1/Wishlist/clear

**Response:**
```json
{
  "success": true,
  "message": "Đã xóa toàn bộ danh sách yêu thích"
}
```

---

### 8. POST /api/v1/Wishlist/move-to-cart

**Request:**
```json
{
  "wishlistItemIds": [1, 2, 3]
  // Hoặc
  "productIds": [123, 456, 789]
  // Còn fields nào?
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "movedCount": 3,
    "failedItems": []
    // Còn fields nào?
  },
  "message": "Đã chuyển 3 sản phẩm vào giỏ hàng"
}
```

---

### 9. POST /api/v1/Product/{id}/favorite

**Response:**
```json
{
  "success": true,
  "data": {
    // Điền response structure
  },
  "message": "Đã thêm vào yêu thích"
}
```

**Question:** Endpoint này có liên quan đến Wishlist không? Hay là feature riêng?

---

### 10. GET /api/v1/Product/favorites

**Response:**
```json
{
  "success": true,
  "data": {
    // Điền response structure
    // Format giống GET /api/v1/Wishlist?
  }
}
```

---

## Additional Questions

### 1. Pagination
GET /api/v1/Wishlist có support pagination không?

```
✅ Yes - Query params: ?pageNumber=1&pageSize=20
❌ No - Trả về toàn bộ
```

Nếu Yes, response format:
```json
{
  "success": true,
  "data": {
    "items": [],
    "pageNumber": 1,
    "pageSize": 20,
    "totalItems": 50,
    "totalPages": 3
  }
}
```

### 2. Authentication
Wishlist endpoints yêu cầu authentication?

```
✅ Yes - Bearer token required
❌ No - Public endpoints
```

### 3. Relationship
Sự khác biệt giữa:
- `/api/v1/Wishlist` endpoints
- `/api/v1/Product/{id}/favorite` + `/api/v1/Product/favorites`

```
✅ Same thing - Đồng bộ với nhau
❌ Different - Favorite là feature riêng, Wishlist là feature riêng
```

### 4. Error Responses

Khi có lỗi, response format:
```json
{
  "success": false,
  "error": {
    "code": "PRODUCT_NOT_FOUND",
    "message": "Không tìm thấy sản phẩm"
  }
}
```

Hoặc format khác?

---

## C# Models Reference

Nếu có thể, cung cấp C# DTO classes:

```csharp
// WishlistItemDto.cs
public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    // ... other properties
}

// WishlistDto.cs
public class WishlistDto
{
    public List<WishlistItemDto> Items { get; set; }
    public int TotalCount { get; set; }
}

// AddToWishlistRequest.cs
public class AddToWishlistRequest
{
    public int ProductId { get; set; }
}

// ... other models
```

---

## Next Steps

Sau khi có đầy đủ thông tin trên, tôi sẽ:

1. ✅ Update `/lib/services/wishlistService.ts` với đầy đủ API calls
2. ✅ Update `/lib/hooks/useWishlist.ts` với React Query integration
3. ✅ Update `/components/WishlistPage.tsx` với UI hoàn chỉnh
4. ✅ Integrate wishlist toggle vào:
   - ProductsPage
   - ProductsPageV2
   - HomePage
   - QuickViewModal
5. ✅ Update Header wishlist badge count
6. ✅ Create comprehensive documentation

---

**Vui lòng điền thông tin vào file này hoặc reply với:**
1. JSON response examples
2. C# DTO models
3. Trả lời các questions về pagination, authentication, relationship
