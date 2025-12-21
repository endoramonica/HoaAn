using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context) { }

    #region Cart Creation & Retrieval

    public async Task<Cart> GetOrCreateCartByUserIdAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User {userId} not found");

            var store = await _context.Stores.FindAsync(user.StoreId);
            if (store == null)
                throw new KeyNotFoundException($"Store {user.StoreId} not found");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

            if (customer == null)
            {
                customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    StoreId = user.StoreId ?? Guid.Empty,
                    TenantId = store.TenantId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CustomerId == customer.Id && c.IsActive && !c.IsDeleted);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CustomerId = customer.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting or creating cart for user {userId}", ex);
        }
    }

    public async Task<Cart?> GetBySessionIdAsync(string sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty");

            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId && c.IsActive && !c.IsDeleted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting cart by session {sessionId}", ex);
        }
    }

    public async Task<Cart> GetOrCreateGuestCartAsync(string sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty");

            var cart = await GetBySessionIdAsync(sessionId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting or creating guest cart for session {sessionId}", ex);
        }
    }

    public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
    {
        try
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting cart {cartId} with items", ex);
        }
    }

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
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive && !c.IsDeleted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting user {userId} cart with items", ex);
        }
    }


    #endregion

    #region Cart Items Management

    public async Task<CartItem?> GetCartItemByProductIdAsync(Guid cartId, Guid productId)
    {
        try
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId && !ci.IsDeleted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting cart item for product {productId}", ex);
        }
    }

    public async Task<CartItem> AddCartItemAsync(Guid cartId, Guid productId, int quantity, decimal unitPrice)
    {
        try
        {
            var existingItem = await GetCartItemByProductIdAsync(cartId, productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                _context.CartItems.Update(existingItem);
            }
            else
            {
                existingItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false, // Explicitly set to false to prevent soft-delete issues
                    IsActive = true
                };
                await _context.CartItems.AddAsync(existingItem);
            }

            await UpdateCartTimestampAsync(cartId);
            await _context.SaveChangesAsync();

            return existingItem;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error adding item {productId} to cart {cartId}", ex);
        }
    }

    public async Task<CartItem> UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity)
    {
        try
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId)
                ?? throw new KeyNotFoundException($"CartItem {cartItemId} not found");

            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            cartItem.Quantity = newQuantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            _context.CartItems.Update(cartItem);

            await UpdateCartTimestampAsync(cartItem.CartId);
            await _context.SaveChangesAsync();

            return cartItem;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating cart item {cartItemId} quantity", ex);
        }
    }

    public async Task<bool> RemoveCartItemAsync(Guid cartItemId)
    {
        try
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await UpdateCartTimestampAsync(cartItem.CartId);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error removing cart item {cartItemId}", ex);
        }
    }

    public async Task<bool> ClearCartItemsAsync(Guid cartId)
    {
        try
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            if (!cartItems.Any())
                return false;

            _context.CartItems.RemoveRange(cartItems);
            await UpdateCartTimestampAsync(cartId);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error clearing cart {cartId}", ex);
        }
    }

    public async Task<List<CartItem>> GetCartItemsAsync(Guid cartId)
    {
        try
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId && !ci.IsDeleted)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting cart items for cart {cartId}", ex);
        }
    }

    public async Task<bool> CartHasItemsAsync(Guid cartId)
    {
        try
        {
            return await _context.CartItems.AnyAsync(ci => ci.CartId == cartId && !ci.IsDeleted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error checking cart {cartId} items", ex);
        }
    }

    #endregion

    #region Cart Status & Operations

    public async Task<bool> UpdateCartStatusAsync(Guid cartId, bool isActive)
    {
        try
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null)
                return false;

            cart.IsActive = isActive;
            cart.UpdatedAt = DateTime.UtcNow;

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating cart {cartId} status", ex);
        }
    }

    public async Task<bool> SoftDeleteCartAsync(Guid cartId, Guid deletedBy)
    {
        try
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null)
                return false;

            cart.IsDeleted = true;
            cart.DeletedAt = DateTime.UtcNow;
            cart.DeletedBy = deletedBy;
            cart.IsActive = false;

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error soft deleting cart {cartId}", ex);
        }
    }

    public async Task<List<Cart>> GetAbandonedCartsAsync(int daysSinceUpdate = 7)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysSinceUpdate);

            return await _context.Carts
                .Include(c => c.CartItems)
                .Where(c => c.IsActive && !c.IsDeleted && c.UpdatedAt < cutoffDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting abandoned carts", ex);
        }
    }

    #endregion

    #region Customer & Session Management

    //public async Task MergeGuestCartToUserCartAsync(string sessionId, Guid userId)
    //{
    //    try
    //    {
    //        var guestCart = await GetBySessionIdAsync(sessionId);
    //        if (guestCart == null || !guestCart.CartItems.Any())
    //            return;

    //        var userCart = await GetOrCreateCartByUserIdAsync(userId);

    //        foreach (var guestItem in guestCart.CartItems)
    //        {
    //            var existingItem = userCart.CartItems
    //                .FirstOrDefault(ci => ci.ProductId == guestItem.ProductId && !ci.IsDeleted);

    //            if (existingItem != null)
    //            {
    //                // Merge: Cộng dồn quantity
    //                existingItem.Quantity += guestItem.Quantity;
    //                existingItem.UpdatedAt = DateTime.UtcNow;
    //                _context.CartItems.Update(existingItem);
    //            }
    //            else
    //            {
    //                // Add new item
    //                var newItem = new CartItem
    //                {
    //                    Id = Guid.NewGuid(),
    //                    CartId = userCart.Id,
    //                    ProductId = guestItem.ProductId,
    //                    Quantity = guestItem.Quantity,
    //                    CreatedAt = DateTime.UtcNow,
    //                    UpdatedAt = DateTime.UtcNow,
    //                    IsDeleted = false, // Explicitly set to false
    //                    IsActive = true
    //                };
    //                await _context.CartItems.AddAsync(newItem);
    //            }
    //        }

    //        // Soft delete guest cart
    //        guestCart.IsDeleted = true;
    //        guestCart.DeletedAt = DateTime.UtcNow;
    //        guestCart.IsActive = false;
    //        _context.Carts.Update(guestCart);

    //        await _context.SaveChangesAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new InvalidOperationException($"Error merging guest cart (session: {sessionId}) to user cart (user: {userId})", ex);
    //    }
    //}
    public async Task MergeGuestCartToUserCartAsync(string sessionId, Guid userId)
    {
        try
        {
            // ✅ Validate input parameters
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            // ✅ Get guest cart with validation
            var guestCart = await GetBySessionIdAsync(sessionId);
            if (guestCart == null || !guestCart.CartItems.Any())
                return;

            // ✅ Filter and validate guest cart items BEFORE processing
            var validGuestItems = guestCart.CartItems
                .Where(item =>
                    item.ProductId != Guid.Empty &&
                    item.ProductId != null &&
                    item.Quantity > 0 &&
                    !item.IsDeleted)
                .ToList();

            // ✅ If no valid items, just cleanup guest cart and exit
            if (!validGuestItems.Any())
            {
                await SoftDeleteGuestCartAsync(guestCart);
                return;
            }

            // ✅ Get or create user cart with validation
            var userCart = await GetOrCreateCartByUserIdAsync(userId);

            if (userCart == null || userCart.Id == Guid.Empty)
                throw new InvalidOperationException($"Failed to create or retrieve valid cart for user {userId}");

            // ✅ Process valid items with additional safety checks
            foreach (var guestItem in validGuestItems)
            {
                // Double-check ProductId (defensive programming)
                if (guestItem.ProductId == Guid.Empty || guestItem.ProductId == null)
                    continue;

                var existingItem = userCart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == guestItem.ProductId && !ci.IsDeleted);

                if (existingItem != null)
                {
                    // ✅ Merge: Add quantities together
                    existingItem.Quantity += guestItem.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    _context.CartItems.Update(existingItem);
                }
                else
                {
                    // ✅ Create new cart item with full validation
                    var newItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = userCart.Id,
                        ProductId = guestItem.ProductId,
                        Quantity = guestItem.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        IsActive = true
                    };

                    await _context.CartItems.AddAsync(newItem);
                }
            }

            // ✅ Soft delete guest cart after successful merge
            await SoftDeleteGuestCartAsync(guestCart);

            // ✅ Update user cart timestamp
            await UpdateCartTimestampAsync(userCart.Id);

            // ✅ Save all changes in one transaction
            await _context.SaveChangesAsync();
        }
        catch (ArgumentException)
        {
            // Re-throw argument exceptions without wrapping
            throw;
        }
        catch (KeyNotFoundException)
        {
            // Re-throw key not found exceptions without wrapping
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error merging guest cart (session: {sessionId}) to user cart (user: {userId})",
                ex);
        }
    }

    // ✅ Helper method to soft delete guest cart (reusable)
    private async Task SoftDeleteGuestCartAsync(Cart guestCart)
    {
        guestCart.IsDeleted = true;
        guestCart.DeletedAt = DateTime.UtcNow;
        guestCart.IsActive = false;
        _context.Carts.Update(guestCart);
        await _context.SaveChangesAsync();
    }

    public async Task LinkCartToCustomerAsync(Guid cartId, Guid customerId)
    {
        try
        {
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart == null)
                throw new KeyNotFoundException($"Cart {cartId} not found");

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"Customer {customerId} not found");

            cart.CustomerId = customerId;
            cart.UpdatedAt = DateTime.UtcNow;

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error linking cart {cartId} to customer {customerId}", ex);
        }
    }

    #endregion

    #region Helper Methods

    private async Task UpdateCartTimestampAsync(Guid cartId)
    {
        var cart = await _context.Carts.FindAsync(cartId);
        if (cart != null)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
        }
    }

    #endregion
}