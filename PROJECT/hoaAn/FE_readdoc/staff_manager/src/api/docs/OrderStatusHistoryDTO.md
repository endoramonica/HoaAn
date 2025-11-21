# OrderStatusHistoryDTO


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**status** | [**OrderStatus**](OrderStatus.md) |  | [optional] [default to undefined]
**statusText** | **string** |  | [optional] [readonly] [default to undefined]
**changedAt** | **string** |  | [optional] [default to undefined]
**changedByName** | **string** |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { OrderStatusHistoryDTO } from './api';

const instance: OrderStatusHistoryDTO = {
    status,
    statusText,
    changedAt,
    changedByName,
    notes,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
