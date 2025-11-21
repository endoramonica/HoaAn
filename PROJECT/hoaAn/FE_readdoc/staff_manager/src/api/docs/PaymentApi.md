# PaymentApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiPaymentCalculateTotalsPost**](#apipaymentcalculatetotalspost) | **POST** /api/Payment/calculate-totals | |
|[**apiPaymentHistoryGet**](#apipaymenthistoryget) | **GET** /api/Payment/history | |
|[**apiPaymentProcessCardPost**](#apipaymentprocesscardpost) | **POST** /api/Payment/process-card | |
|[**apiPaymentProcessCashPost**](#apipaymentprocesscashpost) | **POST** /api/Payment/process-cash | |
|[**apiPaymentProcessPost**](#apipaymentprocesspost) | **POST** /api/Payment/process | |
|[**apiPaymentRefundPaymentIdPost**](#apipaymentrefundpaymentidpost) | **POST** /api/Payment/refund/{paymentId} | |
|[**apiPaymentVoidPaymentIdPost**](#apipaymentvoidpaymentidpost) | **POST** /api/Payment/void/{paymentId} | |

# **apiPaymentCalculateTotalsPost**
> apiPaymentCalculateTotalsPost()


### Example

```typescript
import {
    PaymentApi,
    Configuration,
    CalculateOrderRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let calculateOrderRequest: CalculateOrderRequest; // (optional)

const { status, data } = await apiInstance.apiPaymentCalculateTotalsPost(
    calculateOrderRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **calculateOrderRequest** | **CalculateOrderRequest**|  | |


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

# **apiPaymentHistoryGet**
> apiPaymentHistoryGet()


### Example

```typescript
import {
    PaymentApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let page: number; // (optional) (default to undefined)
let pageSize: number; // (optional) (default to undefined)
let sortBy: string; // (optional) (default to undefined)
let sortDescending: boolean; // (optional) (default to undefined)
let skip: number; // (optional) (default to undefined)
let orderId: string; // (optional) (default to undefined)
let status: PaymentMethodType; // (optional) (default to undefined)
let fromDate: string; // (optional) (default to undefined)
let toDate: string; // (optional) (default to undefined)
let paymentType: PaymentMethodType; // (optional) (default to undefined)
let transactionId: string; // (optional) (default to undefined)

const { status, data } = await apiInstance.apiPaymentHistoryGet(
    page,
    pageSize,
    sortBy,
    sortDescending,
    skip,
    orderId,
    status,
    fromDate,
    toDate,
    paymentType,
    transactionId
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
| **orderId** | [**string**] |  | (optional) defaults to undefined|
| **status** | **PaymentMethodType** |  | (optional) defaults to undefined|
| **fromDate** | [**string**] |  | (optional) defaults to undefined|
| **toDate** | [**string**] |  | (optional) defaults to undefined|
| **paymentType** | **PaymentMethodType** |  | (optional) defaults to undefined|
| **transactionId** | [**string**] |  | (optional) defaults to undefined|


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

# **apiPaymentProcessCardPost**
> apiPaymentProcessCardPost()


### Example

```typescript
import {
    PaymentApi,
    Configuration,
    CardPaymentRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let cardPaymentRequest: CardPaymentRequest; // (optional)

const { status, data } = await apiInstance.apiPaymentProcessCardPost(
    cardPaymentRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **cardPaymentRequest** | **CardPaymentRequest**|  | |


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

# **apiPaymentProcessCashPost**
> apiPaymentProcessCashPost()


### Example

```typescript
import {
    PaymentApi,
    Configuration,
    CashPaymentRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let cashPaymentRequest: CashPaymentRequest; // (optional)

const { status, data } = await apiInstance.apiPaymentProcessCashPost(
    cashPaymentRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **cashPaymentRequest** | **CashPaymentRequest**|  | |


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

# **apiPaymentProcessPost**
> apiPaymentProcessPost()


### Example

```typescript
import {
    PaymentApi,
    Configuration,
    PaymentRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let paymentRequest: PaymentRequest; // (optional)

const { status, data } = await apiInstance.apiPaymentProcessPost(
    paymentRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **paymentRequest** | **PaymentRequest**|  | |


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

# **apiPaymentRefundPaymentIdPost**
> apiPaymentRefundPaymentIdPost()


### Example

```typescript
import {
    PaymentApi,
    Configuration,
    RefundRequest
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let paymentId: string; // (default to undefined)
let refundRequest: RefundRequest; // (optional)

const { status, data } = await apiInstance.apiPaymentRefundPaymentIdPost(
    paymentId,
    refundRequest
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **refundRequest** | **RefundRequest**|  | |
| **paymentId** | [**string**] |  | defaults to undefined|


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

# **apiPaymentVoidPaymentIdPost**
> apiPaymentVoidPaymentIdPost()


### Example

```typescript
import {
    PaymentApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new PaymentApi(configuration);

let paymentId: string; // (default to undefined)
let body: string; // (optional)

const { status, data } = await apiInstance.apiPaymentVoidPaymentIdPost(
    paymentId,
    body
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **body** | **string**|  | |
| **paymentId** | [**string**] |  | defaults to undefined|


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

