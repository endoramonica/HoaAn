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
     * @param requestBody
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
     * @param orderId
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
     * @param orderId
     * @param requestBody
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
