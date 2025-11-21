# InventoryDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**storeId** | **string** |  | [optional] [default to undefined]
**productId** | **string** |  | [optional] [default to undefined]
**quantityAvailable** | **number** |  | [optional] [default to undefined]
**quantityReserved** | **number** |  | [optional] [default to undefined]
**quantityActual** | **number** |  | [optional] [readonly] [default to undefined]
**reorderLevel** | **number** |  | [optional] [default to undefined]
**isLowStock** | **boolean** |  | [optional] [readonly] [default to undefined]
**isActive** | **boolean** |  | [optional] [default to undefined]

## Example

```typescript
import { InventoryDto } from './api';

const instance: InventoryDto = {
    id,
    storeId,
    productId,
    quantityAvailable,
    quantityReserved,
    quantityActual,
    reorderLevel,
    isLowStock,
    isActive,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
