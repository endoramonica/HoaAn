# CustomerApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiCustomerAddressesAddressIdDelete**](#apicustomeraddressesaddressiddelete) | **DELETE** /api/Customer/addresses/{addressId} | |
|[**apiCustomerAddressesAddressIdGet**](#apicustomeraddressesaddressidget) | **GET** /api/Customer/addresses/{addressId} | |
|[**apiCustomerAddressesAddressIdPut**](#apicustomeraddressesaddressidput) | **PUT** /api/Customer/addresses/{addressId} | |
|[**apiCustomerAddressesPost**](#apicustomeraddressespost) | **POST** /api/Customer/addresses | |
|[**apiCustomerGet**](#apicustomerget) | **GET** /api/Customer | |
|[**apiCustomerIdAddressesAddressIdDefaultPut**](#apicustomeridaddressesaddressiddefaultput) | **PUT** /api/Customer/{id}/addresses/{addressId}/default | |
|[**apiCustomerIdAddressesGet**](#apicustomeridaddressesget) | **GET** /api/Customer/{id}/addresses | |
|[**apiCustomerIdDelete**](#apicustomeriddelete) | **DELETE** /api/Customer/{id} | |
|[**apiCustomerIdGet**](#apicustomeridget) | **GET** /api/Customer/{id} | |
|[**apiCustomerIdInteractionsGet**](#apicustomeridinteractionsget) | **GET** /api/Customer/{id}/interactions | |
|[**apiCustomerIdLoyaltyAddPost**](#apicustomeridloyaltyaddpost) | **POST** /api/Customer/{id}/loyalty/add | |
|[**apiCustomerIdLoyaltyDeductPost**](#apicustomeridloyaltydeductpost) | **POST** /api/Customer/{id}/loyalty/deduct | |
|[**apiCustomerIdLoyaltyHistoryGet**](#apicustomeridloyaltyhistoryget) | **GET** /api/Customer/{id}/loyalty-history | |
|[**apiCustomerIdOrdersGet**](#apicustomeridordersget) | **GET** /api/Customer/{id}/orders | |
|[**apiCustomerIdOrdersSummaryGet**](#apicustomeridorderssummaryget) | **GET** /api/Customer/{id}/orders/summary | |
|[**apiCustomerIdPut**](#apicustomeridput) | **PUT** /api/Customer/{id} | |
|[**apiCustomerIdStatisticsGet**](#apicustomeridstatisticsget) | **GET** /api/Customer/{id}/statistics | |
|[**apiCustomerIdTierPut**](#apicustomeridtierput) | **PUT** /api/Customer/{id}/tier | |
|[**apiCustomerInteractionsInteractionIdCompletePut**](#apicustomerinteractionsinteractionidcompleteput) | **PUT** /api/Customer/interactions/{interactionId}/complete | |
|[**apiCustomerInteractionsInteractionIdDelete**](#apicustomerinteractionsinteractioniddelete) | **DELETE** /api/Customer/interactions/{interactionId} | |
|[**apiCustomerInteractionsInteractionIdGet**](#apicustomerinteractionsinteractionidget) | **GET** /api/Customer/interactions/{interactionId} | |
|[**apiCustomerInteractionsInteractionIdPut**](#apicustomerinteractionsinteractionidput) | **PUT** /api/Customer/interactions/{interactionId} | |
|[**apiCustomerInteractionsPost**](#apicustomerinteractionspost) | **POST** /api/Customer/interactions | |
|[**apiCustomerInteractionsUpcomingGet**](#apicustomerinteractionsupcomingget) | **GET** /api/Customer/interactions/upcoming | |
|[**apiCustomerPost**](#apicustomerpost) | **POST** /api/Customer | |
|[**apiCustomerSearchGet**](#apicustomersearchget) | **GET** /api/Customer/search | |

# **apiCustomerAddressesAddressIdDelete**
> BooleanApiResponse apiCustomerAddressesAddressIdDelete()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let addressId: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerAddressesAddressIdDelete(
    addressId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **addressId** | [**string**] |  | defaults to undefined|


### Return type

**BooleanApiResponse**

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

# **apiCustomerAddressesAddressIdGet**
> CustomerAddressDtoApiResponse apiCustomerAddressesAddressIdGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let addressId: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerAddressesAddressIdGet(
    addressId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **addressId** | [**string**] |  | defaults to undefined|


### Return type

**CustomerAddressDtoApiResponse**

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

# **apiCustomerAddressesAddressIdPut**
> CustomerAddressDtoApiResponse apiCustomerAddressesAddressIdPut()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    UpdateCustomerAddressRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let addressId: string; // (default to undefined)
let updateCustomerAddressRequest: UpdateCustomerAddressRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerAddressesAddressIdPut(
    addressId,
    updateCustomerAddressRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateCustomerAddressRequest** | **UpdateCustomerAddressRequest**|  | |
| **addressId** | [**string**] |  | defaults to undefined|


### Return type

**CustomerAddressDtoApiResponse**

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

# **apiCustomerAddressesPost**
> CustomerAddressDtoApiResponse apiCustomerAddressesPost()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    CreateCustomerAddressRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let createCustomerAddressRequest: CreateCustomerAddressRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerAddressesPost(
    createCustomerAddressRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createCustomerAddressRequest** | **CreateCustomerAddressRequest**|  | |


### Return type

**CustomerAddressDtoApiResponse**

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

# **apiCustomerGet**
> CustomerListDtoPaginatedResponseApiResponse apiCustomerGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)
let tier: string; // (optional) (default to undefined)
let isActive: boolean; // (optional) (default to undefined)
let fromDate: string; // (optional) (default to undefined)
let toDate: string; // (optional) (default to undefined)
let minLoyaltyPoints: number; // (optional) (default to undefined)
let maxLoyaltyPoints: number; // (optional) (default to undefined)
let hasEmail: boolean; // (optional) (default to undefined)
let hasPhone: boolean; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCustomerGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    searchTerm,
    tier,
    isActive,
    fromDate,
    toDate,
    minLoyaltyPoints,
    maxLoyaltyPoints,
    hasEmail,
    hasPhone
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
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|
| **tier** | [**string**] |  | (optional) defaults to undefined|
| **isActive** | [**boolean**] |  | (optional) defaults to undefined|
| **fromDate** | [**string**] |  | (optional) defaults to undefined|
| **toDate** | [**string**] |  | (optional) defaults to undefined|
| **minLoyaltyPoints** | [**number**] |  | (optional) defaults to undefined|
| **maxLoyaltyPoints** | [**number**] |  | (optional) defaults to undefined|
| **hasEmail** | [**boolean**] |  | (optional) defaults to undefined|
| **hasPhone** | [**boolean**] |  | (optional) defaults to undefined|


### Return type

**CustomerListDtoPaginatedResponseApiResponse**

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

# **apiCustomerIdAddressesAddressIdDefaultPut**
> BooleanApiResponse apiCustomerIdAddressesAddressIdDefaultPut()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let addressId: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdAddressesAddressIdDefaultPut(
    id,
    addressId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|
| **addressId** | [**string**] |  | defaults to undefined|


### Return type

**BooleanApiResponse**

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

# **apiCustomerIdAddressesGet**
> CustomerAddressDtoListApiResponse apiCustomerIdAddressesGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdAddressesGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerAddressDtoListApiResponse**

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

# **apiCustomerIdDelete**
> BooleanApiResponse apiCustomerIdDelete()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdDelete(
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
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCustomerIdGet**
> CustomerDetailDtoApiResponse apiCustomerIdGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerDetailDtoApiResponse**

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

# **apiCustomerIdInteractionsGet**
> CRMInteractionListDtoPaginatedResponseApiResponse apiCustomerIdInteractionsGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let customerId: string; // (optional) (default to undefined)
let type: CRMInteractionType; // (optional) (default to undefined)
let status: CRMInteractionStatus; // (optional) (default to undefined)
let searchTerm: string; // (optional) (default to undefined)
let fromDate: string; // (optional) (default to undefined)
let toDate: string; // (optional) (default to undefined)
let followUpFromDate: string; // (optional) (default to undefined)
let followUpToDate: string; // (optional) (default to undefined)
let createdBy: string; // (optional) (default to undefined)
let hasPendingFollowUp: boolean; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdInteractionsGet(
    id,
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    customerId,
    type,
    status,
    searchTerm,
    fromDate,
    toDate,
    followUpFromDate,
    followUpToDate,
    createdBy,
    hasPendingFollowUp
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|
| **page** | [**number**] |  | (optional) defaults to undefined|
| **pageSize** | [**number**] |  | (optional) defaults to undefined|
| **sortBy** | [**string**] |  | (optional) defaults to undefined|
| **sortDescending** | [**boolean**] |  | (optional) defaults to undefined|
| **skip** | [**number**] |  | (optional) defaults to undefined|
| **customerId** | [**string**] |  | (optional) defaults to undefined|
| **type** | **CRMInteractionType** |  | (optional) defaults to undefined|
| **status** | **CRMInteractionStatus** |  | (optional) defaults to undefined|
| **searchTerm** | [**string**] |  | (optional) defaults to undefined|
| **fromDate** | [**string**] |  | (optional) defaults to undefined|
| **toDate** | [**string**] |  | (optional) defaults to undefined|
| **followUpFromDate** | [**string**] |  | (optional) defaults to undefined|
| **followUpToDate** | [**string**] |  | (optional) defaults to undefined|
| **createdBy** | [**string**] |  | (optional) defaults to undefined|
| **hasPendingFollowUp** | [**boolean**] |  | (optional) defaults to undefined|


### Return type

**CRMInteractionListDtoPaginatedResponseApiResponse**

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

# **apiCustomerIdLoyaltyAddPost**
> CustomerDetailDtoApiResponse apiCustomerIdLoyaltyAddPost()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    AddPointsRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let addPointsRequest: AddPointsRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerIdLoyaltyAddPost(
    id,
    addPointsRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **addPointsRequest** | **AddPointsRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerDetailDtoApiResponse**

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

# **apiCustomerIdLoyaltyDeductPost**
> CustomerDetailDtoApiResponse apiCustomerIdLoyaltyDeductPost()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    DeductPointsRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let deductPointsRequest: DeductPointsRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerIdLoyaltyDeductPost(
    id,
    deductPointsRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **deductPointsRequest** | **DeductPointsRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerDetailDtoApiResponse**

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

# **apiCustomerIdLoyaltyHistoryGet**
> LoyaltyHistoryDtoListApiResponse apiCustomerIdLoyaltyHistoryGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdLoyaltyHistoryGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**LoyaltyHistoryDtoListApiResponse**

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

# **apiCustomerIdOrdersGet**
> OrderDetailDtoPaginatedResponseApiResponse apiCustomerIdOrdersGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdOrdersGet(
    id,
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
| **id** | [**string**] |  | defaults to undefined|
| **page** | [**number**] |  | (optional) defaults to undefined|
| **pageSize** | [**number**] |  | (optional) defaults to undefined|
| **sortBy** | [**string**] |  | (optional) defaults to undefined|
| **sortDescending** | [**boolean**] |  | (optional) defaults to undefined|
| **skip** | [**number**] |  | (optional) defaults to undefined|


### Return type

**OrderDetailDtoPaginatedResponseApiResponse**

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

# **apiCustomerIdOrdersSummaryGet**
> CustomerOrderSummaryDtoApiResponse apiCustomerIdOrdersSummaryGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdOrdersSummaryGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerOrderSummaryDtoApiResponse**

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

# **apiCustomerIdPut**
> CustomerDetailDtoApiResponse apiCustomerIdPut()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    UpdateCustomerRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let updateCustomerRequest: UpdateCustomerRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerIdPut(
    id,
    updateCustomerRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateCustomerRequest** | **UpdateCustomerRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerDetailDtoApiResponse**

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

# **apiCustomerIdStatisticsGet**
> CustomerStatisticsDtoApiResponse apiCustomerIdStatisticsGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerIdStatisticsGet(
    id
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerStatisticsDtoApiResponse**

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

# **apiCustomerIdTierPut**
> CustomerDetailDtoApiResponse apiCustomerIdTierPut()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    UpdateTierRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let id: string; // (default to undefined)
let updateTierRequest: UpdateTierRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerIdTierPut(
    id,
    updateTierRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateTierRequest** | **UpdateTierRequest**|  | |
| **id** | [**string**] |  | defaults to undefined|


### Return type

**CustomerDetailDtoApiResponse**

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

# **apiCustomerInteractionsInteractionIdCompletePut**
> CRMInteractionDtoApiResponse apiCustomerInteractionsInteractionIdCompletePut()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let interactionId: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerInteractionsInteractionIdCompletePut(
    interactionId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **interactionId** | [**string**] |  | defaults to undefined|


### Return type

**CRMInteractionDtoApiResponse**

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

# **apiCustomerInteractionsInteractionIdDelete**
> BooleanApiResponse apiCustomerInteractionsInteractionIdDelete()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let interactionId: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerInteractionsInteractionIdDelete(
    interactionId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **interactionId** | [**string**] |  | defaults to undefined|


### Return type

**BooleanApiResponse**

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

# **apiCustomerInteractionsInteractionIdGet**
> CRMInteractionDtoApiResponse apiCustomerInteractionsInteractionIdGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let interactionId: string; // (default to undefined)

const { status, data } = await apiInstance.apiCustomerInteractionsInteractionIdGet(
    interactionId
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **interactionId** | [**string**] |  | defaults to undefined|


### Return type

**CRMInteractionDtoApiResponse**

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

# **apiCustomerInteractionsInteractionIdPut**
> CRMInteractionDtoApiResponse apiCustomerInteractionsInteractionIdPut()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    UpdateInteractionRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let interactionId: string; // (default to undefined)
let updateInteractionRequest: UpdateInteractionRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerInteractionsInteractionIdPut(
    interactionId,
    updateInteractionRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **updateInteractionRequest** | **UpdateInteractionRequest**|  | |
| **interactionId** | [**string**] |  | defaults to undefined|


### Return type

**CRMInteractionDtoApiResponse**

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

# **apiCustomerInteractionsPost**
> CRMInteractionDtoApiResponse apiCustomerInteractionsPost()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    CreateInteractionRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let createInteractionRequest: CreateInteractionRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerInteractionsPost(
    createInteractionRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createInteractionRequest** | **CreateInteractionRequest**|  | |


### Return type

**CRMInteractionDtoApiResponse**

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

# **apiCustomerInteractionsUpcomingGet**
> CRMInteractionListDtoListApiResponse apiCustomerInteractionsUpcomingGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let from: string; // (optional) (default to undefined)
let to: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCustomerInteractionsUpcomingGet(
    from,
    to
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **from** | [**string**] |  | (optional) defaults to undefined|
| **to** | [**string**] |  | (optional) defaults to undefined|


### Return type

**CRMInteractionListDtoListApiResponse**

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

# **apiCustomerPost**
> CustomerDetailDtoApiResponse apiCustomerPost()


### Example

```typescript
import {
    CustomerApi,
    Configuration,
    CreateCustomerRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let createCustomerRequest: CreateCustomerRequest; // (optional)

const { status, data } = await apiInstance.apiCustomerPost(
    createCustomerRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **createCustomerRequest** | **CreateCustomerRequest**|  | |


### Return type

**CustomerDetailDtoApiResponse**

### Authorization

[Bearer](../README.md#Bearer)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
|**201** | Created |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

# **apiCustomerSearchGet**
> CustomerListDtoListApiResponse apiCustomerSearchGet()


### Example

```typescript
import {
    CustomerApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CustomerApi(configuration);

let term: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiCustomerSearchGet(
    term
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **term** | [**string**] |  | (optional) defaults to undefined|


### Return type

**CustomerListDtoListApiResponse**

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

