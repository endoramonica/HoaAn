using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart> GetOrCreateCartByUserIdAsync(Guid userId);

        Task<Cart?> GetCartWithItemsAsync(Guid cartId);

        Task<Cart?> GetUserCartWithItemsAsync(Guid userId);

        Task<CartItem?> GetCartItemByProductIdAsync(Guid cartId, Guid productId);

        Task<CartItem> AddCartItemAsync(Guid cartId, Guid productId, int quantity, decimal unitPrice);

        Task<CartItem> UpdateCartItemQuantityAsync(Guid cartItemId, int newQuantity);

        Task<bool> RemoveCartItemAsync(Guid cartItemId);

        Task<bool> ClearCartItemsAsync(Guid cartId);

        Task<List<CartItem>> GetCartItemsAsync(Guid cartId);

        Task<bool> UpdateCartStatusAsync(Guid cartId, bool isActive);

        Task<bool> CartHasItemsAsync(Guid cartId);

        Task<List<Cart>> GetAbandonedCartsAsync(int daysSinceUpdate = 7);

        Task<bool> SoftDeleteCartAsync(Guid cartId, Guid deletedBy);
    }
}