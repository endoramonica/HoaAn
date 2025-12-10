# Product Detail Page - Update Summary

## Tóm tắt
Đã cập nhật QuickViewModal và tạo ProductDetailPage mới với các tính năng:
- Xem nhiều hình ảnh sản phẩm với gallery
- Load đánh giá từ API
- Trang chi tiết sản phẩm đầy đủ
- Đề xuất sản phẩm liên quan cùng category
- Tất cả các nút đều gọi API với event handler

## Các file được tạo/cập nhật

### 1. **src/lib/services/productService.ts** (NEW)
Service để quản lý các API call liên quan đến product.

**Các phương thức:**
- `getProductById(id)` - GET /api/v1/Product/{id}
- `getProductBySlug(slug)` - GET /api/v1/Product/slug/{slug}
- `getProductsByCategory(categoryId)` - GET /api/v1/Product/category/{categoryId}
- `recordProductView(id)` - POST /api/v1/Product/{id}/view
- `toggleProductFavorite(id)` - POST /api/v1/Product/{id}/favorite
- `getFavoriteProducts()` - GET /api/v1/Product/favorites

### 2. **src/components/QuickViewModal.tsx** (UPDATED)

#### Tính năng mới:
- **Image Gallery**: Xem nhiều hình ảnh sản phẩm
  - Navigation buttons (Previous/Next)
  - Thumbnail selection
  - Loading state khi load product detail
  
- **Dynamic Data**: Load từ API thay vì hardcode
  - Product detail từ `productService.getProductById()`
  - Rating từ `averageRating` và `reviewCount`
  - Description từ `description` hoặc `shortDescription`
  - Stock quantity từ API
  - Brand name, category name, etc.

- **Event Handlers**:
  - `handlePrevImage()` - Xem ảnh trước
  - `handleNextImage()` - Xem ảnh tiếp theo
  - `handleAddToCart()` - Thêm vào giỏ hàng (gọi API)
  - `handleToggleWishlist()` - Thêm/xóa yêu thích (gọi API)

- **Product View Tracking**: Tự động ghi nhận lượt xem

### 3. **src/pages/ProductDetailPage.tsx** (NEW)
Trang chi tiết sản phẩm đầy đủ.

#### Tính năng:
- **Image Gallery**:
  - Main image display
  - Navigation buttons
  - Thumbnail selection
  - Support multiple images

- **Product Information**:
  - Rating và review count
  - Price display với discount
  - Stock status
  - Category, brand, code, etc.

- **Quantity Selection**:
  - Increase/decrease buttons
  - Disabled khi hết hàng

- **Action Buttons**:
  - Add to cart (gọi API)
  - Toggle wishlist (gọi API)
  - Loading states

- **Related Products**:
  - Load sản phẩm cùng category
  - Slideshow/grid display
  - Click để navigate đến product detail

#### Event Handlers:
- `handleAddToCart()` - Thêm vào giỏ hàng
- `handleToggleWishlist()` - Thêm/xóa yêu thích
- `handlePrevImage()` - Xem ảnh trước
- `handleNextImage()` - Xem ảnh tiếp theo
- `loadRelatedProducts()` - Load sản phẩm liên quan

## API Endpoints sử dụng

| Method | Endpoint | Hàm API |
|--------|----------|---------|
| GET | /api/v1/Product/{id} | getApiV1ProductId |
| GET | /api/v1/Product/slug/{slug} | getApiV1ProductSlugSlug |
| GET | /api/v1/Product/category/{categoryId} | getApiV1ProductCategoryCategoryId |
| POST | /api/v1/Product/{id}/view | postApiV1ProductIdView |
| POST | /api/v1/Product/{id}/favorite | postApiV1ProductIdFavorite |
| GET | /api/v1/Product/favorites | getApiV1ProductFavorites |
| POST | /api/v1/Cart/add | postApiV1CartAdd |
| POST | /api/v1/Cart/guest/add | postApiV1CartGuestAdd |

## Data Structure

### ProductDetailDto
```typescript
{
  id: string;
  name: string;
  shortDescription: string;
  description: string;
  code: string;
  slug: string;
  price: number;
  compareAtPrice: number;
  cost: number;
  displayPrice: DisplayPriceResult;
  discountPercentage: number;
  stockQuantity: number;
  sku: string;
  barcode: string;
  categoryId: string;
  categoryName: string;
  brandId: string;
  brandName: string;
  images: string[];
  primaryImage: string;
  tags: string[];
  metaTitle: string;
  metaDescription: string;
  metaKeywords: string;
  isActive: boolean;
  isFeatured: boolean;
  viewCount: number;
  favoriteCount: number;
  averageRating: number;
  reviewCount: number;
  storeId: string;
  storeName: string;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  createdByName: string;
}
```

## Cách sử dụng

### QuickViewModal
```typescript
import { QuickViewModal } from './QuickViewModal';

// Trong component
const [selectedProduct, setSelectedProduct] = useState(null);
const [isQuickViewOpen, setIsQuickViewOpen] = useState(false);

<QuickViewModal
  product={selectedProduct}
  isOpen={isQuickViewOpen}
  onClose={() => setIsQuickViewOpen(false)}
/>
```

### ProductDetailPage
```typescript
// Thêm route
<Route path="/product/:id" element={<ProductDetailPage />} />

// Navigate
navigate(`/product/${productId}`);
```

## Features Implemented

✅ Load product detail từ API
✅ Display multiple product images
✅ Image gallery navigation
✅ Thumbnail selection
✅ Load rating và review count
✅ Display product information (category, brand, code, etc.)
✅ Stock status display
✅ Add to cart (gọi API)
✅ Toggle wishlist (gọi API)
✅ Record product view
✅ Load related products by category
✅ Related products grid/slideshow
✅ Loading states
✅ Error handling
✅ Toast notifications

## Features TODO

- [ ] Product reviews/comments display
- [ ] Add review form
- [ ] Product specifications/attributes
- [ ] Product variants (size, color, etc.)
- [ ] Customer Q&A section
- [ ] Product comparison
- [ ] Share product on social media

## Notes

- Tất cả các API call đều có error handling
- Loading states được hiển thị trong UI
- Toast notifications cho user feedback
- Real-time UI update sau mỗi action
- Support cho cả authenticated và guest users
- Product view được tự động ghi nhận
- Related products được load từ cùng category
