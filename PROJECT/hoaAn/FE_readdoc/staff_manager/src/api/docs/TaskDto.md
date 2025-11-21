# TaskDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**title** | **string** |  | [optional] [default to undefined]
**description** | **string** |  | [optional] [default to undefined]
**assignedTo** | **string** |  | [optional] [default to undefined]
**assignedToName** | **string** |  | [optional] [default to undefined]
**assignedToEmail** | **string** |  | [optional] [default to undefined]
**assignedBy** | **string** |  | [optional] [default to undefined]
**assignedByName** | **string** |  | [optional] [default to undefined]
**priority** | [**TaskPriority**](TaskPriority.md) |  | [optional] [default to undefined]
**priorityName** | **string** |  | [optional] [default to undefined]
**status** | [**TaskStatus**](TaskStatus.md) |  | [optional] [default to undefined]
**statusName** | **string** |  | [optional] [default to undefined]
**dueDate** | **string** |  | [optional] [default to undefined]
**completedAt** | **string** |  | [optional] [default to undefined]
**createdAt** | **string** |  | [optional] [default to undefined]
**updatedAt** | **string** |  | [optional] [default to undefined]
**isOverdue** | **boolean** |  | [optional] [readonly] [default to undefined]
**daysUntilDue** | **number** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { TaskDto } from './api';

const instance: TaskDto = {
    id,
    title,
    description,
    assignedTo,
    assignedToName,
    assignedToEmail,
    assignedBy,
    assignedByName,
    priority,
    priorityName,
    status,
    statusName,
    dueDate,
    completedAt,
    createdAt,
    updatedAt,
    isOverdue,
    daysUntilDue,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
