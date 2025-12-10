# ProductsPage Navigation Update

## Tóm tắt
Đã cập nhật ProductsPage để navigate đến ProductDetailPage khi click vào product card.

## Các file được cập nhật

### 1. **src/components/ProductsPage.tsx** (UPDATED)

#### Thay đổi:
- Thêm import `useNavigate` từ `react-router-dom`
- Thêm `const navigate = useNavigate()` trong component
- Thêm handler function `handleViewProductDetail(productId)`
- Thêm `onClick` handler cho product card để navigate

#### Code thay đổi:
```typescript
// Import
import { useNavigate } from 'react-router-dom';

// Component
const navigate = useNavigate();

// Handler
const handleViewProductDetail = (productId: string) => {
  navigate(`/product/${productId}`);
};

// Product Card
<Card 
  onClick={() => handleViewProductDetail(productId)}
  className="cursor-pointer"
>
  {/* ... */}
</Card>
```

### 2. **src/App.tsx** (UPDATED)

#### Thay đổi:
- Thêm import `ProductDetailPage` từ `./pages/ProductDetailPage`
- Thêm route `/product/:id` trong MainLayout

#### Code thay đổi:
```typescript
// Import
import { ProductDetailPage } from "./pages/ProductDetailPage";

// Route
<Route path="/product/:id" element={<ProductDetailPage />} />
```

## Navigation Flow

```
ProductsPage
    ↓
    (click product card)
    ↓
handleViewProductDetail(productId)
    ↓
navigate(`/product/${productId}`)
    ↓
ProductDetailPage
    ↓
    (load product detail từ API)
    ↓
    (display product info, images, related products)
```

## Features

✅ Click product card để navigate đến detail page
✅ Product ID được truyền qua URL parameter
✅ ProductDetailPage load product detail từ API
✅ Back button để quay lại ProductsPage
✅ Related products có thể click để navigate

## Route Structure

```
/products                    → ProductsPage (danh sách sản phẩm)
/product/:id                 → ProductDetailPage (chi tiết sản phẩm)
```

## Notes

- Product card có `cursor-pointer` class để indicate clickable
- Navigation sử dụng React Router `useNavigate` hook
- Product ID được lấy từ product object
- ProductDetailPage tự động load product detail từ API
- Back button trong ProductDetailPage sử dụng `navigate(-1)`
