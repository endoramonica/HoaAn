# LeaveRequestDtoPaginatedResponse


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**items** | [**Array&lt;LeaveRequestDto&gt;**](LeaveRequestDto.md) |  | [optional] [default to undefined]
**pageNumber** | **number** |  | [optional] [default to undefined]
**pageSize** | **number** |  | [optional] [default to undefined]
**totalItems** | **number** |  | [optional] [default to undefined]
**totalPages** | **number** |  | [optional] [readonly] [default to undefined]
**hasPreviousPage** | **boolean** |  | [optional] [readonly] [default to undefined]
**hasNextPage** | **boolean** |  | [optional] [readonly] [default to undefined]

## Example

```typescript
import { LeaveRequestDtoPaginatedResponse } from './api';

const instance: LeaveRequestDtoPaginatedResponse = {
    items,
    pageNumber,
    pageSize,
    totalItems,
    totalPages,
    hasPreviousPage,
    hasNextPage,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
