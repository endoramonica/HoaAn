# LeaveRequestDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **string** |  | [optional] [default to undefined]
**employeeId** | **string** |  | [optional] [default to undefined]
**employeeName** | **string** |  | [optional] [default to undefined]
**leaveType** | [**LeaveRequestType**](LeaveRequestType.md) |  | [optional] [default to undefined]
**startDate** | **string** |  | [optional] [default to undefined]
**endDate** | **string** |  | [optional] [default to undefined]
**days** | **number** |  | [optional] [default to undefined]
**reason** | **string** |  | [optional] [default to undefined]
**status** | [**LeaveRequestStatus**](LeaveRequestStatus.md) |  | [optional] [default to undefined]
**approvedBy** | **string** |  | [optional] [default to undefined]
**approvedByName** | **string** |  | [optional] [default to undefined]
**submittedAt** | **string** |  | [optional] [default to undefined]
**reviewedAt** | **string** |  | [optional] [default to undefined]
**comments** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { LeaveRequestDto } from './api';

const instance: LeaveRequestDto = {
    id,
    employeeId,
    employeeName,
    leaveType,
    startDate,
    endDate,
    days,
    reason,
    status,
    approvedBy,
    approvedByName,
    submittedAt,
    reviewedAt,
    comments,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
