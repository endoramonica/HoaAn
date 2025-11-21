# ShiftsApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiShiftsCloseIdPost**](#apishiftscloseidpost) | **POST** /api/Shifts/close/{id} | |
|[**apiShiftsCurrentUserIdGet**](#apishiftscurrentuseridget) | **GET** /api/Shifts/current/{userId} | |
|[**apiShiftsGet**](#apishiftsget) | **GET** /api/Shifts | |
|[**apiShiftsIdGet**](#apishiftsidget) | **GET** /api/Shifts/{id} | |
|[**apiShiftsIdPut**](#apishiftsidput) | **PUT** /api/Shifts/{id} | |
|[**apiShiftsOpenPost**](#apishiftsopenpost) | **POST** /api/Shifts/open | |

# **apiShiftsCloseIdPost**
> apiShiftsCloseIdPost()


### Example

```typescript
import {
    ShiftsApi,
    Configuration,
    CloseShiftRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new ShiftsApi(configuration);

let id: string; // (default to undefined)
let closeShiftRequest: CloseShiftRequest; // (optional)

const { status, data } = await apiInstance.apiShiftsCloseIdPost(
    id,
    closeShiftRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **closeShiftRequest** | **CloseShiftRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiShiftsCurrentUserIdGet**
> apiShiftsCurrentUserIdGet()


### Example

```typescript
import {
    ShiftsApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new ShiftsApi(configuration);

let userId: string; // (default to undefined)

const { status, data } = await apiInstance.apiShiftsCurrentUserIdGet(
    userId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **userId** | [**string**] |  | defaults to undefined|


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiShiftsGet**
> apiShiftsGet()


### Example

```typescript
import {
    ShiftsApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new ShiftsApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let userId: string; // (optional) (default to undefined)
let storeId: string; // (optional) (default to undefined)
let status: ShiftStatus; // (optional) (default to undefined)
let startTimeFrom: string; // (optional) (default to undefined)
let startTimeTo: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiShiftsGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    userId,
    storeId,
    status,
    startTimeFrom,
    startTimeTo
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
| **userId** | [**string**] |  | (optional) defaults to undefined|
| **storeId** | [**string**] |  | (optional) defaults to undefined|
| **status** | **ShiftStatus** |  | (optional) defaults to undefined|
| **startTimeFrom** | [**string**] |  | (optional) defaults to undefined|
| **startTimeTo** | [**string**] |  | (optional) defaults to undefined|


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiShiftsIdGet**
> apiShiftsIdGet()


### Example

```typescript
import {
    ShiftsApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new ShiftsApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiShiftsIdGet(
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
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiShiftsIdPut**
> apiShiftsIdPut()


### Example

```typescript
import {
    ShiftsApi,
    Configuration,
    UpdateShiftRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new ShiftsApi(configuration);

let id: string; // (default to undefined)
let updateShiftRequest: UpdateShiftRequest; // (optional)

const { status, data } = await apiInstance.apiShiftsIdPut(
    id,
    updateShiftRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateShiftRequest** | **UpdateShiftRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiShiftsOpenPost**
> apiShiftsOpenPost()


### Example

```typescript
import {
    ShiftsApi,
    Configuration,
    OpenShiftRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new ShiftsApi(configuration);

let openShiftRequest: OpenShiftRequest; // (optional)

const { status, data } = await apiInstance.apiShiftsOpenPost(
    openShiftRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **openShiftRequest** | **OpenShiftRequest**|  | |


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

