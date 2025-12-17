# Debug Cart Items Issue

## Symptoms
- Log shows: "Cart items loaded: 0"
- But cart was found successfully
- CartId is correct

## Root Cause Analysis

### Possible Causes:
1. **CartItems bị soft delete** - Đã sửa với filter `.Where(ci => !ci.IsDeleted)`
2. **CartItems không được lưu vào DB** - Cần kiểm tra AddCartItemAsync
3. **Global query filter** - Có thể có query filter trên CartItem
4. **Lazy loading disabled** - CartItems không được load tự động

## Solution Applied

### Fix 1: Filter deleted items in GetCartWithItemsAsync
```csharp
var cart = await _context.Carts
    .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
    .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);

if (cart != null)
{
    // Filter out deleted items
    cart.CartItems = cart.CartItems.Where(ci => !ci.IsDeleted).ToList();
}
```

### Fix 2: Same for GetUserCartWithItemsAsync

## Next Steps to Debug

1. **Check if CartItems are being saved**:
   - Add logging in AddCartItemAsync
   - Verify SaveChangesAsync is called

2. **Check if CartItems are being loaded**:
   - Add logging in GetCartWithItemsAsync
   - Log cart.CartItems.Count before and after filter

3. **Check for global query filters**:
   - Search for HasQueryFilter on CartItem
   - Check AppDbContext configuration

## Testing

Run checkout flow and check logs:
- "Cart items loaded: X" should show > 0
- If still 0, check database directly for CartItems with matching CartId
