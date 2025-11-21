# CustomerOrderSummaryDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**customerId** | **string** |  | [optional] [default to undefined]
**totalOrders** | **number** |  | [optional] [default to undefined]
**completedOrders** | **number** |  | [optional] [default to undefined]
**pendingOrders** | **number** |  | [optional] [default to undefined]
**cancelledOrders** | **number** |  | [optional] [default to undefined]
**totalSpent** | **number** |  | [optional] [default to undefined]
**averageOrderValue** | **number** |  | [optional] [default to undefined]
**firstOrderDate** | **string** |  | [optional] [default to undefined]
**lastOrderDate** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { CustomerOrderSummaryDto } from './api';

const instance: CustomerOrderSummaryDto = {
    customerId,
    totalOrders,
    completedOrders,
    pendingOrders,
    cancelledOrders,
    totalSpent,
    averageOrderValue,
    firstOrderDate,
    lastOrderDate,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
