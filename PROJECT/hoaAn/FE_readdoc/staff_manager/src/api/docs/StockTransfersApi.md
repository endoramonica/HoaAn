# StockTransfersApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiStockTransfersGet**](#apistocktransfersget) | **GET** /api/StockTransfers | |
|[**apiStockTransfersIdCancelPatch**](#apistocktransfersidcancelpatch) | **PATCH** /api/StockTransfers/{id}/cancel | |
|[**apiStockTransfersIdGet**](#apistocktransfersidget) | **GET** /api/StockTransfers/{id} | |
|[**apiStockTransfersIdStatusPut**](#apistocktransfersidstatusput) | **PUT** /api/StockTransfers/{id}/status | |
|[**apiStockTransfersPost**](#apistocktransferspost) | **POST** /api/StockTransfers | |
|[**apiStockTransfersSummaryGet**](#apistocktransferssummaryget) | **GET** /api/StockTransfers/summary | |

# **apiStockTransfersGet**
> StockTransferDtoPaginatedResultApiResponse apiStockTransfersGet()


### Example

```typescript
import {
    StockTransfersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new StockTransfersApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let status: StockTransferStatus; // (optional) (default to undefined)
let fromWarehouse: string; // (optional) (default to undefined)
let toWarehouse: string; // (optional) (default to undefined)
let requestedBy: string; // (optional) (default to undefined)
let approvedBy: string; // (optional) (default to undefined)
let supplierId: string; // (optional) (default to undefined)
let createdFrom: string; // (optional) (default to undefined)
let createdTo: string; // (optional) (default to undefined)
let deliveryFrom: string; // (optional) (default to undefined)
let deliveryTo: string; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiStockTransfersGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    status,
    fromWarehouse,
    toWarehouse,
    requestedBy,
    approvedBy,
    supplierId,
    createdFrom,
    createdTo,
    deliveryFrom,
    deliveryTo,
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
| **status** | **StockTransferStatus** |  | (optional) defaults to undefined|
| **fromWarehouse** | [**string**] |  | (optional) defaults to undefined|
| **toWarehouse** | [**string**] |  | (optional) defaults to undefined|
| **requestedBy** | [**string**] |  | (optional) defaults to undefined|
| **approvedBy** | [**string**] |  | (optional) defaults to undefined|
| **supplierId** | [**string**] |  | (optional) defaults to undefined|
| **createdFrom** | [**string**] |  | (optional) defaults to undefined|
| **createdTo** | [**string**] |  | (optional) defaults to undefined|
| **deliveryFrom** | [**string**] |  | (optional) defaults to undefined|
| **deliveryTo** | [**string**] |  | (optional) defaults to undefined|
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|


### Return type

**StockTransferDtoPaginatedResultApiResponse**

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

# **apiStockTransfersIdCancelPatch**
> apiStockTransfersIdCancelPatch()


### Example

```typescript
import {
    StockTransfersApi,
    Configuration,
    CancelStockTransferRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new StockTransfersApi(configuration);

let id: string; // (default to undefined)
let cancelStockTransferRequest: CancelStockTransferRequest; // (optional)

const { status, data } = await apiInstance.apiStockTransfersIdCancelPatch(
    id,
    cancelStockTransferRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **cancelStockTransferRequest** | **CancelStockTransferRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

void (empty response body)

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
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

# **apiStockTransfersIdGet**
> StockTransferDtoApiResponse apiStockTransfersIdGet()


### Example

```typescript
import {
    StockTransfersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new StockTransfersApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiStockTransfersIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**StockTransferDtoApiResponse**

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

# **apiStockTransfersIdStatusPut**
> StockTransferDtoApiResponse apiStockTransfersIdStatusPut()


### Example

```typescript
import {
    StockTransfersApi,
    Configuration,
    UpdateStockTransferStatusRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new StockTransfersApi(configuration);

let id: string; // (default to undefined)
let updateStockTransferStatusRequest: UpdateStockTransferStatusRequest; // (optional)

const { status, data } = await apiInstance.apiStockTransfersIdStatusPut(
    id,
    updateStockTransferStatusRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateStockTransferStatusRequest** | **UpdateStockTransferStatusRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**StockTransferDtoApiResponse**

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

# **apiStockTransfersPost**
> StockTransferDtoApiResponse apiStockTransfersPost()


### Example

```typescript
import {
    StockTransfersApi,
    Configuration,
    CreateStockTransferRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new StockTransfersApi(configuration);

let createStockTransferRequest: CreateStockTransferRequest; // (optional)

const { status, data } = await apiInstance.apiStockTransfersPost(
    createStockTransferRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createStockTransferRequest** | **CreateStockTransferRequest**|  | |


### Return type

**StockTransferDtoApiResponse**

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

# **apiStockTransfersSummaryGet**
> StockTransferSummaryDtoApiResponse apiStockTransfersSummaryGet()


### Example

```typescript
import {
    StockTransfersApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new StockTransfersApi(configuration);

let status: StockTransferStatus; // (optional) (default to undefined)
let fromWarehouse: string; // (optional) (default to undefined)
let toWarehouse: string; // (optional) (default to undefined)
let requestedBy: string; // (optional) (default to undefined)
let approvedBy: string; // (optional) (default to undefined)
let supplierId: string; // (optional) (default to undefined)
let createdFrom: string; // (optional) (default to undefined)
let createdTo: string; // (optional) (default to undefined)
let deliveryFrom: string; // (optional) (default to undefined)
let deliveryTo: string; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiStockTransfersSummaryGet(
    status,
    fromWarehouse,
    toWarehouse,
    requestedBy,
    approvedBy,
    supplierId,
    createdFrom,
    createdTo,
    deliveryFrom,
    deliveryTo,
    searchTerm
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **status** | **StockTransferStatus** |  | (optional) defaults to undefined|
| **fromWarehouse** | [**string**] |  | (optional) defaults to undefined|
| **toWarehouse** | [**string**] |  | (optional) defaults to undefined|
| **requestedBy** | [**string**] |  | (optional) defaults to undefined|
| **approvedBy** | [**string**] |  | (optional) defaults to undefined|
| **supplierId** | [**string**] |  | (optional) defaults to undefined|
| **createdFrom** | [**string**] |  | (optional) defaults to undefined|
| **createdTo** | [**string**] |  | (optional) defaults to undefined|
| **deliveryFrom** | [**string**] |  | (optional) defaults to undefined|
| **deliveryTo** | [**string**] |  | (optional) defaults to undefined|
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|


### Return type

**StockTransferSummaryDtoApiResponse**

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

