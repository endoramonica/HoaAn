// VietCommerce.Core/DTOs/Cart/CartItemDetailDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class CartItemDetailDto
    {

        /// ID của cart item

        public Guid CartItemId { get; set; }


        /// ID của sản phẩm

        public Guid ProductId { get; set; }


        /// Tên sản phẩm

        public string ProductName { get; set; } = string.Empty;


        /// Slug của sản phẩm (để tạo URL)

        public string ProductSlug { get; set; } = string.Empty;


        /// SKU - Mã hàng tồn kho

        public string SKU { get; set; } = string.Empty;


        /// URL hình ảnh sản phẩm (primary image)

        public string? ProductImage { get; set; }


        /// Đơn giá sản phẩm (giá tại thời điểm thêm vào giỏ)

        public decimal UnitPrice { get; set; }


        /// Số lượng trong giỏ

        public int Quantity { get; set; }


        /// Tổng tiền của item này (UnitPrice * Quantity)

        public decimal TotalPrice { get; set; }


        /// Số lượng tồn kho hiện tại

        public int AvailableStock { get; set; }


        /// Sản phẩm còn hoạt động hay không

        public bool IsProductActive { get; set; }


        /// Số lần sản phẩm đã được bán

        public int PurchaseCount { get; set; }


        /// Điểm đánh giá trung bình (1-5)

        public decimal AvgRating { get; set; }


        /// Số lượng review

        public int ReviewCount { get; set; }


        /// Thời gian thêm vào giỏ

        public DateTime CreatedAt { get; set; }


        /// Thời gian cập nhật cuối cùng

        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the base price of the package product before customizations.
        /// This is the fixed price of the package itself.
        /// Example: 3,500,000đ for a ritual package
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the total price of all customizations applied to this cart item.
        /// Calculated as the sum of all customization option prices.
        /// Example: 785,000đ (sum of customization surcharges)
        /// </summary>
        public decimal CustomizationPrice { get; set; }

        /// <summary>
        /// Gets or sets the final total price for this cart item.
        /// Calculated as: BasePrice + CustomizationPrice
        /// Example: 4,285,000đ (3,500,000 + 785,000)
        /// </summary>
        public decimal FinalPrice { get; set; }

        /// <summary>
        /// Gets or sets the list of customizations applied to this cart item.
        /// Contains details about which options were customized and their quantities.
        /// Null if the product is not customizable or no customizations were applied.
        /// </summary>
        public List<CartItemCustomizationDto>? Customizations { get; set; }
    }
}



