# InventoryApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiInventoryBulkCheckStockPost**](#apiinventorybulkcheckstockpost) | **POST** /api/Inventory/bulk-check-stock | |
|[**apiInventoryCheckStockGet**](#apiinventorycheckstockget) | **GET** /api/Inventory/check-stock | |
|[**apiInventoryInventoryIdMovementsGet**](#apiinventoryinventoryidmovementsget) | **GET** /api/Inventory/{inventoryId}/movements | |
|[**apiInventoryStoreStoreIdAdjustPost**](#apiinventorystorestoreidadjustpost) | **POST** /api/Inventory/store/{storeId}/adjust | |
|[**apiInventoryStoreStoreIdLowStockGet**](#apiinventorystorestoreidlowstockget) | **GET** /api/Inventory/store/{storeId}/low-stock | |
|[**apiInventoryStoreStoreIdProductProductIdGet**](#apiinventorystorestoreidproductproductidget) | **GET** /api/Inventory/store/{storeId}/product/{productId} | |
|[**apiInventoryStoreStoreIdReleaseReservedPost**](#apiinventorystorestoreidreleasereservedpost) | **POST** /api/Inventory/store/{storeId}/release-reserved | |
|[**apiInventoryStoreStoreIdReservePost**](#apiinventorystorestoreidreservepost) | **POST** /api/Inventory/store/{storeId}/reserve | |

# **apiInventoryBulkCheckStockPost**
> BulkCheckStockResponse apiInventoryBulkCheckStockPost()


### Example

```typescript
import {
    InventoryApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (optional) (default to undefined)
let requestBody: Array<string>; // (optional)

const { status, data } = await apiInstance.apiInventoryBulkCheckStockPost(
    storeId,
    requestBody
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **requestBody** | **Array<string>**|  | |
| **storeId** | [**string**] |  | (optional) defaults to undefined|


### Return type

**BulkCheckStockResponse**

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

# **apiInventoryCheckStockGet**
> CheckStockResponse apiInventoryCheckStockGet()


### Example

```typescript
import {
    InventoryApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (optional) (default to undefined)
let productId: string; // (optional) (default to undefined)
let requiredQuantity: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiInventoryCheckStockGet(
    storeId,
    productId,
    requiredQuantity
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **storeId** | [**string**] |  | (optional) defaults to undefined|
| **productId** | [**string**] |  | (optional) defaults to undefined|
| **requiredQuantity** | [**number**] |  | (optional) defaults to undefined|


### Return type

**CheckStockResponse**

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

# **apiInventoryInventoryIdMovementsGet**
> Array<InventoryMovementDto> apiInventoryInventoryIdMovementsGet()


### Example

```typescript
import {
    InventoryApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let inventoryId: string; // (default to undefined)
let pageNumber: number; // (optional) (default to 1)
let pageSize: number; // (optional) (default to 10)

const { status, data } = await apiInstance.apiInventoryInventoryIdMovementsGet(
    inventoryId,
    pageNumber,
    pageSize
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **inventoryId** | [**string**] |  | defaults to undefined|
| **pageNumber** | [**number**] |  | (optional) defaults to 1|
| **pageSize** | [**number**] |  | (optional) defaults to 10|


### Return type

**Array<InventoryMovementDto>**

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

# **apiInventoryStoreStoreIdAdjustPost**
> InventoryDto apiInventoryStoreStoreIdAdjustPost()


### Example

```typescript
import {
    InventoryApi,
    Configuration,
    AdjustInventoryRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (default to undefined)
let adjustInventoryRequest: AdjustInventoryRequest; // (optional)

const { status, data } = await apiInstance.apiInventoryStoreStoreIdAdjustPost(
    storeId,
    adjustInventoryRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **adjustInventoryRequest** | **AdjustInventoryRequest**|  | |
| **storeId** | [**string**] |  | defaults to undefined|


### Return type

**InventoryDto**

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

# **apiInventoryStoreStoreIdLowStockGet**
> Array<InventoryDto> apiInventoryStoreStoreIdLowStockGet()


### Example

```typescript
import {
    InventoryApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (default to undefined)

const { status, data } = await apiInstance.apiInventoryStoreStoreIdLowStockGet(
    storeId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **storeId** | [**string**] |  | defaults to undefined|


### Return type

**Array<InventoryDto>**

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

# **apiInventoryStoreStoreIdProductProductIdGet**
> InventoryDto apiInventoryStoreStoreIdProductProductIdGet()


### Example

```typescript
import {
    InventoryApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (default to undefined)
let productId: string; // (default to undefined)

const { status, data } = await apiInstance.apiInventoryStoreStoreIdProductProductIdGet(
    storeId,
    productId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **storeId** | [**string**] |  | defaults to undefined|
| **productId** | [**string**] |  | defaults to undefined|


### Return type

**InventoryDto**

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

# **apiInventoryStoreStoreIdReleaseReservedPost**
> InventoryDto apiInventoryStoreStoreIdReleaseReservedPost()


### Example

```typescript
import {
    InventoryApi,
    Configuration,
    ReleaseReservedStockRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (default to undefined)
let releaseReservedStockRequest: ReleaseReservedStockRequest; // (optional)

const { status, data } = await apiInstance.apiInventoryStoreStoreIdReleaseReservedPost(
    storeId,
    releaseReservedStockRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **releaseReservedStockRequest** | **ReleaseReservedStockRequest**|  | |
| **storeId** | [**string**] |  | defaults to undefined|


### Return type

**InventoryDto**

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

# **apiInventoryStoreStoreIdReservePost**
> InventoryDto apiInventoryStoreStoreIdReservePost()


### Example

```typescript
import {
    InventoryApi,
    Configuration,
    ReserveStockRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new InventoryApi(configuration);

let storeId: string; // (default to undefined)
let reserveStockRequest: ReserveStockRequest; // (optional)

const { status, data } = await apiInstance.apiInventoryStoreStoreIdReservePost(
    storeId,
    reserveStockRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **reserveStockRequest** | **ReserveStockRequest**|  | |
| **storeId** | [**string**] |  | defaults to undefined|


### Return type

**InventoryDto**

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

