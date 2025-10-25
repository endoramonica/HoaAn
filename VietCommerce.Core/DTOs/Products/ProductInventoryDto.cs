// ============================================
// FILE: ProductDto.cs
// Mô t?: DTO co b?n cho danh sách s?n ph?m
// ============================================
// ============================================
// FILE: ProductInventoryDto.cs
// Mô t?: DTO cho inventory c?a s?n ph?m
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductInventoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity => Quantity - ReservedQuantity;
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public bool NeedsReorder => AvailableQuantity <= ReorderLevel;
        public DateTime? LastRestocked { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
