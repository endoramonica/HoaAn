/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { StringDateTime_f__AnonymousType6 } from '../models/StringDateTime_f__AnonymousType6';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class VietCommerceApiService {
    /**
     * @returns StringDateTime_f__AnonymousType6 OK
     * @throws ApiError
     */
    public static getHealth(): CancelablePromise<StringDateTime_f__AnonymousType6> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/health',
        });
    }
}
