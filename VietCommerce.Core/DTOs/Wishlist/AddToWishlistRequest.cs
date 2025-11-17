// ============================================
// FILE 1: DTOs - AddToWishlistRequest.cs
// Path: VietCommerce.Core/DTOs/Wishlist/AddToWishlistRequest.cs
// ============================================

namespace VietCommerce.Core.DTOs.Wishlist
{
    public class AddToWishlistRequest
    {
        public Guid ProductId { get; set; }
    }
    public class IsInWishlistResponse
    {
        public bool IsInWishlist { get; set; }
    }

    public class ToggleWishlistResponse
    {
        public bool IsInWishlist { get; set; }
    }
}
