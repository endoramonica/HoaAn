using System;
using VietCommerce.Core.Enums.Products;
using static VietCommerce.Core.Common.Constants.InventoryConstants;

namespace VietCommerce.Core.Mappers
{
    public static class MovementTypeMapper
    {
        /// <summary>
        /// Enum -> String
        /// </summary>
        public static string ToStringValue(this InventoryMovementType movementType)
        {
            return movementType switch
            {
                InventoryMovementType.PURCHASE => MovementTypes.PURCHASE,
                InventoryMovementType.SALE => MovementTypes.SALE,
                InventoryMovementType.ADJUSTMENT => MovementTypes.ADJUSTMENT,
                InventoryMovementType.RETURN => MovementTypes.RETURN,
                InventoryMovementType.TRANSFER => MovementTypes.TRANSFER,
                _ => throw new ArgumentOutOfRangeException(nameof(movementType), movementType, null)
            };
        }

        /// <summary>
        /// String -> Enum
        /// </summary>
        public static InventoryMovementType ToEnum(string movementType)
        {
            return movementType?.ToUpperInvariant() switch
            {
                var s when s == MovementTypes.PURCHASE => InventoryMovementType.PURCHASE,
                var s when s == MovementTypes.SALE => InventoryMovementType.SALE,
                var s when s == MovementTypes.ADJUSTMENT => InventoryMovementType.ADJUSTMENT,
                var s when s == MovementTypes.RETURN => InventoryMovementType.RETURN,
                var s when s == MovementTypes.TRANSFER => InventoryMovementType.TRANSFER,
                _ => throw new ArgumentException($"Invalid movement type: {movementType}")
            };
        }
    }
}
