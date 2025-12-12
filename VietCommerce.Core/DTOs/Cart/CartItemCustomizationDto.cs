namespace VietCommerce.Core.DTOs.Cart
{
    /// <summary>
    /// Represents a customization option selected for a cart item.
    /// Used when customers customize package products by adjusting quantities of optional items.
    /// </summary>
    public class CartItemCustomizationDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customizable option.
        /// Example: "opt-xoi" for "Xôi gấc đậu xanh"
        /// </summary>
        public string OptionId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the quantity of this option selected by the customer.
        /// Must be within the min/max bounds defined in the product's customizable options.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price of this option at the time of cart addition.
        /// Example: 45,000đ per dĩa of xôi
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the total price for this customization.
        /// Calculated as: Quantity × UnitPrice
        /// Example: 10 dĩa × 45,000đ = 450,000đ
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
