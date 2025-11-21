# StockTransferDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**fromWarehouse** | **string** |  | [optional] [default to undefined]
**toWarehouse** | **string** |  | [optional] [default to undefined]
**status** | [**StockTransferStatus**](StockTransferStatus.md) |  | [optional] [default to undefined]
**requestedBy** | **string** |  | [optional] [default to undefined]
**requestedByName** | **string** |  | [optional] [default to undefined]
**approvedBy** | **string** |  | [optional] [default to undefined]
**approvedByName** | **string** |  | [optional] [default to undefined]
**supplierId** | **string** |  | [optional] [default to undefined]
**supplierName** | **string** |  | [optional] [default to undefined]
**deliveryDate** | **string** |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]
**createdAt** | **string** |  | [optional] [default to undefined]
**updatedAt** | **string** |  | [optional] [default to undefined]
**transferItems** | [**Array&lt;TransferItemDto&gt;**](TransferItemDto.md) |  | [optional] [default to undefined]
**totalItems** | **number** |  | [optional] [readonly] [default to undefined]
**totalQuantity** | **number** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { StockTransferDto } from './api';

const instance: StockTransferDto = {
    id,
    fromWarehouse,
    toWarehouse,
    status,
    requestedBy,
    requestedByName,
    approvedBy,
    approvedByName,
    supplierId,
    supplierName,
    deliveryDate,
    notes,
    createdAt,
    updatedAt,
    transferItems,
    totalItems,
    totalQuantity,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
