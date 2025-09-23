using System;

namespace VietCommerce.Core.Models
{
    public class InventoryAlert
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public int CurrentQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public InventoryAlertType AlertType { get; set; }
        public DateTime AlertDate { get; set; }
    }
    
    public enum InventoryAlertType
    {
        LOW_STOCK = 1,
        OUT_OF_STOCK = 2,
        OVERSTOCK = 3
    }
}
