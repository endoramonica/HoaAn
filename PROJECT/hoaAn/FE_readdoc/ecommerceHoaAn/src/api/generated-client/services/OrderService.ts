/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { BulkUpdateStatusRequest } from '../models/BulkUpdateStatusRequest';
import type { CancelOrderRequest } from '../models/CancelOrderRequest';
import type { DecimalApiResponse } from '../models/DecimalApiResponse';
import type { Int32ApiResponse } from '../models/Int32ApiResponse';
import type { OrderDetailDtoApiResponse } from '../models/OrderDetailDtoApiResponse';
import type { OrderDetailDtoListApiResponse } from '../models/OrderDetailDtoListApiResponse';
import type { OrderDetailDtoPaginatedResultApiResponse } from '../models/OrderDetailDtoPaginatedResultApiResponse';
import type { OrderStatus } from '../models/OrderStatus';
import type { OrderStatusHistoryDTOListApiResponse } from '../models/OrderStatusHistoryDTOListApiResponse';
import type { OrderStatusUpdateDTO } from '../models/OrderStatusUpdateDTO';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OrderService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Get order by ID
     * ✅ Service automatically checks if user has permission to view this order
     * @param id Order ID
     * @returns OrderDetailDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1Order(
        id: string,
    ): CancelablePromise<OrderDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/{id}',
            path: {
                'id': id,
            },
            errors: {
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Get current user's orders (paginated)
     * ✅ Service automatically uses CustomerId from JWT token
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
     * @returns OrderDetailDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiV1OrderMyOrders(
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
    ): CancelablePromise<OrderDetailDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/my-orders',
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
     * Get all orders (admin/seller only)
     * ✅ Service enforces admin check internally
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
     * @returns OrderDetailDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiV1Order1(
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
    ): CancelablePromise<OrderDetailDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order',
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
                403: `Forbidden`,
            },
        });
    }
    /**
     * Get orders by status
     * ✅ Service automatically filters based on user role
     * @param status Order status
     * @returns OrderDetailDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1OrderStatus(
        status: OrderStatus,
    ): CancelablePromise<OrderDetailDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/status/{status}',
            path: {
                'status': status,
            },
        });
    }
    /**
     * Get order status history
     * ✅ Service checks permission before returning history
     * @param orderId Order ID
     * @returns OrderStatusHistoryDTOListApiResponse OK
     * @throws ApiError
     */
    public getApiV1OrderStatusHistory(
        orderId: string,
    ): CancelablePromise<OrderStatusHistoryDTOListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/{orderId}/status-history',
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
     * Update order status
     * ✅ Service automatically uses current UserId as changedBy
     * @param orderId Order ID
     * @param requestBody Status update request
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public putApiV1OrderStatus(
        orderId: string,
        requestBody?: OrderStatusUpdateDTO,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/Order/{orderId}/status',
            path: {
                'orderId': orderId,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Cancel an order
     * ✅ Service automatically uses current UserId as changedBy
     * ✅ Service checks if user has permission to cancel
     * @param orderId Order ID
     * @param requestBody Cancel request with reason
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public postApiV1OrderCancel(
        orderId: string,
        requestBody?: CancelOrderRequest,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Order/{orderId}/cancel',
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
    /**
     * Check if order status can be changed
     * @param orderId Order ID
     * @param newStatus Desired new status
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public getApiV1OrderCanChangeStatus(
        orderId: string,
        newStatus?: OrderStatus,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/{orderId}/can-change-status',
            path: {
                'orderId': orderId,
            },
            query: {
                'newStatus': newStatus,
            },
        });
    }
    /**
     * Get order statistics - total count
     * ✅ Service automatically filters based on user role
     * @param storeId Optional store filter
     * @param fromDate Optional start date
     * @param toDate Optional end date
     * @returns Int32ApiResponse OK
     * @throws ApiError
     */
    public getApiV1OrderStatsCount(
        storeId?: string,
        fromDate?: string,
        toDate?: string,
    ): CancelablePromise<Int32ApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/stats/count',
            query: {
                'storeId': storeId,
                'fromDate': fromDate,
                'toDate': toDate,
            },
        });
    }
    /**
     * Get order statistics - total revenue
     * ✅ ADMIN ONLY - Service enforces this
     * @param storeId Optional store filter
     * @param fromDate Optional start date
     * @param toDate Optional end date
     * @returns DecimalApiResponse OK
     * @throws ApiError
     */
    public getApiV1OrderStatsRevenue(
        storeId?: string,
        fromDate?: string,
        toDate?: string,
    ): CancelablePromise<DecimalApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Order/stats/revenue',
            query: {
                'storeId': storeId,
                'fromDate': fromDate,
                'toDate': toDate,
            },
            errors: {
                403: `Forbidden`,
            },
        });
    }
    /**
     * Bulk update order statuses
     * ✅ ADMIN ONLY - Service enforces this
     * ✅ Service automatically uses current UserId as changedBy
     * @param requestBody Bulk update request
     * @returns Int32ApiResponse OK
     * @throws ApiError
     */
    public postApiV1OrderBulkUpdateStatus(
        requestBody?: BulkUpdateStatusRequest,
    ): CancelablePromise<Int32ApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Order/bulk-update-status',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                403: `Forbidden`,
            },
        });
    }
}
