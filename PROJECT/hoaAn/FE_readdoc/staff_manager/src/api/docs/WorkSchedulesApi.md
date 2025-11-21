# WorkSchedulesApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiWorkSchedulesGet**](#apiworkschedulesget) | **GET** /api/WorkSchedules | |
|[**apiWorkSchedulesIdDelete**](#apiworkschedulesiddelete) | **DELETE** /api/WorkSchedules/{id} | |
|[**apiWorkSchedulesIdGet**](#apiworkschedulesidget) | **GET** /api/WorkSchedules/{id} | |
|[**apiWorkSchedulesIdPut**](#apiworkschedulesidput) | **PUT** /api/WorkSchedules/{id} | |
|[**apiWorkSchedulesPost**](#apiworkschedulespost) | **POST** /api/WorkSchedules | |

# **apiWorkSchedulesGet**
> WorkScheduleDtoPaginatedResponseApiResponse apiWorkSchedulesGet()


### Example

```typescript
import {
    WorkSchedulesApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new WorkSchedulesApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let shiftName: string; // (optional) (default to undefined)
let employeeId: string; // (optional) (default to undefined)
let storeId: string; // (optional) (default to undefined)
let dateFrom: string; // (optional) (default to undefined)
let dateTo: string; // (optional) (default to undefined)
let type: WorkScheduleType; // (optional) (default to undefined)
let status: WorkScheduleStatus; // (optional) (default to undefined)
let search: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiWorkSchedulesGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    shiftName,
    employeeId,
    storeId,
    dateFrom,
    dateTo,
    type,
    status,
    search
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
| **shiftName** | [**string**] |  | (optional) defaults to undefined|
| **employeeId** | [**string**] |  | (optional) defaults to undefined|
| **storeId** | [**string**] |  | (optional) defaults to undefined|
| **dateFrom** | [**string**] |  | (optional) defaults to undefined|
| **dateTo** | [**string**] |  | (optional) defaults to undefined|
| **type** | **WorkScheduleType** |  | (optional) defaults to undefined|
| **status** | **WorkScheduleStatus** |  | (optional) defaults to undefined|
| **search** | [**string**] |  | (optional) defaults to undefined|


### Return type

**WorkScheduleDtoPaginatedResponseApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiWorkSchedulesIdDelete**
> apiWorkSchedulesIdDelete()


### Example

```typescript
import {
    WorkSchedulesApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new WorkSchedulesApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiWorkSchedulesIdDelete(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**204** | No Content |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**404** | Not Found |  -  |
|**409** | Conflict |  -  |
|**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiWorkSchedulesIdGet**
> WorkScheduleDtoApiResponse apiWorkSchedulesIdGet()


### Example

```typescript
import {
    WorkSchedulesApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new WorkSchedulesApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiWorkSchedulesIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**WorkScheduleDtoApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**404** | Not Found |  -  |
|**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiWorkSchedulesIdPut**
> WorkScheduleDtoApiResponse apiWorkSchedulesIdPut()


### Example

```typescript
import {
    WorkSchedulesApi,
    Configuration,
    UpdateWorkScheduleRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new WorkSchedulesApi(configuration);

let id: string; // (default to undefined)
let updateWorkScheduleRequest: UpdateWorkScheduleRequest; // (optional)

const { status, data } = await apiInstance.apiWorkSchedulesIdPut(
    id,
    updateWorkScheduleRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateWorkScheduleRequest** | **UpdateWorkScheduleRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**WorkScheduleDtoApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**404** | Not Found |  -  |
|**409** | Conflict |  -  |
|**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiWorkSchedulesPost**
> WorkScheduleDtoApiResponse apiWorkSchedulesPost()


### Example

```typescript
import {
    WorkSchedulesApi,
    Configuration,
    CreateWorkScheduleRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new WorkSchedulesApi(configuration);

let createWorkScheduleRequest: CreateWorkScheduleRequest; // (optional)

const { status, data } = await apiInstance.apiWorkSchedulesPost(
    createWorkScheduleRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createWorkScheduleRequest** | **CreateWorkScheduleRequest**|  | |


### Return type

**WorkScheduleDtoApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**201** | Created |  -  |
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**409** | Conflict |  -  |
|**500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

