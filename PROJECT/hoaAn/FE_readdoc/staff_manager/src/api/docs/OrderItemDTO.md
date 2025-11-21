# OrderItemDTO


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**orderId** | **string** |  | [optional] [default to undefined]
**productId** | **string** |  | [optional] [default to undefined]
**productName** | **string** |  | [optional] [default to undefined]
**productSKU** | **string** |  | [optional] [default to undefined]
**productImageUrl** | **string** |  | [optional] [default to undefined]
**unitPrice** | **number** |  | [optional] [default to undefined]
**quantity** | **number** |  | [optional] [default to undefined]
**totalPrice** | **number** |  | [optional] [default to undefined]
**subtotal** | **number** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { OrderItemDTO } from './api';

const instance: OrderItemDTO = {
    id,
    orderId,
    productId,
    productName,
    productSKU,
    productImageUrl,
    unitPrice,
    quantity,
    totalPrice,
    subtotal,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
