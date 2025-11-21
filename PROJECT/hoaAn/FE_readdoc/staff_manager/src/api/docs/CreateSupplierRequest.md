# CreateSupplierRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **string** |  | [default to undefined]
**email** | **string** |  | [default to undefined]
**phone** | **string** |  | [default to undefined]
**address** | **string** |  | [default to undefined]
**status** | [**SupplierStatus**](SupplierStatus.md) |  | [default to undefined]
**paymentTerms** | **string** |  | [default to undefined]
**deliverySchedule** | **string** |  | [default to undefined]

## Example

```typescript
import { CreateSupplierRequest } from './api';

const instance: CreateSupplierRequest = {
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
