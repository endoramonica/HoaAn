# TaskApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiTaskAssigneeAssigneeIdGet**](#apitaskassigneeassigneeidget) | **GET** /api/Task/assignee/{assigneeId} | |
|[**apiTaskGet**](#apitaskget) | **GET** /api/Task | |
|[**apiTaskIdDelete**](#apitaskiddelete) | **DELETE** /api/Task/{id} | |
|[**apiTaskIdGet**](#apitaskidget) | **GET** /api/Task/{id} | |
|[**apiTaskIdPut**](#apitaskidput) | **PUT** /api/Task/{id} | |
|[**apiTaskIdStatusPatch**](#apitaskidstatuspatch) | **PATCH** /api/Task/{id}/status | |
|[**apiTaskPost**](#apitaskpost) | **POST** /api/Task | |
|[**apiTaskTaskIdAssignAssigneeIdPost**](#apitasktaskidassignassigneeidpost) | **POST** /api/Task/{taskId}/assign/{assigneeId} | |
|[**apiTaskTaskIdCompletePost**](#apitasktaskidcompletepost) | **POST** /api/Task/{taskId}/complete | |

# **apiTaskAssigneeAssigneeIdGet**
> TaskDtoPaginatedResponse apiTaskAssigneeAssigneeIdGet()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let assigneeId: string; // (default to undefined)
let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiTaskAssigneeAssigneeIdGet(
    assigneeId,
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **assigneeId** | [**string**] |  | defaults to undefined|
| **page** | [**number**] |  | (optional) defaults to undefined|
| **pageSize** | [**number**] |  | (optional) defaults to undefined|
| **sortBy** | [**string**] |  | (optional) defaults to undefined|
| **sortDescending** | [**boolean**] |  | (optional) defaults to undefined|
| **skip** | [**number**] |  | (optional) defaults to undefined|


### Return type

**TaskDtoPaginatedResponse**

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

# **apiTaskGet**
> TaskDtoPaginatedResponse apiTaskGet()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let userId: string; // (optional) (default to undefined)
let assignedTo: string; // (optional) (default to undefined)
let assignedBy: string; // (optional) (default to undefined)
let status: TaskStatus; // (optional) (default to undefined)
let priority: TaskPriority; // (optional) (default to undefined)
let dueDateFrom: string; // (optional) (default to undefined)
let dueDateTo: string; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)
let isOverdue: boolean; // (optional) (default to undefined)
let storeId: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiTaskGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    userId,
    assignedTo,
    assignedBy,
    status,
    priority,
    dueDateFrom,
    dueDateTo,
    searchTerm,
    isOverdue,
    storeId
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
| **assignedTo** | [**string**] |  | (optional) defaults to undefined|
| **assignedBy** | [**string**] |  | (optional) defaults to undefined|
| **status** | **TaskStatus** |  | (optional) defaults to undefined|
| **priority** | **TaskPriority** |  | (optional) defaults to undefined|
| **dueDateFrom** | [**string**] |  | (optional) defaults to undefined|
| **dueDateTo** | [**string**] |  | (optional) defaults to undefined|
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|
| **isOverdue** | [**boolean**] |  | (optional) defaults to undefined|
| **storeId** | [**string**] |  | (optional) defaults to undefined|


### Return type

**TaskDtoPaginatedResponse**

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

# **apiTaskIdDelete**
> apiTaskIdDelete()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiTaskIdDelete(
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

# **apiTaskIdGet**
> TaskDto apiTaskIdGet()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiTaskIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**TaskDto**

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

# **apiTaskIdPut**
> TaskDto apiTaskIdPut()


### Example

```typescript
import {
    TaskApi,
    Configuration,
    UpdateTaskRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let id: string; // (default to undefined)
let updateTaskRequest: UpdateTaskRequest; // (optional)

const { status, data } = await apiInstance.apiTaskIdPut(
    id,
    updateTaskRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateTaskRequest** | **UpdateTaskRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**TaskDto**

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

# **apiTaskIdStatusPatch**
> TaskDto apiTaskIdStatusPatch()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let id: string; // (default to undefined)
let body: string; // (optional)

const { status, data } = await apiInstance.apiTaskIdStatusPatch(
    id,
    body
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **body** | **string**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**TaskDto**

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

# **apiTaskPost**
> TaskDto apiTaskPost()


### Example

```typescript
import {
    TaskApi,
    Configuration,
    CreateTaskRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let createTaskRequest: CreateTaskRequest; // (optional)

const { status, data } = await apiInstance.apiTaskPost(
    createTaskRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createTaskRequest** | **CreateTaskRequest**|  | |


### Return type

**TaskDto**

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

# **apiTaskTaskIdAssignAssigneeIdPost**
> apiTaskTaskIdAssignAssigneeIdPost()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let taskId: string; // (default to undefined)
let assigneeId: string; // (default to undefined)

const { status, data } = await apiInstance.apiTaskTaskIdAssignAssigneeIdPost(
    taskId,
    assigneeId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **taskId** | [**string**] |  | defaults to undefined|
| **assigneeId** | [**string**] |  | defaults to undefined|


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

# **apiTaskTaskIdCompletePost**
> apiTaskTaskIdCompletePost()


### Example

```typescript
import {
    TaskApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new TaskApi(configuration);

let taskId: string; // (default to undefined)
let body: string; // (optional)

const { status, data } = await apiInstance.apiTaskTaskIdCompletePost(
    taskId,
    body
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **body** | **string**|  | |
| **taskId** | [**string**] |  | defaults to undefined|


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

