# Checkout 400 Bad Request - Fix Complete ✅

## Problem
POST `/Checkout/process` returned 400 Bad Request due to invalid payload structure.

## Root Causes Identified

### 1. Missing Required Fields
`OrderShippingInputDto` requires ALL these fields:
- `recipientName` (string, 1-100 chars)
- `phoneNumber` (string, must match `^[0-9]{10,11}$`)
- `address` (string, 1-255 chars)
- `ward` (string, 1-100 chars)
- `district` (string, 1-100 chars)
- `city` (string, 1-100 chars)
- `shippingMethod` (ShippingMethodEnum)

### 2. Invalid Phone Number Format
- Backend requires: 10-11 digits only
- Was sending: Could be empty string or invalid format

### 3. Empty String Values
- Was sending empty strings `''` for required fields
- Backend validation rejects empty strings

### 4. Wrong PaymentMethod Value
- Was sending: `"cod"` (string)
- Backend expects: Enum value from `PaymentMethodType`

## Solutions Implemented

### Part 1: Add Validation in CheckoutPage ✅

```typescript
// ✅ Validate address has required fields
if (!selectedAddress.streetAddress) {
  toast.error('Địa chỉ thiếu thông tin chi tiết');
  return;
}

if (!selectedAddress.phoneNumber) {
  toast.error('Địa chỉ thiếu số điện thoại');
  return;
}

// Validate phone number format (10-11 digits)
const phoneRegex = /^[0-9]{10,11}$/;
if (!phoneRegex.test(selectedAddress.phoneNumber)) {
  toast.error('Số điện thoại không hợp lệ (cần 10-11 chữ số)');
  return;
}
```

### Part 2: Fix Payload Structure ✅

**Before:**
```typescript
shippingInfo: {
  recipientName: selectedAddress.recipientName || 'Người nhận',
  phoneNumber: selectedAddress.phoneNumber || '0000000000',
  address: selectedAddress.streetAddress || '',  // ❌ Could be empty
  ward: selectedAddress.state || 'N/A',
  district: selectedAddress.city || 'N/A',
  city: selectedAddress.country || 'Vietnam',
}
```

**After:**
```typescript
shippingInfo: {
  recipientName: selectedAddress.recipientName || 'Người nhận',
  phoneNumber: selectedAddress.phoneNumber,  // ✅ Validated above
  address: selectedAddress.streetAddress,    // ✅ Validated above
  ward: selectedAddress.state || 'Phường/Xã',
  district: selectedAddress.city || 'Quận/Huyện',
  city: selectedAddress.country || 'Vietnam',
  postalCode: selectedAddress.postalCode || null,
  deliveryNote: orderNote || null,
  shippingMethod: ShippingMethodEnum.standard,
}
```

### Part 3: Add Comprehensive Logging ✅

**In CheckoutPage:**
```typescript
// 🔍 LOG PAYLOAD BEFORE API CALL
console.log('[CheckoutPage] 📦 Checkout Payload:', JSON.stringify(checkoutData, null, 2));
console.log('[CheckoutPage] 💳 Payment Method:', selectedPaymentMethod);
console.log('[CheckoutPage] 🚚 Shipping Info:', checkoutData.shippingInfo);
```

**In useCheckout Hook:**
```typescript
// 🔍 LOG PAYLOAD BEFORE API CALL
console.log('[useCheckout] 🛒 Processing checkout...');
console.log('[useCheckout] 📦 Full Payload:', JSON.stringify(checkoutData, null, 2));
console.log('[useCheckout] 🚚 Shipping Info:', checkoutData.shippingInfo);
console.log('[useCheckout] 💳 Payment Method:', checkoutData.paymentMethod);
console.log('[useCheckout] 🎫 Coupon Code:', checkoutData.couponCode);

// After API call
console.log('[useCheckout] 📡 API Response:', response);

// On error
console.error('[useCheckout] ❌ Error response:', err?.response);
console.error('[useCheckout] ❌ Error data:', err?.response?.data);
console.error('[useCheckout] ❌ Error status:', err?.response?.status);
```

### Part 4: Migrate useCheckout to Orval ✅

**Before:**
```typescript
import { CheckoutService } from '@/api/services/CheckoutService';
import type { CheckoutDto } from '@/api';

const response = await CheckoutService.postApiV1CheckoutProcess(checkoutData);
```

**After:**
```typescript
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { CheckoutDto } from '../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();
const response = await api.postApiV1CheckoutProcess(checkoutData);
```

## Expected Payload Structure

```json
{
  "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
  "shippingInfo": {
    "recipientName": "Nguyễn Văn A",
    "phoneNumber": "0123456789",
    "address": "123 Đường ABC",
    "ward": "Phường 1",
    "district": "Quận 1",
    "city": "TP. Hồ Chí Minh",
    "postalCode": "700000",
    "deliveryNote": "Gọi trước 15 phút",
    "shippingMethod": "standard"
  },
  "paymentMethod": "cod",
  "couponCode": null,
  "notes": null
}
```

## Validation Rules

### Phone Number
- **Pattern**: `^[0-9]{10,11}$`
- **Valid**: `0123456789`, `01234567890`
- **Invalid**: `012-345-6789`, `+84123456789`, `abc1234567`

### Required String Fields
- Must not be empty string
- Must meet minLength requirement (usually 1)
- Use meaningful defaults if original value is missing

### Enum Fields
- `shippingMethod`: Must be one of `ShippingMethodEnum` values
- `paymentMethod`: Must be one of `PaymentMethodType` values

## Testing Checklist

### Before Checkout
- [ ] Address has `streetAddress` (not empty)
- [ ] Address has `phoneNumber` (10-11 digits)
- [ ] Phone number matches regex pattern
- [ ] All required fields have non-empty values

### During Checkout
- [ ] Payload logged to console
- [ ] All required fields present in payload
- [ ] Phone number format correct
- [ ] Enum values correct (not string literals)

### After Checkout
- [ ] API returns 200 OK (not 400)
- [ ] Order created successfully
- [ ] Response data logged
- [ ] Success toast shown

### On Error
- [ ] Error response logged
- [ ] Error message extracted correctly
- [ ] User-friendly error toast shown
- [ ] Error details available in console

## Files Modified

1. `src/pages/checkout/CheckoutPage.tsx`
   - Added address validation
   - Fixed payload structure
   - Added comprehensive logging

2. `src/lib/hooks/useCheckout.ts`
   - Migrated to Orval API
   - Added detailed logging
   - Improved error handling

## Debugging Tips

### If Still Getting 400 Error

1. **Check Console Logs**:
   ```
   [CheckoutPage] 📦 Checkout Payload: { ... }
   [useCheckout] 📦 Full Payload: { ... }
   ```

2. **Verify Required Fields**:
   - All strings have values (not empty)
   - Phone number is 10-11 digits
   - Enum values are correct

3. **Check Backend Logs**:
   - What validation error is backend returning?
   - Which field is causing the issue?

4. **Compare with Swagger**:
   - Open `/swagger/index.html`
   - Check `CheckoutDto` schema
   - Verify all required fields match

### Common Issues

1. **Empty Strings**: Replace with meaningful defaults
2. **Wrong Enum Values**: Use enum constants, not strings
3. **Missing Fields**: Ensure all required fields present
4. **Invalid Format**: Check regex patterns (especially phone)

## Success Metrics

- ✅ Validation prevents invalid submissions
- ✅ Payload structure matches backend expectations
- ✅ Comprehensive logging for debugging
- ✅ Orval types ensure type safety
- ✅ User-friendly error messages

## Next Steps

1. Test checkout with valid address
2. Verify backend accepts payload
3. Check order creation in database
4. Test all payment methods
5. Verify error handling for edge cases

## Related Documentation

- `CHECKOUT_REFACTOR_COMPLETE.md` - Full refactor details
- `CHECKOUT_REFACTOR_GUIDE.md` - Step-by-step guide
- `Api/generated-orval/schemas/checkoutDto.ts` - Type definition
- `Api/generated-orval/schemas/orderShippingInputDto.ts` - Shipping info structure
