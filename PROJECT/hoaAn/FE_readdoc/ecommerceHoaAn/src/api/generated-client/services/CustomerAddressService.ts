/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddressResponseDtoApiResponse } from '../models/AddressResponseDtoApiResponse';
import type { AddressResponseDtoListApiResponse } from '../models/AddressResponseDtoListApiResponse';
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { CreateAddressDto } from '../models/CreateAddressDto';
import type { UpdateAddressDto } from '../models/UpdateAddressDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CustomerAddressService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns AddressResponseDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAddresses(): CancelablePromise<AddressResponseDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/customer/addresses',
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns AddressResponseDtoApiResponse Created
     * @throws ApiError
     */
    public postApiV1CustomerAddresses(
        requestBody?: CreateAddressDto,
    ): CancelablePromise<AddressResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/customer/addresses',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * @param id
     * @returns AddressResponseDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1CustomerAddresses1(
        id: string,
    ): CancelablePromise<AddressResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/customer/addresses/{id}',
            path: {
                'id': id,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns AddressResponseDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1CustomerAddresses(
        id: string,
        requestBody?: UpdateAddressDto,
    ): CancelablePromise<AddressResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/customer/addresses/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1CustomerAddresses(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/customer/addresses/{id}',
            path: {
                'id': id,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public postApiV1CustomerAddressesSetDefault(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/customer/addresses/{id}/set-default',
            path: {
                'id': id,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
}
