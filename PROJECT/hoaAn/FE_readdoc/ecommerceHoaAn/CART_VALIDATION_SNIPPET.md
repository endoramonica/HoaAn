# Cart Validation Snippet for CheckoutPage

## Problem
Backend returns `EMPTY_CART` error even though cart has items. This happens when:
1. Cart `userId` doesn't match the authenticated user's token
2. Cart session expired
3. Cart was cleared but frontend still has stale data

## Solution
Add cart validation in `CheckoutPage.tsx` BEFORE calling `processCheckout()`.

## Snippet to Add in CheckoutPage

### Step 1: Add Validation Before handlePlaceOrder

```typescript
const handlePlaceOrder = async () => {
  try {
    // ✅ STEP 1: Validate cart exists and has items
    console.log('[CheckoutPage] 🔍 Validating cart before checkout...');
    
    if (!cart) {
      toast.error('Không tìm thấy giỏ hàng');
      return;
    }

    if (!cart.id) {
      toast.error('Giỏ hàng không hợp lệ');
      return;
    }

    if (!cart.items || cart.items.length === 0) {
      toast.error('Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.');
      return;
    }

    console.log('[CheckoutPage] ✅ Cart validation passed:', {
      cartId: cart.id,
      itemCount: cart.items.length,
      totalAmount: cart.totalAmount
    });

    // ✅ STEP 2: Validate address
    if (!selectedAddressId) {
      toast.error('Vui lòng chọn địa chỉ giao hàng');
      return;
    }

    const selectedAddress = addresses.find(a => a.id === selectedAddressId);
    if (!selectedAddress) {
      toast.error('Địa chỉ không hợp lệ');
      return;
    }

    // ... rest of your checkout logic
  } catch (err: any) {
    // ... error handling
  }
};
```

### Step 2: Add Cart Refresh Before Checkout (Optional but Recommended)

```typescript
const handlePlaceOrder = async () => {
  try {
    // ✅ Refresh cart to ensure latest data
    console.log('[CheckoutPage] 🔄 Refreshing cart before checkout...');
    await refreshCart();
    
    // Small delay to ensure cart state is updated
    await new Promise(resolve => setTimeout(resolve, 100));
    
    // Then validate cart
    if (!cart || !cart.items || cart.items.length === 0) {
      toast.error('Giỏ hàng trống');
      return;
    }

    // ... proceed with checkout
  } catch (err: any) {
    // ... error handling
  }
};
```

### Step 3: Add Loading State During Validation

```typescript
const [isValidating, setIsValidating] = useState(false);

const handlePlaceOrder = async () => {
  try {
    setIsValidating(true);
    
    // Validate cart
    if (!cart || cart.items.length === 0) {
      toast.error('Giỏ hàng trống');
      return;
    }
    
    setIsValidating(false);
    
    // Proceed with checkout
    const result = await processCheckout(checkoutData);
    
    // ... handle result
  } catch (err: any) {
    // ... error handling
  } finally {
    setIsValidating(false);
  }
};

// Update button
<Button
  onClick={handlePlaceOrder}
  disabled={isProcessing || isValidating || !selectedAddressId}
>
  {isValidating ? 'Đang kiểm tra...' : isProcessing ? 'Đang xử lý...' : 'Đặt hàng'}
</Button>
```

## Complete Example

```typescript
const handlePlaceOrder = async () => {
  try {
    // ============================================================================
    // STEP 1: Validate Cart
    // ============================================================================
    console.log('[CheckoutPage] 🔍 Validating cart...');
    
    if (!cart) {
      toast.error('Không tìm thấy giỏ hàng');
      return;
    }

    if (!cart.id) {
      toast.error('Giỏ hàng không hợp lệ');
      return;
    }

    if (!cart.items || cart.items.length === 0) {
      toast.error('Giỏ hàng trống. Vui lòng thêm sản phẩm.');
      navigate('/cart');
      return;
    }

    console.log('[CheckoutPage] ✅ Cart valid:', {
      cartId: cart.id,
      itemCount: cart.items.length,
      totalAmount: cart.totalAmount
    });

    // ============================================================================
    // STEP 2: Validate Address
    // ============================================================================
    if (!selectedAddressId) {
      toast.error('Vui lòng chọn địa chỉ giao hàng');
      return;
    }

    const selectedAddress = addresses.find(a => a.id === selectedAddressId);
    if (!selectedAddress) {
      toast.error('Địa chỉ không hợp lệ');
      return;
    }

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

    console.log('[CheckoutPage] ✅ Address valid');

    // ============================================================================
    // STEP 3: Create Checkout Data
    // ============================================================================
    const checkoutData: CheckoutDto = {
      cartId: cart.id,
      shippingInfo: {
        recipientName: selectedAddress.recipientName || 'Người nhận',
        phoneNumber: selectedAddress.phoneNumber,
        address: selectedAddress.streetAddress,
        ward: selectedAddress.state || 'Phường/Xã',
        district: selectedAddress.city || 'Quận/Huyện',
        city: selectedAddress.country || 'Vietnam',
        postalCode: selectedAddress.postalCode || null,
        deliveryNote: orderNote || null,
        shippingMethod: ShippingMethodEnum.standard,
      },
      paymentMethod: selectedPaymentMethod,
      couponCode: cart.couponCode || null,
      notes: orderNote || null,
    };

    console.log('[CheckoutPage] 📦 Checkout data prepared');

    // ============================================================================
    // STEP 4: Process Checkout
    // ============================================================================
    const result = await processCheckout(checkoutData);

    if (!result) {
      toast.error('Không thể đặt hàng');
      onNavigate('failed', { reason: 'Checkout failed' });
      return;
    }

    console.log('[CheckoutPage] ✅ Checkout successful');

    // ============================================================================
    // STEP 5: Handle Payment
    // ============================================================================
    if (selectedPaymentMethod === PaymentMethod.cod || selectedPaymentMethod === PaymentMethod.cash) {
      onNavigate('success', { 
        orderId: result.orderId || '', 
        orderNumber: result.orderNumber || '' 
      });
    } else if (selectedPaymentMethod === PaymentMethod.banK_TRANSFER) {
      onNavigate('success', { 
        orderId: result.orderId || '', 
        orderNumber: result.orderNumber || '',
        paymentMethod: 'BankTransfer',
      });
    } else {
      // Online payment
      const paymentResponse = await paymentService.createPayment({
        orderId: result.orderId || '',
        amount: result.totalAmount || 0,
        paymentMethod: selectedPaymentMethod as any,
        returnUrl: `${window.location.origin}/checkout?step=success&orderId=${result.orderId}&orderNumber=${result.orderNumber}`,
        cancelUrl: `${window.location.origin}/checkout?step=failed&orderId=${result.orderId}&reason=Payment cancelled`,
      });

      if (paymentResponse.paymentUrl) {
        toast.success('Đang chuyển đến cổng thanh toán...');
        sessionStorage.setItem('pendingOrderId', result.orderId || '');
        sessionStorage.setItem('pendingOrderNumber', result.orderNumber || '');
        window.location.href = paymentResponse.paymentUrl;
      } else {
        onNavigate('success', { 
          orderId: result.orderId || '', 
          orderNumber: result.orderNumber || '' 
        });
      }
    }
  } catch (err: any) {
    console.error('[CheckoutPage] ❌ Place order error:', err);
    const errorMsg = err.message || 'Không thể đặt hàng';
    toast.error(errorMsg);
    onNavigate('failed', { reason: errorMsg });
  }
};
```

## Key Points

### ✅ Validate Cart
- Check `cart` exists
- Check `cart.id` exists
- Check `cart.items` has length > 0

### ✅ Validate Address
- Check `selectedAddressId` exists
- Check address has required fields
- Validate phone number format

### ✅ Log Everything
- Log validation steps
- Log checkout data
- Log success/failure

### ✅ User-Friendly Errors
- Show specific error messages
- Guide user to fix the issue
- Redirect if needed (e.g., to cart page)

## Testing

1. **Empty Cart**: Remove all items → Should show "Giỏ hàng trống"
2. **No Address**: Don't select address → Should show "Vui lòng chọn địa chỉ"
3. **Invalid Phone**: Use invalid phone → Should show "Số điện thoại không hợp lệ"
4. **Valid Cart**: Normal flow → Should proceed to checkout

## Related Files

- `src/pages/checkout/CheckoutPage.tsx` - Add validation here
- `src/lib/hooks/useCheckout.ts` - Checkout hook (no validation)
- `src/lib/hooks/useCart.ts` - Cart hook for refreshCart()
