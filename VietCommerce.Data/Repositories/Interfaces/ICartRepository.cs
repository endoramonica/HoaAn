using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        // Cart Creation & Retrieval
        Task<Cart> GetOrCreateCartByUserIdAsync(Guid userId);
        Task<Cart?> GetBySessionIdAsync(string sessionId);
        Task<Cart> GetOrCreateGuestCartAsync(string sessionId);
        Task<Cart?> GetCartWithItemsAsync(Guid cartId);
        Task<Cart?> GetUserCartWithItemsAsync(Guid userId);

        // Cart Items Management
        Task<CartItem?> GetCartItemByProductIdAsync(Guid cartId, Guid productId);
        Task<CartItem> AddCartItemAsync(Guid cartId, Guid productId, int quantity, decimal unitPrice);
        Task<CartItem> UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity);
        Task<bool> RemoveCartItemAsync(Guid cartItemId);
        Task<bool> ClearCartItemsAsync(Guid cartId);
        Task<List<CartItem>> GetCartItemsAsync(Guid cartId);
        Task<bool> CartHasItemsAsync(Guid cartId);

        // Cart Status & Operations
        Task<bool> UpdateCartStatusAsync(Guid cartId, bool isActive);
        Task<bool> SoftDeleteCartAsync(Guid cartId, Guid deletedBy);
        Task<List<Cart>> GetAbandonedCartsAsync(int daysSinceUpdate = 7);

        // Customer & Session Management
        Task MergeGuestCartToUserCartAsync(string sessionId, Guid userId);
        Task LinkCartToCustomerAsync(Guid cartId, Guid customerId);
    }
}
