using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Core.DTOs.Inventory
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityReserved { get; set; }
        public int QuantityActual => QuantityAvailable - QuantityReserved;
        public int ReorderLevel { get; set; }
        public bool IsLowStock => QuantityActual <= ReorderLevel;
        public bool IsActive { get; set; }
    }
}
