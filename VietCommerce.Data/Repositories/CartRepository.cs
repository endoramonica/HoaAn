using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context) { }

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
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive && !c.IsDeleted);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting user {userId} cart with items", ex);
        }
    }

    public async Task<CartItem?> GetCartItemByProductIdAsync(Guid cartId, Guid productId)
    {
        try
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
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
                    UpdatedAt = DateTime.UtcNow
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
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting cart items for cart {cartId}", ex);
        }
    }

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

    public async Task<bool> CartHasItemsAsync(Guid cartId)
    {
        try
        {
            return await _context.CartItems.AnyAsync(ci => ci.CartId == cartId);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error checking cart {cartId} items", ex);
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

    private async Task UpdateCartTimestampAsync(Guid cartId)
    {
        var cart = await _context.Carts.FindAsync(cartId);
        if (cart != null)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
        }
    }
}