/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { CancelOrderRequest } from '../models/CancelOrderRequest';
import type { CheckoutDto } from '../models/CheckoutDto';
import type { CheckoutResponseDtoApiResponse } from '../models/CheckoutResponseDtoApiResponse';
import type { OrderDetailDtoApiResponse } from '../models/OrderDetailDtoApiResponse';
import type { OrderDetailDtoListApiResponse } from '../models/OrderDetailDtoListApiResponse';
import type { OrderStatus } from '../models/OrderStatus';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CheckoutService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Process checkout and create order from cart
     * Flow:
     * 1. Validate cart exists and belongs to user
     * 2. Validate cart has items and products are available
     * 3. Create order with items, shipping, and calculate totals
     * 4. Clear cart after successful order creation
     *
     * Example Request:
     * POST /api/v1/checkout/process
     * {
         * "cartId": "guid",
         * "shippingInfo": {
             * "recipientName": "Nguyễn Văn A",
             * "phoneNumber": "0123456789",
             * "address": "123 Đường ABC",
             * "ward": "Phường 1",
             * "district": "Quận 1",
             * "city": "TP. Hồ Chí Minh",
             * "postalCode": "70000",
             * "shippingMethod": "STANDARD"
             * },
             * "notes": "Please ring doorbell twice"
             * }
             * @param requestBody Checkout request containing cart ID and shipping info
             * @returns CheckoutResponseDtoApiResponse OK
             * @throws ApiError
             */
            public postApiV1CheckoutProcess(
                requestBody?: CheckoutDto,
            ): CancelablePromise<CheckoutResponseDtoApiResponse> {
                return this.httpRequest.request({
                    method: 'POST',
                    url: '/api/v1/Checkout/process',
                    body: requestBody,
                    mediaType: 'application/json',
                    errors: {
                        400: `Bad Request`,
                        404: `Not Found`,
                    },
                });
            }
            /**
             * Get order by ID (customer can only see their own orders)
             * @param orderId Order ID
             * @returns OrderDetailDtoApiResponse OK
             * @throws ApiError
             */
            public getApiV1Checkout(
                orderId: string,
            ): CancelablePromise<OrderDetailDtoApiResponse> {
                return this.httpRequest.request({
                    method: 'GET',
                    url: '/api/v1/Checkout/{orderId}',
                    path: {
                        'orderId': orderId,
                    },
                    errors: {
                        403: `Forbidden`,
                        404: `Not Found`,
                    },
                });
            }
            /**
             * @param page
             * @param pageSize
             * @param keyword Search by OrderNumber or CustomerName (partial match, case-insensitive)
             * @param customerId Filter by specific customer
             * @param storeId Filter by specific store
             * @param status Filter by order status (enum)
             * Example: 1 (Pending), 2 (Confirmed), etc.
             * @param fromDate Filter orders created on or after this date
             * @param toDate Filter orders created on or before this date
             * @param minAmount Filter by minimum order amount
             * @param maxAmount Filter by maximum order amount
             * @param sortBy Sort by field: "CreatedAt", "TotalAmount", "OrderNumber", "Status", "CustomerName"
             * @param sortDescending Sort descending (true) or ascending (false)
             * @param type Filter by type: "product" or "service"
             * If null, returns all types (backward compatible)
             * @param serviceCategory Filter by service category (only applies when type='service')
             * Values: ancestor-worship, opening-ceremony, wedding, buddha-worship, new-house, feng-shui-consultation
             * @returns OrderDetailDtoListApiResponse OK
             * @throws ApiError
             */
            public getApiV1CheckoutMyOrders(
                page?: number,
                pageSize?: number,
                keyword?: string,
                customerId?: string,
                storeId?: string,
                status?: OrderStatus,
                fromDate?: string,
                toDate?: string,
                minAmount?: number,
                maxAmount?: number,
                sortBy?: string,
                sortDescending?: boolean,
                type?: string,
                serviceCategory?: string,
            ): CancelablePromise<OrderDetailDtoListApiResponse> {
                return this.httpRequest.request({
                    method: 'GET',
                    url: '/api/v1/Checkout/my-orders',
                    query: {
                        'Page': page,
                        'PageSize': pageSize,
                        'Keyword': keyword,
                        'CustomerId': customerId,
                        'StoreId': storeId,
                        'Status': status,
                        'FromDate': fromDate,
                        'ToDate': toDate,
                        'MinAmount': minAmount,
                        'MaxAmount': maxAmount,
                        'SortBy': sortBy,
                        'SortDescending': sortDescending,
                        'Type': type,
                        'ServiceCategory': serviceCategory,
                    },
                    errors: {
                        400: `Bad Request`,
                    },
                });
            }
            /**
             * Cancel an order (customer can only cancel their own orders)
             * Business Rules:
             * - Only orders with status Pending or Confirmed can be cancelled
             * - User must own the order
             * - Provides reason for cancellation (tracked in status history)
             * - Automatically restores product stock (if implemented)
             *
             * Example Request:
             * POST /api/v1/checkout/{orderId}/cancel
             * {
                 * "reason": "Changed my mind about this order"
                 * }
                 * @param orderId Order ID to cancel
                 * @param requestBody Cancellation request with reason
                 * @returns BooleanApiResponse OK
                 * @throws ApiError
                 */
                public postApiV1CheckoutCancel(
                    orderId: string,
                    requestBody?: CancelOrderRequest,
                ): CancelablePromise<BooleanApiResponse> {
                    return this.httpRequest.request({
                        method: 'POST',
                        url: '/api/v1/Checkout/{orderId}/cancel',
                        path: {
                            'orderId': orderId,
                        },
                        body: requestBody,
                        mediaType: 'application/json',
                        errors: {
                            400: `Bad Request`,
                            403: `Forbidden`,
                            404: `Not Found`,
                        },
                    });
                }
            }
