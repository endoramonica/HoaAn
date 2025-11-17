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
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CustomerAddressService {
    /**
     * @returns AddressResponseDtoListApiResponse OK
     * @throws ApiError
     */
    public static getApiV1CustomerAddresses(): CancelablePromise<AddressResponseDtoListApiResponse> {
        return __request(OpenAPI, {
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
    public static postApiV1CustomerAddresses(
        requestBody?: CreateAddressDto,
    ): CancelablePromise<AddressResponseDtoApiResponse> {
        return __request(OpenAPI, {
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
    public static getApiV1CustomerAddresses1(
        id: string,
    ): CancelablePromise<AddressResponseDtoApiResponse> {
        return __request(OpenAPI, {
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
    public static putApiV1CustomerAddresses(
        id: string,
        requestBody?: UpdateAddressDto,
    ): CancelablePromise<AddressResponseDtoApiResponse> {
        return __request(OpenAPI, {
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
    public static deleteApiV1CustomerAddresses(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
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
    public static postApiV1CustomerAddressesSetDefault(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
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
