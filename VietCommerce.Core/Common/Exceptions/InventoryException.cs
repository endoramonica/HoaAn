using System;
namespace VietCommerce.Core.Common.Exceptions
{
    public class InventoryException : Exception
    {
        public InventoryException(string message) : base(message) { }
        public InventoryException(string message, Exception innerException)
            : base(message, innerException) { }
        public static InventoryException InsufficientStock(int productId, int requested, int available)
        {
            return new InventoryException(
                $"Insufficient stock for product {productId}. Requested: {requested}, Available: {available}");
        }
        public static InventoryException InvalidQuantity(int quantity)
        {
            return new InventoryException($"Invalid quantity: {quantity}");
        }
        public static InventoryException NotFound(int productId, int storeId)
        {
            return new InventoryException($"Inventory not found for product {productId} in store {storeId}");
        }
    }
}
