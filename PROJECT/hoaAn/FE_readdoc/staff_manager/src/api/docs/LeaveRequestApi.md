# LeaveRequestApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiLeaveRequestGet**](#apileaverequestget) | **GET** /api/LeaveRequest | |
|[**apiLeaveRequestIdGet**](#apileaverequestidget) | **GET** /api/LeaveRequest/{id} | |
|[**apiLeaveRequestIdStatusPatch**](#apileaverequestidstatuspatch) | **PATCH** /api/LeaveRequest/{id}/status | |
|[**apiLeaveRequestPost**](#apileaverequestpost) | **POST** /api/LeaveRequest | |

# **apiLeaveRequestGet**
> LeaveRequestDtoPaginatedResponse apiLeaveRequestGet()


### Example

```typescript
import {
    LeaveRequestApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new LeaveRequestApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let employeeId: string; // (optional) (default to undefined)
let status: LeaveRequestStatus; // (optional) (default to undefined)
let leaveType: LeaveRequestType; // (optional) (default to undefined)
let startDate: string; // (optional) (default to undefined)
let endDate: string; // (optional) (default to undefined)
let submittedFrom: string; // (optional) (default to undefined)
let submittedTo: string; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiLeaveRequestGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    employeeId,
    status,
    leaveType,
    startDate,
    endDate,
    submittedFrom,
    submittedTo,
    searchTerm
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **page** | [**number**] |  | (optional) defaults to undefined|
| **pageSize** | [**number**] |  | (optional) defaults to undefined|
| **sortBy** | [**string**] |  | (optional) defaults to undefined|
| **sortDescending** | [**boolean**] |  | (optional) defaults to undefined|
| **skip** | [**number**] |  | (optional) defaults to undefined|
| **employeeId** | [**string**] |  | (optional) defaults to undefined|
| **status** | **LeaveRequestStatus** |  | (optional) defaults to undefined|
| **leaveType** | **LeaveRequestType** |  | (optional) defaults to undefined|
| **startDate** | [**string**] |  | (optional) defaults to undefined|
| **endDate** | [**string**] |  | (optional) defaults to undefined|
| **submittedFrom** | [**string**] |  | (optional) defaults to undefined|
| **submittedTo** | [**string**] |  | (optional) defaults to undefined|
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|


### Return type

**LeaveRequestDtoPaginatedResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiLeaveRequestIdGet**
> LeaveRequestDto apiLeaveRequestIdGet()


### Example

```typescript
import {
    LeaveRequestApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new LeaveRequestApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiLeaveRequestIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**LeaveRequestDto**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiLeaveRequestIdStatusPatch**
> LeaveRequestDto apiLeaveRequestIdStatusPatch()


### Example

```typescript
import {
    LeaveRequestApi,
    Configuration,
    UpdateLeaveRequestStatusDto
} from './api';

const configuration = new Configuration();
const apiInstance = new LeaveRequestApi(configuration);

let id: string; // (default to undefined)
let updateLeaveRequestStatusDto: UpdateLeaveRequestStatusDto; // (optional)

const { status, data } = await apiInstance.apiLeaveRequestIdStatusPatch(
    id,
    updateLeaveRequestStatusDto
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateLeaveRequestStatusDto** | **UpdateLeaveRequestStatusDto**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**LeaveRequestDto**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiLeaveRequestPost**
> LeaveRequestDto apiLeaveRequestPost()


### Example

```typescript
import {
    LeaveRequestApi,
    Configuration,
    CreateLeaveRequestRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new LeaveRequestApi(configuration);

let createLeaveRequestRequest: CreateLeaveRequestRequest; // (optional)

const { status, data } = await apiInstance.apiLeaveRequestPost(
    createLeaveRequestRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createLeaveRequestRequest** | **CreateLeaveRequestRequest**|  | |


### Return type

**LeaveRequestDto**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

