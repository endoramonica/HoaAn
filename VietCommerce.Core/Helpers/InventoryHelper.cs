using System;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Core.Helpers
{
    public static class InventoryHelper
    {
        public static int GetAvailableQuantity(Inventory inventory)
        {
            return inventory.QuantityAvailable - inventory.QuantityReserved;
        }

        public static bool IsLowStock(Inventory inventory)
        {
            return GetAvailableQuantity(inventory) <= inventory.ReorderLevel;
        }

        public static bool CanFulfillOrder(Inventory inventory, int requestedQuantity)
        {
            return GetAvailableQuantity(inventory) >= requestedQuantity;
        }

        public static void RecordMovement(
            Inventory inventory,
            int quantity,
            InventoryMovementType movementType,
            string? notes = null,
            string? referenceType = null,
            int? referenceId = null,
            Guid? createdBy = null  )
        {
            switch (movementType)
            {
                case InventoryMovementType.PURCHASE:
                case InventoryMovementType.RETURN:
                    inventory.QuantityAvailable += quantity;
                    break;

                case InventoryMovementType.SALE:
                    inventory.QuantityAvailable -= quantity;
                    break;

                case InventoryMovementType.ADJUSTMENT:
                    inventory.QuantityAvailable = quantity;
                    break;

                case InventoryMovementType.TRANSFER:
                    inventory.QuantityAvailable -= quantity;
                    break;
            }

            var movement = new InventoryMovement
            {
                InventoryId = inventory.Id,
                MovementType = movementType,
                ChangeAmount = quantity,
                QuantityBefore = inventory.QuantityAvailable, // trước khi thay đổi
                QuantityAfter = inventory.QuantityAvailable,  // sau khi switch-case update xong
                Reason = notes,                               // dùng notes làm lý do
                PerformedById = createdBy,                    // thay vì CreatedBy int, giờ link tới User
                                     // vẫn giữ log audit từ AuditableEntity
                CreatedAt = DateTime.UtcNow
            };

            inventory.InventoryMovements.Add(movement);


            inventory.InventoryMovements.Add(movement);
        }
    }
}
