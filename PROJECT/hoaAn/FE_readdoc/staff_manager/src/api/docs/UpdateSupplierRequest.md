# UpdateSupplierRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **string** |  | [optional] [default to undefined]
**email** | **string** |  | [optional] [default to undefined]
**phone** | **string** |  | [optional] [default to undefined]
**address** | **string** |  | [optional] [default to undefined]
**status** | [**SupplierStatus**](SupplierStatus.md) |  | [optional] [default to undefined]
**paymentTerms** | **string** |  | [optional] [default to undefined]
**deliverySchedule** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { UpdateSupplierRequest } from './api';

const instance: UpdateSupplierRequest = {
    name,
    email,
    phone,
    address,
    status,
    paymentTerms,
    deliverySchedule,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
