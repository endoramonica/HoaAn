# WorkScheduleDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**employeeId** | **string** |  | [optional] [default to undefined]
**employeeName** | **string** |  | [optional] [default to undefined]
**date** | **string** |  | [optional] [default to undefined]
**shiftName** | **string** |  | [optional] [default to undefined]
**startTime** | **string** |  | [optional] [default to undefined]
**endTime** | **string** |  | [optional] [default to undefined]
**type** | [**WorkScheduleType**](WorkScheduleType.md) |  | [optional] [default to undefined]
**status** | [**WorkScheduleStatus**](WorkScheduleStatus.md) |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { WorkScheduleDto } from './api';

const instance: WorkScheduleDto = {
    id,
    employeeId,
    employeeName,
    date,
    shiftName,
    startTime,
    endTime,
    type,
    status,
    notes,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
