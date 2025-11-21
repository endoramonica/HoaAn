# CRMInteractionDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**customerId** | **string** |  | [optional] [default to undefined]
**customerName** | **string** |  | [optional] [default to undefined]
**customerEmail** | **string** |  | [optional] [default to undefined]
**customerPhone** | **string** |  | [optional] [default to undefined]
**type** | [**CRMInteractionType**](CRMInteractionType.md) |  | [optional] [default to undefined]
**typeText** | **string** |  | [optional] [readonly] [default to undefined]
**title** | **string** |  | [optional] [default to undefined]
**description** | **string** |  | [optional] [default to undefined]
**status** | [**CRMInteractionStatus**](CRMInteractionStatus.md) |  | [optional] [default to undefined]
**statusText** | **string** |  | [optional] [readonly] [default to undefined]
**followUpDate** | **string** |  | [optional] [default to undefined]
**createdAt** | **string** |  | [optional] [default to undefined]
**updatedAt** | **string** |  | [optional] [default to undefined]
**createdBy** | **string** |  | [optional] [default to undefined]
**createdByName** | **string** |  | [optional] [default to undefined]
**updatedBy** | **string** |  | [optional] [default to undefined]
**updatedByName** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { CRMInteractionDto } from './api';

const instance: CRMInteractionDto = {
    id,
    customerId,
    customerName,
    customerEmail,
    customerPhone,
    type,
    typeText,
    title,
    description,
    status,
    statusText,
    followUpDate,
    createdAt,
    updatedAt,
    createdBy,
    createdByName,
    updatedBy,
    updatedByName,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
