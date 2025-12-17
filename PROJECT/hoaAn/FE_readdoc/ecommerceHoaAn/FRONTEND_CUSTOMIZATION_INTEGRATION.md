# Frontend Customization Integration Guide

## 📋 Tóm tắt
Backend đã hoàn thành implement customization. Frontend có thể sử dụng ngay.

---

## 🎯 Các bước integrate

### 1. Import types
```typescript
import type {
  CustomizableOption,
  CartItemCustomization,
  CartItemDetail,
  GetCartResponse,
} from '@/lib/types/customization';
import {
  calculateCustomizationPrice,
  calculateFinalPrice,
  formatPrice,
  calculatePriceBreakdown,
} from '@/lib/types/customization';
```

### 2. Lấy product detail
```typescript
const product = await productService.getProductById(productId);

// Product có customizableOptions
if (product.customizableOptions) {
  // Hiển thị form cho user chọn customizations
}
```

### 3. Cho user chọn customizations
```typescript
// State để lưu số lượng của mỗi option
const [customizationQuantities, setCustomizationQuantities] = useState<Record<string, number>>({});

// Khi user thay đổi quantity
const handleUpdateCustomization = (optionId: string, newQuantity: number) => {
  const option = product.customizableOptions?.find(o => o.id === optionId);
  if (!option) return;

  // Validate quantity
  const min = option.minQuantity || 0;
  const max = option.maxQuantity || Infinity;
  const validQuantity = Math.max(min, Math.min(max, newQuantity));

  setCustomizationQuantities(prev => ({
    ...prev,
    [optionId]: validQuantity
  }));
};
```

### 4. Build customizations array
```typescript
const customizations = product.customizableOptions?.map(option => {
  const qty = customizationQuantities[option.id] || 0;
  return {
    optionId: option.id,
    quantity: qty,
    unitPrice: option.unitPrice || 0,
    totalPrice: qty * (option.unitPrice || 0)
  };
}) || undefined;
```

### 5. Add to cart
```typescript
const dto: AddToCartDto = {
  productId: product.id,
  quantity: 1,
  customizations: customizations
};

await api.postApiV1CartAdd(dto);
```

### 6. Hiển thị cart
```typescript
const cart = await api.getApiV1Cart();

// Mỗi item trong cart có:
// - basePrice: giá gốc
// - customizationPrice: tổng giá customizations
// - finalPrice: basePrice + customizationPrice
// - customizations: array của customizations

cart.items.forEach(item => {
  console.log(`Item: ${item.productName}`);
  console.log(`Base Price: ${formatPrice(item.basePrice)}`);
  console.log(`Customization Price: ${formatPrice(item.customizationPrice)}`);
  console.log(`Final Price: ${formatPrice(item.finalPrice)}`);
  console.log(`Total: ${formatPrice(item.finalPrice * item.quantity)}`);
});
```

---

## 💡 Ví dụ thực tế

### ProductDetailPage.tsx
```typescript
import { useState, useEffect } from 'react';
import type { ProductDetailDto } from '@/Api/generated-orval/schemas';
import type { AddToCartDto } from '@/Api/generated-orval/schemas';
import { getVietCommerceAPI } from '@/Api/generated-orval';
import { formatPrice } from '@/lib/types/customization';

const api = getVietCommerceAPI();

export function ProductDetailPage() {
  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [customizationQuantities, setCustomizationQuantities] = useState<Record<string, number>>({});

  // Load product
  useEffect(() => {
    const loadProduct = async () => {
      const data = await productService.getProductById(id);
      setProduct(data);

      // Initialize customization quantities
      if (data.customizableOptions) {
        const quantities: Record<string, number> = {};
        data.customizableOptions.forEach(option => {
          quantities[option.id] = option.baseQuantity || 0;
        });
        setCustomizationQuantities(quantities);
      }
    };
    loadProduct();
  }, [id]);

  // Update customization quantity
  const handleUpdateCustomization = (optionId: string, newQuantity: number) => {
    const option = product?.customizableOptions?.find(o => o.id === optionId);
    if (!option) return;

    const min = option.minQuantity || 0;
    const max = option.maxQuantity || Infinity;
    const validQuantity = Math.max(min, Math.min(max, newQuantity));

    setCustomizationQuantities(prev => ({
      ...prev,
      [optionId]: validQuantity
    }));
  };

  // Add to cart
  const handleAddToCart = async () => {
    const customizations = product?.customizableOptions?.map(option => {
      const qty = customizationQuantities[option.id] || 0;
      return {
        optionId: option.id,
        quantity: qty,
        unitPrice: option.unitPrice || 0,
        totalPrice: qty * (option.unitPrice || 0)
      };
    });

    const dto: AddToCartDto = {
      productId: product!.id,
      quantity: 1,
      customizations
    };

    await api.postApiV1CartAdd(dto);
    await refreshCart();
  };

  // Render customization options
  return (
    <div>
      {product?.customizableOptions && (
        <div className="space-y-4">
          <h3>Tùy chọn thêm</h3>
          {product.customizableOptions.map(option => (
            <div key={option.id} className="border rounded-lg p-4">
              <div className="flex justify-between mb-3">
                <div>
                  <p className="font-medium">{option.name}</p>
                  <p className="text-sm text-gray-600">
                    {formatPrice(option.unitPrice)} / {option.unit}
                  </p>
                </div>
                <p className="font-semibold text-rose-600">
                  {formatPrice((customizationQuantities[option.id] || 0) * option.unitPrice)}
                </p>
              </div>
              <div className="flex items-center gap-2">
                <button
                  onClick={() => handleUpdateCustomization(option.id, (customizationQuantities[option.id] || 0) - 1)}
                >
                  −
                </button>
                <span className="flex-1 text-center">
                  {customizationQuantities[option.id] || 0}
                </span>
                <button
                  onClick={() => handleUpdateCustomization(option.id, (customizationQuantities[option.id] || 0) + 1)}
                >
                  +
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
```

### CartPage.tsx
```typescript
import { CartItemCustomizations } from '@/components/CartItemCustomizations';
import { formatPrice } from '@/lib/types/customization';

export function CartPage() {
  const [cart, setCart] = useState<GetCartResponse | null>(null);

  useEffect(() => {
    const loadCart = async () => {
      const response = await api.getApiV1Cart();
      setCart(response.data);
    };
    loadCart();
  }, []);

  return (
    <div>
      {cart?.items.map(item => (
        <div key={item.cartItemId} className="border rounded-lg p-4">
          <div className="flex justify-between mb-2">
            <h3>{item.productName}</h3>
            <span className="font-semibold">{formatPrice(item.finalPrice * item.quantity)}</span>
          </div>

          {/* Price breakdown */}
          <div className="text-sm text-gray-600 space-y-1">
            <div className="flex justify-between">
              <span>Giá gốc:</span>
              <span>{formatPrice(item.basePrice)}</span>
            </div>
            {item.customizationPrice > 0 && (
              <div className="flex justify-between">
                <span>Tùy chọn:</span>
                <span>{formatPrice(item.customizationPrice)}</span>
              </div>
            )}
            <div className="flex justify-between font-medium text-gray-900 border-t pt-1">
              <span>Giá cuối:</span>
              <span>{formatPrice(item.finalPrice)}</span>
            </div>
          </div>

          {/* Customizations details */}
          <CartItemCustomizations customizations={item.customizations} />
        </div>
      ))}

      {/* Cart total */}
      <div className="mt-6 border-t pt-4">
        <div className="flex justify-between text-lg font-bold">
          <span>Tổng cộng:</span>
          <span>{formatPrice(cart?.totalAmount || 0)}</span>
        </div>
      </div>
    </div>
  );
}
```

---

## ⚠️ Lưu ý quan trọng

### 1. Giá hiển thị
- **Không dùng:** `unitPrice` (giá gốc)
- **Dùng:** `finalPrice` (giá gốc + customizations)

### 2. Customizations với quantity = 0
- Không được tính vào giá
- Nhưng vẫn được lưu trong database (để reference)

### 3. Validation
Backend sẽ validate:
- Option phải tồn tại trong product
- Quantity phải trong range `[minQuantity, maxQuantity]`
- Nếu lỗi, API trả về error message

### 4. Guest cart
- Sử dụng `sessionId` từ cookie
- Tự động tạo nếu chưa có
- Merge vào user cart khi login

---

## 🔄 Flow hoàn chỉnh

```
1. User xem product detail
   ↓
2. Product có customizableOptions
   ↓
3. User chọn quantity cho mỗi option
   ↓
4. Frontend build customizations array
   ↓
5. Frontend gửi request add to cart với customizations
   ↓
6. Backend validate customizations
   ↓
7. Backend tính: customizationPrice = sum(customization.totalPrice)
   ↓
8. Backend tính: finalPrice = basePrice + customizationPrice
   ↓
9. Backend lưu vào database
   ↓
10. Backend trả về response với cartItemId
    ↓
11. Frontend refresh cart
    ↓
12. Frontend hiển thị cart với customizations breakdown
    ↓
13. User checkout
    ↓
14. Backend tạo order với finalPrice
```

---

## 📚 API Reference

### Add to Cart
```
POST /api/v1/Cart/add
Authorization: Bearer {token}

Request:
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-incense-burner",
      "quantity": 1,
      "unitPrice": 1200000,
      "totalPrice": 1200000
    }
  ]
}

Response:
{
  "success": true,
  "data": {
    "cartItemId": "6ddb7cae-f333-4c48-b026-3aa498c98b2b",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 7900000.00,
    "cartItemCount": 1,
    "cartTotalAmount": 7900000.00
  }
}
```

### Get Cart
```
GET /api/v1/Cart
Authorization: Bearer {token}

Response:
{
  "success": true,
  "data": {
    "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
    "items": [
      {
        "cartItemId": "6ddb7cae-f333-4c48-b026-3aa498c98b2b",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "productName": "Lễ Tân Gia Trọn Gói Đầy Đủ",
        "basePrice": 5500000.00,
        "customizationPrice": 1800000.00,
        "finalPrice": 7300000.00,
        "customizations": [
          {
            "optionId": "opt-incense-burner",
            "quantity": 1,
            "unitPrice": 1200000,
            "totalPrice": 1200000
          }
        ]
      }
    ],
    "totalAmount": 7300000.00
  }
}
```

---

## ✅ Checklist

- [ ] Import customization types
- [ ] Hiển thị customizable options trên product detail page
- [ ] Cho user chọn quantity cho mỗi option
- [ ] Validate quantity trong range min/max
- [ ] Build customizations array khi add to cart
- [ ] Hiển thị customizations breakdown trong cart
- [ ] Sử dụng `finalPrice` thay vì `unitPrice`
- [ ] Test với multiple customizations
- [ ] Test với zero quantity customizations
- [ ] Test guest cart
- [ ] Test checkout với customizations
