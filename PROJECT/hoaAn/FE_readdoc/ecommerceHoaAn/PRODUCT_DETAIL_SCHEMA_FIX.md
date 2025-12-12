# ProductDetailDto Schema Mismatch - Hướng Giải Quyết

## 🔍 Vấn Đề Phát Hiện

### OpenAPI Spec (swagger.json) định nghĩa:
```json
"images": {
  "type": "array",
  "items": {
    "type": "string"  // ❌ Mong đợi array of strings
  },
  "nullable": true
}
```

### Response thực tế từ BE:
```json
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
  // ❌ Thực tế là array of objects
]
```

## ✅ Hướng Giải Quyết

### Tùy Chọn 1: Cập Nhật OpenAPI Spec (Khuyến Nghị)
**Nếu BE có thể thay đổi:**

1. Tạo schema mới `ProductImageDto` trong swagger.json:
```json
"ProductImageDto": {
  "type": "object",
  "properties": {
    "id": { "type": "string", "format": "uuid" },
    "productId": { "type": "string", "format": "uuid" },
    "url": { "type": "string" },
    "thumbnailUrl": { "type": "string" },
    "displayOrder": { "type": "integer" },
    "mediaType": { "type": "integer" },
    "isMain": { "type": "boolean" },
    "createdAt": { "type": "string", "format": "date-time" },
    "updatedAt": { "type": "string", "format": "date-time" }
  }
}
```

2. Cập nhật `ProductDetailDto`:
```json
"images": {
  "type": "array",
  "items": {
    "$ref": "#/components/schemas/ProductImageDto"  // ✅ Thay đổi từ string
  },
  "nullable": true
}
```

3. Regenerate Orval:
```bash
npm run generate:api
```

### Tùy Chọn 2: Transform Response trong FE (Workaround)
**Nếu BE không thể thay đổi:**

Tạo adapter layer trong `productService.ts`:

```typescript
// src/lib/services/productService.ts

interface ProductImageDto {
  id: string;
  productId: string;
  url: string;
  thumbnailUrl: string;
  displayOrder: number;
  mediaType: number;
  isMain: boolean;
  createdAt: string;
  updatedAt: string;
}

interface ProductDetailDtoResponse {
  // ... other fields
  images?: ProductImageDto[] | string[];
}

export const productService = {
  async getProductById(id: string) {
    const response = await api.getApiV1ProductsId(id);
    
    if (!response.data) {
      throw new Error('Product not found');
    }

    // Transform response để match với schema
    const product = response.data as any;
    
    return {
      ...product,
      images: transformImages(product.images),
      inStock: product.stockQuantity > 0 // Thêm field này nếu cần
    };
  }
};

function transformImages(images: any): string[] {
  if (!Array.isArray(images)) {
    return [];
  }

  return images.map(img => {
    if (typeof img === 'string') {
      return img;
    }
    if (typeof img === 'object' && img.url) {
      return img.url;
    }
    return '';
  }).filter(Boolean);
}
```

### Tùy Chọn 3: Cập Nhật Type Definition (Hybrid)
**Nếu muốn support cả 2 format:**

```typescript
// src/lib/api/types.ts

export interface ProductImageDto {
  id: string;
  productId: string;
  url: string;
  thumbnailUrl: string;
  displayOrder: number;
  mediaType: number;
  isMain: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ProductDetailDto {
  // ... existing fields
  images?: (string | ProductImageDto)[] | null;
  inStock?: boolean; // Thêm field này
}
```

Cập nhật `ProductDetailPage.tsx`:

```typescript
const getImageUrls = (): string[] => {
  if (!product?.images) return [];
  
  return product.images
    .map(img => {
      if (typeof img === 'string') return img;
      if (typeof img === 'object' && 'url' in img) return img.url;
      return '';
    })
    .filter(Boolean);
};
```

## 📋 Checklist Thực Hiện

- [ ] **Tùy Chọn 1 (Khuyến Nghị):**
  - [ ] Cập nhật swagger.json với ProductImageDto schema
  - [ ] Regenerate Orval: `npm run generate:api`
  - [ ] Xóa workaround code nếu có
  - [ ] Test ProductDetailPage

- [ ] **Tùy Chọn 2 (Workaround):**
  - [ ] Tạo `transformImages()` function
  - [ ] Cập nhật `productService.getProductById()`
  - [ ] Cập nhật `ProductDetailPage.tsx` để handle cả 2 format
  - [ ] Test ProductDetailPage

- [ ] **Tùy Chọn 3 (Hybrid):**
  - [ ] Cập nhật type definitions
  - [ ] Cập nhật `getImageUrls()` logic
  - [ ] Test ProductDetailPage

## 🎯 Khuyến Nghị

**Tùy Chọn 1** là tốt nhất vì:
- ✅ Giữ type safety
- ✅ Không cần workaround
- ✅ Dễ maintain
- ✅ Tương lai-proof

**Tùy Chọn 2** nếu BE không thể thay đổi ngay.

**Tùy Chọn 3** là giải pháp tạm thời để support cả 2 format.

## 🔗 Liên Quan

- OpenAPI Spec: `swagger.json` (line 11588-11748)
- Generated Schema: `Api/generated-orval/schemas/productDetailDto.ts`
- Component: `src/pages/ProductDetailPage.tsx`
- Service: `src/lib/services/productService.ts`
