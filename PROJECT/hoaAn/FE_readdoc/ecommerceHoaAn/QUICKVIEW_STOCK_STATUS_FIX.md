# QuickViewModal & Stock Status Fix - Hoàn Thành ✅

## 🔍 Vấn Đề Phát Hiện

### 1. QuickViewModal - Images Mapping
**Vấn đề:** Code đang xử lý `images` như `string[]`, nhưng thực tế là `ProductImageDto[]`

```typescript
// ❌ Cũ - Sai
const images = productDetail?.images || [product?.image];
// images[0] là object, không phải string

// ✅ Mới - Đúng
const getImageUrls = (): string[] => {
  if (productDetail?.images && Array.isArray(productDetail.images)) {
    return productDetail.images
      .map((img: any) => img.url)
      .filter(Boolean) as string[];
  }
  return product?.image ? [product.image] : [];
};
```

### 2. Stock Status Check
**Vấn đề:** Kiểm tra `stockQuantity` không chính xác

```typescript
// ❌ Cũ - Sai (0 là falsy)
{productDetail?.stockQuantity ? `Còn ${productDetail.stockQuantity}` : 'Hết hàng'}

// ✅ Mới - Đúng
{productDetail?.stockQuantity && productDetail.stockQuantity > 0 ? `Còn ${productDetail.stockQuantity}` : 'Hết hàng'}
```

### 3. Button Disabled State
**Vấn đề:** Button "Thêm vào giỏ" và "+" không disable khi hết hàng

```typescript
// ❌ Cũ
disabled={isAddingToCart}

// ✅ Mới
disabled={isAddingToCart || !productDetail?.stockQuantity || productDetail.stockQuantity <= 0}
```

## ✅ Fixes Applied

### File: src/components/QuickViewModal.tsx

1. **Thêm `getImageUrls()` function**
   - Extract URLs từ `ProductImageDto[]`
   - Fallback to `product.image` nếu không có detail

2. **Cập nhật `handlePrevImage()` & `handleNextImage()`**
   - Sử dụng `getImageUrls()` thay vì trực tiếp access `images`

3. **Cập nhật Main Image Display**
   - Sử dụng `getImageUrls()` để lấy current image

4. **Cập nhật Thumbnails**
   - Map qua `getImageUrls()` thay vì `productDetail?.images`

5. **Sửa Stock Status Check**
   - Kiểm tra `stockQuantity > 0` thay vì chỉ `stockQuantity`

6. **Disable Buttons khi Hết Hàng**
   - "Thêm vào giỏ" button
   - "+" button (increase quantity)

### File: src/pages/ProductDetailPage.tsx

1. **Sửa Related Products Image**
   - Từ: `relatedProduct.images?.[0]`
   - Thành: `relatedProduct.images?.[0]?.url`

## 📊 Mapping Reference

### ProductImageDto Structure
```typescript
interface ProductImageDto {
  id: string;
  productId: string;
  url: string;              // ← URL để display
  thumbnailUrl: string;     // ← Thumbnail URL
  displayOrder: number;
  mediaType: number;
  isMain: boolean;
  createdAt: string;
  updatedAt: string | null;
}
```

### Usage Pattern
```typescript
// ✅ Đúng
const imageUrl = productImageDto.url;
const thumbUrl = productImageDto.thumbnailUrl;

// ❌ Sai
const imageUrl = productImageDto; // Là object, không phải string
```

## 🧪 Testing Checklist

- [ ] Mở QuickViewModal
- [ ] Xác nhận images hiển thị đúng
- [ ] Xác nhận thumbnails hoạt động
- [ ] Xác nhận prev/next image hoạt động
- [ ] Kiểm tra product có stock:
  - [ ] "Còn X sản phẩm" hiển thị (green)
  - [ ] Button "Thêm vào giỏ" enabled
  - [ ] Button "+" enabled
- [ ] Kiểm tra product hết hàng:
  - [ ] "Hết hàng" hiển thị (red)
  - [ ] Button "Thêm vào giỏ" disabled
  - [ ] Button "+" disabled
- [ ] Kiểm tra ProductDetailPage related products

## 📁 Files Changed

1. **src/components/QuickViewModal.tsx**
   - Thêm `getImageUrls()` function
   - Cập nhật image handling
   - Sửa stock status check
   - Disable buttons khi hết hàng

2. **src/pages/ProductDetailPage.tsx**
   - Sửa related product image access

## 🎯 Status: COMPLETE ✅

Tất cả issues liên quan đến images mapping và stock status đã được fix.
