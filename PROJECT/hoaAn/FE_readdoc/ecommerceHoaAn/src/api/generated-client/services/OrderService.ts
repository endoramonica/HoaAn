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
     * @param id
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
     * @param page
     * @param pageSize
     * @param keyword
     * @param customerId
     * @param storeId
     * @param status
     * @param fromDate
     * @param toDate
     * @param minAmount
     * @param maxAmount
     * @param sortBy
     * @param sortDescending
     * @param type
     * @param serviceCategory
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
     * @param page
     * @param pageSize
     * @param keyword
     * @param customerId
     * @param storeId
     * @param status
     * @param fromDate
     * @param toDate
     * @param minAmount
     * @param maxAmount
     * @param sortBy
     * @param sortDescending
     * @param type
     * @param serviceCategory
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
     * @param status
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
     * @param orderId
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
     * @param orderId
     * @param requestBody
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
     * @param orderId
     * @param requestBody
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
     * @param orderId
     * @param newStatus
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
     * @param storeId
     * @param fromDate
     * @param toDate
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
     * @param storeId
     * @param fromDate
     * @param toDate
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
     * @param requestBody
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
