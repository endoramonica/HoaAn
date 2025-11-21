# OrderDetailDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**orderId** | **string** |  | [optional] [default to undefined]
**orderNumber** | **string** |  | [optional] [default to undefined]
**storeId** | **string** |  | [optional] [default to undefined]
**storeName** | **string** |  | [optional] [default to undefined]
**customerId** | **string** |  | [optional] [default to undefined]
**customerName** | **string** |  | [optional] [default to undefined]
**customerEmail** | **string** |  | [optional] [default to undefined]
**customerPhone** | **string** |  | [optional] [default to undefined]
**status** | [**OrderStatus**](OrderStatus.md) |  | [optional] [default to undefined]
**statusText** | **string** |  | [optional] [readonly] [default to undefined]
**subTotal** | **number** |  | [optional] [default to undefined]
**shippingFee** | **number** |  | [optional] [default to undefined]
**taxAmount** | **number** |  | [optional] [default to undefined]
**discountAmount** | **number** |  | [optional] [default to undefined]
**totalAmount** | **number** |  | [optional] [default to undefined]
**notes** | **string** |  | [optional] [default to undefined]
**createdAt** | **string** |  | [optional] [default to undefined]
**updatedAt** | **string** |  | [optional] [default to undefined]
**completedAt** | **string** |  | [optional] [default to undefined]
**createdById** | **string** |  | [optional] [default to undefined]
**createdByName** | **string** |  | [optional] [default to undefined]
**shipping** | [**OrderShippingDto**](OrderShippingDto.md) |  | [optional] [default to undefined]
**items** | [**Array&lt;OrderItemDTO&gt;**](OrderItemDTO.md) |  | [optional] [default to undefined]
**itemsCount** | **number** |  | [optional] [readonly] [default to undefined]
**statusHistories** | [**Array&lt;OrderStatusHistoryDTO&gt;**](OrderStatusHistoryDTO.md) |  | [optional] [default to undefined]
**isPaid** | **boolean** |  | [optional] [default to undefined]
**paidAmount** | **number** |  | [optional] [default to undefined]
**paymentMethod** | **string** |  | [optional] [default to undefined]
**paidAt** | **string** |  | [optional] [default to undefined]
**transactionId** | **string** |  | [optional] [default to undefined]

## Example

```typescript
import { OrderDetailDto } from './api';

const instance: OrderDetailDto = {
    orderId,
    orderNumber,
    storeId,
    storeName,
    customerId,
    customerName,
    customerEmail,
    customerPhone,
    status,
    statusText,
    subTotal,
    shippingFee,
    taxAmount,
    discountAmount,
    totalAmount,
    notes,
    createdAt,
    updatedAt,
    completedAt,
    createdById,
    createdByName,
    shipping,
    items,
    itemsCount,
    statusHistories,
    isPaid,
    paidAmount,
    paymentMethod,
    paidAt,
    transactionId,
};
```

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
