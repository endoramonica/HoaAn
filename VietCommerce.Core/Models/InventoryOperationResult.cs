using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Core.Models
{
    public class InventoryOperationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Inventory? Inventory { get; set; }

        public static InventoryOperationResult SuccessResult(Inventory inventory)
        {
            return new InventoryOperationResult
            {
                Success = true,
                Inventory = inventory
            };
        }

        public static InventoryOperationResult FailureResult(string errorMessage)
        {
            return new InventoryOperationResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }

}
