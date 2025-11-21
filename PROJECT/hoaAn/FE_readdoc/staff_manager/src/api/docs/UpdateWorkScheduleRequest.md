# UpdateWorkScheduleRequest


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**date** | **string** |  | [optional] [default to undefined]
**startTime** | **string** |  | [optional] [default to undefined]
**endTime** | **string** |  | [optional] [default to undefined]
**shiftName** | **string** |  | [optional] [default to undefined]
**type** | [**WorkScheduleType**](WorkScheduleType.md) |  | [optional] [default to undefined]
**status** | [**WorkScheduleStatus**](WorkScheduleStatus.md) |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { UpdateWorkScheduleRequest } from './api';

const instance: UpdateWorkScheduleRequest = {
    date,
    startTime,
    endTime,
    shiftName,
    type,
    status,
    notes,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
