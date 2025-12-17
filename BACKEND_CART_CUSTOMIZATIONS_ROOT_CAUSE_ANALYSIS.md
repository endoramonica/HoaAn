# 🔴 ROOT CAUSE ANALYSIS: Cart Items with Customizations Not Saved to Database

## 🎯 Problem Summary
- **Frontend gửi**: Cart response hiển thị `itemCount: 1` với customizations đầy đủ
- **Backend nhận**: Dữ liệu đúng, nhưng khi checkout → database trống (0 items)
- **Mismatch**: GET /api/v1/Cart → 1 item | POST /api/v1/Checkout/process → 0 items

---

## 🔍 ROOT CAUSE FOUND

### Issue 1: `GetCartWithItemsAsync()` KHÔNG JOIN CartItemCustomizations
**File**: `VietCommerce.Data/Repositories/CartRepository.cs` (Line 126-135)

```csharp
public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
{
    try
    {
        return await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)  // ❌ MISSING: CartItemCustomizations
            .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error getting cart {cartId} with items", ex);
    }
}
```

**Problem**: 
- Chỉ load `CartItems` và `Product`
- **KHÔNG** load `CartItemCustomizations` 
- Khi checkout, customizations data bị mất

---

### Issue 2: CheckoutService Gọi Sai Method
**File**: `VietCommerce.Application/Services/Services/CheckoutService.cs` (Line 216)

```csharp
LogInfo("📦 [Checkout] Retrieving cart with items...");
var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(dto.CartId);  // ❌ WRONG METHOD
```

**Problem**:
- Gọi `GetCartWithItemsAsync()` thay vì `GetUserCartWithItemsAsync()`
- `GetCartWithItemsAsync()` không load customizations
- Dữ liệu customizations bị mất trong checkout process

---

### Issue 3: `GetUserCartWithItemsAsync()` Cũng Thiếu CartItemCustomizations
**File**: `VietCommerce.Data/Repositories/CartRepository.cs` (Line 137-150)

```csharp
public async Task<Cart?> GetUserCartWithItemsAsync(Guid userId)
{
    try
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Prices)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
            // ❌ MISSING: .Include(c => c.CartItems).ThenInclude(ci => ci.CartItemCustomizations)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive && !c.IsDeleted);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error getting user {userId} cart with items", ex);
    }
}
```

---

## 📊 Data Flow Analysis

### ✅ AddToCart (Working Correctly)
```
Frontend → AddToCartDto (with customizations)
    ↓
CartService.AddToCartAsync()
    ↓
cartRepository.AddCartItemWithCustomizationsAsync()
    ↓
CartItem created with:
  - BasePrice ✅
  - CustomizationPrice ✅
  - FinalPrice ✅
  - CustomizationsJson ✅
    ↓
Database: CartItems table ✅ (customizations saved as JSON)
```

### ❌ Checkout (BROKEN)
```
Frontend → CheckoutDto (with cartId)
    ↓
CheckoutService.CheckoutAsync()
    ↓
cart = await _unitOfWork.Carts.GetCartWithItemsAsync(dto.CartId)  // ❌ WRONG
    ↓
CartItems loaded WITHOUT customizations
    ↓
OrderItems created WITHOUT customization data
    ↓
Database: OrderItems table ❌ (customizations lost)
```

---

## 🔧 Solution Required

### Fix 1: Update `GetCartWithItemsAsync()` to Include CartItemCustomizations
```csharp
public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
{
    try
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.CartItemCustomizations)  // ✅ ADD THIS
            .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error getting cart {cartId} with items", ex);
    }
}
```

### Fix 2: Update `GetUserCartWithItemsAsync()` to Include CartItemCustomizations
```csharp
public async Task<Cart?> GetUserCartWithItemsAsync(Guid userId)
{
    try
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Prices)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.CartItemCustomizations)  // ✅ ADD THIS
            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive && !c.IsDeleted);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Error getting user {userId} cart with items", ex);
    }
}
```

### Fix 3: Ensure OrderService Properly Maps Customizations
**File**: `VietCommerce.Application/Services/Services/OrderService.cs`

Verify that `CreateOrderItemFromCartItemAsync()` method:
1. Reads `CartItem.CustomizationsJson` ✅
2. Reads `CartItem.CartItemCustomizations` collection ✅
3. Saves customizations to `OrderItemCustomizations` table ✅

---

## 📋 Verification Checklist

After applying fixes, verify:

- [ ] `GetCartWithItemsAsync()` includes `.Include(c => c.CartItems).ThenInclude(ci => ci.CartItemCustomizations)`
- [ ] `GetUserCartWithItemsAsync()` includes `.Include(c => c.CartItems).ThenInclude(ci => ci.CartItemCustomizations)`
- [ ] CheckoutService loads cart items WITH customizations
- [ ] OrderService properly maps customizations from CartItem to OrderItem
- [ ] Database: OrderItemCustomizations table has records after checkout
- [ ] Test: Add item with customizations → Checkout → Verify OrderItemCustomizations saved

---

## 🧪 Test Case

```
1. Add product with customizations to cart
   - Frontend sends: customizations array
   - Backend saves: CartItem + CartItemCustomizations ✅

2. GET /api/v1/Cart
   - Response shows: itemCount: 1, customizations: [...] ✅

3. POST /api/v1/Checkout/process
   - Backend loads: cart.CartItems (WITH customizations)
   - Creates: OrderItems + OrderItemCustomizations
   - Database: OrderItemCustomizations has records ✅

4. GET /api/v1/Orders/{orderId}
   - Response shows: order items with customizations ✅
```

---

## 📌 Summary

| Component | Issue | Fix |
|-----------|-------|-----|
| `GetCartWithItemsAsync()` | Missing CartItemCustomizations join | Add `.Include(c => c.CartItems).ThenInclude(ci => ci.CartItemCustomizations)` |
| `GetUserCartWithItemsAsync()` | Missing CartItemCustomizations join | Add `.Include(c => c.CartItems).ThenInclude(ci => ci.CartItemCustomizations)` |
| `CheckoutService` | Loads cart without customizations | Customizations will be loaded after repository fix |
| `OrderService` | Needs verification | Ensure it maps customizations correctly |

