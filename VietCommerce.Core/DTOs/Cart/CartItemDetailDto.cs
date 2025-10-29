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
    }
}



