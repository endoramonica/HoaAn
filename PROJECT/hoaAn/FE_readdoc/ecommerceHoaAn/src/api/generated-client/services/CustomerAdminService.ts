/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddPointsRequest } from '../models/AddPointsRequest';
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { CreateCustomerAddressRequest } from '../models/CreateCustomerAddressRequest';
import type { CreateCustomerRequest } from '../models/CreateCustomerRequest';
import type { CreateInteractionRequest } from '../models/CreateInteractionRequest';
import type { CRMInteractionDtoApiResponse } from '../models/CRMInteractionDtoApiResponse';
import type { CRMInteractionListDtoListApiResponse } from '../models/CRMInteractionListDtoListApiResponse';
import type { CRMInteractionListDtoPaginatedResponseApiResponse } from '../models/CRMInteractionListDtoPaginatedResponseApiResponse';
import type { CRMInteractionStatus } from '../models/CRMInteractionStatus';
import type { CRMInteractionType } from '../models/CRMInteractionType';
import type { CustomerAddressDtoApiResponse } from '../models/CustomerAddressDtoApiResponse';
import type { CustomerAddressDtoListApiResponse } from '../models/CustomerAddressDtoListApiResponse';
import type { CustomerDetailDtoApiResponse } from '../models/CustomerDetailDtoApiResponse';
import type { CustomerListDtoListApiResponse } from '../models/CustomerListDtoListApiResponse';
import type { CustomerListDtoPaginatedResponseApiResponse } from '../models/CustomerListDtoPaginatedResponseApiResponse';
import type { CustomerOrderSummaryDtoApiResponse } from '../models/CustomerOrderSummaryDtoApiResponse';
import type { CustomerStatisticsDtoApiResponse } from '../models/CustomerStatisticsDtoApiResponse';
import type { DeductPointsRequest } from '../models/DeductPointsRequest';
import type { LoyaltyHistoryDtoListApiResponse } from '../models/LoyaltyHistoryDtoListApiResponse';
import type { OrderDetailDtoPaginatedResponseApiResponse } from '../models/OrderDetailDtoPaginatedResponseApiResponse';
import type { UpdateCustomerAddressRequest } from '../models/UpdateCustomerAddressRequest';
import type { UpdateCustomerRequest } from '../models/UpdateCustomerRequest';
import type { UpdateInteractionRequest } from '../models/UpdateInteractionRequest';
import type { UpdateTierRequest } from '../models/UpdateTierRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CustomerAdminService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @param page
     * @param pageSize
     * @param sortBy
     * @param sortDescending
     * @param skip
     * @param searchTerm
     * @param tier
     * @param isActive
     * @param fromDate
     * @param toDate
     * @param minLoyaltyPoints
     * @param maxLoyaltyPoints
     * @param hasEmail
     * @param hasPhone
     * @returns CustomerListDtoPaginatedResponseApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdmin(
        page?: number,
        pageSize?: number,
        sortBy?: string,
        sortDescending?: boolean,
        skip?: number,
        searchTerm?: string,
        tier?: string,
        isActive?: boolean,
        fromDate?: string,
        toDate?: string,
        minLoyaltyPoints?: number,
        maxLoyaltyPoints?: number,
        hasEmail?: boolean,
        hasPhone?: boolean,
    ): CancelablePromise<CustomerListDtoPaginatedResponseApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin',
            query: {
                'Page': page,
                'PageSize': pageSize,
                'SortBy': sortBy,
                'SortDescending': sortDescending,
                'Skip': skip,
                'SearchTerm': searchTerm,
                'Tier': tier,
                'IsActive': isActive,
                'FromDate': fromDate,
                'ToDate': toDate,
                'MinLoyaltyPoints': minLoyaltyPoints,
                'MaxLoyaltyPoints': maxLoyaltyPoints,
                'HasEmail': hasEmail,
                'HasPhone': hasPhone,
            },
        });
    }
    /**
     * @param requestBody
     * @returns CustomerDetailDtoApiResponse Created
     * @throws ApiError
     */
    public postApiV1CustomerAdmin(
        requestBody?: CreateCustomerRequest,
    ): CancelablePromise<CustomerDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/CustomerAdmin',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns CustomerDetailDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdmin1(
        id: string,
    ): CancelablePromise<CustomerDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns CustomerDetailDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAdmin(
        id: string,
        requestBody?: UpdateCustomerRequest,
    ): CancelablePromise<CustomerDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/CustomerAdmin/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1CustomerAdmin(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/CustomerAdmin/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param term
     * @returns CustomerListDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminSearch(
        term?: string,
    ): CancelablePromise<CustomerListDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/search',
            query: {
                'term': term,
            },
        });
    }
    /**
     * @param id
     * @returns CustomerStatisticsDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminStatistics(
        id: string,
    ): CancelablePromise<CustomerStatisticsDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}/statistics',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param page
     * @param pageSize
     * @param sortBy
     * @param sortDescending
     * @param skip
     * @returns OrderDetailDtoPaginatedResponseApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminOrders(
        id: string,
        page?: number,
        pageSize?: number,
        sortBy?: string,
        sortDescending?: boolean,
        skip?: number,
    ): CancelablePromise<OrderDetailDtoPaginatedResponseApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}/orders',
            path: {
                'id': id,
            },
            query: {
                'Page': page,
                'PageSize': pageSize,
                'SortBy': sortBy,
                'SortDescending': sortDescending,
                'Skip': skip,
            },
        });
    }
    /**
     * @param id
     * @returns CustomerOrderSummaryDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminOrdersSummary(
        id: string,
    ): CancelablePromise<CustomerOrderSummaryDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}/orders/summary',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @returns CustomerAddressDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminAddresses(
        id: string,
    ): CancelablePromise<CustomerAddressDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}/addresses',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param addressId
     * @returns CustomerAddressDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminAddresses1(
        addressId: string,
    ): CancelablePromise<CustomerAddressDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/addresses/{addressId}',
            path: {
                'addressId': addressId,
            },
        });
    }
    /**
     * @param addressId
     * @param requestBody
     * @returns CustomerAddressDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAdminAddresses(
        addressId: string,
        requestBody?: UpdateCustomerAddressRequest,
    ): CancelablePromise<CustomerAddressDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/CustomerAdmin/addresses/{addressId}',
            path: {
                'addressId': addressId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param addressId
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1CustomerAdminAddresses(
        addressId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/CustomerAdmin/addresses/{addressId}',
            path: {
                'addressId': addressId,
            },
        });
    }
    /**
     * @param requestBody
     * @returns CustomerAddressDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1CustomerAdminAddresses(
        requestBody?: CreateCustomerAddressRequest,
    ): CancelablePromise<CustomerAddressDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/CustomerAdmin/addresses',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @param addressId
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAdminAddressesDefault(
        id: string,
        addressId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/CustomerAdmin/{id}/addresses/{addressId}/default',
            path: {
                'id': id,
                'addressId': addressId,
            },
        });
    }
    /**
     * @param id
     * @param page
     * @param pageSize
     * @param sortBy
     * @param sortDescending
     * @param skip
     * @param customerId
     * @param type
     * @param status
     * @param searchTerm
     * @param fromDate
     * @param toDate
     * @param followUpFromDate
     * @param followUpToDate
     * @param createdBy
     * @param hasPendingFollowUp
     * @returns CRMInteractionListDtoPaginatedResponseApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminInteractions(
        id: string,
        page?: number,
        pageSize?: number,
        sortBy?: string,
        sortDescending?: boolean,
        skip?: number,
        customerId?: string,
        type?: CRMInteractionType,
        status?: CRMInteractionStatus,
        searchTerm?: string,
        fromDate?: string,
        toDate?: string,
        followUpFromDate?: string,
        followUpToDate?: string,
        createdBy?: string,
        hasPendingFollowUp?: boolean,
    ): CancelablePromise<CRMInteractionListDtoPaginatedResponseApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}/interactions',
            path: {
                'id': id,
            },
            query: {
                'Page': page,
                'PageSize': pageSize,
                'SortBy': sortBy,
                'SortDescending': sortDescending,
                'Skip': skip,
                'CustomerId': customerId,
                'Type': type,
                'Status': status,
                'SearchTerm': searchTerm,
                'FromDate': fromDate,
                'ToDate': toDate,
                'FollowUpFromDate': followUpFromDate,
                'FollowUpToDate': followUpToDate,
                'CreatedBy': createdBy,
                'HasPendingFollowUp': hasPendingFollowUp,
            },
        });
    }
    /**
     * @param interactionId
     * @returns CRMInteractionDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminInteractions1(
        interactionId: string,
    ): CancelablePromise<CRMInteractionDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/interactions/{interactionId}',
            path: {
                'interactionId': interactionId,
            },
        });
    }
    /**
     * @param interactionId
     * @param requestBody
     * @returns CRMInteractionDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAdminInteractions(
        interactionId: string,
        requestBody?: UpdateInteractionRequest,
    ): CancelablePromise<CRMInteractionDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/CustomerAdmin/interactions/{interactionId}',
            path: {
                'interactionId': interactionId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param interactionId
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1CustomerAdminInteractions(
        interactionId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/CustomerAdmin/interactions/{interactionId}',
            path: {
                'interactionId': interactionId,
            },
        });
    }
    /**
     * @param requestBody
     * @returns CRMInteractionDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1CustomerAdminInteractions(
        requestBody?: CreateInteractionRequest,
    ): CancelablePromise<CRMInteractionDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/CustomerAdmin/interactions',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param interactionId
     * @returns CRMInteractionDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAdminInteractionsComplete(
        interactionId: string,
    ): CancelablePromise<CRMInteractionDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/CustomerAdmin/interactions/{interactionId}/complete',
            path: {
                'interactionId': interactionId,
            },
        });
    }
    /**
     * @param from
     * @param to
     * @returns CRMInteractionListDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminInteractionsUpcoming(
        from?: string,
        to?: string,
    ): CancelablePromise<CRMInteractionListDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/interactions/upcoming',
            query: {
                'from': from,
                'to': to,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns CustomerDetailDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1CustomerAdminLoyaltyAdd(
        id: string,
        requestBody?: AddPointsRequest,
    ): CancelablePromise<CustomerDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/CustomerAdmin/{id}/loyalty/add',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns CustomerDetailDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1CustomerAdminLoyaltyDeduct(
        id: string,
        requestBody?: DeductPointsRequest,
    ): CancelablePromise<CustomerDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/CustomerAdmin/{id}/loyalty/deduct',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns CustomerDetailDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAdminTier(
        id: string,
        requestBody?: UpdateTierRequest,
    ): CancelablePromise<CustomerDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/CustomerAdmin/{id}/tier',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns LoyaltyHistoryDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAdminLoyaltyHistory(
        id: string,
    ): CancelablePromise<LoyaltyHistoryDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/CustomerAdmin/{id}/loyalty-history',
            path: {
                'id': id,
            },
        });
    }
}
