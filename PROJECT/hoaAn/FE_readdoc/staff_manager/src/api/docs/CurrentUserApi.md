# CurrentUserApi

All URIs are relative to *http://localhost*

|Method | HTTP request | Description|
|------------- | ------------- | -------------|
|[**apiCurrentUserDetailsGet**](#apicurrentuserdetailsget) | **GET** /api/CurrentUser/details | |
|[**apiCurrentUserGet**](#apicurrentuserget) | **GET** /api/CurrentUser | |
|[**apiCurrentUserHasPermissionPermissionPost**](#apicurrentuserhaspermissionpermissionpost) | **POST** /api/CurrentUser/has-permission/{permission} | |

# **apiCurrentUserDetailsGet**
> apiCurrentUserDetailsGet()


### Example

```typescript
import {
    CurrentUserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CurrentUserApi(configuration);

const { status, data } = await apiInstance.apiCurrentUserDetailsGet();
```

### Parameters
This endpoint does not have any parameters.


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

# **apiCurrentUserGet**
> apiCurrentUserGet()


### Example

```typescript
import {
    CurrentUserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CurrentUserApi(configuration);

const { status, data } = await apiInstance.apiCurrentUserGet();
```

### Parameters
This endpoint does not have any parameters.


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

# **apiCurrentUserHasPermissionPermissionPost**
> apiCurrentUserHasPermissionPermissionPost()


### Example

```typescript
import {
    CurrentUserApi,
    Configuration
} from './api';

const configuration = new Configuration();
const apiInstance = new CurrentUserApi(configuration);

let permission: string; // (default to undefined)

const { status, data } = await apiInstance.apiCurrentUserHasPermissionPermissionPost(
    permission
);
```

### Parameters

|Name | Type | Description  | Notes|
|------------- | ------------- | ------------- | -------------|
| **permission** | [**string**] |  | defaults to undefined|


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

