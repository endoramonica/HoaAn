# CreateInteractionRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**customerId** | **string** |  | [default to undefined]
**type** | [**CRMInteractionType**](CRMInteractionType.md) |  | [default to undefined]
**title** | **string** |  | [default to undefined]
**description** | **string** |  | [default to undefined]
**followUpDate** | **string** |  | [optional] [default to undefined]
**status** | [**CRMInteractionStatus**](CRMInteractionStatus.md) |  | [default to undefined]

## Example

```typescript
import { CreateInteractionRequest } from './api';

const instance: CreateInteractionRequest = {
    customerId,
    type,
    title,
    description,
    followUpDate,
    status,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
