# CheckoutPage Refactor Guide - Using Orval Generated Types

## Current Issues Summary
- 44 TypeScript errors
- Wrong address field names (fullName, addressLine1, ward, district, province)
- Wrong payment method values (string literals instead of enum)
- Missing required fields (addressType)
- No proper null/undefined handling

## Step-by-Step Refactor Plan

### Step 1: Update Imports ✅

```typescript
// ❌ OLD - Wrong imports
import { PaymentMethod } from '../../lib/api/types';
import type { CheckoutDto, CreateAddressDto } from '@/api';

// ✅ NEW - Use Orval generated types
import type { 
  CreateAddressDto,
  AddressResponseDto,
  PaymentMethodType 
} from '../../../Api/generated-orval/schemas';
import { AddressType, PaymentMethodType as PaymentMethod } from '../../../Api/generated-orval/schemas';
```

### Step 2: Fix Address State Structure

```typescript
// ❌ OLD - Wrong structure
const [newAddress, setNewAddress] = useState<CreateAddressDto>({
  fullName: '',
  phoneNumber: '',
  addressLine1: '',
  ward: '',
  district: '',
  province: '',
  isDefault: false,
});

// ✅ NEW - Correct Orval structure
const [newAddress, setNewAddress] = useState<CreateAddressDto>({
  streetAddress: '',
  city: null,
  postalCode: null,
  state: null,
  country: 'Vietnam',
  addressType: AddressType.home,
  recipientName: null,
  phoneNumber: null,
  email: null,
  isDefault: false,
  isPrimary: false,
});
```

### Step 3: Fix Payment Method State

```typescript
// ❌ OLD - String literal
const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<PaymentMethod>('COD');

// ✅ NEW - Use enum
const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<PaymentMethodType>(
  PaymentMethodType.cod
);
```

### Step 4: Update Address Form Inputs

```typescript
// ❌ OLD - Wrong fields
<Input
  placeholder="Họ và tên"
  value={newAddress.fullName}
  onChange={(e) => setNewAddress({ ...newAddress, fullName: e.target.value })}
/>

// ✅ NEW - Correct fields
<Input
  placeholder="Tên người nhận"
  value={newAddress.recipientName || ''}
  onChange={(e) => setNewAddress({ ...newAddress, recipientName: e.target.value })}
/>

<Input
  placeholder="Địa chỉ chi tiết"
  value={newAddress.streetAddress}
  onChange={(e) => setNewAddress({ ...newAddress, streetAddress: e.target.value })}
/>

<Input
  placeholder="Thành phố/Tỉnh"
  value={newAddress.city || ''}
  onChange={(e) => setNewAddress({ ...newAddress, city: e.target.value })}
/>

<Input
  placeholder="Quận/Huyện"
  value={newAddress.state || ''}
  onChange={(e) => setNewAddress({ ...newAddress, state: e.target.value })}
/>

<Input
  placeholder="Mã bưu điện"
  value={newAddress.postalCode || ''}
  onChange={(e) => setNewAddress({ ...newAddress, postalCode: e.target.value })}
/>
```

### Step 5: Add Address Type Selector

```typescript
<Select 
  value={newAddress.addressType} 
  onValueChange={(value) => setNewAddress({ ...newAddress, addressType: value as AddressType })}
>
  <SelectTrigger>
    <SelectValue placeholder="Loại địa chỉ" />
  </SelectTrigger>
  <SelectContent>
    <SelectItem value={AddressType.home}>🏠 Nhà riêng</SelectItem>
    <SelectItem value={AddressType.office}>🏢 Văn phòng</SelectItem>
    <SelectItem value={AddressType.other}>📍 Khác</SelectItem>
  </SelectContent>
</Select>
```

### Step 6: Update Address Validation

```typescript
// ❌ OLD
if (!newAddress.fullName || !newAddress.phoneNumber || !newAddress.addressLine1 ||
    !newAddress.ward || !newAddress.district || !newAddress.province) {
  toast.error('Vui lòng điền đầy đủ thông tin địa chỉ');
  return;
}

// ✅ NEW
if (!newAddress.streetAddress || !newAddress.addressType) {
  toast.error('Vui lòng điền địa chỉ và chọn loại địa chỉ');
  return;
}

if (!newAddress.recipientName && !newAddress.phoneNumber) {
  toast.error('Vui lòng điền tên người nhận hoặc số điện thoại');
  return;
}
```

### Step 7: Update Address Display

```typescript
// ❌ OLD - Wrong fields
<span className="text-[#92400E]">{address.fullName}</span>
<span className="text-[#92400E]/70">{address.phoneNumber}</span>
<p className="text-sm text-[#92400E]/70">
  {address.addressLine1}
  {address.addressLine2 && `, ${address.addressLine2}`}
</p>
<p className="text-sm text-[#92400E]/60">
  {address.ward}, {address.district}, {address.province}
</p>

// ✅ NEW - Correct fields with null checks
<span className="text-[#92400E]">{address.recipientName || 'Người nhận'}</span>
<span className="text-[#92400E]/70">{address.phoneNumber || 'N/A'}</span>
<p className="text-sm text-[#92400E]/70">
  {address.streetAddress || 'Địa chỉ'}
</p>
<p className="text-sm text-[#92400E]/60">
  {[address.state, address.city, address.country].filter(Boolean).join(', ')}
</p>
{address.addressTypeDisplay && (
  <Badge variant="outline">{address.addressTypeDisplay}</Badge>
)}
```

### Step 8: Fix Payment Method Selection

```typescript
// ❌ OLD - String literals
onClick={() => setSelectedPaymentMethod('COD')}
onClick={() => setSelectedPaymentMethod('VNPay')}
onClick={() => setSelectedPaymentMethod('Momo')}

// ✅ NEW - Use enum values
onClick={() => setSelectedPaymentMethod(PaymentMethodType.cod)}
onClick={() => setSelectedPaymentMethod(PaymentMethodType.crediT_CARD)} // VNPay
onClick={() => setSelectedPaymentMethod(PaymentMethodType.e_WALLET)} // Momo/ZaloPay
onClick={() => setSelectedPaymentMethod(PaymentMethodType.banK_TRANSFER)}
```

### Step 9: Update RadioGroup Values

```typescript
// ❌ OLD
<RadioGroup value={selectedPaymentMethod} onValueChange={(value) => setSelectedPaymentMethod(value as PaymentMethod)}>
  <RadioGroupItem value="COD" id="cod" />
  <RadioGroupItem value="VNPay" id="vnpay" />
</RadioGroup>

// ✅ NEW
<RadioGroup 
  value={selectedPaymentMethod} 
  onValueChange={(value) => setSelectedPaymentMethod(value as PaymentMethodType)}
>
  <RadioGroupItem value={PaymentMethodType.cod} id="cod" />
  <RadioGroupItem value={PaymentMethodType.crediT_CARD} id="vnpay" />
  <RadioGroupItem value={PaymentMethodType.e_WALLET} id="momo" />
  <RadioGroupItem value={PaymentMethodType.banK_TRANSFER} id="bank" />
</RadioGroup>
```

### Step 10: Fix Checkout Data Structure

```typescript
// Need to check CheckoutDto structure from Orval
// Update shippingInfo to match backend expectations

const checkoutData: CheckoutDto = {
  cartId: cart.id,
  shippingInfo: {
    recipientName: selectedAddress.recipientName || '',
    phoneNumber: selectedAddress.phoneNumber || '',
    address: selectedAddress.streetAddress || '',
    city: selectedAddress.city || '',
    state: selectedAddress.state || '',
    country: selectedAddress.country || 'Vietnam',
    postalCode: selectedAddress.postalCode || '',
    deliveryNote: orderNote || '',
  },
  paymentMethod: selectedPaymentMethod,
  couponCode: cart.couponCode || couponCode || undefined,
  notes: orderNote || undefined,
};
```

### Step 11: Add Proper Loading States

```typescript
// ✅ Show loading while auth or cart is loading
if (cartLoading || addressLoading) {
  return <LoadingSpinner message="Đang tải thông tin thanh toán..." />;
}

// ✅ Show empty state if no cart
if (!cart || cart.items.length === 0) {
  return <EmptyCartMessage />;
}

// ✅ Disable checkout button properly
<Button
  onClick={handlePlaceOrder}
  disabled={isProcessing || !selectedAddressId || !cart}
  className="w-full"
>
  {isProcessing ? 'Đang xử lý...' : 'Đặt hàng'}
</Button>
```

### Step 12: Add Null Safety

```typescript
// ✅ Safe access to optional fields
{(item.price || 0).toLocaleString('vi-VN')}₫
{(cart.subtotal || 0).toLocaleString('vi-VN')}₫
{address.recipientName || 'Người nhận'}
{address.phoneNumber || 'N/A'}
```

## Implementation Checklist

- [ ] Update all imports to use Orval types
- [ ] Fix newAddress state structure
- [ ] Fix selectedPaymentMethod state
- [ ] Update address form inputs
- [ ] Add address type selector
- [ ] Update address validation
- [ ] Fix address display logic
- [ ] Fix payment method selection
- [ ] Update RadioGroup values
- [ ] Fix checkout data structure
- [ ] Add proper loading states
- [ ] Add null safety checks
- [ ] Test address creation
- [ ] Test address selection
- [ ] Test payment method selection
- [ ] Test checkout process
- [ ] Verify TypeScript has 0 errors

## Testing After Refactor

1. **Address Management**
   - [ ] Can add new address with all fields
   - [ ] Address type selector works
   - [ ] Address displays correctly
   - [ ] Can select address
   - [ ] Default address auto-selected

2. **Payment Methods**
   - [ ] All payment methods selectable
   - [ ] Correct enum values sent to backend
   - [ ] Payment method persists selection

3. **Checkout Flow**
   - [ ] Can complete checkout with COD
   - [ ] Can complete checkout with online payment
   - [ ] Proper error handling
   - [ ] Success/failure navigation works

4. **TypeScript**
   - [ ] 0 TypeScript errors
   - [ ] All types match Orval schemas
   - [ ] No `any` types used

## Notes

- Backend uses `streetAddress`, not `addressLine1`
- Backend uses `city` and `state`, not `ward`, `district`, `province`
- Backend uses `recipientName`, not `fullName`
- `addressType` is REQUIRED field
- Payment methods use specific enum values from backend
- All optional fields should have null checks

## Related Files

- `Api/generated-orval/schemas/createAddressDto.ts`
- `Api/generated-orval/schemas/addressResponseDto.ts`
- `Api/generated-orval/schemas/addressType.ts`
- `Api/generated-orval/schemas/paymentMethodType.ts`
- `src/lib/hooks/useCustomerAddress.ts`
- `src/lib/hooks/useCheckout.ts`
