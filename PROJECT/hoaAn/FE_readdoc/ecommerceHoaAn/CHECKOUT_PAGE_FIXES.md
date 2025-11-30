# CheckoutPage Fixes

## Issue 1: toLocaleString Error ✅ FIXED

### Problem
```
Uncaught TypeError: Cannot read properties of undefined (reading 'toLocaleString')
at CheckoutPage.tsx:572:47
```

### Root Cause
Cart item fields were accessed incorrectly:
- Used `item.unitPrice` → should be `item.price`
- Used `item.productName` → should be `item.name`
- Used `item.productImageUrl` → should be `item.image`
- Used `cart.discountAmount` → should be `cart.discount`
- No null/undefined checks before calling `toLocaleString()`

### Solution Applied
```typescript
// ❌ Before
{item.unitPrice.toLocaleString('vi-VN')}₫
{cart.discountAmount.toLocaleString('vi-VN')}₫

// ✅ After
{(item.price || 0).toLocaleString('vi-VN')}₫
{(cart.discount || 0).toLocaleString('vi-VN')}₫
```

### Changes Made
1. Fixed field names to match Cart interface from useCart:
   - `item.unitPrice` → `item.price`
   - `item.productName` → `item.name`
   - `item.productImageUrl` → `item.image`
   - `cart.discountAmount` → `cart.discount`

2. Added safe access with default values:
   - `(cart.subtotal || 0)`
   - `(cart.shippingFee || 0)`
   - `(cart.discount || 0)`
   - `(cart.totalAmount || 0)`
   - `(item.price || 0)`

## Issue 2: Address Structure Mismatch ⚠️ NEEDS FIXING

### Problem
CheckoutPage uses incorrect address field names that don't match the backend API.

### Current (Wrong) Structure
```typescript
interface CreateAddressDto {
  fullName: string;
  phoneNumber: string;
  addressLine1: string;
  ward: string;
  district: string;
  province: string;
  isDefault: boolean;
}
```

### Correct Structure (from Orval API)
```typescript
interface CreateAddressDto {
  streetAddress: string;           // ✅ Required
  city?: string | null;             // Optional
  postalCode?: string | null;       // Optional
  state?: string | null;            // Optional
  country?: string | null;          // Optional
  addressType: AddressType;         // ✅ Required (enum)
  recipientName?: string | null;    // Optional (not fullName!)
  phoneNumber?: string | null;      // Optional
  email?: string | null;            // Optional
  isDefault?: boolean;              // Optional
  isPrimary?: boolean;              // Optional
}
```

### Impact
- 44 TypeScript errors in CheckoutPage
- Address form won't work correctly
- API calls will fail due to wrong field names

### Required Changes
1. Update `newAddress` state to use correct fields
2. Update form inputs to match API structure
3. Add `addressType` field (required)
4. Rename fields:
   - `fullName` → `recipientName`
   - `addressLine1` → `streetAddress`
   - `ward` → remove (use `city` or `state`)
   - `district` → remove (use `city` or `state`)
   - `province` → `state` or `city`

5. Update address display logic
6. Update validation logic

## Issue 3: PaymentMethod Type Mismatch ⚠️ NEEDS FIXING

### Problem
```typescript
// ❌ Wrong
setSelectedPaymentMethod('COD')
setSelectedPaymentMethod('VNPay')
```

### Solution Needed
Check the correct `PaymentMethod` enum from API and use proper values.

## Recommended Next Steps

### Priority 1: Fix Address Structure (High Priority)
The entire address handling in CheckoutPage needs to be rewritten to match the backend API structure.

### Priority 2: Fix PaymentMethod Enum
Update payment method values to match backend enum.

### Priority 3: Add Proper Loading States
Ensure cart data is fully loaded before rendering checkout form.

## Files Affected
- `src/pages/checkout/CheckoutPage.tsx` - Main file with issues
- `Api/generated-orval/schemas/createAddressDto.ts` - Correct structure
- `Api/generated-orval/schemas/addressResponseDto.ts` - Response structure
- `src/lib/hooks/useCart.ts` - Cart interface

## Testing Checklist
- [x] Cart items display without toLocaleString error
- [x] Price formatting works correctly
- [ ] Address form uses correct field names
- [ ] Address creation API call succeeds
- [ ] Payment method selection works
- [ ] Checkout process completes successfully
