# CreateCustomerAddressRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**customerId** | **string** |  | [default to undefined]
**streetAddress** | **string** |  | [default to undefined]
**city** | **string** |  | [optional] [default to undefined]
**postalCode** | **string** |  | [optional] [default to undefined]
**state** | **string** |  | [optional] [default to undefined]
**country** | **string** |  | [optional] [default to undefined]
**addressType** | [**AddressType**](AddressType.md) |  | [default to undefined]
**recipientName** | **string** |  | [optional] [default to undefined]
**phoneNumber** | **string** |  | [optional] [default to undefined]
**email** | **string** |  | [optional] [default to undefined]
**isDefault** | **boolean** |  | [optional] [default to undefined]
**isPrimary** | **boolean** |  | [optional] [default to undefined]
**isActive** | **boolean** |  | [optional] [default to undefined]

## Example

```typescript
import { CreateCustomerAddressRequest } from './api';

const instance: CreateCustomerAddressRequest = {
    customerId,
    streetAddress,
    city,
    postalCode,
    state,
    country,
    addressType,
    recipientName,
    phoneNumber,
    email,
    isDefault,
    isPrimary,
    isActive,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
