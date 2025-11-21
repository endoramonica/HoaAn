# CRMInteractionListDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**customerId** | **string** |  | [optional] [default to undefined]
**customerName** | **string** |  | [optional] [default to undefined]
**type** | [**CRMInteractionType**](CRMInteractionType.md) |  | [optional] [default to undefined]
**typeText** | **string** |  | [optional] [readonly] [default to undefined]
**title** | **string** |  | [optional] [default to undefined]
**status** | [**CRMInteractionStatus**](CRMInteractionStatus.md) |  | [optional] [default to undefined]
**statusText** | **string** |  | [optional] [readonly] [default to undefined]
**followUpDate** | **string** |  | [optional] [default to undefined]
**createdAt** | **string** |  | [optional] [default to undefined]
**createdByName** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { CRMInteractionListDto } from './api';

const instance: CRMInteractionListDto = {
    id,
    customerId,
    customerName,
    type,
    typeText,
    title,
    status,
    statusText,
    followUpDate,
    createdAt,
    createdByName,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
