# ProductDetailPage - API Parsing Update

## Tóm tắt
Đã cập nhật ProductDetailPage để parsing đúng các trường từ API response theo cấu trúc thực tế.

## API Response Structure

```json
{
  "id": "00000000-0000-0000-0000-000000001014",
  "name": "Đĩa ngũ quả thờ cúng gốm men lam",
  "code": "DQ01",
  "slug": "dia-ngu-qua-tho-cung-gom-men-lam",
  "price": 950000.00,
  "compareAtPrice": 950000.00,
  "displayPrice": {
    "originalPrice": 950000.00,
    "discountedPrice": 950000.00,
    "discountAmount": 0
  },
  "stockQuantity": 75,
  "inStock": true,
  "primaryImage": "https://...",
  "images": [
    {
      "id": "da441d1c-d591-45f3-afef-e732606a0e0d",
      "productId": "00000000-0000-0000-0000-000000001014",
      "url": "https://...",
      "thumbnailUrl": "https://...",
      "displayOrder": 0,
      "mediaType": 0,
      "isMain": true,
      "createdAt": "2025-12-08T12:05:01.55",
      "updatedAt": "2025-12-08T12:11:27.9866667"
    }
  ],
  "isActive": true,
  "isFeatured": false,
  "categoryId": "00000000-0000-0000-0000-000000000013",
  "categoryName": "Hoa, quả & đồ cúng lễ",
  "viewCount": 320,
  "favoriteCount": 54,
  "averageRating": 4.60,
  "createdAt": "2025-11-07T17:04:41.9166667",
  "type": ""
}
```

## Các trường được parsing

### Images Handling
- **API Response**: `images` là array of objects với `url` property
- **Parsing**: Tạo helper function `getImageUrls()` để extract URLs
- **Fallback**: Sử dụng `primaryImage` nếu `images` array rỗng

```typescript
const getImageUrls = (): string[] => {
  if (!product) return [];
  
  // If images is array of objects with url property
  if (Array.isArray(product.images) && product.images.length > 0) {
    if (typeof product.images[0] === 'object' && 'url' in product.images[0]) {
      return (product.images as any[]).map(img => img.url).filter(Boolean);
    }
    // If images is array of strings
    if (typeof product.images[0] === 'string') {
      return product.images as string[];
    }
  }
  
  // Fallback to primaryImage
  return product.primaryImage ? [product.primaryImage] : [];
};
```

### Price Display
- **Original Price**: `displayPrice.originalPrice`
- **Discounted Price**: `displayPrice.discountedPrice`
- **Discount Amount**: `displayPrice.discountAmount`
- **Display**: Hiển thị `discountedPrice` làm giá chính

```typescript
<span className="text-4xl text-red-600 font-bold">
  {formatPrice(product.displayPrice?.discountedPrice || product.price || 0)}
</span>
```

### Stock Status
- **In Stock**: `inStock` boolean property
- **Stock Quantity**: `stockQuantity` number
- **Display**: Sử dụng `inStock` để check, hiển thị `stockQuantity`

```typescript
<span className={(product as any).inStock ? 'text-green-600 font-semibold' : 'text-red-600 font-semibold'}>
  {(product as any).inStock ? `Còn ${product.stockQuantity || 0} sản phẩm` : 'Hết hàng'}
</span>
```

### Product Information
- **Code**: `code` (mã sản phẩm)
- **Slug**: `slug` (URL-friendly name)
- **Category**: `categoryName`
- **Brand**: `brandName`
- **View Count**: `viewCount`
- **Favorite Count**: `favoriteCount`
- **Rating**: `averageRating`
- **Review Count**: `reviewCount`

### Additional Fields
- **Primary Image**: `primaryImage` (fallback khi images rỗng)
- **Is Featured**: `isFeatured` (hiển thị badge "Nổi bật")
- **Is Active**: `isActive`
- **Created At**: `createdAt`
- **Type**: `type` (empty string trong response)

## Các file được cập nhật

### src/pages/ProductDetailPage.tsx

#### Thay đổi:
1. **Thêm helper function `getImageUrls()`**:
   - Extract URLs từ images array
   - Support cả array of objects và array of strings
   - Fallback đến primaryImage

2. **Cập nhật image handling**:
   - `const images = getImageUrls()`
   - `const currentImage = images[currentImageIndex] || product.primaryImage || ''`

3. **Cập nhật price display**:
   - Sử dụng `displayPrice.discountedPrice` làm giá chính
   - Sử dụng `displayPrice.originalPrice` làm giá gốc
   - Sử dụng `displayPrice.discountAmount` để tính tiết kiệm

4. **Cập nhật stock status**:
   - Sử dụng `inStock` property để check
   - Hiển thị `stockQuantity` nếu còn hàng

5. **Thêm slug display**:
   - Hiển thị slug trong Product Info card

## Features

✅ Parse images array of objects
✅ Extract image URLs correctly
✅ Display price từ displayPrice object
✅ Show discount amount
✅ Check stock status từ inStock property
✅ Display product code và slug
✅ Show view count và favorite count
✅ Support fallback đến primaryImage

## Notes

- API trả về `images` là array of objects, không phải array of strings
- Mỗi image object có `url` property chứa link ảnh
- `displayPrice` object chứa thông tin giá chi tiết
- `inStock` boolean property để check tình trạng hàng
- `primaryImage` được sử dụng làm fallback
- Tất cả các trường được parse đúng theo API response structure
