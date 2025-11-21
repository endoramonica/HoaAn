# EmployeesApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiEmployeesGet**](#apiemployeesget) | **GET** /api/Employees | |
|[**apiEmployeesPost**](#apiemployeespost) | **POST** /api/Employees | |
|[**apiEmployeesUserIdDelete**](#apiemployeesuseriddelete) | **DELETE** /api/Employees/{userId} | |
|[**apiEmployeesUserIdGet**](#apiemployeesuseridget) | **GET** /api/Employees/{userId} | |
|[**apiEmployeesUserIdPut**](#apiemployeesuseridput) | **PUT** /api/Employees/{userId} | |

# **apiEmployeesGet**
> EmployeeDtoPaginatedResponseApiResponse apiEmployeesGet()


### Example

```typescript
import {
    EmployeesApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new EmployeesApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let userId: string; // (optional) (default to undefined)
let search: string; // (optional) (default to undefined)
let department: string; // (optional) (default to undefined)
let status: EmployeeStatus; // (optional) (default to undefined)
let managerId: string; // (optional) (default to undefined)
let storeId: string; // (optional) (default to undefined)
let hireDateFrom: string; // (optional) (default to undefined)
let hireDateTo: string; // (optional) (default to undefined)
let position: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiEmployeesGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    userId,
    search,
    department,
    status,
    managerId,
    storeId,
    hireDateFrom,
    hireDateTo,
    position
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
| **search** | [**string**] |  | (optional) defaults to undefined|
| **department** | [**string**] |  | (optional) defaults to undefined|
| **status** | **EmployeeStatus** |  | (optional) defaults to undefined|
| **managerId** | [**string**] |  | (optional) defaults to undefined|
| **storeId** | [**string**] |  | (optional) defaults to undefined|
| **hireDateFrom** | [**string**] |  | (optional) defaults to undefined|
| **hireDateTo** | [**string**] |  | (optional) defaults to undefined|
| **position** | [**string**] |  | (optional) defaults to undefined|


### Return type

**EmployeeDtoPaginatedResponseApiResponse**

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

# **apiEmployeesPost**
> EmployeeDtoApiResponse apiEmployeesPost()


### Example

```typescript
import {
    EmployeesApi,
    Configuration,
    CreateEmployeeRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new EmployeesApi(configuration);

let createEmployeeRequest: CreateEmployeeRequest; // (optional)

const { status, data } = await apiInstance.apiEmployeesPost(
    createEmployeeRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createEmployeeRequest** | **CreateEmployeeRequest**|  | |


### Return type

**EmployeeDtoApiResponse**

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

# **apiEmployeesUserIdDelete**
> apiEmployeesUserIdDelete()


### Example

```typescript
import {
    EmployeesApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new EmployeesApi(configuration);

let userId: string; // (default to undefined)

const { status, data } = await apiInstance.apiEmployeesUserIdDelete(
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

# **apiEmployeesUserIdGet**
> EmployeeDtoApiResponse apiEmployeesUserIdGet()


### Example

```typescript
import {
    EmployeesApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new EmployeesApi(configuration);

let userId: string; // (default to undefined)

const { status, data } = await apiInstance.apiEmployeesUserIdGet(
    userId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **userId** | [**string**] |  | defaults to undefined|


### Return type

**EmployeeDtoApiResponse**

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

# **apiEmployeesUserIdPut**
> EmployeeDtoApiResponse apiEmployeesUserIdPut()


### Example

```typescript
import {
    EmployeesApi,
    Configuration,
    UpdateEmployeeRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new EmployeesApi(configuration);

let userId: string; // (default to undefined)
let updateEmployeeRequest: UpdateEmployeeRequest; // (optional)

const { status, data } = await apiInstance.apiEmployeesUserIdPut(
    userId,
    updateEmployeeRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateEmployeeRequest** | **UpdateEmployeeRequest**|  | |
| **userId** | [**string**] |  | defaults to undefined|


### Return type

**EmployeeDtoApiResponse**

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

