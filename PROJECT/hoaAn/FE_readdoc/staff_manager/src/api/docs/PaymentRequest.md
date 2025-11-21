# PaymentRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**orderId** | **string** |  | [optional] [default to undefined]
**paymentType** | [**PaymentMethodType**](PaymentMethodType.md) |  | [optional] [default to undefined]
**amount** | **number** |  | [optional] [default to undefined]
**cardNumber** | **string** |  | [optional] [default to undefined]
**cvv** | **string** |  | [optional] [default to undefined]
**shippingAddress** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { PaymentRequest } from './api';

const instance: PaymentRequest = {
    orderId,
    paymentType,
    amount,
    cardNumber,
    cvv,
    shippingAddress,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
