# UpdateTaskRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**title** | **string** |  | [optional] [default to undefined]
**description** | **string** |  | [optional] [default to undefined]
**assignedTo** | **string** |  | [optional] [default to undefined]
**priority** | [**TaskPriority**](TaskPriority.md) |  | [optional] [default to undefined]
**status** | [**TaskStatus**](TaskStatus.md) |  | [optional] [default to undefined]
**dueDate** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { UpdateTaskRequest } from './api';

const instance: UpdateTaskRequest = {
    title,
    description,
    assignedTo,
    priority,
    status,
    dueDate,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
