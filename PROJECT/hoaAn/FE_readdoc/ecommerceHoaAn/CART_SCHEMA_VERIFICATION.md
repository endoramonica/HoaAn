# Cart API Schema Verification

## ✅ Schema Verification Complete

### API Response Structure Confirmed

Your cart API response includes all required fields:

```json
{
  "success": true,
  "data": {
    "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
    "userId": "48fdbb7b-9d91-4e41-872e-dd8296a0f315",
    "items": [
      {
        "cartItemId": "cef179da-fcfb-4f8f-ad33-48bd57c613d7",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "productName": "Lễ Tân Gia Trọn Gói Đầy Đủ",
        "productSlug": "le-tan-gia-tron-goi-day-du",
        "sku": "SVC-NEWHOUSE-001",
        "productImage": "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800",
        "unitPrice": 5500000.00,
        "quantity": 1,
        "totalPrice": 5500000.00,
        "availableStock": 100,
        "isProductActive": true,
        "purchaseCount": 18,
        "avgRating": 4.50,
        "reviewCount": 0,
        "createdAt": "2025-12-12T10:50:45.8775072",
        "updatedAt": "2025-12-12T10:50:45.8775073",
        "basePrice": 0.00,
        "customizationPrice": 0.00,
        "finalPrice": 0.00
      }
    ],
    "totalItems": 3,
    "subTotal": 9060000.00,
    "taxAmount": 0,
    "shippingFee": 0,
    "totalAmount": 9060000.00,
    "createdAt": "2025-10-27T09:19:37.2681566",
    "updatedAt": "2025-12-12T10:50:45.8852814"
  },
  "message": "Cart retrieved successfully"
}
```

### Summary Fields ✅

All required summary fields are present in the response:

| Field | Value | Type | Status |
|-------|-------|------|--------|
| `totalItems` | 3 | number | ✅ Present |
| `subTotal` | 9060000.00 | number | ✅ Present |
| `taxAmount` | 0 | number | ✅ Present |
| `shippingFee` | 0 | number | ✅ Present |
| `totalAmount` | 9060000.00 | number | ✅ Present |

### TypeScript Types Added

Created proper TypeScript interfaces in `src/lib/api/types.ts`:

```typescript
export interface CartItemDto {
  cartItemId: string;
  productId: string;
  productName: string;
  productSlug: string;
  sku: string;
  productImage: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  availableStock: number;
  isProductActive: boolean;
  purchaseCount: number;
  avgRating: number;
  reviewCount: number;
  basePrice: number;
  customizationPrice: number;
  finalPrice: number;
  createdAt: string;
  updatedAt: string;
}

export interface CartDto {
  cartId: string;
  userId: string;
  items: CartItemDto[];
  totalItems: number;
  subTotal: number;
  taxAmount: number;
  shippingFee: number;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CartSummaryDto {
  totalItems: number;
  subTotal: number;
  taxAmount: number;
  shippingFee: number;
  totalAmount: number;
  appliedCoupon?: string;
}

export interface CartResponseDto {
  success: boolean;
  data: CartDto;
  message: string;
}

export interface CartSummaryResponseDto {
  success: boolean;
  data: CartSummaryDto;
  message: string;
}
```

### Parsing Updates in useCart Hook

Updated `src/lib/hooks/useCart.ts` to:

1. **Use proper types** - CartResponseDto and CartSummaryResponseDto
2. **Correct field mapping for cart items**:
   - `cartItemId` → `id` (cart item identifier)
   - `productName` → `name`
   - `unitPrice` → `price`
   - `productImage` → `image`
   - `availableStock` → `inStock` check and `maxQuantity`
3. **Correct summary mapping from `/api/v1/Cart` response**:
   - `cartData.subTotal` → `subtotal`
   - `cartData.totalAmount` → `totalAmount`
   - `cartData.totalItems` → `itemCount`
   - `cartData.taxAmount` → `discount` (for UI display)
   - `cartData.shippingFee` → `shippingFee` ✅ **Parsed from main cart response**
4. **Fallback to summary response** if main cart response doesn't have values

### CartPage Display

CartPage now correctly displays:
- ✅ Product names from `productName`
- ✅ Prices from `unitPrice`
- ✅ Images from `productImage`
- ✅ Stock availability from `availableStock`
- ✅ Cart summary with correct totals
- ✅ Item count from `totalItems`
- ✅ **Shipping fee from `/api/v1/Cart` response** (cartData.shippingFee)

### Verification Checklist

- ✅ All required fields present in API response
- ✅ TypeScript types created and exported
- ✅ useCart hook updated with proper parsing
- ✅ Field mappings corrected
- ✅ No type errors in diagnostics
- ✅ CartPage ready to display data correctly
