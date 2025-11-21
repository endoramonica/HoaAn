# CreateWorkScheduleRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**employeeId** | **string** |  | [optional] [default to undefined]
**date** | **string** |  | [optional] [default to undefined]
**startTime** | **string** |  | [optional] [default to undefined]
**endTime** | **string** |  | [optional] [default to undefined]
**type** | [**WorkScheduleType**](WorkScheduleType.md) |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { CreateWorkScheduleRequest } from './api';

const instance: CreateWorkScheduleRequest = {
    employeeId,
    date,
    startTime,
    endTime,
    type,
    notes,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
