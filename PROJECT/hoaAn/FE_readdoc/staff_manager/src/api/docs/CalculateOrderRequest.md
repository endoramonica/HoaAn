# CalculateOrderRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**customerId** | **string** |  | [optional] [default to undefined]
**items** | [**Array&lt;OrderItemRequest&gt;**](OrderItemRequest.md) |  | [optional] [default to undefined]
**shippingAddress** | **string** |  | [optional] [default to undefined]
**voucherCode** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { CalculateOrderRequest } from './api';

const instance: CalculateOrderRequest = {
    customerId,
    items,
    shippingAddress,
    voucherCode,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
