# UpdateStockTransferStatusRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**StockTransferStatus**](StockTransferStatus.md) |  | [default to undefined]
**approvedBy** | **string** |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]
**receivedItems** | [**Array&lt;UpdateReceivedQuantityRequest&gt;**](UpdateReceivedQuantityRequest.md) |  | [optional] [default to undefined]

## Example

```typescript
import { UpdateStockTransferStatusRequest } from './api';

const instance: UpdateStockTransferStatusRequest = {
    status,
    approvedBy,
    notes,
    receivedItems,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
