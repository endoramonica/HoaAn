// VietCommerce.Core/DTOs/Cart/AddToCartDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    /// <summary>
    /// Data transfer object for adding a product to the shopping cart.
    /// Supports both regular products and customizable package products.
    /// </summary>
    public class AddToCartDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the product to add to cart.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product to add.
        /// Defaults to 1 if not specified.
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Gets or sets the list of customizations for package products.
        /// Nullable - only populated when adding customizable package products.
        /// Each customization specifies an option ID and desired quantity.
        /// Example: Customer adds "Mâm Cúng Khai Trương" with 10 dĩa xôi instead of 5.
        /// </summary>
        public List<CartItemCustomizationDto>? Customizations { get; set; }
    }
}

