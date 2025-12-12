using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Products
{
    /// <summary>
    /// DTO representing a customizable option within a package product.
    /// Customizable options allow customers to modify quantities of specific items in a package.
    /// </summary>
    public class CustomizableOptionDto
    {
        /// <summary>
        /// Unique identifier for the customizable option (e.g., "opt-xoi", "opt-che").
        /// </summary>
        [Required(ErrorMessage = "Option ID is required")]
        [StringLength(100, ErrorMessage = "Option ID cannot exceed 100 characters")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the customizable option (e.g., "Xôi gấc đậu xanh", "Chè trôi nước").
        /// </summary>
        [Required(ErrorMessage = "Option name is required")]
        [StringLength(200, ErrorMessage = "Option name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Default quantity included in the package for this option.
        /// </summary>
        [Required(ErrorMessage = "Base quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Base quantity must be greater than 0")]
        public int BaseQuantity { get; set; }

        /// <summary>
        /// Price per unit of this option (e.g., 45,000đ per dĩa of xôi).
        /// </summary>
        [Required(ErrorMessage = "Unit price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be greater than or equal to 0")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Minimum quantity a customer can order for this option.
        /// </summary>
        [Required(ErrorMessage = "Minimum quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum quantity must be greater than 0")]
        public int MinQuantity { get; set; }

        /// <summary>
        /// Maximum quantity a customer can order for this option. Null means unlimited.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Maximum quantity must be greater than 0")]
        public int? MaxQuantity { get; set; }

        /// <summary>
        /// Unit of measurement for this option (e.g., "dĩa", "chén", "bộ").
        /// </summary>
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string Unit { get; set; } = string.Empty;
    }
}
