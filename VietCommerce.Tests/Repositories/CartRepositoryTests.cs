using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Data.Context;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories
{
    public class CartRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly CartRepository _cartRepository;

        public CartRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);
            _cartRepository = new CartRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            var storeId = Guid.NewGuid();

            // ✅ CREATE PRODUCTS FIRST - Compatible với Product entity mới
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            var product1 = new Product
            {
                Id = productId1,
                StoreId = storeId,
                Name = "Test Product 1",
                Code = "TP1",
                Slug = "test-product-1",
                SKU = "SKU001",
                Stock = 100,
                IsActive = true,
                IsDeleted = false,
                ViewCount = 10,
                FavoriteCount = 5,
                PurchaseCount = 2,
                ReviewCount = 3,
                AvgRating = 4.5m,
                TrendingScore = 12.5m,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                CreatedBy = Guid.NewGuid()
            };

            var product2 = new Product
            {
                Id = productId2,
                StoreId = storeId,
                Name = "Test Product 2",
                Code = "TP2",
                Slug = "test-product-2",
                SKU = "SKU002",
                Stock = 50,
                IsActive = true,
                IsDeleted = false,
                ViewCount = 20,
                FavoriteCount = 8,
                PurchaseCount = 5,
                ReviewCount = 7,
                AvgRating = 4.8m,
                TrendingScore = 15.2m,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                CreatedBy = Guid.NewGuid()
            };

            // ✅ CREATE STORE (required for Product.Store navigation)
            var store = new Store
            {
                Id = storeId,
                Name = "Test Store",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };

            // Active cart for user1 WITH cart item
            var activeCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                CustomerId = customerId1,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-1)
            };

            var activeCartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = activeCart.Id,
                ProductId = productId1,
                Quantity = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddHours(-1)
            };

            // Empty active cart for user2
            var emptyCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId2,
                CustomerId = customerId2,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            // Inactive cart for user1
            var inactiveCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                CustomerId = customerId1,
                IsActive = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            };

            // Deleted cart for user1
            var deletedCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId1,
                CustomerId = customerId1,
                IsActive = true,
                IsDeleted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            };

            // ✅ ADD ALL DATA
            _context.Stores.Add(store);
            _context.Products.AddRange(product1, product2);
            _context.Carts.AddRange(activeCart, emptyCart, inactiveCart, deletedCart);
            _context.CartItems.Add(activeCartItem);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region GetOrCreateCartByUserIdAsync Tests

        [Fact]
        public async Task GetOrCreateCartByUserIdAsync_ExistingActiveCart_ReturnsCartWithItems()
        {
            // Arrange
            var userId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).UserId;
            var customerId = Guid.NewGuid();

            // Act
            var result = await _cartRepository.GetOrCreateCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.True(result.IsActive);
            Assert.False(result.IsDeleted);
            Assert.Single(result.CartItems);
            Assert.Equal(2, result.CartItems.First().Quantity);
            Assert.NotNull(result.CartItems.First().Product); // ✅ Product loaded
        }

        [Fact]
        public async Task GetOrCreateCartByUserIdAsync_NoActiveCart_CreatesNewCart()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            // Act
            var result = await _cartRepository.GetOrCreateCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(customerId, result.CustomerId);
            Assert.True(result.IsActive);
            Assert.False(result.IsDeleted);
            Assert.Empty(result.CartItems);

            // Verify saved to DB
            var savedCart = await _context.Carts.FindAsync(result.Id);
            Assert.NotNull(savedCart);
        }

        [Fact]
        public async Task GetOrCreateCartByUserIdAsync_InactiveCartIgnored_ReturnsNewCart()
        {
            // Arrange
            var userId = _context.Carts.First(c => !c.IsActive).UserId;
            var customerId = Guid.NewGuid();

            // Act
            var result = await _cartRepository.GetOrCreateCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.CartItems); // New cart has no items
        }

        [Fact]
        public async Task GetOrCreateCartByUserIdAsync_DeletedCartIgnored_ReturnsNewCart()
        {
            // Arrange
            var userId = _context.Carts.First(c => c.IsDeleted).UserId;
            var customerId = Guid.NewGuid();

            // Act
            var result = await _cartRepository.GetOrCreateCartByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.CartItems);
        }

        #endregion

        #region GetCartWithItemsAsync Tests

        [Fact]
        public async Task GetCartWithItemsAsync_ValidCartId_ReturnsCartWithItems()
        {
            // Arrange
            var cartId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).Id;

            // Act
            var result = await _cartRepository.GetCartWithItemsAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartId, result.Id);
            Assert.Single(result.CartItems);
            Assert.NotNull(result.CartItems.First().Product);
        }

        [Fact]
        public async Task GetCartWithItemsAsync_DeletedCart_ReturnsNull()
        {
            // Arrange
            var cartId = _context.Carts.First(c => c.IsDeleted).Id;

            // Act
            var result = await _cartRepository.GetCartWithItemsAsync(cartId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCartWithItemsAsync_InvalidCartId_ReturnsNull()
        {
            // Arrange
            var invalidCartId = Guid.NewGuid();

            // Act
            var result = await _cartRepository.GetCartWithItemsAsync(invalidCartId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetUserCartWithItemsAsync Tests

        [Fact]
        public async Task GetUserCartWithItemsAsync_ValidUserWithActiveCart_ReturnsCart()
        {
            // Arrange
            var userId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).UserId;

            // Act
            var result = await _cartRepository.GetUserCartWithItemsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Single(result.CartItems);
            Assert.NotNull(result.CartItems.First().Product);
        }

        [Fact]
        public async Task GetUserCartWithItemsAsync_UserWithNoActiveCart_ReturnsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _cartRepository.GetUserCartWithItemsAsync(userId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region CartItem Operations Tests

        [Fact]
        public async Task AddCartItemAsync_NewItem_AddsItemAndUpdatesCartTimestamp()
        {
            // Arrange
            var cartId = _context.Carts.First(c => !c.CartItems.Any()).Id;
            var productId = _context.Products.First().Id;
            var quantity = 3;

            // Act
            var result = await _cartRepository.AddCartItemAsync(cartId, productId, quantity, 150m);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartId, result.CartId);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(quantity, result.Quantity);
            Assert.NotNull(result.Product);

            // Verify cart timestamp updated
            var cart = await _context.Carts.FindAsync(cartId);
            Assert.True(cart.UpdatedAt >= result.UpdatedAt);

            // Verify saved
            _context.Entry(result).Reload();
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddCartItemAsync_ExistingItem_IncrementsQuantity()
        {
            // Arrange
            var cartId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).Id;
            var existingProductId = _context.CartItems.First().ProductId;
            var addQuantity = 2;
            var originalQuantity = _context.CartItems.First().Quantity;

            // Act
            var result = await _cartRepository.AddCartItemAsync(cartId, existingProductId, addQuantity, 100m);

            // Assert
            Assert.Equal(originalQuantity + addQuantity, result.Quantity); // 2 + 2 = 4
            Assert.NotNull(result.Product);
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_ValidItem_UpdatesQuantity()
        {
            // Arrange
            var cartItemId = _context.CartItems.First().Id;
            var newQuantity = 5;

            // Act
            var result = await _cartRepository.UpdateCartItemQuantityAsync(cartItemId, newQuantity);

            // Assert
            Assert.Equal(newQuantity, result.Quantity);
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_ZeroQuantity_ThrowsArgumentException()
        {
            var cartItemId = _context.CartItems.First().Id;
            await Assert.ThrowsAsync<ArgumentException>(
                () => _cartRepository.UpdateCartItemQuantityAsync(cartItemId, 0));
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_NegativeQuantity_ThrowsArgumentException()
        {
            var cartItemId = _context.CartItems.First().Id;
            await Assert.ThrowsAsync<ArgumentException>(
                () => _cartRepository.UpdateCartItemQuantityAsync(cartItemId, -1));
        }

        [Fact]
        public async Task UpdateCartItemQuantityAsync_InvalidItemId_ThrowsKeyNotFoundException()
        {
            var invalidItemId = Guid.NewGuid();
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _cartRepository.UpdateCartItemQuantityAsync(invalidItemId, 5));
        }

        [Fact]
        public async Task RemoveCartItemAsync_ValidItem_RemovesItemAndUpdatesCartTimestamp()
        {
            // Arrange
            var cartItemId = _context.CartItems.First().Id;
            var cartId = _context.CartItems.First().CartId;

            // Act
            var result = await _cartRepository.RemoveCartItemAsync(cartItemId);

            // Assert
            Assert.True(result);

            // Verify item removed
            var removedItem = await _context.CartItems.FindAsync(cartItemId);
            Assert.Null(removedItem);

            // Verify cart timestamp updated
            var cart = await _context.Carts.FindAsync(cartId);
            Assert.NotNull(cart);
            Assert.True(cart.UpdatedAt >= DateTime.UtcNow.AddSeconds(-2));
        }

        [Fact]
        public async Task RemoveCartItemAsync_InvalidItemId_ReturnsFalse()
        {
            var invalidItemId = Guid.NewGuid();
            var result = await _cartRepository.RemoveCartItemAsync(invalidItemId);
            Assert.False(result);
        }

        [Fact]
        public async Task ClearCartItemsAsync_CartWithItems_ClearsAllItems()
        {
            var cartId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).Id;
            var result = await _cartRepository.ClearCartItemsAsync(cartId);
            Assert.True(result);
            Assert.Empty(await _context.CartItems.Where(ci => ci.CartId == cartId).ToListAsync());
        }

        [Fact]
        public async Task ClearCartItemsAsync_EmptyCart_ReturnsFalse()
        {
            var cartId = _context.Carts.First(c => !c.CartItems.Any()).Id;
            var result = await _cartRepository.ClearCartItemsAsync(cartId);
            Assert.False(result);
        }

        [Fact]
        public async Task GetCartItemsAsync_ValidCartId_ReturnsItemsWithProducts()
        {
            // Arrange
            var cartId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).Id;

            // Act
            var result = await _cartRepository.GetCartItemsAsync(cartId);

            // Assert
            Assert.Single(result);
            Assert.NotNull(result.First().Product);
            Assert.Equal("Test Product 1", result.First().Product.Name);
        }

        #endregion

        #region Utility Methods Tests

        [Fact]
        public async Task CartHasItemsAsync_CartWithItems_ReturnsTrue()
        {
            var cartId = _context.Carts.Include(c => c.CartItems).First(c => c.CartItems.Any()).Id;
            var result = await _cartRepository.CartHasItemsAsync(cartId);
            Assert.True(result);
        }

        [Fact]
        public async Task CartHasItemsAsync_EmptyCart_ReturnsFalse()
        {
            var cartId = _context.Carts.First(c => !c.CartItems.Any()).Id;
            var result = await _cartRepository.CartHasItemsAsync(cartId);
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateCartStatusAsync_ValidCart_UpdatesStatus()
        {
            var cartId = _context.Carts.First().Id;
            var originalStatus = _context.Carts.First(c => c.Id == cartId).IsActive;
            var result = await _cartRepository.UpdateCartStatusAsync(cartId, !originalStatus);
            Assert.True(result);
            var updatedCart = await _context.Carts.FindAsync(cartId);
            Assert.Equal(!originalStatus, updatedCart.IsActive);
        }

        [Fact]
        public async Task SoftDeleteCartAsync_ValidCart_SoftDeletesCart()
        {
            var cartId = _context.Carts.First().Id;
            var deletedBy = Guid.NewGuid();
            var result = await _cartRepository.SoftDeleteCartAsync(cartId, deletedBy);
            Assert.True(result);
            var deletedCart = await _context.Carts.FindAsync(cartId);
            Assert.True(deletedCart.IsDeleted);
            Assert.False(deletedCart.IsActive);
            Assert.Equal(deletedBy, deletedCart.DeletedBy);
        }

        [Fact]
        public async Task GetAbandonedCartsAsync_ReturnsCartsNotUpdatedRecently()
        {
            // Create abandoned cart
            var abandonedCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            };
            _context.Carts.Add(abandonedCart);
            await _context.SaveChangesAsync();

            var result = await _cartRepository.GetAbandonedCartsAsync(daysSinceUpdate: 7);
            Assert.Contains(result, c => c.Id == abandonedCart.Id);
        }

        #endregion

        #region Full Workflow Test - FIXED

        [Fact]
        public async Task FullCartWorkflow_CreateAddUpdateRemove()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var productId1 = _context.Products.First().Id;
            var productId2 = _context.Products.Skip(1).First().Id;

            // 1. Create cart
            var cart = await _cartRepository.GetOrCreateCartByUserIdAsync(userId);
            Assert.NotNull(cart);
            Assert.Empty(cart.CartItems);

            // 2. Add first item
            var item1 = await _cartRepository.AddCartItemAsync(cart.Id, productId1, 3, 100m);
            Assert.Equal(3, item1.Quantity);

            // 3. Add second item
            var item2 = await _cartRepository.AddCartItemAsync(cart.Id, productId2, 2, 200m);
            Assert.Equal(2, item2.Quantity);

            // 4. Update first item quantity
            var updatedItem1 = await _cartRepository.UpdateCartItemQuantityAsync(item1.Id, 5);
            Assert.Equal(5, updatedItem1.Quantity);

            // 5. Verify cart has 2 items
            var cartItems = await _cartRepository.GetCartItemsAsync(cart.Id);
            Assert.Equal(2, cartItems.Count);
            Assert.Equal(5, cartItems.First(ci => ci.ProductId == productId1).Quantity);
            Assert.Equal(2, cartItems.First(ci => ci.ProductId == productId2).Quantity);

            // 6. Remove second item
            var removed = await _cartRepository.RemoveCartItemAsync(item2.Id);
            Assert.True(removed);

            // 7. Verify only 1 item remains
            cartItems = await _cartRepository.GetCartItemsAsync(cart.Id);
            Assert.Single(cartItems);
            Assert.Equal(productId1, cartItems.First().ProductId);

            // 8. Clear cart
            var cleared = await _cartRepository.ClearCartItemsAsync(cart.Id);
            Assert.True(cleared);
            Assert.False(await _cartRepository.CartHasItemsAsync(cart.Id));
        }

        #endregion
    }
}