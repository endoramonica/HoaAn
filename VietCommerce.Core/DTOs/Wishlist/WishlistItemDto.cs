// ============================================
// FILE 1: DTOs - WishlistItemDto.cs
// Path: VietCommerce.Core/DTOs/Wishlist/WishlistItemDto.cs
// ============================================

using System;

namespace VietCommerce.Core.DTOs.Wishlist
{
    public class WishlistItemDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public ProductInWishlistDto Product { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class ProductInWishlistDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public int FavoriteCount { get; set; }
    }
}

