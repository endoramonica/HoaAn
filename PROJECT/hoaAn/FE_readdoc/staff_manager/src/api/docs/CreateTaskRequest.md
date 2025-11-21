# CreateTaskRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**title** | **string** |  | [default to undefined]
**description** | **string** |  | [default to undefined]
**assignedTo** | **string** |  | [default to undefined]
**priority** | [**TaskPriority**](TaskPriority.md) |  | [default to undefined]
**dueDate** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { CreateTaskRequest } from './api';

const instance: CreateTaskRequest = {
    title,
    description,
    assignedTo,
    priority,
    dueDate,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
