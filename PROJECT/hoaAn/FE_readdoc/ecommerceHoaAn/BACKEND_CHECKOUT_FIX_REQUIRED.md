# 🔴 BACKEND CHECKOUT FIX REQUIRED

## I. CRITICAL ISSUES TO FIX

### Issue #1: Cart Not Closed After Checkout
**Severity:** CRITICAL  
**Impact:** Cart remains ACTIVE, can be reused, booking state sticks to new cart

**Current Behavior:**
```
POST /Checkout/process
Response:
{
  "orderId": "0b539155-8744-4ed1-a48a-93f79e510c6f",
  "totalAmount": 30000.000
}

GET /Cart/summary (after checkout)
Response:
{
  "status": "ACTIVE",  // ❌ Should be CHECKED_OUT or COMPLETED
  "itemCount": 1,
  "subTotal": 1100000.00
}
```

**Required Fix:**
In `CheckoutService.Process()` method, after creating order:

```csharp
public async Task<CheckoutResponseDto> Process(CheckoutDto checkoutData)
{
    // ... existing code ...
    
    // Create order from cart
    var order = await CreateOrderFromCart(cart, checkoutData);
    
    // ✅ FIX: Close the cart after checkout
    cart.Status = CartStatus.CHECKED_OUT;  // or COMPLETED
    cart.Items.Clear();  // or archive items
    await _cartRepository.SaveAsync(cart);
    
    // ✅ Log for verification
    _logger.LogInformation($"[Checkout] Cart {cart.Id} closed after checkout");
    
    return new CheckoutResponseDto { ... };
}
```

**Verification:**
```
After fix:
GET /Cart/summary (after checkout)
Response:
{
  "status": "CHECKED_OUT",  // ✅ Correct
  "itemCount": 0,           // ✅ Cleared
  "subTotal": 0.00
}
```

---

### Issue #2: Order Items Price Missing
**Severity:** CRITICAL  
**Impact:** Order shows 0 price, only shipping fee visible

**Current Behavior:**
```
Cart Item:
{
  "unitPrice": 1100000.00,
  "quantity": 1,
  "totalPrice": 1100000.00
}

Order Item (after checkout):
{
  "unitPrice": 0.00,  // ❌ Lost price
  "quantity": 1,
  "totalPrice": 0.00
}

Order Summary:
{
  "subTotal": 0.00,  // ❌ Calculated from items
  "shippingFee": 30000.00,
  "totalAmount": 30000.00  // ❌ Only shipping
}
```

**Required Fix:**
In `CheckoutService.CreateOrderFromCart()` or similar method:

```csharp
private async Task<Order> CreateOrderFromCart(Cart cart, CheckoutDto checkoutData)
{
    var order = new Order
    {
        OrderId = Guid.NewGuid(),
        CartId = cart.Id,
        CustomerId = cart.UserId,
        Status = OrderStatus.Pending,
        // ... other fields ...
    };
    
    // ✅ FIX: Copy items with correct prices
    foreach (var cartItem in cart.Items)
    {
        var orderItem = new OrderItem
        {
            OrderId = order.OrderId,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.ProductName,
            ProductSKU = cartItem.SKU,
            
            // ✅ CRITICAL: Copy unitPrice from cart item
            UnitPrice = cartItem.UnitPrice,  // NOT 0!
            
            Quantity = cartItem.Quantity,
            
            // ✅ Calculate totalPrice correctly
            TotalPrice = cartItem.UnitPrice * cartItem.Quantity,
            
            Type = "product"
        };
        
        order.Items.Add(orderItem);
    }
    
    // ✅ Calculate order totals correctly
    order.SubTotal = order.Items.Sum(i => i.TotalPrice);
    order.TaxAmount = 0;  // or calculate tax
    order.DiscountAmount = checkoutData.CouponCode != null ? GetCouponDiscount(...) : 0;
    order.ShippingFee = GetShippingFee(checkoutData.ShippingInfo);
    order.TotalAmount = order.SubTotal + order.ShippingFee - order.DiscountAmount;
    
    // ✅ Log for verification
    _logger.LogInformation($"[Checkout] Order created with SubTotal={order.SubTotal}, ShippingFee={order.ShippingFee}, Total={order.TotalAmount}");
    
    await _orderRepository.SaveAsync(order);
    return order;
}
```

**Verification:**
```
After fix:
POST /Checkout/process
Response:
{
  "orderId": "0b539155-8744-4ed1-a48a-93f79e510c6f",
  "items": [
    {
      "unitPrice": 1100000.00,  // ✅ Correct
      "quantity": 1,
      "totalPrice": 1100000.00
    }
  ],
  "subTotal": 1100000.00,  // ✅ Correct
  "shippingFee": 30000.00,
  "totalAmount": 1130000.00  // ✅ Correct
}
```

---

### Issue #3: Cart Status Enum Missing
**Severity:** HIGH  
**Impact:** Cannot set cart status to CHECKED_OUT

**Required Fix:**
Ensure `CartStatus` enum has these values:

```csharp
public enum CartStatus
{
    ACTIVE = 0,
    CHECKED_OUT = 1,  // ✅ Add this
    COMPLETED = 2,    // ✅ Add this
    ABANDONED = 3,
    ARCHIVED = 4
}
```

---

## II. IMPLEMENTATION CHECKLIST

### Backend Changes Required:

- [ ] **CartStatus Enum**
  - [ ] Add `CHECKED_OUT` status
  - [ ] Add `COMPLETED` status
  - [ ] Update database migrations if needed

- [ ] **CheckoutService.Process() Method**
  - [ ] After creating order, set `cart.Status = CartStatus.CHECKED_OUT`
  - [ ] Clear `cart.Items` or mark as archived
  - [ ] Save cart to database
  - [ ] Add logging for verification

- [ ] **Order Item Mapping**
  - [ ] Copy `UnitPrice` from cart item to order item
  - [ ] Calculate `TotalPrice = UnitPrice * Quantity`
  - [ ] Verify `Order.SubTotal` calculation
  - [ ] Verify `Order.TotalAmount` calculation

- [ ] **Logging & Monitoring**
  - [ ] Log cart status change
  - [ ] Log order creation with prices
  - [ ] Log any price discrepancies

- [ ] **Testing**
  - [ ] Test checkout with single item
  - [ ] Test checkout with multiple items
  - [ ] Verify cart status after checkout
  - [ ] Verify order prices match cart prices
  - [ ] Verify cart cannot be reused after checkout

---

## III. TESTING SCENARIOS

### Scenario 1: Single Item Checkout
```
1. Add product (price: 1,100,000₫) to cart
2. Checkout with COD
3. Verify:
   - Order.SubTotal = 1,100,000₫ ✅
   - Order.ShippingFee = 30,000₫ ✅
   - Order.TotalAmount = 1,130,000₫ ✅
   - Cart.Status = CHECKED_OUT ✅
   - Cart.Items.Count = 0 ✅
```

### Scenario 2: Multiple Items Checkout
```
1. Add product 1 (price: 500,000₫) to cart
2. Add product 2 (price: 600,000₫) to cart
3. Checkout with COD
4. Verify:
   - Order.SubTotal = 1,100,000₫ ✅
   - Order.Items.Count = 2 ✅
   - Order.Items[0].UnitPrice = 500,000₫ ✅
   - Order.Items[1].UnitPrice = 600,000₫ ✅
   - Cart.Status = CHECKED_OUT ✅
```

### Scenario 3: Cart Cannot Be Reused
```
1. Checkout order 1 (cart A)
2. Verify cart A status = CHECKED_OUT
3. Try to add item to cart A
4. Verify: Error or new cart created ✅
```

---

## IV. API RESPONSE VERIFICATION

### Before Fix (BROKEN):
```json
{
  "success": true,
  "data": {
    "orderId": "0b539155-8744-4ed1-a48a-93f79e510c6f",
    "orderNumber": "251217144200227SDP",
    "status": "pending",
    "subTotal": 0.00,
    "shippingFee": 30000.00,
    "totalAmount": 30000.00,
    "items": [
      {
        "unitPrice": 0.00,
        "quantity": 1,
        "totalPrice": 0.00
      }
    ]
  }
}
```

### After Fix (CORRECT):
```json
{
  "success": true,
  "data": {
    "orderId": "0b539155-8744-4ed1-a48a-93f79e510c6f",
    "orderNumber": "251217144200227SDP",
    "status": "pending",
    "subTotal": 1100000.00,
    "shippingFee": 30000.00,
    "totalAmount": 1130000.00,
    "items": [
      {
        "unitPrice": 1100000.00,
        "quantity": 1,
        "totalPrice": 1100000.00
      }
    ]
  }
}
```

---

## V. RELATED FRONTEND FIXES

Frontend has been updated to clear booking state:
- ✅ `src/pages/checkout/CheckoutPage.tsx` - Clear booking state before navigate
- ✅ `src/pages/checkout/OrderSuccessPage.tsx` - Backup clear booking state

These frontend fixes will work correctly once backend closes cart properly.

