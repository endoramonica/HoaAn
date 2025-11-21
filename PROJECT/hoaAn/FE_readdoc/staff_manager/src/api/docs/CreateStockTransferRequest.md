# CreateStockTransferRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**fromWarehouse** | **string** |  | [default to undefined]
**toWarehouse** | **string** |  | [default to undefined]
**requestedBy** | **string** |  | [default to undefined]
**supplierId** | **string** |  | [optional] [default to undefined]
**deliveryDate** | **string** |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]
**items** | [**Array&lt;CreateTransferItemRequest&gt;**](CreateTransferItemRequest.md) |  | [default to undefined]

## Example

```typescript
import { CreateStockTransferRequest } from './api';

const instance: CreateStockTransferRequest = {
    fromWarehouse,
    toWarehouse,
    requestedBy,
    supplierId,
    deliveryDate,
    notes,
    items,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
