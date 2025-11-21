# SuppliersApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiSuppliersGet**](#apisuppliersget) | **GET** /api/Suppliers | |
|[**apiSuppliersIdCanDeleteGet**](#apisuppliersidcandeleteget) | **GET** /api/Suppliers/{id}/can-delete | |
|[**apiSuppliersIdDelete**](#apisuppliersiddelete) | **DELETE** /api/Suppliers/{id} | |
|[**apiSuppliersIdGet**](#apisuppliersidget) | **GET** /api/Suppliers/{id} | |
|[**apiSuppliersIdPut**](#apisuppliersidput) | **PUT** /api/Suppliers/{id} | |
|[**apiSuppliersIdStatsGet**](#apisuppliersidstatsget) | **GET** /api/Suppliers/{id}/stats | |
|[**apiSuppliersPost**](#apisupplierspost) | **POST** /api/Suppliers | |

# **apiSuppliersGet**
> SupplierDtoPaginatedResultApiResponse apiSuppliersGet()


### Example

```typescript
import {
    SuppliersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let name: string; // (optional) (default to undefined)
let email: string; // (optional) (default to undefined)
let phone: string; // (optional) (default to undefined)
let status: SupplierStatus; // (optional) (default to undefined)
let address: string; // (optional) (default to undefined)
let createdFrom: string; // (optional) (default to undefined)
let createdTo: string; // (optional) (default to undefined)
let paymentTerms: string; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiSuppliersGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    name,
    email,
    phone,
    status,
    address,
    createdFrom,
    createdTo,
    paymentTerms,
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
| **name** | [**string**] |  | (optional) defaults to undefined|
| **email** | [**string**] |  | (optional) defaults to undefined|
| **phone** | [**string**] |  | (optional) defaults to undefined|
| **status** | **SupplierStatus** |  | (optional) defaults to undefined|
| **address** | [**string**] |  | (optional) defaults to undefined|
| **createdFrom** | [**string**] |  | (optional) defaults to undefined|
| **createdTo** | [**string**] |  | (optional) defaults to undefined|
| **paymentTerms** | [**string**] |  | (optional) defaults to undefined|
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|


### Return type

**SupplierDtoPaginatedResultApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiSuppliersIdCanDeleteGet**
> BooleanApiResponse apiSuppliersIdCanDeleteGet()


### Example

```typescript
import {
    SuppliersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiSuppliersIdCanDeleteGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**BooleanApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiSuppliersIdDelete**
> apiSuppliersIdDelete()


### Example

```typescript
import {
    SuppliersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiSuppliersIdDelete(
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
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**404** | Not Found |  -  |
|**409** | Conflict |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiSuppliersIdGet**
> SupplierDtoApiResponse apiSuppliersIdGet()


### Example

```typescript
import {
    SuppliersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiSuppliersIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**SupplierDtoApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiSuppliersIdPut**
> SupplierDtoApiResponse apiSuppliersIdPut()


### Example

```typescript
import {
    SuppliersApi,
    Configuration,
    UpdateSupplierRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let id: string; // (default to undefined)
let updateSupplierRequest: UpdateSupplierRequest; // (optional)

const { status, data } = await apiInstance.apiSuppliersIdPut(
    id,
    updateSupplierRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateSupplierRequest** | **UpdateSupplierRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**SupplierDtoApiResponse**

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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiSuppliersIdStatsGet**
> StringInt32DictionaryApiResponse apiSuppliersIdStatsGet()


### Example

```typescript
import {
    SuppliersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiSuppliersIdStatsGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**StringInt32DictionaryApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |
|**400** | Bad Request |  -  |
|**401** | Unauthorized |  -  |
|**403** | Forbidden |  -  |
|**404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiSuppliersPost**
> SupplierDtoApiResponse apiSuppliersPost()


### Example

```typescript
import {
    SuppliersApi,
    Configuration,
    CreateSupplierRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new SuppliersApi(configuration);

let createSupplierRequest: CreateSupplierRequest; // (optional)

const { status, data } = await apiInstance.apiSuppliersPost(
    createSupplierRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createSupplierRequest** | **CreateSupplierRequest**|  | |


### Return type

**SupplierDtoApiResponse**

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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

