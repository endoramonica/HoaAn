# UpdateInteractionRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**type** | [**CRMInteractionType**](CRMInteractionType.md) |  | [optional] [default to undefined]
**title** | **string** |  | [optional] [default to undefined]
**description** | **string** |  | [optional] [default to undefined]
**followUpDate** | **string** |  | [optional] [default to undefined]
**status** | [**CRMInteractionStatus**](CRMInteractionStatus.md) |  | [optional] [default to undefined]

## Example

```typescript
import { UpdateInteractionRequest } from './api';

const instance: UpdateInteractionRequest = {
    type,
    title,
    description,
    followUpDate,
    status,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
